


namespace Metatrader4ClientApp.Modules.Position.PositionSummary
{
    using Metatrader4ClientApp.Infrastructure;
    using Metatrader4ClientApp.Infrastructure.Interfaces;
    using Microsoft.Win32;
    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using Metatrader4ClientApp.Infrastructure.Models;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using TradingAPI.MT4Server;
    public class PositionSummaryViewModel : PluginBindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private PositionSummaryItem? currentPositionSummaryItem;
        private ObservableCollection<PositionSummaryItem> positionSummaryItemCollection = new ObservableCollection<PositionSummaryItem>();
        private IAccountPositionService accountPositionService;
        private readonly IExportService exportService;
        private readonly IMarketFeedService marketFeedService;
        
        public PositionSummaryViewModel(IEventAggregator eventAggregator, IMarketFeedService marketFeedService, IAccountPositionService accountPositionService, IExportService exportService)
        {
            this.eventAggregator = eventAggregator;
            this.marketFeedService = marketFeedService;
            this.accountPositionService = accountPositionService;
            this. exportService= exportService;
            this.PackIcon = PackIconNames.Position;
            this.Label = "POSITION";
            this.PopulateItems();
            this.ExportCommand = new DelegateCommand(() => this.ExecuteExportAll(), () => this.PositionSummaryItemCollection.Any());
            this.Command = new DelegateCommand(() =>
            {
                if (this.IsSelected)
                {

                }

                this.TradeCopy();
                this.ExportCommand.RaiseCanExecuteChanged();
             }
            );

            //Listen STOCK from Internet
            eventAggregator.GetEvent<MarketPricesUpdatedEvent>().Subscribe(this.MarketPricesUpdated, ThreadOption.UIThread);
        }

        private  void MarketPricesUpdated(IDictionary<string, decimal> tickerSymbolsPrice)
        {
          
            if (tickerSymbolsPrice == null)
            {
                throw new ArgumentNullException("tickerSymbolsPrice");
            }

            foreach (PositionSummaryItem position in this.PositionSummaryItemCollection)
            {
                if (tickerSymbolsPrice.ContainsKey(position.TickerSymbol))
                {
                    position.CurrentPrice = tickerSymbolsPrice[position.TickerSymbol];
                }
            }
        }
        private async Task TradeCopy()
        {
            //User: 500476959
            //Password: ehj4bod
            //Host: mt4-demo.roboforex.com
            //Port: 443

            //User:  63837866
            //Password: anp1eyjw
            //Host: mt4-demo.roboforex.com
            //Port: 443

            var quoteClient = new QuoteClient(63837866, "anp1eyjw", "mt4-demo.roboforex.com", 443);
            var orderClient = new OrderClient(quoteClient);
            await Task.Run(() => quoteClient.Connect());
            if (quoteClient.Connected)
            {

            }
            quoteClient.OnOrderUpdate += Qc1_OnOrderUpdate;
          
        }


        private void Qc1_OnOrderUpdate(object sender, OrderUpdateEventArgs update)
        {

            var qc1 = (QuoteClient)sender;
            var order = update.Order;
            //if (update.Action == UpdateAction.PositionOpen)
            //{
            //    DestQC.Subscribe(order.Symbol);
            //    var destOrder = DestOC.OrderSend(order.Symbol, order.Type, order.Lots, 0, 0, 0, 0, order.Ticket.ToString());
            //    Tickets.Add(order.Ticket, destOrder.Ticket);
            //    Console.WriteLine("Open copied");
            //}
            //if (update.Action == UpdateAction.PositionClose)
            //{
            //    DestOC.OrderClose(order.Symbol, Tickets[order.Ticket], order.Lots, 0, 0);
            //    Console.WriteLine("Close copied");
            //}
        }
        private async void PopulateItems()
        {
            var items = await this.accountPositionService.GetAccountPositionsAsync();
            this.positionSummaryItemCollection.Clear();
            items.ToList().ForEach(accountPosition => this.PositionSummaryItemCollection.Add(new PositionSummaryItem(accountPosition.TickerSymbol, accountPosition.CostBasis, accountPosition.Shares, this.marketFeedService.GetPrice(accountPosition.TickerSymbol))));
        }

        public PositionSummaryItem? CurrentPositionSummaryItem
        {
            get { return currentPositionSummaryItem; }
            set
            {
                if (SetProperty(ref currentPositionSummaryItem, value))
                {
                    if (currentPositionSummaryItem != null)
                    {
                        eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Publish(this.CurrentPositionSummaryItem?.TickerSymbol!);
                    }
                }
            }
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

                this.TradeCopy();
                await this.accountPositionService.ExportToTextFileAsync(this.PositionSummaryItemCollection, saveFileDialog.FileName);
                this.exportService.Export(this.PositionSummaryItemCollection.Select(i => i.AccountPosition), saveFileDialog.FileName, ExportFileType.CSV); ;
            }

            catch (Exception exception)

            {
                // TODO
                // Logger.Instance.Log(exception);

            }

        }


        public ObservableCollection<PositionSummaryItem> PositionSummaryItemCollection
        {
            get => this.positionSummaryItemCollection;
            set => SetProperty(ref this.positionSummaryItemCollection, value);
        }

        public DelegateCommand ExportCommand { get; }
    }
}
