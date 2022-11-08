

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
        bool StoreConnectionParameter(int accountNumber, string password, string host, int port);

        bool StoreConnectionParameter(ConnectionParameter connectionParameter);
        

    }
}
