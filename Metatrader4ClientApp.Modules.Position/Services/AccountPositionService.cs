

namespace Metatrader4ClientApp.Modules.Position.Services
{
    using Metatrader4ClientApp.Infrastructure.Interfaces;
    using Metatrader4ClientApp.Infrastructure.Models;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class AccountPositionService : IAccountPositionService
    {
        

        public AccountPositionService()
        {
         
        }

 

        public event EventHandler<AccountPositionModelEventArgs> Updated = delegate { };

        public IList<AccountPosition> GetAccountPositions()
        {
           
            return Enumerable.Range(1, 8).Select(ite => new AccountPosition() { CostBasis = decimal.Parse("1.5", CultureInfo.InvariantCulture), Shares = ite, TickerSymbol = "STOCK " + ite }).ToList(); ;
        }

        public async Task<IList<AccountPosition>> GetAccountPositionsAsync()
        {
            return await Task.Run(this.GetAccountPositions);
        }
 
        // TODO : Columnseparator in Setting page 
     

    }
}