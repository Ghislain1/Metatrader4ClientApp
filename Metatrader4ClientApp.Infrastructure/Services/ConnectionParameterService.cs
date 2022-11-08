

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

       public string ErrorMessage { get; set; }
        public bool CheckConnectionParameter(ConnectionParameter connectionParameter)
        {
            // Try to connect first
            bool isConnectionSuccess = false;
            try
            {
                QuoteClient qc = new QuoteClient(connectionParameter.AccountNumber, connectionParameter.Password, connectionParameter.Host, connectionParameter.Port);
                qc.Connect();
                isConnectionSuccess = true;
                qc.Disconnect();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                isConnectionSuccess = false;
                this.ErrorMessage = ex.Message;

            }
            var alres= this.GetConnectionParameters().Any(user => this.IsSame(user, connectionParameter) );
            // this.GetConnectionParameters().Any(user => user.AccountNumber.Equals(loginName) && HashManager.VerifyPassword(password, user.Password));
            return isConnectionSuccess;
        }

        private bool IsSame(ConnectionParameter user, ConnectionParameter connectionParameter)
        {
           return user.AccountNumber.Equals(connectionParameter.AccountNumber)
                && HashManager.VerifyPassword(connectionParameter?.Password, user.Password)
                && user.Host.Equals(connectionParameter.Host)
                && user.Port.Equals(connectionParameter.Port);
        }

        public async Task<bool> CheckConnectionParameterAsync(ConnectionParameter connectionParameter)
        {
            return await Task.Run(() => this.CheckConnectionParameter(connectionParameter)); 
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
