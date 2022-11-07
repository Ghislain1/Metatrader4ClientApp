

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
    public class MarketFeedService : IMarketFeedService, IDisposable
    {
        private readonly IEventAggregator eventAggregator;
        private readonly ISettingsService settingsService;
        private readonly Dictionary<string, decimal> _priceList = new Dictionary<string, decimal>();
        private readonly Dictionary<string, long> _volumeList = new Dictionary<string, long>();
        static readonly Random randomGenerator = new Random(unchecked((int)DateTime.Now.Ticks));
        private readonly Timer timer;
        private int _refreshInterval = 1000;
        private readonly object lockObject = new object();
        public MarketFeedService(IEventAggregator eventAggregator, ISettingsService settingsService)
        {
            this.eventAggregator = eventAggregator;
           this. settingsService = settingsService;
            this.timer = new Timer(this.TimerTick);

            this.RefreshInterval = this.settingsService.Get().RefreshIntervalInMilliSeconde;

            // TODO: FAKE STOCK
            //ClrWrapper metatrader = new ClrWrapper();

        
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
        private async Task<IEnumerable<AccountPosition>> GetQuote()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = await client.GetAsync("https://type.fit/api/quotes");  // Perform a GET call against your endpoint asynchronously
                if (result.IsSuccessStatusCode)     // Check that the request returned successfully before we proceed
                {
                    var quoteListString = await result.Content.ReadAsStringAsync();     // Your endpoint returns text/plain, not JSON, so we'll grab the string...
                    var quoteList = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<AccountPosition>>(quoteListString);     // ... and can use Newtonsoft (or System.Text.Json) to deserialize it into a list we can manipulate.
                    return quoteList;  // Now we have an enumeration of quotes, so we can return a random element somewhere between index 0 and the count of entries minus 1
                }
                else
                {
                    return null;  // But if the call didn't find anything, return a 404 instead
                }
            }
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
                // Samples
                this.GetQuote();
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