﻿

namespace Metatrader4ClientApp.Infrastructure.Interfaces
{
    using Metatrader4ClientApp.Infrastructure.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public interface IMarketFeedService
    {
        double GetPrice(int tickerSymbol);
        long GetVolume(int tickerSymbol);
        bool SymbolExists(int tickerSymbol);
        string ErrorMessage { get; set; }
        bool CheckConnectionParameter(ConnectionParameter connectionParameter);
        Task<bool> CheckConnectionParameterAsync(ConnectionParameter connectionParameter);
    }
}
