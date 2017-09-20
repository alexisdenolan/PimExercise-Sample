using inRiver.Remoting;
using inRiver.Remoting.Objects;
using inRiver.Remoting.Security;
using System.Collections.Generic;

namespace TestInRiver
{
    public class PIM_UserService
    {
        private Permission _permission;
        private Role _role;
        private User _user;

        public PIM_UserService()
        {
            _permission = new Permission()
            {
                Description = "Sample permission1",
                Id = 28,
                Name = "SampPermission1"
            };
            _role = new Role()
            {
                Description = "Sample Role",
                Id = 4,
                Name = "SampRole",
                Permissions = new List<Permission> { _permission }
            };
            _user = new User()
            {
                Username = "alexis",
                Email = "pimuser11@inriver.com",
                FirstName = "Pimuser",
                LastName = "Eleven",
                AuthenticationType = AuthenticationType.Forms,
            };
        }

        public void Permission()
        {
            ////Add Permission
            //var permission = RemoteManager.UserService.AddPermission(_permission);
            ////Add Permission to Role
            //bool permisionRole = RemoteManager.UserService.AddPermissionToRole(_permission.Id, 2);
            //Restrict permission
            var restrictPermission = RemoteManager.UserService.GetRestrictedFieldPermission(1);
            if (restrictPermission == null)
                RemoteManager.UserService.AddRestrictedFieldPermission(new RestrictedFieldPermission()
                {
                    EntityTypeId = "Product",
                    FieldTypeId = "ProductId",
                    RestrictionType = "Readonly",
                    RoleId = RemoteManager.UserService.GetRoleByName("Reader").Id
                });


        }


        public void User()
        {
            _user = RemoteManager.UserService.AddUser(_user);
            RemoteManager.UserService.SetUserPassword(_user.Username, "alexis");
            Role reader = RemoteManager.UserService.GetRoleByName("Reader");
            RemoteManager.UserService.AddUserToRole(reader.Id, _user.Id);
        }

        public void Role()
        {

            var role = RemoteManager.UserService.GetRoleByName("Reader");
            if (role == null)
                RemoteManager.UserService.AddRole(_role);

        }

        public void Settings()
        {
            var userSetting = RemoteManager.UserService.GetUserSetting(_user.Username, "ModelLanguage");
            bool setUserSetting = RemoteManager.UserService.SetUserSetting(_user.Username, "ModelLanguage", "en");
        }
    }
}
