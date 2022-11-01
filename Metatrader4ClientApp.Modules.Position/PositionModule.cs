using Metatrader4ClientApp.Infrastructure;
using Metatrader4ClientApp.Modules.Position.PositionSummary;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Metatrader4ClientApp.Modules.Position
{
     
        public class PositionModule : IModule
        {
            public void OnInitialized(IContainerProvider containerProvider)
            {
                var regionManager = containerProvider.Resolve<IRegionManager>();

                regionManager.RegisterViewWithRegion(RegionNames.MainRegion,
                                                            () => containerProvider.Resolve<PositionSummaryView>());
              //  var _ordersController = containerProvider.Resolve<OrdersController>();


            }

            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
        

            //containerRegistry.Register<IAccountPositionService, AccountPositionService>();
            //containerRegistry.Register<IOrdersService, XmlOrdersService>();
            //containerRegistry.Register<IOrdersController, OrdersController>();
            //containerRegistry.Register<IObservablePosition, ObservablePosition>();
            containerRegistry.Register<IPositionSummaryViewModel, PositionSummaryViewModel>();
            //containerRegistry.Register<IPositionPieChartViewModel, PositionPieChartViewModel>();

            //containerRegistry.Register<IOrderCompositeViewModel, OrderCompositeViewModel>();
            //containerRegistry.Register<IOrderDetailsViewModel, OrderDetailsViewModel>();

            //containerRegistry.Register<IOrdersView, OrdersView>();
        }
        }
    
}