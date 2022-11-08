

namespace Metatrader4ClientApp.Infrastructure.Services
{
    using Metatrader4ClientApp.Infrastructure;
    using Metatrader4ClientApp.Infrastructure.Interfaces;
    using Metatrader4ClientApp.Infrastructure.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TradingAPI.MT4Server;

    public class ConnectionParameterService : IConnectionParameterService
    {

        public ConnectionParameterService()
        {
         
        }
      

        /// <summary>
        /// TODO : Not work fine
        /// </summary>
        /// <param name="enteredConnectionParameter"></param>
        /// <param name="savedConnectionParameter"></param>
        /// <returns></returns>
        private bool IsSame(ConnectionParameter enteredConnectionParameter, ConnectionParameter savedConnectionParameter)
        {
           return enteredConnectionParameter.AccountNumber.Equals(savedConnectionParameter.AccountNumber)
                && HashManager.VerifyPassword(enteredConnectionParameter?.Password, savedConnectionParameter?.Password)
                && enteredConnectionParameter.Host.Equals(savedConnectionParameter.Host)
                && enteredConnectionParameter.Port.Equals(savedConnectionParameter.Port);
        }

       
        private IList<ConnectionParameter> GetConnectionParameters() => ConnectionParameterRepository.LoadAll();     
 
      


        public async Task<IList<ConnectionParameter>> GetConnectionParametersAsync()
        {
           return await Task.Run(() => GetConnectionParameters());    
        }

   

       public bool StoreConnectionParameter(int accountNumber,  string password,  string host, int   port)
        {
            var hashedPasword = HashManager.HashPassword(password);
            ConnectionParameterRepository.Save(new ConnectionParameter() { AccountNumber = accountNumber, Password = hashedPasword, Host=host, Port= port });
            return true;
        }

        public bool StoreConnectionParameter(ConnectionParameter connectionParameter)
        {
            var allreadyStored = this.GetConnectionParameters().Any(savedUser => this.IsSame(connectionParameter, savedUser));
            if (allreadyStored)
            {
                return true;
            }
           return this.StoreConnectionParameter(connectionParameter.AccountNumber, connectionParameter.Password, connectionParameter.Host, connectionParameter.Port);

        }
    }
}
