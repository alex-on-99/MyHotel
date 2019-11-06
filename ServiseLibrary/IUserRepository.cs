using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public interface IUserRepository
    {
        void CreateUser(string firstName, string secondName,
            string login, string email, string password, DateTime dateOfBirth);
        User GetUserById(int id);
        User GetUserByLogin(string login);
        User GetUserByEmail(string email);
        string GetUserRole(string login);
        bool CheckUserAuthorization(string login, string password);
    }
}
