using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public class Booking
    {
        [Display(Name = "Id")]
        public int Id { get; set; }
        
        [Display(Name = "Дата приезда")]
        [DataType(DataType.Date)]
        public DateTime DateStart { get; set; }
        
        [Display(Name = "Дата отъезда")]
        [DataType(DataType.Date)]
        public DateTime DateEnd { get; set; }
        public DateTime DateBooking { get; set; }
        public int BookingStatusId { get; set; }
        public virtual BookingStatus BookingStatus { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Room")]
        public int? RoomNumber { get; set; }
        public virtual Room Room { get; set; }
        public int? RoomRequestId { get; set; }
        public virtual RoomRequest RoomRequest { get; set; }
    }
}