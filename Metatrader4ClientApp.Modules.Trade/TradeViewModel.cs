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

    public class TradeViewModel : PluginBindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private ObservableCollection<OrderViewModel> orderItems = new ObservableCollection<OrderViewModel>();
        private IAccountPositionService accountPositionService;
        private readonly IExportService exportService;
        private readonly IApplicationUserService applicationUserService;
        private readonly Dictionary<string, Order> orderDic = new Dictionary<string, Order>();

        public TradeViewModel(IEventAggregator eventAggregator, IApplicationUserService applicationUserService, IExportService exportService)
        {
            this.eventAggregator = eventAggregator;
            this.applicationUserService = applicationUserService;
            this.accountPositionService = accountPositionService;
            this.exportService = exportService;
            this.PackIcon = PackIconNames.Trade;
            this.Label = "TRADE";
            // this.PopulateItems();
            this.ExportCommand = new DelegateCommand(() => this.ExecuteExportAll(), () => this.OrderItems.Any());
            this.Command = new DelegateCommand(() =>
            {
                if (this.IsSelected)
                {

                }

                this.TradeCopy();
                this.ExportCommand.RaiseCanExecuteChanged();
            });
        }
        /// <summary>
        /// http://mtapi.online/2017/12/21/list-of-opened-orders/
        /// </summary>
        private async void TradeCopy()
        {
            var items = await this.applicationUserService.GetConnectionParametersAsync();
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

            this.OrderItems = new ObservableCollection<OrderViewModel>(this.orderDic.Values.Select(i => new OrderViewModel(i)));
        }
    

    private void Qc_OnOrderUpdate(object sender, OrderUpdateEventArgs update)
    {
        var qc1 = (QuoteClient)sender;
        var order = update.Order;
        if (update.Action == UpdateAction.PositionOpen)
        {
            // DestQC.Subscribe(order.Symbol);
            // var destOrder = DestOC.OrderSend(order.Symbol, order.Type, order.Lots, 0, 0, 0, 0, order.Ticket.ToString());
            // Tickets.Add(order.Ticket, destOrder.Ticket);
            Console.WriteLine("Open copied");
        }
        if (update.Action == UpdateAction.PositionClose)
        {
            //DestOC.OrderClose(order.Symbol, Tickets[order.Ticket], order.Lots, 0, 0);
            // Console.WriteLine("Close copied");
        }

    }

    private void Qc_OnQuote(object sender, QuoteEventArgs args)
    {

    }

    //Listen STOCK from Internet
    // eventAggregator.GetEvent<MarketPricesUpdatedEvent>().Subscribe(this.MarketPricesUpdated, ThreadOption.UIThread);


    public DelegateCommand ExportCommand { get; }
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

            // this.TradeCopy();
            await this.accountPositionService.ExportToTextFileAsync(this.OrderItems, saveFileDialog.FileName);
            //  this.exportService.Export(this.PositionSummaryItemCollection.Select(i => i.AccountPosition), saveFileDialog.FileName, ExportFileType.CSV); ;
        }

        catch (Exception exception)

        {
            // TODO
            // Logger.Instance.Log(exception);

        }

    }

        public ObservableCollection<OrderViewModel> OrderItems
        {
            get => this.orderItems;
            set => this.SetProperty(ref this.orderItems, value);
        }
       
}


}