﻿using Metatrader4ClientApp.Infrastructure.Interfaces;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Metatrader4ClientApp.Infrastructure.Services
{
    public class MarketFeedService : IMarketFeedService, IDisposable
    {
        private IEventAggregator EventAggregator { get; set; }
        private readonly Dictionary<string, decimal> _priceList = new Dictionary<string, decimal>();
        private readonly Dictionary<string, long> _volumeList = new Dictionary<string, long>();
        static readonly Random randomGenerator = new Random(unchecked((int)DateTime.Now.Ticks));
        private Timer _timer;
        private int _refreshInterval = 10000;
        private readonly object _lockObject = new object();
        //private static string ResourceMarket = @"Data\Market.xml";

      

        public MarketFeedService( IEventAggregator eventAggregator)
        {
           

            EventAggregator = eventAggregator;
            _timer = new Timer(TimerTick);       
           
             this.RefreshInterval = CalculateRefreshIntervalMillisecondsFromSeconds(_refreshInterval);


            var itemElements = Enumerable.Range(1, 40).Select(item => ("STOCK " + item, Convert.ToInt64(item*randomGenerator.NextDouble()*100, CultureInfo.InvariantCulture) , Convert.ToInt64(item, CultureInfo.InvariantCulture)));
            foreach ((string tickerSymbol, long lastPrice, long volume)  in itemElements)
            {
               // string tickerSymbol = item.Attribute("TickerSymbol").Value;
               // decimal lastPrice = decimal.Parse(item.Attribute("LastPrice").Value, NumberStyles.Float, CultureInfo.InvariantCulture);
               // long volume = Convert.ToInt64(item.Attribute("Volume").Value, CultureInfo.InvariantCulture);
                _priceList.Add(tickerSymbol, lastPrice);
                _volumeList.Add(tickerSymbol, volume);
            }
        }

        public int RefreshInterval
        {
            get { return _refreshInterval; }
            set
            {
                _refreshInterval = value;
                _timer.Change(_refreshInterval, _refreshInterval);
            }
        }

        /// <summary>
        /// Callback for Timer
        /// </summary>
        /// <param name="state"></param>
        private void TimerTick(object state)
        {
            UpdatePrices();
        }

        public decimal GetPrice(string tickerSymbol)
        {
            if (!SymbolExists(tickerSymbol))
                throw new ArgumentException("Resources.MarketFeedTickerSymbolNotFoundException", "tickerSymbol");

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
            lock (_lockObject)
            {
                _priceList[tickerSymbol] = newPrice;
                _volumeList[tickerSymbol] = newVolume;
            }
            OnMarketPricesUpdated();
        }

        protected void UpdatePrices()
        {
            lock (_lockObject)
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
            Dictionary<string, decimal> clonedPriceList = null;
            lock (_lockObject)
            {
                clonedPriceList = new Dictionary<string, decimal>(_priceList);
            }
            EventAggregator.GetEvent<MarketPricesUpdatedEvent>().Publish(clonedPriceList);
        }

        private static int CalculateRefreshIntervalMillisecondsFromSeconds(int seconds)
        {
            return seconds * 1000;
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_timer != null)
                _timer.Dispose();
            _timer = null;
        }

        // Use C# destructor syntax for finalization code.
        ~MarketFeedService()
        {
            Dispose(false);
        }

        #endregion
    }
}