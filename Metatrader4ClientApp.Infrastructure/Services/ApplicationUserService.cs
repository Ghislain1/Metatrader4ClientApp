using Metatrader4ClientApp.Infrastructure;
using Metatrader4ClientApp.Infrastructure.Interfaces;
using Metatrader4ClientApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metatrader4ClientApp.Infrastructure.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        public IList<ApplicationUser> GetUsers()
        {
            //var users= new List<ApplicationUser>();
            //for (int i = 1; i < 10; i++)
            //{
            //    users.Add(new ApplicationUser()
            //    {
            //        Name ="User "+1,
            //        Email="Email@" + i,
            //        Password = HashManager.HashPassword($"{i}")
            //    });
            //}
            return ApplicationUserRepository.LoadAll();
        }

        public async Task<IList<ApplicationUser>> GetUsersAsync()
        {
           return await Task.Run(() =>GetUsers());    
        }

        public bool LogIn(string loginName, string password)
        {
           var hashedPasword = HashManager.HashPassword(password);
            ApplicationUserRepository.Save(new ApplicationUser() {  Name = loginName, Password= hashedPasword} );
            return true;
        }
    }
}
