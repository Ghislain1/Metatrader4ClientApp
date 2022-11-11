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
    using Metatrader4ClientApp.Infrastructure.Services;
    using System.Windows.Data;
    using System.Diagnostics;

    public class TradeViewModel : PluginBindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IExportService exportService;
        private readonly IConnectionParameterService connectionParameterService;
        private readonly IMarketFeedService marketFeedService;
        private readonly object lockObject = new object();
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
            BindingOperations.EnableCollectionSynchronization(this.TradeItems, this.lockObject);

            this.Command = new DelegateCommand(() =>
            {
                // this.PopulateTradeItems();
#if DEBUG
               // this.DoMockDataTrade();
              //  this.MockDataOrders();
#endif

                //this.ExportCommand.RaiseCanExecuteChanged();
            });


            this.eventAggregator.GetEvent<TradeItemCreatedEvent>().Subscribe(this.OnTradeItemCreatedEvent);
        }

        private void OnTradeItemCreatedEvent(TradeItem newTradeItem)
        {
            if (this.TradeItems.Any(i => i.Equals(newTradeItem)))
            {
                return;
            }
            var newTradeItemViewModel = new TradeItemViewModel(newTradeItem, this.eventAggregator, this.marketFeedService, this.exportService);
            newTradeItem.Orders.ToList().ForEach(el=> newTradeItemViewModel.OrderItems.Add(new OrderItemViewModel(el)));
            this.TradeItems.Add(newTradeItemViewModel);
        }

        private void TradItemUpdated(IDictionary<string, ConnectionParameter> dict)
        {

        }
       
 

        public ObservableCollection<TradeItemViewModel> TradeItems
        {
            get => this.tradeItems;
            set => this.SetProperty(ref this.tradeItems, value);
        }
        private TradeItemViewModel selectedTradeItem;
        public TradeItemViewModel SelectedTradeItem
        {
            get => this.selectedTradeItem;
            set => this.SetProperty(ref this.selectedTradeItem, value);
        }



    }

}