// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Metatrader4ClientApp.Modules.Trade
{
    using Metatrader4ClientApp.Infrastructure.Interfaces;
    using Metatrader4ClientApp.Infrastructure;
    using Prism.Commands;
    using Prism.Events;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Metatrader4ClientApp.Infrastructure.Models;
    using Microsoft.Win32;
    using TradingAPI.MT4Server;
    using System.Windows.Threading;
    using Metatrader4ClientApp.Infrastructure.Events;

    public class TradeViewModel : PluginBindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IExportService exportService;
        private readonly IConnectionParameterService connectionParameterService;
        private readonly IMarketFeedService marketFeedService;
        private ObservableCollection<TradeItemViewModel> tradeItems = new ObservableCollection<TradeItemViewModel>();
        private readonly Dictionary<string, Order> orderDic = new Dictionary<string, Order>();
        public TradeViewModel(IEventAggregator eventAggregator, IMarketFeedService marketFeedService, IConnectionParameterService connectionParameterService, IExportService exportService)
        {
            this.eventAggregator = eventAggregator;
            this.connectionParameterService = connectionParameterService;
            this.marketFeedService = marketFeedService;
            this.exportService = exportService;

            this.PackIcon = PackIconNames.Trade;
            this.Label = "TRADE";

            this.Command = new DelegateCommand(() =>
            {
                // this.PopulateTradeItems();
#if DEBUG
                this.MockDataTradeCopy();
#endif

                //this.ExportCommand.RaiseCanExecuteChanged();
            });


            this.eventAggregator.GetEvent<ConnectionParameterCreatedEvent>().Subscribe(this.OnConnectionParameterCreated, ThreadOption.UIThread);
        }

        private void OnConnectionParameterCreated(ConnectionParameter newConnectionParameter)
        {
            if (this.TradeItems.Any(i => i.ConnectionParameter.Equals(newConnectionParameter)))
            {
                return;
            }
            this.TradeItems.Add(new TradeItemViewModel(newConnectionParameter, this.eventAggregator, this.marketFeedService, this.exportService));
        }

        private void TradItemUpdated(IDictionary<string, ConnectionParameter> dict)
        {

        }
        private TradeItemViewModel CreateTradeItemViewModel(ConnectionParameter connectionParameter)
        {
            return new TradeItemViewModel(connectionParameter, this.eventAggregator, this.marketFeedService, this.exportService);
        }
        private async void PopulateTradeItems()
        {
            var cpList = await this.connectionParameterService.GetConnectionParametersAsync();
            var myTradeItems = new List<TradeItemViewModel>();
            foreach (var item in cpList)
            {
                myTradeItems.Add(this.CreateTradeItemViewModel(item));
            }
            this.TradeItems = new ObservableCollection<TradeItemViewModel>(myTradeItems);
        }
        private bool processing = false;
        /// <summary>
        /// http://mtapi.online/2017/12/21/list-of-opened-orders/
        /// </summary>
        private async void MockDataTradeCopy()
        {

            int count = 111;
            while (count > 0)
            {
                await Task.Delay(1000);
                count--;
                if (!this.processing) { 
                    this.processing = true;    
               
                await Task.Run(() =>
                {
                    var orders = Enumerable.Range(1, 1110).Select(item => new OrderViewModel(item, 10, 77));
                    var items = Enumerable.Range(1, 10).Select(ite => new TradeItemViewModel(" Title " + ite, orders));
                    this.TradeItems = new ObservableCollection<TradeItemViewModel>(items);

                });
                    this.processing = false;
                }
            }
        }








        public ObservableCollection<TradeItemViewModel> TradeItems
        {
            get => this.tradeItems;
            set => this.SetProperty(ref this.tradeItems, value);
        }

    }

}