using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public interface IRoomRepository
    {
        List<Room> GetAllRooms();
        List<ClassRoom> GetRoomClasses();
        Room GetRoomByNumber(int number);
        void CreateRoom(int number, int classRoomId, int countOfPlaces);
        void UpdateRoomAvailability(int number, bool availability);
    }
}
