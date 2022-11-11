﻿

namespace Metatrader4ClientApp.Infrastructure.Services
{
    using Metatrader4ClientApp.Infrastructure.Events;
    using Metatrader4ClientApp.Infrastructure.Interfaces;
    using Metatrader4ClientApp.Infrastructure.Models;
    using Prism.Events;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using TradingAPI.MT4Server;

    public class MarketFeedService : IMarketFeedService, IDisposable
    {
        private readonly IConnectionParameterService connectionParameterService;
        private readonly IEventAggregator eventAggregator;
        private readonly ISettingsService settingsService;
        private readonly Dictionary<int, double> priceList = new Dictionary<int, double>();
        private readonly Dictionary<int, long> _volumeList = new Dictionary<int, long>();
        private readonly Dictionary<QuoteClient, TradeItem> QuoteClientDic = new Dictionary<QuoteClient, TradeItem>();
        static readonly Random randomGenerator = new Random(unchecked((int)DateTime.Now.Ticks));
        private readonly Timer timer;
        private int _refreshInterval = 1000;
        private readonly object lockObject = new object();
        public MarketFeedService(IConnectionParameterService connectionParameterService, IEventAggregator eventAggregator, ISettingsService settingsService)
        {
            this.connectionParameterService = connectionParameterService;
            this.eventAggregator = eventAggregator;
            this.settingsService = settingsService;
            this.timer = new Timer(this.TimerTick);
            this.RefreshInterval = this.settingsService.Get().RefreshIntervalInMilliSeconde;
            // this.LoadconnectionParameterAsync();


        }

        private async void LoadconnectionParameterAsync()
        {
            var cpList = await this.connectionParameterService.GetConnectionParametersAsync();
            foreach (var connectionParameter in cpList)
            {
                await this.CheckConnectionParameterAsync(connectionParameter);
            }
        }

        public async Task<bool> CheckConnectionParameterAsync(ConnectionParameter connectionParameter)
        {
            return await Task.Run(() => this.CheckConnectionParameter(connectionParameter));
        }
        
        public string ErrorMessage { get; set; }
        public bool CheckConnectionParameter(ConnectionParameter connectionParameter)
        {
#if DEBUG
            //var ds = this.connectionParameterService.GetConnectionParametersAsync().Result.Count;
            //var i1= MockDataDebugProvider.GetTradeItem("TradeItem Nr "+ ds);
            //i1.Orders= MockDataDebugProvider.GetOrderListAsync(i1.Id).Result.ToArray();
            //this.eventAggregator.GetEvent<TradeItemCreatedEvent>().Publish(i1);
            //return true;
#endif
            // Try to connect first
            bool isConnectionSuccess = false;
            try
            {
                QuoteClient quoteClient = new QuoteClient(connectionParameter.AccountNumber, connectionParameter.Password, connectionParameter.Host, connectionParameter.Port);
                quoteClient.Connect();
                isConnectionSuccess = true;
                var orders = quoteClient.GetOpenedOrders();
                var tradeItem = this.CreateTradeItem(quoteClient, orders);                
                this.QuoteClientDic.Add(quoteClient, tradeItem);
                foreach (var item in orders)
                {
                    
                    long volume = 1;
                    this.priceList.Add(item.Ticket, item.OpenPrice);
                    //_volumeList.Add(tickerSymbol, volume);
                }
                this.eventAggregator.GetEvent<TradeItemCreatedEvent>().Publish(tradeItem);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                isConnectionSuccess = false;
                this.ErrorMessage = ex.Message;

            }
            finally
            {
                if (isConnectionSuccess)
                {
                    this.ErrorMessage = "Login success, go to Trade tab to see the desired Order!..";
                }
            }

            return isConnectionSuccess;
        }

        public int RefreshInterval
        {
            get { return _refreshInterval; }
            set
            {
                _refreshInterval = value;
                this.timer.Change(_refreshInterval, _refreshInterval);
            }
        }

        /// <summary>
        /// Callback for Timer
        /// </summary>
        /// <param name="state"></param>
        private void TimerTick(object? state)
        {
            this. UpdatePrices();
            
        }
        protected void UpdatePrices()
        {
            lock (this.lockObject)
            {
               // var qc = QuoteClientDic.Keys.ToArray()[0];
               // var de= QuoteClientDic[qc];
               //var order= qc.GetOpenedOrder(de.Orders[0].Ticket);
                foreach (string symbol in this.priceList.Keys.ToArray())
                {

                    var newValue = this.priceList[symbol];
                  //  newValue += Convert.ToDecimal(randomGenerator.NextDouble() * 10f) - 5m;
                    this.priceList[symbol] = newValue > 0 ? newValue : 0.1D;
                }
            }
            this.OnMarketPricesUpdated();
        }

        private void OnMarketPricesUpdated()
        {
            Dictionary<string, double> clonedPriceList = null;
            lock (this.lockObject)
            {
                 clonedPriceList = new Dictionary<string, double>(this.priceList);
            }
            this.eventAggregator.GetEvent<MarketPricesUpdatedEvent>().Publish(clonedPriceList);
        }
        public double GetPrice(string tickerSymbol)
        {
            if (!SymbolExists(tickerSymbol))
            {
                throw new ArgumentException(nameof(tickerSymbol));
            }

            return this.priceList[tickerSymbol];
        }

        public long GetVolume(string tickerSymbol)
        {
            return _volumeList[tickerSymbol];
        }

        public bool SymbolExists(string tickerSymbol)
        {
            return this.priceList.ContainsKey(tickerSymbol);
        }

        private void UpdateTradeList() => this.OnTradeListUpdated();

        private void OnTradeListUpdated()
        {
            Dictionary<QuoteClient, TradeItem> clonedQuoteClientDic;
            Dictionary<ConnectionParameter, Order[]> payLoad; 
            lock (this.lockObject)
            {
                clonedQuoteClientDic = new Dictionary<QuoteClient ,TradeItem>(this.QuoteClientDic);              
              
            }
            
        }
        private TradeItem CreateTradeItem(QuoteClient quoteClient, Order[] orders)
        {
            var orderItems = new List<OrderItem>();
            foreach (var item in orders)
            {
                orderItems.Add( new OrderItem(item.Ticket, item.Profit, item.OpenPrice, item.Symbol));
            }
            var result= new TradeItem(quoteClient.AccountName, quoteClient.AccountProfit, quoteClient.AccountBalance, quoteClient.AccountCredit, quoteClient.AccountEquity, quoteClient.AccountType, orderItems.ToArray());
            orderItems.ForEach(item => item.ParentId = result.Id);
            return result;
        }     



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            if (this.timer != null)
            {
                this.timer.Dispose();
                foreach (var quoteClient in this.QuoteClientDic.Keys)
                {
                    quoteClient?.Disconnect();

                }
            }

            // this.timer = null;
        }

        // Use C# destructor syntax for finalization code.
        ~MarketFeedService()
        {
            Dispose(false);
        }


    }
}