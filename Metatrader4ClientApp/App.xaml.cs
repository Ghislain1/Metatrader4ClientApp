using Metatrader4ClientApp.Modules.Position;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Metatrader4ClientApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
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
            //moduleCatalog.AddModule<WatchModule>();
            //moduleCatalog.AddModule<NewsModule>();

        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
           // containerRegistry.RegisterSingleton<IStockTraderRICommandProxy, StockTraderRICommandProxy>();
            // containerRegistry.RegisterDialog<NotificationDialog, NotificationDialogViewModel>();
            // containerRegistry.RegisterSingleton<IDialogService, DialogService>();
            //    containerRegistry.RegisterSingleton<ITaskbarService, TaskbarService>();
        }
    }
}
