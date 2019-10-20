using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.WebPages;
using ServiceLibrary;

namespace HotelEpam.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // user sign out and him authorize cookies will be deleted
        public ActionResult Exit()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);
            return RedirectToAction("Authorization", "Account");
        }

        private IEnumerable<SelectListItem> GetClassRoomItems(List<Room> rooms)
        {
            IEnumerable<SelectListItem> classRoomItems = rooms.Select(r => r.ClassRoom).Distinct().
                OrderBy(r => r.Id).Select(
                    r => new SelectListItem()
                    {
                        Text = r.Name.ToString(),
                        Value = r.Id.ToString()
                    }
                );

            return classRoomItems;
        }

        private IEnumerable<SelectListItem> GetCountOfPlacesItems(List<Room> rooms)
        {
            IEnumerable<SelectListItem> countOfPlacesItems = rooms.Select(r => r.CountOfPlaces).
                Distinct().OrderBy(r => r).Select(
                    r => new SelectListItem()
                    {
                        Text = r.ToString(),
                        Value = r.ToString()
                    }
                );

            return countOfPlacesItems;
        }

        [Authorize(Roles = "user")]
        public ActionResult RoomReservation()
        {
            List<Room> rooms = CommunicationWithDataBase.GetAllRooms();

            ViewData["RoomClassItems"] = GetClassRoomItems(rooms);
            ViewData["CountOfPlacesItems"] = GetCountOfPlacesItems(rooms);

            return View(rooms);
        }


        [Authorize(Roles = "user")]
        [HttpPost]
        public ActionResult RoomReservation(FormCollection form)
        {
            int roomClassId = Convert.ToInt32(form["RoomClass"]);
            int countOfPlaces = Convert.ToInt32(form["CountOfPlaces"]);
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            try
            {
                startDate = Convert.ToDateTime(form["StartDate"]);
                endDate = Convert.ToDateTime(form["EndDate"]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ModelState.AddModelError("DateValidationError", "Даты указаны некорректно");
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
                string userLogin = User.Identity.Name;
                CommunicationWithDataBase.CreateRequest(userLogin, roomClassId, countOfPlaces, startDate, endDate);
                return RedirectToAction("Index");
            }

            List<Room> rooms = CommunicationWithDataBase.GetAllRooms();

            ViewData["RoomClassItems"] = GetClassRoomItems(rooms);
            ViewData["CountOfPlacesItems"] = GetCountOfPlacesItems(rooms);

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
            List<Room> rooms = CommunicationWithDataBase.GetAllRooms();

            IEnumerable<SelectListItem> roomNumberItems = rooms.Select(r => r.Numder).
                OrderBy(r => r).Select(
                    r => new SelectListItem()
                    {
                        Text = r.ToString(),
                        Value = r.ToString()
                    }
                );
            ViewData["RoomNumberItems"] = roomNumberItems;

            List<Booking> bookings = CommunicationWithDataBase.GetBookingRequests();
            var notProcessedBookings = bookings.Where(b => b.BookingStatusId == 1);
            return View(notProcessedBookings);
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public ActionResult ProcessingRequest(Booking booking)
        {
            CommunicationWithDataBase.UpdateRoomInBooking(booking.Id,Convert.ToInt32(booking.RoomNumber));
            return RedirectToAction("NotProcessedRequests");
        }

        [Authorize(Roles = "manager")]
        public ActionResult AllRooms()
        {
            List<SelectListItem> roomItems = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "Свободен",
                    Value = "1"
                },
                new SelectListItem()
                {
                    Text = "Забронирован",
                    Value = "2"
                },
                new SelectListItem()
                {
                    Text = "Занят",
                    Value = "3"
                },
                new SelectListItem()
                {
                    Text = "Недоступен",
                    Value = "4"
                },
            };
            ViewData["RoomItems"] = roomItems;

            List<Room> rooms = CommunicationWithDataBase.GetAllRooms();
            ViewData["RoomClassItems"] = GetClassRoomItems(rooms);

            return View(rooms);
        }
    }
}