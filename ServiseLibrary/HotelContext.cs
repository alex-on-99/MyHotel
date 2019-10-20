using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public class HotelContext : DbContext
    {
        public HotelContext(string nameOrConnectionString) 
            : base(nameOrConnectionString) 
        { }
        public IDbSet<Role> Roles { get; set; }
        public IDbSet<User> Users { get; set; }
        public IDbSet<Booking> Bookings { get; set; }
        public IDbSet<Room> Rooms { get; set; }
        public IDbSet<ClassRoom> ClassRooms { get; set; }
        public IDbSet<BookingStatus> BookingStatuses { get; set; }
        public IDbSet<RoomRequest> RoomRequests { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>()
                .HasRequired(r => r.ClassRoom)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RoomRequest>()
                .HasRequired(s => s.ClassRoom)
                .WithMany()
                .WillCascadeOnDelete(false);
        }

    }
}
