using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    // Class for business logic.
    public class ComputationLogic
    {
        // Calculate the price of the user's accommodation.
        public static int GetAccommodationPrice(double priceByDay, int countOfPeople, DateTime startDate,
            DateTime endDate)
        {
            double price = 0;
            double currentPriceByDay = priceByDay;


            int days = endDate.Date.Subtract(startDate.Date).Days;

            for (int i = 0; i < countOfPeople; i++)
            {
                price += currentPriceByDay;
                currentPriceByDay -= currentPriceByDay * 0.15;
            }

            return (int) price * days;
        }

        // Determine the deadline date of payment for accommodation.
        public static DateTime LastDate(DateTime bookingDate, DateTime startDate)
        {
            if (startDate.Subtract(bookingDate).Days < 2)
            {
                var tempDate = startDate.Add(new TimeSpan(14,0,0));
                return tempDate;
            }
            else
            {
                var tempDate = bookingDate.AddDays(2);
                return tempDate;
            }
        }

        // Definition of late payment.
        public static bool OverdueBookingPayment(Booking booking)
        {
            DateTime last = LastDate(booking.DateBooking, booking.DateStart);
            if (DateTime.Now > last&& booking.BookingStatusId == 2)
            {
                return true;
            }

            return false;
        }
    }
}