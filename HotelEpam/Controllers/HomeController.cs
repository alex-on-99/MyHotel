using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.WebPages;
using NLog;
using ServiceLibrary;

namespace HotelEpam.Controllers
{
    public class HomeController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public ActionResult Index()
        {
            if (User.IsInRole("manager"))
            {
                return RedirectToAction("ManagerMainPage", "Home");
            }
            else if (User.IsInRole("admin"))
            {
                return RedirectToAction("AdminMainPage", "Admin");
            }

            return View();
        }

        // User sign out and him authorize cookies will be deleted.
        public ActionResult Exit()
        {
            logger.Info($"Пользователь {User.Identity.Name} покинул систему");
            
            FormsAuthentication.SignOut();
            Session.Abandon();

            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);
            return RedirectToAction("Authorization", "Account");
        }


        [Authorize(Roles = "user")]
        public ActionResult RoomReservation()
        {
            IRepository repository = new CommunicationWithDataBase();

            logger.Debug($"Обращение к базе данных, получение комнат");
            List<Room> rooms = repository.GetAllRooms();

            ViewData["RoomClassItems"] = FormingSelectItems.GetRoomClassRoomItems(rooms);
            ViewData["CountOfPlacesItems"] = FormingSelectItems.GetCurrentRoomCountOfPlacesItems(rooms);

            return View(rooms);
        }


        [Authorize(Roles = "user")]
        [HttpPost]
        public ActionResult RoomReservation(FormCollection form)
        {
            IRepository repository = new CommunicationWithDataBase();

            int roomClassId = Convert.ToInt32(form["RoomClass"]);
            int countOfPlaces = Convert.ToInt32(form["CountOfPlaces"]);
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            try
            {
                startDate = Convert.ToDateTime(form["StartDate"]);
                endDate = Convert.ToDateTime(form["EndDate"]);
            }
            catch
            {
                ModelState.AddModelError("DateValidationError", "Даты указаны некорректно");
                logger.Error($"Ошибка. Невозможно конвертировать дату");
            }

            if (startDate < DateTime.Now || startDate >= endDate)
            {
                ModelState.AddModelError("DateValidationError", "Даты указаны некорректно");
            }

            if (startDate.AddDays(14) < endDate)
            {
                ModelState.AddModelError("DateValidationError", "Проживание в отеле допустимо не более 2х недель");
            }

            if (ModelState.IsValid)
            {
                logger.Debug($"Обращение к базе данных, создание запроса на поселение");
                string userLogin = User.Identity.Name;
                repository.CreateRequest(userLogin, roomClassId, countOfPlaces, startDate, endDate);
                logger.Info($"Пользователем создан новый запрос на поселение");
                return RedirectToAction("Index");
            }

            logger.Debug($"Обращение к базе данных, получение комнат");
            List<Room> rooms = repository.GetAllRooms();

            ViewData["RoomClassItems"] = FormingSelectItems.GetRoomClassRoomItems(rooms);
            ViewData["CountOfPlacesItems"] = FormingSelectItems.GetCurrentRoomCountOfPlacesItems(rooms);

            return View();
        }

        [Authorize(Roles = "manager")]
        public ActionResult ManagerMainPage()
        {
            return View();
        }

        [Authorize(Roles = "manager")]
        public ActionResult NotProcessedRequests()
        {
            IRepository repository = new CommunicationWithDataBase();

            logger.Debug($"Обращение к базе данных, получение комнат");
            List<Room> rooms = repository.GetAllRooms();

            IEnumerable<SelectListItem> roomNumberItems = FormingSelectItems.GetRoomNumberItems(rooms);
            ViewData["RoomNumberItems"] = roomNumberItems;

            logger.Debug($"Обращение к базе данных, получение запросов на проживание");
            List<Booking> bookings = repository.GetBookingRequests();
            var notProcessedBookings = bookings.Where(b => b.BookingStatusId == 1);
            return View(notProcessedBookings);
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public ActionResult ProcessingRequest(Booking booking)
        {
            IRepository repository = new CommunicationWithDataBase();

            logger.Debug($"Обращение к базе данных, обновление бронирования комнаты");
            repository.UpdateRoomInBooking(booking.Id, Convert.ToInt32(booking.RoomNumber));
            logger.Info($"Обновление бронирование с {booking.Id} выполнено успешно");
            return RedirectToAction("NotProcessedRequests");
        }

        [Authorize(Roles = "manager")]
        public ActionResult AllRooms()
        {
            IRepository repository = new CommunicationWithDataBase();

            List<SelectListItem> roomItems = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "Свободны",
                    Value = "1"
                },
                new SelectListItem()
                {
                    Text = "Заняты",
                    Value = "2"
                },
                new SelectListItem()
                {
                    Text = "Забронированы",
                    Value = "3"
                },
                new SelectListItem()
                {
                    Text = "Недоступны",
                    Value = "4"
                },
            };

            ViewData["RoomItems"] = roomItems;

            logger.Debug($"Обращение к базе данных, получение комнат");
            List<Room> rooms = repository.GetAllRooms();
            ViewData["RoomClassItems"] = FormingSelectItems.GetRoomClassRoomItems(rooms);

            return View(rooms);
        }

        [Authorize(Roles = "manager")]
        public ActionResult PartialRooms(FormCollection form)
        {
            IRepository repository = new CommunicationWithDataBase();

            logger.Debug($"Обращение к базе данных, получение комнат");
            List<Room> rooms = repository.GetAllRooms();
            List<Room> currentRooms = new List<Room>();
            int roomTypeNumber = 0;

            logger.Debug($"Выборка нужных комнат из всех доступных");
            switch (form["RoomItems"])
            {
                case "1":
                    roomTypeNumber = 1;
                    currentRooms = rooms.Where(r =>
                            r.Bookings.Where(b => b.DateStart <= DateTime.Now && b.DateEnd > DateTime.Now)
                                .Select(rn => rn.RoomNumber).ToList().Contains(r.Numder) == false && r.Availability)
                        .ToList();
                    break;
                case "2":
                    roomTypeNumber = 2;
                    currentRooms = rooms.Where(r =>
                        r.Bookings.Where(b => b.DateStart <= DateTime.Now && b.DateEnd > DateTime.Now)
                            .Select(rn => rn.RoomNumber).ToList().Contains(r.Numder) && r.Availability).ToList();
                    break;
                case "3":
                    roomTypeNumber = 3;
                    currentRooms = rooms.Where(r =>
                        r.Bookings.Where(b => b.DateStart > DateTime.Now)
                            .Select(rn => rn.RoomNumber).ToList().Contains(r.Numder) && r.Availability).ToList();
                    break;
                case "4":
                    roomTypeNumber = 4;
                    currentRooms = rooms.Where(r => r.Availability == false).ToList();
                    break;
            }

            ViewData["RoomTypeNumber"] = roomTypeNumber;
            return View(currentRooms);
        }

        [Authorize(Roles = "user")]
        public ActionResult BookingRoom()
        {
            return View();
        }

        [Authorize(Roles = "user")]
        public ActionResult FreeRoomsForBooking(Booking booking)
        {
            IRepository repository = new CommunicationWithDataBase();

            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            try
            {
                startDate = Convert.ToDateTime(booking.DateStart);
                endDate = Convert.ToDateTime(booking.DateEnd);
            }
            catch
            {
                logger.Error($"Ошибка. Невозможно конвертировать дату");
                ModelState.AddModelError("WrongDateMessage", "Даты указаны некорректно");
                return View();
            }

            if (startDate > DateTime.Now.AddMonths(3))
            {
                ModelState.AddModelError("WrongDateMessage", "Комнаты нельзя бронировать позже чем за 3 месяца");
            }

            if (startDate < DateTime.Now || startDate >= endDate)
            {
                ModelState.AddModelError("WrongDateMessage", "Даты указаны некорректно");
            }

            if (startDate.AddDays(14) < endDate)
            {
                ModelState.AddModelError("WrongDateMessage", "Проживание в отеле допустимо не более 2х недель");
            }

            if (ModelState.IsValid)
            {
                logger.Debug($"Обращение к базе данных, получение комнат сводных с {startDate} по {endDate} число");
                List<Room> freeRooms = repository.GetFreeRooms(startDate, endDate);
                HttpCookie accommodationCookie = new HttpCookie("accommodation date");

                try
                {
                    accommodationCookie["start"] = startDate.ToString();
                    accommodationCookie["end"] = endDate.ToString();
                }
                catch
                {
                    logger.Error("Ошибка. некорректная дата");
                }

                ViewData["StartDate"] = startDate;
                ViewData["EndDate"] = endDate;

                HttpContext.Response.Cookies.Add(accommodationCookie);
                logger.Debug($"Установка куки даты проживания пользователя");

                return View(freeRooms);
            }

            return View();
        }

        public ActionResult BookingRoomByUser(int roomNumber)
        {
            IRepository repository = new CommunicationWithDataBase();

            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            HttpCookie cookie = HttpContext.Request.Cookies["accommodation date"];

            if (cookie == null)
            {
                logger.Error($"Ошибка. Значение куки было null");
            }

            try
            {
                startDate = Convert.ToDateTime(cookie.Values["start"]);
                endDate = Convert.ToDateTime(cookie.Values["end"]);
            }
            catch
            {
                logger.Error($"Ошибка. Невозможно конвертировать дату");
                ModelState.AddModelError("DateValidationError", "Даты указаны некорректно");
            }

            string userLogin = User.Identity.Name;
            logger.Info($"Обращение к базе данных для создания нового бронирования");
            repository.CreateBooking(userLogin,roomNumber, startDate, endDate);
            logger.Info($"Бронирование комнаты {roomNumber} пользователем {userLogin}");
            return JavaScript($"window.location = 'https://localhost:44399/Home/UserBookings'");
        }

        [Authorize(Roles = "user")]
        public ActionResult UserBookings()
        {
            IRepository repository = new CommunicationWithDataBase();

            logger.Debug($"Обращение к базе данных, получение пользователя по логину");
            User user = repository.GetUserByLogin(User.Identity.Name);
            if (user == null)
            {
                logger.Debug($"Ошибка. не возможно получить пользователя");
            }
            logger.Debug($"Обращение к базе данных, получение комнат забронированых пользователем {user.Login}");
            List<Booking> bookings = repository.GetUserBookings(user.Id);
            List<Booking> currentBookings = bookings.Where(b => b.BookingStatusId == 2).ToList();

            logger.Info($"Возвращение забронированных пользователем комнат");
            return View(currentBookings);
        }

        [Authorize(Roles = "manager")]
        public ActionResult ActuallyBookings()
        {
            IRepository repository = new CommunicationWithDataBase();

            logger.Debug($"Обращение к базе данных, обновление статуса бронирований");
            repository.UpdateAllOverdueBookings();

            logger.Debug($"Обращение к базе данных, получение обработанных запросов");
            List<Booking> currentBookings = repository.GetBookings()
                .Where(b => b.BookingStatusId != 1 && b.DateEnd.AddDays(7) > DateTime.Now).ToList();

            return View(currentBookings);
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        public ActionResult ReportUser(int id)
        {
            IRepository repository = new CommunicationWithDataBase();

            User user = repository.GetUserById(id);
            if (user == null)
            {
                logger.Error($"Пользователя с id {id} не сущестувует");
            }

            logger.Debug($"Обращение к базе данных, жалоба на пользователя");
            repository.ReportUser(id);
            logger.Debug($"Жалоба на пользователя была оставлена");

            return View();
        }

        // Process of changing booking status on paid.
        [Authorize(Roles = "manager")]
        [HttpPost]
        public ActionResult ConfirmPayment(int id)
        {
            IRepository repository = new CommunicationWithDataBase();

            logger.Debug($"Обращение к базе данных, для изменения статуса бронирования");
            repository.ConfirmPayment(id);
            logger.Debug($"Оплата подтверждена");
            return JavaScript($"window.location = 'https://localhost:44399/Home/ActuallyBookings'");
        }

        // Process of deleting booking.
        [Authorize(Roles = "manager")]
        [HttpPost]
        public ActionResult DeleteBooking(int id)
        {
            IRepository repository = new CommunicationWithDataBase();

            logger.Debug($"Обращение к базе данных, для изменения статуса бронирования");
            repository.DeleteBooking(id);
            logger.Info($"Бронирование снято");
            return JavaScript($"window.location = 'https://localhost:44399/Home/ActuallyBookings'");
        }
    }
}