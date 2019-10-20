using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public class RoomRequest
    {
        [Key] public int Id { get; set; }
        public int CountOfPlaces { get; set; }
        public int ClassRoomId { get; set; }
        public virtual ClassRoom ClassRoom { get; set; }
    }
}
