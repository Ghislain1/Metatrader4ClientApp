// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Metatrader4ClientApp.Infrastructure.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TradingAPI.MT4Server;

    public class OrderItem
    {
        public string Id { get; }
        public string ?ParentId { get; set; }
        public OrderItem(int ticket=1, double profit=0, double openPrice=0, string symbol="")
        {
            this.Id =Guid.NewGuid().ToString();
            this.Profit = profit;
            this.Ticket = ticket;
            this.OpenPrice = openPrice;
            this.Symbol = symbol;
        }

        public int Ticket { get; }
        //
        // Summary:
        //     Extended order information.
        public TradeRecord Ex { get; }
    //
    // Summary:
    //     Rate of convertation from margin currency to deposit one.
    public double RateMargin { get; }
    //
    // Summary:
    //     Convertation rate from profit currency to group deposit currency for close time.
    public double RateClose;
        //
        // Summary:
        //     Convertation rate from profit currency to group deposit currency for open time.
        public double RateOpen;
        //
        // Summary:
        //     Net profit value (without swaps or commissions) in base currency.
        public double Profit;
        //
        // Summary:
        //     Order comment.
        public string Comment;
        //
        // Summary:
        //     Commission value.
        public double Commission;
        //
        // Summary:
        //     Swap value.
        public double Swap;
        //
        // Summary:
        //     Close price. Just for history orders.
        public double ClosePrice;
        //
        // Summary:
        //     Identifying (magic) number.
        public int MagicNumber { get; }
        //
        // Summary:
        //     Stop loss.
        public double StopLoss { get; }
        //
        // Summary:
        //     Open price.
        public double OpenPrice { get; }
        //
        // Summary:
        //     Trading instrument.
        public string Symbol { get; }
        //
        // Summary:
        //     Amount of lots. Be carefull some brokers use non standart lots.
        public double Lots;
        //
        // Summary:
        //     Order type.
        public Op Type;
        //
        // Summary:
        //     Expiration time of pending order.
        public DateTime Expiration;
        //
        // Summary:
        //     Close time. Just for history orders.
        public DateTime CloseTime;
        //
        // Summary:
        //     Open time.
        public DateTime OpenTime;
        //
        // Summary:
        //     Take profit.
        public double TakeProfit { get; }



        //
        // Summary:
        //     Placed manually or by expert
        public PlacedType PlacedType { get; }
    }
}
