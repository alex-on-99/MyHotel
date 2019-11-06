using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceLibrary.Tests
{
    [TestClass]
    public class ComputationLogicTests
    {
        private static List<Booking> bookings;

        [ClassInitialize]
        public static void TestInitializer(TestContext testContext)
        {
            List<Booking> bookingsTest = new List<Booking>();

            Booking booking1 = new Booking()
            {
                DateBooking = new DateTime(2019, 10, 10, 10, 30, 0),
                DateStart = new DateTime(2019, 10, 11)
            };

            Booking booking2 = new Booking()
            {
                DateBooking = new DateTime(2019, 10, 10, 10, 30, 0),
                DateStart = new DateTime(2019, 10, 20)
            };

            bookingsTest.Add(booking1);
            bookingsTest.Add(booking2);

            bookings = bookingsTest;
        }

        [TestMethod]
        public void GetAccommodationPrice_AccommodationData_PriceReturned()
        {
            double priceByDay = 500;
            int countOfPeople = 3;
            DateTime start = new DateTime(2000, 12, 12);
            DateTime end = new DateTime(2000, 12, 15);
            int expected = 3858;

            int actual = ComputationLogic.GetAccommodationPrice(priceByDay, countOfPeople, start, end);

            Assert.AreEqual(expected, actual, $"Expected: {expected}, Actual {actual}", expected, actual);
        }

        [TestMethod]
        public void LastDate_BookingDates_LastPaymentDayReturned()
        {
            DateTime expected1 = new DateTime(2019, 10, 11, 14, 00, 0);
            DateTime expected2 = new DateTime(2019, 10, 12, 10, 30, 0);

            DateTime actual1 = ComputationLogic.LastDate(bookings[0].DateBooking, bookings[0].DateStart);
            DateTime actual2 = ComputationLogic.LastDate(bookings[1].DateBooking, bookings[1].DateStart);

            Assert.AreEqual(expected1, actual1, $"Expected1: {expected1}, Actual1: {actual1}", expected1, actual1);
            Assert.AreEqual(expected2, actual2, $"Expected2: {expected2}, Actual2: {actual2}", expected2, actual2);
        }
    }
}
