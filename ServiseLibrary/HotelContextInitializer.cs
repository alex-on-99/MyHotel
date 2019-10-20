using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public class HotelContextInitializer
        : CreateDatabaseIfNotExists<HotelContext>
    {
        protected override void Seed(HotelContext context)
        {
            var roles = new List<Role>()
            {
                new Role() {Id = 1, Name = "user"},
                new Role() {Id = 2, Name = "manager"},
                new Role() {Id = 3, Name = "admin"}
            };

            var classRooms = new List<ClassRoom>()
            {
                new ClassRoom(){Id = 1,Name = "Економ", Price = 400},
                new ClassRoom(){Id = 2,Name = "Стандарт", Price = 620},
                new ClassRoom(){Id = 3,Name = "Люкс", Price = 900},
                new ClassRoom(){Id = 4,Name = "Королевский", Price = 1450},
            };
            
            var bookingStatuses = new List<BookingStatus>()
            {
                new BookingStatus(){Id = 1,Name = "В обработке"},
                new BookingStatus(){Id = 2,Name = "Обработан"},
                new BookingStatus(){Id = 3,Name = "Оплачен"},
                new BookingStatus(){Id = 4,Name = "Отменён"},
                new BookingStatus(){Id = 5,Name = "Проссрочен"},
            };


            using (var ctx = new HotelContext("HotelContext"))
            {
                roles.ForEach(role => ctx.Roles.Add(role));
                classRooms.ForEach(classRoom => ctx.ClassRooms.Add(classRoom));
                bookingStatuses.ForEach(bs => ctx.BookingStatuses.Add(bs));
                ctx.SaveChanges();
            }

            var users = new List<User>()
            {
                new User()
                {
                    Id = 1,
                    FirstName = "oleksandr",
                    SecondName = "kovalov",
                    Login = "alex-on-99",
                    Email = "olek.sandr@gmail.com",
                    Password = EncryptionMD5.Encript("123456"),
                    Passport = "МТ451293",
                    DateOfBirth = new DateTime(1999,12,8),
                    ViolatingPower = 0,
                    BlockDate = DateTime.Now.AddYears(-1),
                    RoleId = 2
                },
                new User()
                {
                    Id = 2,
                    FirstName = "llex",
                    SecondName = "stalone",
                    Login = "Aloha",
                    Email = "alex.23.kovalov@gmail.com",
                    Password = EncryptionMD5.Encript("123456"),
                    Passport = "МТ221133",
                    DateOfBirth = new DateTime(1999,12,8),
                    ViolatingPower = 0,
                    BlockDate = DateTime.Now.AddYears(-1),
                    RoleId = 3
                }
            };

            var rooms = new List<Room>()
            {
                new Room()
                {
                    Numder = 101,
                    ClassRoomId = 1,
                    CountOfPlaces = 2,
                    Availability = true
                },
                new Room()
                {
                    Numder = 102,
                    ClassRoomId = 2,
                    CountOfPlaces = 3,
                    Availability = true
                },
                new Room()
                {
                    Numder = 103,
                    ClassRoomId = 1,
                    CountOfPlaces = 1,
                    Availability = true
                },
                new Room()
                {
                    Numder = 201,
                    ClassRoomId = 3,
                    CountOfPlaces = 2,
                    Availability = true
                },
                new Room()
                {
                    Numder = 202,
                    ClassRoomId = 3,
                    CountOfPlaces = 2,
                    Availability = true
                },
                new Room()
                {
                    Numder = 203,
                    ClassRoomId = 4,
                    CountOfPlaces = 3,
                    Availability = true
                },
            };

            using (var ctx = new HotelContext("HotelContext"))
            {
                users.ForEach(user => ctx.Users.Add(user));
                rooms.ForEach(room => ctx.Rooms.Add(room));
                ctx.SaveChanges();
            }
        }
    }
}
