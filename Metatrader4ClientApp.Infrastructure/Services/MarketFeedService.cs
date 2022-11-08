

namespace Metatrader4ClientApp.Infrastructure.Services
{
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
    using TradingAPI.MT4Server;

    public class MarketFeedService : IMarketFeedService, IDisposable
    {
        private readonly IConnectionParameterService connectionParameterService;
        private readonly IEventAggregator eventAggregator;
        private readonly ISettingsService settingsService;
        private readonly Dictionary<string, decimal> _priceList = new Dictionary<string, decimal>();
        private readonly Dictionary<string, long> _volumeList = new Dictionary<string, long>();
        private readonly Dictionary<QuoteClient, ConnectionParameter> QuoteClientDic = new Dictionary<QuoteClient, ConnectionParameter>();
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
            this.LoadconnectionParameterAsync();


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
            // Try to connect first
            bool isConnectionSuccess = false;
            try
            {
                QuoteClient qc = new QuoteClient(connectionParameter.AccountNumber, connectionParameter.Password, connectionParameter.Host, connectionParameter.Port);
                qc.Connect();
                isConnectionSuccess = true;
                // qc.Disconnect();
                this.QuoteClientDic.Add(qc,connectionParameter );
                this.eventAggregator.GetEvent<ConnectionParameterCreatedEvent>().Publish(connectionParameter);

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
            // UpdatePrices();
            this.UpdateTradeList();
        }

        public decimal GetPrice(string tickerSymbol)
        {
            if (!SymbolExists(tickerSymbol))
            {
                throw new ArgumentException(nameof(tickerSymbol));
            }

            return _priceList[tickerSymbol];
        }

        public long GetVolume(string tickerSymbol)
        {
            return _volumeList[tickerSymbol];
        }

        public bool SymbolExists(string tickerSymbol)
        {
            return _priceList.ContainsKey(tickerSymbol);
        }

        private void UpdateTradeList() => this.OnTradeListUpdated();

        private void OnTradeListUpdated()
        {
            Dictionary<QuoteClient,ConnectionParameter> clonedQuoteClientDic;
            Dictionary<ConnectionParameter, Order[]> payLoad; 
            lock (this.lockObject)
            {
                clonedQuoteClientDic = new Dictionary<QuoteClient ,ConnectionParameter>(this.QuoteClientDic);
                payLoad= new Dictionary<ConnectionParameter, Order[]>();    
                foreach (var itemQc in clonedQuoteClientDic.Keys)
                {
                    var orders =  itemQc.GetOpenedOrders();
                    payLoad.Add(clonedQuoteClientDic[itemQc], orders);
                }
            }
            this.eventAggregator.GetEvent<TradeListUpdatedEvent>().Publish(payLoad);
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