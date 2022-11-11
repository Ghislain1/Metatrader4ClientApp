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
       

        //private async void DoMockDataTrade()
        //{

        //    int count = 111;
        //    this.TradeItems.Clear();
        //    while (count > 0)
        //    {
        //        if (this.processing)
        //        {
        //            continue;
        //        }
        //        this.processing = true;
        //        Debug.WriteLine("  =================================> " + this.processing);
        //        var newTradeItem = await this.MockDataTrade(count);
        //        this.TradeItems.Add(newTradeItem.Item1);
        //        if (this.SelectedTradeItem is null)
        //        {
        //            this.SelectedTradeItem = newTradeItem.Item1;
        //        }
        //        newTradeItem.Item1.OrderItems = new ObservableCollection<OrderItemViewModel>(newTradeItem.Item2);
        //        this.processing = false;
        //        count--;
        //    }
        //}
        private async void PopulateTradeItems()
        {
            var cpList = await this.connectionParameterService.GetConnectionParametersAsync();
            var myTradeItems = new List<TradeItemViewModel>();
            foreach (var item in cpList)
            {
               // myTradeItems.Add(this.CreateTradeItemViewModel(item));
            }
            this.TradeItems = new ObservableCollection<TradeItemViewModel>(myTradeItems);
        }
        private bool processing = false;
        /// <summary>
        /// http://mtapi.online/2017/12/21/list-of-opened-orders/
        /// </summary>
     
        private async void MockDataOrders()
        {
            if (this.SelectedTradeItem is null)
            {
                return;
            }

            int count = 111;
            this.TradeItems.Clear();
            while (count > 0)
            {
                await Task.Delay(1000);
                if (this.processing)
                {
                    continue;
                }
                count--;
                this.processing = true;


                this.processing = false;
            }

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