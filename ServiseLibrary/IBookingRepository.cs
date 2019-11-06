using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public interface IBookingRepository
    {
        void CreateBooking(string userLogin, int roomNumber, DateTime start,
            DateTime end);
        Booking GetBookingById(int id);
        BookingStatus GetBookingStatusById(int id);
        List<Booking> GetBookingRequests();
        void UpdateRoomInBooking(int bookingId, int roomNumber);
        List<Booking> GetBookings();
        List<Booking> GetUserBookings(int userId);
        void DeleteBooking(int bookingId);
    }
}
