using Metatrader4ClientApp.Infrastructure;
using Metatrader4ClientApp.Infrastructure.Interfaces;
using Metatrader4ClientApp.Modules.Position.Controllers;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;


namespace Metatrader4ClientApp.Modules.Position.PositionSummary
{
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    public class PositionSummaryViewModel : PluginBindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private PositionSummaryItem? currentPositionSummaryItem;
        private ObservableCollection<PositionSummaryItem> positionSummaryItemCollection = new ObservableCollection<PositionSummaryItem>();
        private IAccountPositionService accountPositionService;
        private IMarketFeedService marketFeedService;
        public PositionSummaryViewModel(IEventAggregator eventAggregator, IMarketFeedService marketFeedService, IAccountPositionService accountPositionService)
        {
            this.eventAggregator = eventAggregator;
            this.marketFeedService = marketFeedService;
            this.accountPositionService = accountPositionService;
            this.PackIcon = PackIconNames.Position;
            this.Label = "POSITION";
            this.PopulateItems();
            this.ExportCommand = new DelegateCommand(() => this.ExecuteExportAll(), () => this.PositionSummaryItemCollection.Any());
            this.Command = new DelegateCommand(() => this.ExportCommand.RaiseCanExecuteChanged());

            //Listen STOCK from Internet
            eventAggregator.GetEvent<MarketPricesUpdatedEvent>().Subscribe(this.MarketPricesUpdated, ThreadOption.UIThread);
        }

        private  void MarketPricesUpdated(IDictionary<string, decimal> tickerSymbolsPrice)
        {
            int  count = 10;
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
