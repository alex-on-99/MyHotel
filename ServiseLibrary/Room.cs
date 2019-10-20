﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Numder { get; set; }
        public int CountOfPlaces { get; set; }
        public int ClassRoomId { get; set; }
        public bool Availability { get; set; }

        public virtual ClassRoom ClassRoom { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}