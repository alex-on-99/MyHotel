using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using ServiceLibrary;

namespace HotelEpam.Providers
{
    public class HotelRoleProvider : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            IRepository repository = new CommunicationWithDataBase();

            string userRole = repository.GetUserRole(username);
            string[] roles = new string[]{};
            
            if (userRole != null)
            {
                roles = new string[] { userRole };
            }

            return roles;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            string[] userRoles = GetRolesForUser(username);

            if (Array.IndexOf(userRoles,roleName) != -1)
            {
                return true;
            }

            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}