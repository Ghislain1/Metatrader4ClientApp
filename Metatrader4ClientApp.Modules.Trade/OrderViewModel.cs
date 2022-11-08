// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Metatrader4ClientApp.Modules.Trade
{
    using Prism.Mvvm;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class OrderViewModel: BindableBase
    {
    
        // Summary:
        //     Convertation rate from profit currency to group deposit currency for close time.
        public double RateClose { get; set; }
        //
        // Summary:
        //     Convertation rate from profit currency to group deposit currency for open time.
        public double RateOpen { get; set; }
        //
        // Summary:
        //     Net profit value (without swaps or commissions) in base currency.
        public double Profit { get; set; }
        //
        // Summary:
        //     Order comment.
        public string Comment { get; set; }
        //
        // Summary:
        //     Commission value.
        public double Commission { get; set; }
        //
        // Summary:
        //     Swap value.
        public double Swap { get; set; }
        //
        // Summary:
        //     Close price. Just for history orders.
        public double ClosePrice { get; set; }
        //
        // Summary:
        //     Identifying (magic) number.
        public int MagicNumber { get; set; }
        //
        // Summary:
        //     Stop loss.
        public double StopLoss { get; set; }
        //
        // Summary:
        //     Open price.
        public double OpenPrice { get; set; }
        //
        // Summary:
        //     Trading instrument.
        public string Symbol { get; set; }
        //
        // Summary:
        //     Amount of lots. Be carefull some brokers use non standart lots.
        public double Lots { get; set; }
        public OrderViewModel(TradingAPI.MT4Server.Order order)
        {
            this.StopLoss = order.StopLoss;
            this.MagicNumber = order.MagicNumber;
            this.OpenPrice = order.OpenPrice;
        }
    }
}
