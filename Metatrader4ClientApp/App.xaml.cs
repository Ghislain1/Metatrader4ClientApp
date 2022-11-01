using Metatrader4ClientApp.Infrastructure.Interfaces;
using Metatrader4ClientApp.Infrastructure.Services;
using Metatrader4ClientApp.Modules.Login;
using Metatrader4ClientApp.Modules.Position;
using Metatrader4ClientApp.Modules.UserManagement;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Metatrader4ClientApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication   {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            //ChangeConvention
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType.FullName;
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var viewModelName = $"{viewName}Model, {viewAssemblyName}";
                return Type.GetType(viewModelName);
            });
        }
        protected override Window CreateShell()
        {           
            return Container.Resolve<MainWindow>();
        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // Load all 4 modules using code            
            //moduleCatalog.AddModule<MarketModule>();
            moduleCatalog.AddModule<PositionModule>();
            moduleCatalog.AddModule<UserManagementModule>();
            moduleCatalog.AddModule<LoginModule>();

        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

           containerRegistry.RegisterSingleton<IMarketFeedService, MarketFeedService>();

           // containerRegistry.RegisterSingleton<IStockTraderRICommandProxy, StockTraderRICommandProxy>();
            // containerRegistry.RegisterDialog<NotificationDialog, NotificationDialogViewModel>();
            // containerRegistry.RegisterSingleton<IDialogService, DialogService>();
            //    containerRegistry.RegisterSingleton<ITaskbarService, TaskbarService>();
        }
    }
}
