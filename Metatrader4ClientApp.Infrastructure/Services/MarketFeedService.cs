

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
        private readonly Dictionary<string, ConnectionParameter> QuoteClientDic = new Dictionary<string, ConnectionParameter>();
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
            var itemElements = Enumerable.Range(1, 40).Select(item => ("STOCK " + item, Convert.ToInt64(item * randomGenerator.NextDouble() * 100, CultureInfo.InvariantCulture), Convert.ToInt64(item, CultureInfo.InvariantCulture)));
            foreach ((string tickerSymbol, long lastPrice, long volume) in itemElements)
            {
                // string tickerSymbol = item.Attribute("TickerSymbol").Value;
                // decimal lastPrice = decimal.Parse(item.Attribute("LastPrice").Value, NumberStyles.Float, CultureInfo.InvariantCulture);
                // long volume = Convert.ToInt64(item.Attribute("Volume").Value, CultureInfo.InvariantCulture);
                _priceList.Add(tickerSymbol, lastPrice);
                _volumeList.Add(tickerSymbol, volume);
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
                this.QuoteClientDic.Add(Guid.NewGuid().ToString(), connectionParameter);
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
            UpdatePrices();
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

        protected void UpdatePrice(string tickerSymbol, decimal newPrice, long newVolume)
        {
            lock (this.lockObject)
            {
                _priceList[tickerSymbol] = newPrice;
                _volumeList[tickerSymbol] = newVolume;
            }
            OnMarketPricesUpdated();
        }

        protected void UpdatePrices()
        {
            lock (this.lockObject)
            {
                foreach (string symbol in _priceList.Keys.ToArray())
                {
                    decimal newValue = _priceList[symbol];
                    newValue += Convert.ToDecimal(randomGenerator.NextDouble() * 10f) - 5m;
                    _priceList[symbol] = newValue > 0 ? newValue : 0.1m;
                }

            }
            OnMarketPricesUpdated();
        }

        private void OnMarketPricesUpdated()
        {
            Dictionary<string, decimal> clonedPriceList;
            lock (this.lockObject)
            {
                clonedPriceList = new Dictionary<string, decimal>(_priceList);
            }
            this.eventAggregator.GetEvent<MarketPricesUpdatedEvent>().Publish(clonedPriceList);
        }

        private void OnQuoteClientUpdated()
        {
            Dictionary<string, ConnectionParameter> clonedQuoteClientDic;
            lock (this.lockObject)
            {
                clonedQuoteClientDic = new Dictionary<string, ConnectionParameter>(this.QuoteClientDic);
            }
            this.eventAggregator.GetEvent<TradItemUpdatedEvent>().Publish(clonedQuoteClientDic);
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