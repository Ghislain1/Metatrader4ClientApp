

namespace Metatrader4ClientApp.Infrastructure.Interfaces
{
    using Metatrader4ClientApp.Infrastructure.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IConnectionParameterService
    {     
       
        Task<IList<ConnectionParameter>> GetConnectionParametersAsync();
        bool StoreConnectionParameter(string loginName, string password);
        bool LogIn(string loginName, string password);
        string ErrorMessage { get; set; }
        bool CheckConnectionParameter(ConnectionParameter connectionParameter);
        Task<bool> CheckConnectionParameterAsync(ConnectionParameter connectionParameter);
    }
}
