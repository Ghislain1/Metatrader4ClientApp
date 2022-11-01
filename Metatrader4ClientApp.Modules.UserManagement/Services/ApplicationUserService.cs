using Metatrader4ClientApp.Infrastructure;
using Metatrader4ClientApp.Infrastructure.Interfaces;
using Metatrader4ClientApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metatrader4ClientApp.Modules.UserManagement.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        public IList<ApplicationUser> GetUsers()
        {
            var users= new List<ApplicationUser>();
            for (int i = 1; i < 10; i++)
            {
                users.Add(new ApplicationUser()
                {
                    Name ="User "+1,
                    Email="Email@" + i,
                    Password = HashManager.HashPassword($"{i}")
                });
            }
            return users;
        }

        public async Task<IList<ApplicationUser>> GetUsersAsync()
        {
           return await Task.Run(() =>GetUsers());    
        }
    }
}
