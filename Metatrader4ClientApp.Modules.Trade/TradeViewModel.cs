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
                this.PopulateTradeItems();

                this.TradeCopy();
                //this.ExportCommand.RaiseCanExecuteChanged();
            });

           // this. eventAggregator.GetEvent<MarketPricesUpdatedEvent>().Subscribe(this.MarketPricesUpdated, ThreadOption.UIThread);
           // this.eventAggregator.GetEvent<TradItemUpdatedEvent>().Subscribe(this.TradItemUpdated, ThreadOption.UIThread);
            this.eventAggregator.GetEvent<ConnectionParameterCreatedEvent>().Subscribe(this.OnConnectionParameterCreated, ThreadOption.UIThread);
        }

        private void OnConnectionParameterCreated(ConnectionParameter newConnectionParameter)
        {
            if(this.TradeItems.Any(i => i.ConnectionParameter.Equals(newConnectionParameter)))
            {
                return;
            }
            this.TradeItems .Add(new TradeItemViewModel(newConnectionParameter,this.eventAggregator, this.marketFeedService, this.exportService));
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
           var cpList= await this.connectionParameterService.GetConnectionParametersAsync();
            var myTradeItems= new List<TradeItemViewModel>();
            foreach (var item in cpList)
            {
                myTradeItems.Add(this.CreateTradeItemViewModel(item));
            }
            this.TradeItems = new ObservableCollection<TradeItemViewModel>(myTradeItems);
        }

        /// <summary>
        /// http://mtapi.online/2017/12/21/list-of-opened-orders/
        /// </summary>
        private async void TradeCopy()
        {
            var items = await this.connectionParameterService.GetConnectionParametersAsync();
            if(items is null)
            {
                return;
            }
           // this.TradeItems = new ObservableCollection<TradeItemViewModel>(items.Select(item =>   new TradeItemViewModel(item, this.marketFeedService)));
            await Task.Run(() =>
            {
                foreach (var cp in items)
                {
                    try
                    {
                        QuoteClient qc = new QuoteClient(cp.AccountNumber, cp.Password, cp.Host, cp.Port);
                        Console.WriteLine("Connecting...");
                        qc.Connect();


                        foreach (Order order in qc.GetOpenedOrders())
                        {
                            this.orderDic.Add(Guid.NewGuid().ToString(), order);
                            //Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                            //{
                            //    this.OrderItems.Add(order!);
                            //}));
                        }

                        qc.Disconnect();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("Press any key...");

                    }
                }
            });

           // this.OrderItems = new ObservableCollection<OrderViewModel>(this.orderDic.Values.Select(i => new OrderViewModel(i)));
        }

 
 
        
        

        public ObservableCollection<TradeItemViewModel> TradeItems
        {
            get => this.tradeItems;
            set => this.SetProperty(ref this.tradeItems, value);
        }

    }

}