// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Metatrader4ClientApp.Infrastructure.Services
{
    using Metatrader4ClientApp.Infrastructure.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using TradingAPI.MT4Server;

    public static class MockDataDebugProvider
    {
        public static async Task<IEnumerable<OrderItem>> GetOrderListAsync(string parentId, int length = 5)
        {
            return await Task.Run(() => GetOrderList(parentId, length));

        }

        private static IEnumerable<OrderItem> GetOrderList(string parentId, int length = 5)
        {
            for (int i = 0; i < length; i++)
            {
                yield return new OrderItem() { ParentId = parentId };
            }
        }
        public static async Task<TradeItem> GetTradeItemAsync(string accountName)
        {
            return await Task.Run(() => new TradeItem(accountName));

        }

        public static TradeItem GetTradeItem(string accountName)
        {
            var ordersList = new List<OrderItem>();
            for (int i = 0; i < 150; i++)
            {
                ordersList.Add(new OrderItem());
            }
            var result = new TradeItem(accountName,
              10, 20,
             40, 15, AccountType.Demo, ordersList.ToArray());
            return result;

        }
    }
}
