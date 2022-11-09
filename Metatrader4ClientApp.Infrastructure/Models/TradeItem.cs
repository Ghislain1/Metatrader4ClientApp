

namespace Metatrader4ClientApp.Infrastructure.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TradingAPI.MT4Server;
    public class TradeItem
    {
        public event EventHandler<AccountPositionEventArgs> Updated = delegate { };
        public TradeItem(string accountName,
            double accountProfit, double countBalance,
           double accountCredit, double accountEquity, AccountType accountType, Order[] orders, ConnectionParameter connectionParameter)
        {
            this.AccountName = accountName;
            this.AccountProfit = accountProfit;
            this.CountBalance = countBalance;
            this.AccountCredit = accountCredit;
            this.AccountEquity = accountEquity;
            this.AccountType = accountType;
            this.Orders = orders;
            this.ConnectionParameter = connectionParameter;
        }

        public ConnectionParameter ConnectionParameter { get; }
        public string AccountName { get; }
        public AccountType AccountType { get; }
        public double AccountFreeMargin { get; }
        public Order[] Orders { get; }

        //
        // Summary:
        //     Account margin.
        public double AccountMargin { get; }
        //
        // Summary:
        //     Account equity.
        public double AccountEquity { get; }
        //
        // Summary:
        //     Account profit.
        public double AccountProfit { get; }
        //
        // Summary:
        //     Account credit.
        public double AccountCredit { get; }

        public double CountBalance { get; }


        //
        // Summary:
        //     Account balance.
        public double AccountBalance { get; }
        private string _tickerSymbol;
        public string TickerSymbol
        {
            get
            {
                return _tickerSymbol;
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                if (!value.Equals(_tickerSymbol))
                {
                    _tickerSymbol = value;
                    Updated(this, new AccountPositionEventArgs());
                }
            }
        }


        private decimal _costBasis;

        public decimal CostBasis
        {
            get
            {
                return _costBasis;
            }
            set
            {
                if (!value.Equals(_costBasis))
                {
                    _costBasis = value;
                    Updated(this, new AccountPositionEventArgs());
                }
            }
        }


        private long _shares;

        public long Shares
        {
            get
            {
                return _shares;
            }
            set
            {
                if (!value.Equals(_shares))
                {
                    _shares = value;
                    Updated(this, new AccountPositionEventArgs());
                }
            }
        }
    }
}