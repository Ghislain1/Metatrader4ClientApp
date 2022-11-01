using Metatrader4ClientApp.Infrastructure;
using Metatrader4ClientApp.Infrastructure.Interfaces;
using Metatrader4ClientApp.Modules.Position.Controllers;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Metatrader4ClientApp.Modules.Position.PositionSummary
{
    public class PositionSummaryViewModel : BindableBase, IPositionSummaryViewModel
    {
        private readonly IEventAggregator eventAggregator;
        private PositionSummaryItem currentPositionSummaryItem;
        private ObservableCollection<PositionSummaryItem> positionSummaryItemCollection = new ObservableCollection<PositionSummaryItem>();
        private IAccountPositionService accountPositionService;
        private IMarketFeedService marketFeedService;

        public PositionSummaryViewModel(IOrdersController ordersController, IEventAggregator eventAggregator, IMarketFeedService marketFeedService,IAccountPositionService accountPositionService)
        {
            if (ordersController is null)
            {
                throw new ArgumentNullException("ordersController");
            }


            this.eventAggregator = eventAggregator;
            this.marketFeedService= marketFeedService;
           this.accountPositionService=accountPositionService;

            this.PopulateItems();

            BuyCommand = ordersController.BuyCommand;
            SellCommand = ordersController.SellCommand;

         
        }
        private async void PopulateItems()
        {
            var items = await this.accountPositionService.GetAccountPositionsAsync();
            this.positionSummaryItemCollection.Clear();
            items.ToList().ForEach(accountPosition =>    this.PositionSummaryItemCollection.Add(new PositionSummaryItem(accountPosition.TickerSymbol, accountPosition.CostBasis, accountPosition.Shares, this.marketFeedService.GetPrice(accountPosition.TickerSymbol))));

        }
       

        public ICommand AddToWatchCommand { get; private set; }
        public ICommand BuyCommand { get; private set; }

        public PositionSummaryItem CurrentPositionSummaryItem
        {
            get { return currentPositionSummaryItem; }
            set
            {
                if (SetProperty(ref currentPositionSummaryItem, value))
                {
                    if (currentPositionSummaryItem != null)
                    {
                        eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Publish(
                            CurrentPositionSummaryItem.TickerSymbol);
                    }
                }
            }
        }

        public string HeaderInfo => "POSITION";

        public IObservablePosition Position { get; private set; }

        public ObservableCollection<PositionSummaryItem> PositionSummaryItemCollection
        {
            get =>  this.positionSummaryItemCollection; 
            set =>SetProperty(ref this.positionSummaryItemCollection, value); 
        }


    
        public ICommand SellCommand { get; private set; }
    }
}