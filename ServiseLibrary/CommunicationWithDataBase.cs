using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    // Class that is responsible for communication with the database.
    public static class CommunicationWithDataBase
    {
        // Creating user.
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

        // Select user by id.
        public static User GetUserById(int id)
        {
            User user;
            using (var ctx = new HotelContext("HotelContext"))
            {
                user = ctx.Users.FirstOrDefault(u => u.Id == id);
            }

            return user;
        }

        // Select user by login.
        public static User GetUserByLogin(string login)
        {
            User user;
            using (var ctx = new HotelContext("HotelContext"))
            {
                user = ctx.Users.FirstOrDefault(u => u.Login == login);
            }

            return user;
        }

        // Select user by email.
        public static User GetUserByEmail(string email)
        {
            User user;
            using (var ctx = new HotelContext("HotelContext"))
            {
                user = ctx.Users.FirstOrDefault(u => u.Email == email);
            }

            return user;
        }

        // Select user's role.
        public static string GetUserRole(string login)
        {
            User user = GetUserByLogin(login);
            string role = null;

            if (user != null)
            {
                using (var ctx = new HotelContext("HotelContext"))
                {
                    role = ctx.Roles.FirstOrDefault(r => r.Id == user.RoleId)?.Name;
                }
            }

            return role;
        }

        // Check user authorization data.
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

        // Select all rooms in hotel.
        public static List<Room> GetAllRooms()
        {
            using (var ctx = new HotelContext("HotelContext"))
            {
                return ctx.Rooms.Include(r => r.ClassRoom).Include(r => r.Bookings).ToList();
            }
        }

        // Create request.
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
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        // Create booking.
        public static void CreateBooking(string userLogin, int roomNumber, DateTime start,
            DateTime end)
        {
            using (var ctx = new HotelContext("HotelContext"))
            {
                User user = GetUserByLogin(userLogin);

                try
                {
                    Booking booking = new Booking()
                    {
                        UserId = user.Id,
                        RoomNumber = roomNumber,
                        DateStart = start,
                        DateEnd = end,
                        DateBooking = DateTime.Now,
                        BookingStatusId = 2
                    };

                    ctx.Bookings.Add(booking);
                    ctx.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        // Select bookings by id.
        public static Booking GetBookingById(int id)
        {
            Booking booking;

            using (var ctx = new HotelContext("HotelContext"))
            {
                booking = ctx.Bookings.Include(b => b.BookingStatus).Include(b => b.Room)
                    .Include(b => b.User).FirstOrDefault(b => b.Id == id);
            }

            return booking;
        }

        // Get booking status by id.
        public static BookingStatus GetBookingStatusById(int id)
        {
            BookingStatus bookingStatus;

            using (var ctx = new HotelContext("HotelContext"))
            {
                bookingStatus = ctx.BookingStatuses.FirstOrDefault(b => b.Id == id);
            }

            return bookingStatus;
        }

        // Select all booking requests.
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

        // Set room in booking.
        public static void UpdateRoomInBooking(int bookingId, int roomNumber)
        {
            using (var ctx = new HotelContext("HotelContext"))
            {
                try
                {
                    Booking booking = ctx.Bookings.FirstOrDefault(b => b.Id == bookingId);
                    booking.RoomNumber = roomNumber;
                    booking.BookingStatusId = 2;
                    ctx.Entry(booking).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        // Select all free rooms in certain dates.
        public static List<Room> GetFreeRooms(DateTime start, DateTime end)
        {
            List<Room> rooms = CommunicationWithDataBase.GetAllRooms();
            List<Room> freeRooms = new List<Room>();
            using (var ctx = new HotelContext("HotelContext"))
            {
                List<Booking> bookings = ctx.Bookings.Where(b =>
                    (start >= b.DateStart && start < b.DateEnd) ||
                    (end > b.DateStart && end < b.DateEnd)).ToList();

                foreach (Room room in rooms)
                {
                    bool confirmAdding = true;

                    foreach (Booking booking in bookings)
                    {
                        if (room.Numder == booking.RoomNumber)
                        {
                            confirmAdding = false;
                        }
                    }

                    if (confirmAdding)
                    {
                        freeRooms.Add(room);
                    }
                }
            }

            return freeRooms.Where(r => r.Availability == true).ToList();
        }

        // Select all bookings.
        public static List<Booking> GetBookings()
        {
            List<Booking> bookings = new List<Booking>();

            using (HotelContext ctx = new HotelContext("HotelContext"))
            {
                bookings = ctx.Bookings.Include(b => b.Room).Include(b => b.Room.ClassRoom).Include(b => b.User)
                    .Include(b => b.BookingStatus)
                    .ToList();
            }

            return bookings;
        }

        // Select user's bookings by user id.
        public static List<Booking> GetUserBookings(int userId)
        {
            List<Booking> bookings = GetBookings();
            return bookings.Where(b => b.UserId == userId).ToList();
        }

        // Select room classes.
        public static List<ClassRoom> GetRoomClasses()
        {
            List<ClassRoom> classRooms;
            using (var ctx = new HotelContext("HotelContext"))
            {
                classRooms = ctx.ClassRooms.ToList();
            }

            return classRooms;
        }

        // Select room by number.
        public static Room GetRoomByNumber(int number)
        {
            Room room;
            using (var ctx = new HotelContext("HotelContext"))
            {
                room = ctx.Rooms.FirstOrDefault(r => r.Numder == number);
            }

            return room;
        }

        // Create new room.
        public static void CreateRoom(int number, int classRoomId, int countOfPlaces)
        {
            Room room = new Room()
            {
                Numder = number,
                ClassRoomId = classRoomId,
                CountOfPlaces = countOfPlaces,
                Availability = false
            };

            using (var ctx = new HotelContext("HotelContext"))
            {
                ctx.Rooms.Add(room);
                ctx.SaveChanges();
            }
        }

        // Update room availability.
        public static void UpdateRoomAvailability(int number, bool availability)
        {
            try
            {
                using (var ctx = new HotelContext("HotelContext"))
                {
                    Room room = ctx.Rooms.FirstOrDefault(r => r.Numder == number);
                    room.Availability = availability;
                    ctx.Entry(room).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // Change booking status.
        private static void UpdateBookingStatus(int bookingId, int statusId)
        {
            using (var ctx = new HotelContext("HotelContext"))
            {
                Booking booking = ctx.Bookings.FirstOrDefault(b => b.Id == bookingId);
                booking.BookingStatusId = statusId;
                ctx.Entry(booking).State = EntityState.Modified;
                ctx.SaveChanges();
            }
        }

        // Change status in overdue bookings.
        public static void UpdateAllOverdueBookings()
        {
            List<Booking> currentBookings = CommunicationWithDataBase.GetBookings()
                .Where(b => b.BookingStatusId != 1 && b.DateEnd.AddDays(7) > DateTime.Now).ToList();

            foreach (Booking booking in currentBookings)
            {
                if (ComputationLogic.OverdueBookingPayment(booking))
                {
                    UpdateBookingStatus(booking.Id, 4);
                }
            }
        }

        // Blocking user.
        public static void ReportUser(int userId)
        {
            using (var ctx = new HotelContext("HotelContext"))
            {
                User user = GetUserById(userId);

                user.ViolatingPower++;

                switch (user.ViolatingPower)
                {
                    case 1:
                        DateTime block1 = DateTime.Now.AddMonths(1);
                        user.BlockDate = block1;
                        break;
                    case 2:
                        DateTime block2 = DateTime.Now.AddYears(1);
                        user.BlockDate = block2;
                        break;
                    case 3:
                        DateTime block3 = DateTime.Now.AddYears(100);
                        user.BlockDate = block3;
                        break;
                }

                ctx.Entry(user).State = EntityState.Modified;
                ctx.SaveChanges();
            }
        }

        // Change booking status on payed.
        public static void ConfirmPayment(int bookingId)
        {
            using (var ctx = new HotelContext("HotelContext"))
            {
                Booking booking = GetBookingById(bookingId);
                booking.BookingStatusId = 3;

                BookingStatus bookingStatus = GetBookingStatusById(3);
                booking.BookingStatus = bookingStatus;

                ctx.Entry(booking).State = EntityState.Modified;
                ctx.SaveChanges();
            }
        }

        // Delete booking.
        public static void DeleteBooking(int bookingId)
        {
            using (var ctx = new HotelContext("HotelContext"))
            {
                Booking booking = GetBookingById(bookingId);

                ctx.Entry(booking).State = EntityState.Deleted;
                ctx.SaveChanges();
            }
        }
    }
}