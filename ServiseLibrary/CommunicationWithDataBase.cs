using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public static class CommunicationWithDataBase
    {
        public static void CreateUser(string firstName, string secondName,
            string login, string email, string password, DateTime dateOfBirth)
        {
            using (var ctx = new HotelContext("HotelContext"))
            {
                ctx.Users.Add(new User()
                {
                    FirstName = firstName,
                    SecondName = secondName,
                    DateOfBirth = dateOfBirth,
                    Email = email,
                    Login = login,
                    Password = EncryptionMD5.Encript(password),
                    ViolatingPower = 0,
                    BlockDate = DateTime.Now.AddYears(-1),
                    RoleId = 1
                });

                ctx.SaveChanges();
            }
        }

        public static User GetUserByLogin(string login)
        {
            User user;
            using (var ctx = new HotelContext("HotelContext"))
            {
                user = ctx.Users.FirstOrDefault(u => u.Login == login);
            }

            return user;
        }

        public static User GetUserByEmail(string email)
        {
            User user;
            using (var ctx = new HotelContext("HotelContext"))
            {
                user = ctx.Users.FirstOrDefault(u => u.Email == email);
            }

            return user;
        }

        public static string GetUserRole(string login)
        {
            User user = GetUserByLogin(login);
            string role = null;

            if (user != null)
            {
                using (var ctx = new HotelContext("HotelContext"))
                {
                    role = ctx.Roles.FirstOrDefault(r => r.Id == user.RoleId).Name;
                }
            }

            return role;
        }

        public static bool CheckUserAuthorization(string login, string password)
        {
            using (var ctx = new HotelContext("HotelContext"))
            {
                User user = ctx.Users.FirstOrDefault(u => u.Login == login);

                if (user != null && user.Password == EncryptionMD5.Encript(password))
                {
                    return true;
                }
            }

            return false;
        }

        public static List<Room> GetAllRooms()
        {
            using (var ctx = new HotelContext("HotelContext"))
            {
                return ctx.Rooms.Include(r => r.ClassRoom).Include(r => r.Bookings).ToList();
            }
        }

        public static void CreateRequest(string userLogin, int classRoomId, int countOfPlaces, DateTime start,
            DateTime end)
        {
            using (var ctx = new HotelContext("HotelContext"))
            {
                User user = GetUserByLogin(userLogin);

                try
                {
                    RoomRequest roomRequest = new RoomRequest()
                    {
                        ClassRoomId = classRoomId,
                        CountOfPlaces = countOfPlaces
                    };

                    ctx.RoomRequests.Add(roomRequest);
                    ctx.SaveChanges();

                    Booking booking = new Booking()
                    {
                        UserId = user.Id,
                        RoomRequestId = roomRequest.Id,
                        DateStart = start,
                        DateEnd = end,
                        DateBooking = DateTime.Now,
                        BookingStatusId = 1
                    };

                    ctx.Bookings.Add(booking);
                    ctx.SaveChanges();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static List<Booking> GetBookingRequests()
        {
            var roomRequests = new List<Booking>();
            using (var ctx = new HotelContext("HotelContext"))
            {
                roomRequests = ctx.Bookings.Include(r => r.RoomRequest)
                    .Include(r => r.RoomRequest.ClassRoom).ToList();
            }

            return roomRequests;
        }

        public static void UpdateRoomInBooking(int bookingId, int roomNumber)
        {
            using (var ctx = new HotelContext("HotelContext"))
            {
                Booking booking = ctx.Bookings.FirstOrDefault(b => b.Id == bookingId);
                booking.RoomNumber = roomNumber;
                booking.BookingStatusId = 2;
                ctx.Entry(booking).State = EntityState.Modified;
                ctx.SaveChanges();
            }
        }
    }
}