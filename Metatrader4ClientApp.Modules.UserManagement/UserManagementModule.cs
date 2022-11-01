using Metatrader4ClientApp.Infrastructure;
using Metatrader4ClientApp.Infrastructure.Interfaces;
using Metatrader4ClientApp.Modules.UserManagement.Services;
using Metatrader4ClientApp.Modules.UserManagement.Users;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Metatrader4ClientApp.Modules.UserManagement
{

    public class UserManagementModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion(RegionNames.MainRegion,
                                                        () => containerProvider.Resolve<UserListView>());
            //  var _ordersController = containerProvider.Resolve<OrdersController>();


        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
         containerRegistry.Register<IApplicationUserService, ApplicationUserService>();


       
            //containerRegistry.Register<IOrdersService, XmlOrdersService>();
            // containerRegistry.Register<IOrdersController, OrdersController>();
            //containerRegistry.Register<IObservablePosition, ObservablePosition>();

        }
    }


}