using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public interface IRepository : IBookingRepository, IRequestRepository, IRoomRepository, IUserRepository
    {
        List<Room> GetFreeRooms(DateTime start, DateTime end);
        void UpdateAllOverdueBookings();
        void ConfirmPayment(int bookingId);
        void ReportUser(int userId);
    }
}
