using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;
using ServiceLibrary;

namespace HotelEpam.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult AdminMainPage()
        {
            return View();
        }

        public ActionResult CreatingRoom()
        {

            IEnumerable<SelectListItem> roomCountOfPlacesItems = FormingSelectItems.GetRoomCountOfPlacesItems();
            IEnumerable<SelectListItem> classRoomItems = FormingSelectItems.GetClassRoomItems();

            ViewData["RoomCountOfPlacesItems"] = roomCountOfPlacesItems;
            ViewData["ClassRoomItems"] = classRoomItems;

            return View();
        }

        [HttpPost]
        public ActionResult CreatingRoom(Room room)
        {
            IRepository repository = new CommunicationWithDataBase();

            logger.Debug($"Обращение к базе данных, получение комнат");
            Room existedRoom = repository.GetRoomByNumber(room.Numder);

            if (existedRoom != null)
            {
                ModelState.AddModelError(nameof(room.Numder), "Комната с таким номером уже существует");
            }

            if (ModelState.IsValid)
            {
                logger.Debug($"Обращение к базе данных, для добавления новой комнаты");
                logger.Info($"В базу данных добавлена новая комната");
                repository.CreateRoom(room.Numder,room.ClassRoomId,room.CountOfPlaces);
                return RedirectToAction("AdminMainPage");
            }

            IEnumerable<SelectListItem> roomCountOfPlacesItems = FormingSelectItems.GetRoomCountOfPlacesItems();
            IEnumerable<SelectListItem> classRoomItems = FormingSelectItems.GetClassRoomItems();

            ViewData["RoomCountOfPlacesItems"] = roomCountOfPlacesItems;
            ViewData["ClassRoomItems"] = classRoomItems;

            logger.Info($"Отказ в добавлении новой комнаты");
            return View();
        }

        public ActionResult UpdateRoomStatus()
        {
            IRepository repository = new CommunicationWithDataBase();

            logger.Debug($"Обращение к базе данных, получение комнат");
            List<Room> rooms = repository.GetAllRooms();

            return View(rooms);
        }

        public ActionResult ProcessingOfRoomStatusUpdating(Room room)
        {
            IRepository repository = new CommunicationWithDataBase();

            logger.Debug($"Обращение к базе данных, для обновления статуса комнаты");
            logger.Info($"Стату комнаты {room.Numder} обновлен");
            repository.UpdateRoomAvailability(room.Numder,room.Availability);
            return RedirectToAction("UpdateRoomStatus");
        }
    }
}