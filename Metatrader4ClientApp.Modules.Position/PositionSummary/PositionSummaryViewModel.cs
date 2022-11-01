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
    public class PositionSummaryViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private PositionSummaryItem currentPositionSummaryItem;
        private ObservableCollection<PositionSummaryItem> positionSummaryItemCollection = new ObservableCollection<PositionSummaryItem>();
        private IAccountPositionService accountPositionService;
        private IMarketFeedService marketFeedService;
        private string headerInfo;

        public PositionSummaryViewModel( IEventAggregator eventAggregator, IMarketFeedService marketFeedService,IAccountPositionService accountPositionService)
        {  
           this.eventAggregator = eventAggregator;
           this.marketFeedService= marketFeedService;
           this.accountPositionService=accountPositionService;
           this.HeaderInfo ="POSITION";
           this.PopulateItems();   

         
        }
        public string HeaderInfo
        {
            get => this.headerInfo;

            set => this.SetProperty(ref this.headerInfo, value);

        }
        private async void PopulateItems()
        {
            var items = await this.accountPositionService.GetAccountPositionsAsync();
            this.positionSummaryItemCollection.Clear();
            items.ToList().ForEach(accountPosition =>    this.PositionSummaryItemCollection.Add(new PositionSummaryItem(accountPosition.TickerSymbol, accountPosition.CostBasis, accountPosition.Shares, this.marketFeedService.GetPrice(accountPosition.TickerSymbol))));

        }
       

    

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

       

     

        public ObservableCollection<PositionSummaryItem> PositionSummaryItemCollection
        {
            get =>  this.positionSummaryItemCollection; 
            set =>SetProperty(ref this.positionSummaryItemCollection, value); 
        }


    
        public ICommand SellCommand { get; private set; }
    }
}