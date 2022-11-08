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
    using TradingAPI.MT4Server;

    public class TradeItemViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IMarketFeedService marketFeedService;
        private readonly IExportService exportService;
        private ObservableCollection<OrderViewModel> orderItems = new();
       
        public TradeItemViewModel(ConnectionParameter model, IEventAggregator eventAggregator, IMarketFeedService marketFeedService, IExportService exportService)
        {
            this.ConnectionParameter = model;
            this.eventAggregator = eventAggregator;
            this.marketFeedService = marketFeedService;
            this.exportService = exportService;
            this.ExportCommand = new DelegateCommand(() => this.ExecuteExportAll(), () => this.OrderItems.Any());
            this.FetchDataCommand = new DelegateCommand(() => this.ExecuteFetchData(), () => true);
            this.eventAggregator.GetEvent<TradeListUpdatedEvent>().Subscribe(this.OnTradeListUpdated, ThreadOption.UIThread);

        }

        private void OnTradeListUpdated(IDictionary<ConnectionParameter, Order[]> connectionParameterOrderDic)
        {
           
            if (connectionParameterOrderDic is null || !connectionParameterOrderDic.Keys.Any())
            {
                return;
            }
            var currentKey =  connectionParameterOrderDic.Keys.FirstOrDefault(i => this.ConnectionParameter.Equals(i));
            var collectionOrders = connectionParameterOrderDic[currentKey];
            var vms= new List<OrderViewModel>();    
            foreach (var item in collectionOrders)
            {
                vms.Add( new OrderViewModel(item));
            }
            this.OrderItems = new ObservableCollection<OrderViewModel>(vms);
        }

        public DelegateCommand ExportCommand { get; }
        public DelegateCommand FetchDataCommand { get; }
        public ConnectionParameter ConnectionParameter { get; }
        public ObservableCollection<OrderViewModel> OrderItems
        {
            get => this.orderItems;
            set => this.SetProperty(ref this.orderItems, value);
        }
        public string Header => $"({this.ConnectionParameter.AccountNumber},{this.ConnectionParameter.Host}, {this.ConnectionParameter.Port})";
             
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
