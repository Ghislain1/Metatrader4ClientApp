using Metatrader4ClientApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metatrader4ClientApp.Infrastructure.Interfaces
{
    public interface IAccountPositionService
    {
        event EventHandler<AccountPositionModelEventArgs> Updated;
        IList<AccountPosition> GetAccountPositions();
        Task<IList<AccountPosition>> GetAccountPositionsAsync();
    }
}
