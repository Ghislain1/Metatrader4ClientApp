

namespace Metatrader4ClientApp.Modules.Login
{
    using Metatrader4ClientApp.Infrastructure;
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;
    using TradingAPI.MT4Server;

    public class LoginModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion(RegionNames.MainRegion,
                                                        () => containerProvider.Resolve<LoginView>());
            


        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {


            //containerRegistry.Register<IAccountPositionService, AccountPositionService>();
            //containerRegistry.Register<IOrdersService, XmlOrdersService>();
            // containerRegistry.Register<IOrdersController, OrdersController>();
            //containerRegistry.Register<IObservablePosition, ObservablePosition>();
           
        }
    }


}