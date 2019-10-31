using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceLibrary;

namespace HotelEpam
{
    public static class FormingSelectItems
    {
        public static IEnumerable<SelectListItem> GetRoomClassRoomItems(List<Room> rooms)
        {
            IEnumerable<SelectListItem> classRoomItems = rooms.Select(r => r.ClassRoom).Distinct().OrderBy(r => r.Id)
                .Select(
                    r => new SelectListItem()
                    {
                        Text = r.Name.ToString(),
                        Value = r.Id.ToString()
                    }
                );

            return classRoomItems;
        }

        public static IEnumerable<SelectListItem> GetCurrentRoomCountOfPlacesItems(List<Room> rooms)
        {
            IEnumerable<SelectListItem> countOfPlacesItems = rooms.Select(r => r.CountOfPlaces).Distinct()
                .OrderBy(r => r).Select(
                    r => new SelectListItem()
                    {
                        Text = r.ToString(),
                        Value = r.ToString()
                    }
                );

            return countOfPlacesItems;
        }

        public static IEnumerable<SelectListItem> GetRoomNumberItems(List<Room> rooms)
        {
            IEnumerable<SelectListItem> roomNumberItems = rooms.Select(r => r.Numder).OrderBy(r => r).Select(
                r => new SelectListItem()
                {
                    Text = r.ToString(),
                    Value = r.ToString()
                }
            );

            return roomNumberItems;
        }

        public static IEnumerable<SelectListItem> GetClassRoomItems()
        {
            List<ClassRoom> classRooms = CommunicationWithDataBase.GetRoomClasses();

            IEnumerable<SelectListItem> roomClassItems = classRooms.Select(
                rc => new SelectListItem()
                {
                    Text = rc.Name,
                    Value = rc.Id.ToString()
                }
            );

            return roomClassItems;
        }

        public static IEnumerable<SelectListItem> GetRoomCountOfPlacesItems()
        {
            List<SelectListItem> roomCountOfPlacesItems = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "1",
                    Value = "1"
                },
                new SelectListItem()
                {
                    Text = "2",
                    Value = "2"
                },
                new SelectListItem()
                {
                    Text = "3",
                    Value = "3"
                },
                new SelectListItem()
                {
                    Text = "4",
                    Value = "4"
                },
            };

            return roomCountOfPlacesItems;
        }
    }
}