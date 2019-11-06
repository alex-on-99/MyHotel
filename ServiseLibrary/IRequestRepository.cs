using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public interface IRequestRepository
    {
        void CreateRequest(string userLogin, int classRoomId, int countOfPlaces, DateTime start,
            DateTime end);
    }
}
