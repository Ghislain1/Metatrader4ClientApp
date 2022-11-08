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
        public bool CheckConnectionParameter(string loginName, string password)
        {

            return this.GetConnectionParameters().Any(user => user.AccountNumber.Equals(loginName)&& HashManager.VerifyPassword(password, user.Password));
        }

        private IList<ConnectionParameter> GetConnectionParameters() => ConnectionParameterRepository.LoadAll();






        public bool StoreConnectionParameter(string loginName, string password)
        {
            throw new NotImplementedException();
        }

        public bool LogIn(string loginName, string password)
        {
            throw new NotImplementedException();
        }

      


        public async Task<IList<ConnectionParameter>> GetConnectionParametersAsync()
        {
           return await Task.Run(() => GetConnectionParameters());    
        }

      

        public bool StoreConnectionParameter(int accountNumber, short port,string host, string password)
        {
            var hashedPasword = HashManager.HashPassword(password);
            ConnectionParameterRepository.Save(new ConnectionParameter() { AccountNumber = accountNumber, Password = hashedPasword, Host=host, Port= port });
            return true;
        }
    }
}
