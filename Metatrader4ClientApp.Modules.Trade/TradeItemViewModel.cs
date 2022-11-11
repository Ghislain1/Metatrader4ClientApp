// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Metatrader4ClientApp.Modules.Trade
{
    using Metatrader4ClientApp.Infrastructure;
    using Metatrader4ClientApp.Infrastructure.Events;
    using Metatrader4ClientApp.Infrastructure.Interfaces;
    using Metatrader4ClientApp.Infrastructure.Models;
    using Metatrader4ClientApp.Infrastructure.Services;
    using Microsoft.Win32;
    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Data;
    using TradingAPI.MT4Server;

    public class TradeItemViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IMarketFeedService marketFeedService;
        private readonly IExportService exportService;
        private ObservableCollection<OrderItemViewModel> orderItems = new();
        private bool isDataProcessing;
        private readonly object lockObject = new object();

     
        public TradeItemViewModel(TradeItem model, IEventAggregator eventAggregator, IMarketFeedService marketFeedService, IExportService exportService)
        {
            this.TradeItem = model;
            this.Title = model.AccountName;
            this.Id = model.Id;
            this.eventAggregator = eventAggregator;
            this.marketFeedService = marketFeedService;
            this.exportService = exportService;
            this.ExportCommand = new DelegateCommand(() => this.ExecuteExportAll(), () => this.OrderItems.Any());
            this.FetchDataCommand = new DelegateCommand(() => this.ExecuteFetchData(), () => true);
            this.eventAggregator.GetEvent<TradeItemUpdatedEvent>().Subscribe(this.OnTradeItemListUpdated);
            BindingOperations.EnableCollectionSynchronization(this.OrderItems, this.lockObject);
            eventAggregator.GetEvent<MarketPricesUpdatedEvent>().Subscribe(this.MarketPricesUpdated, ThreadOption.UIThread);

        }
        public void MarketPricesUpdated(IDictionary<int, double> tickerSymbolsPrice)
        {
            if (tickerSymbolsPrice == null)
            {
                throw new ArgumentNullException("tickerSymbolsPrice");
            }

            foreach (var position in this.OrderItems)
            {
                if (tickerSymbolsPrice.ContainsKey(position.Model.Ticket))
                {
                    position.OpenPrice = tickerSymbolsPrice[position.Model.Ticket];
                }
            }
        }
        private async void OnTradeItemListUpdated(TradeItem obj)
        {
            //if (!obj.ConnectionParameter.Equals(this.ConnectionParameter) || this.isDataProcessing)
            //{
            //    return;
            //}


            this.isDataProcessing = true;
            //  var newListOfOrders= await Task.Run(() => obj.Orders.Select(i => new OrderItemViewModel(i)));
            //  this.OrderItems = new ObservableCollection<OrderViewModel>(newListOfOrders);
            this.isDataProcessing = false;
        }


        public DelegateCommand ExportCommand { get; }
        public DelegateCommand FetchDataCommand { get; }
        public TradeItem TradeItem { get; }
        public ObservableCollection<OrderItemViewModel> OrderItems
        {
            get => this.orderItems;
            set => this.SetProperty(ref this.orderItems, value);
        }
        public string Header => $"({this.TradeItem.AccountName},{this.TradeItem.AccountProfit}, {this.TradeItem.AccountBalance})";

        public string Title { get; set; }
        public string Id { get; }
        private async void ExecuteFetchData()
        {
            // var sd= await this.marketFeedService.GetOrderListBy(ConnectionParameter);

        }
        private async void ExecuteExportAll()
        {

            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Exporting...",
                    FileName = $"Postion_{DateTime.Now.ToFileTime()}{AppConstants.TXT_EXT}",
                    FilterIndex = 1,
                    Filter = $"Txt Files (*{AppConstants.TXT_EXT})|*{AppConstants.TXT_EXT}",
                    InitialDirectory = KnownFolders.ExportedFolderUri.LocalPath

                };

                if (saveFileDialog.ShowDialog() is not true)
                {
                    return;

                }


                await this.exportService.ExportToTextFileAsync(this.OrderItems, saveFileDialog.FileName);

            }

            catch (Exception exception)

            {
                // TODO
                // Logger.Instance.Log(exception);

            }

        }
    }
}
