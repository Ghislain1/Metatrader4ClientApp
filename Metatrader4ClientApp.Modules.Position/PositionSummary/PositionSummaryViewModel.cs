


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
        private void TradeCopy()
        {
            var qc1 = new QuoteClient(8681572, "zy3ojco", "mt4-demopro-dc1.roboforex.com", 443);
            //qc1.Connect();
            //DestQC = new QuoteClient(61013955, "h0200Da6A", "185.10.45.25", 443);
            //DestQC.Connect();
            //DestOC = new OrderClient(DestQC);
            //qc1.OnOrderUpdate += Qc1_OnOrderUpdate;
          
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
                    Filter = $"Log Files (*{AppConstants.TXT_EXT})|*{AppConstants.TXT_EXT}",
                    InitialDirectory = KnownFolders.ExportedFolderUri.LocalPath

                };

                if (saveFileDialog.ShowDialog() is not true)
                {
                    return;

                }
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
