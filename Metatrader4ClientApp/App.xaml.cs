using MahApps.Metro.Controls;

using MaterialDesignThemes.Wpf;

using Metatrader4ClientApp.Adapters;
using Metatrader4ClientApp.Infrastructure.Interfaces;
using Metatrader4ClientApp.Infrastructure.Services;
using Metatrader4ClientApp.Modules.Login;
using Metatrader4ClientApp.Modules.Position;
using Metatrader4ClientApp.Modules.UserManagement;
using Metatrader4ClientApp.Services;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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

        /// <summary>
        /// The ConfigureRegionAdapterMappings.
        /// </summary>
        /// <param name="regionAdapterMappings">The regionAdapterMappings <see cref="RegionAdapterMappings"/>.</param>
        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings mappings)
        {
            // WOrk around: https://github.com/dotnet/wpf/issues/738
            // Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            base.ConfigureRegionAdapterMappings(mappings);
            mappings.RegisterMapping(typeof(HamburgerMenuItemCollection), this.Container.Resolve<HamburgerMenuItemCollectionRegionAdapter>());
        }

        protected override Window CreateShell()
        {           
            return Container.Resolve<ShellView>();
        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // Load all 4 modules using code            
            moduleCatalog.AddModule<LoginModule>();
            moduleCatalog.AddModule<UserManagementModule>();
            moduleCatalog.AddModule<PositionModule>();
           
            

        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

           containerRegistry.RegisterSingleton<IMarketFeedService, MarketFeedService>();
           containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();
            containerRegistry.RegisterSingleton<IApplicationUserService, ApplicationUserService>();
 

           // containerRegistry.RegisterSingleton<IStockTraderRICommandProxy, StockTraderRICommandProxy>();
           // containerRegistry.RegisterDialog<NotificationDialog, NotificationDialogViewModel>();
            containerRegistry.RegisterSingleton<IDialogService, DialogService>();
           //    containerRegistry.RegisterSingleton<ITaskbarService, TaskbarService>();
        }


        private static Theme LightTheme { get; } = Theme.Create(  new MaterialDesignLightTheme(), Colors.WhiteSmoke, Colors.WhiteSmoke  );

        private static Theme DarkTheme { get; } = Theme.Create(            new MaterialDesignDarkTheme(), Colors.DarkGray, Colors.DarkKhaki                 );
        public static void SetLightTheme()
        {
            var paletteHelper = new PaletteHelper();
            paletteHelper.SetTheme(LightTheme);

            Current.Resources["SuccessBrush"] = new SolidColorBrush(Colors.DarkGreen);
            Current.Resources["CanceledBrush"] = new SolidColorBrush(Colors.DarkOrange);
            Current.Resources["FailedBrush"] = new SolidColorBrush(Colors.DarkRed);
        }

        public static void SetDarkTheme()
        {
            var paletteHelper = new PaletteHelper();
            paletteHelper.SetTheme(DarkTheme);

            Current.Resources["SuccessBrush"] = new SolidColorBrush(Colors.LightGreen);
            Current.Resources["CanceledBrush"] = new SolidColorBrush(Colors.Orange);
            Current.Resources["FailedBrush"] = new SolidColorBrush(Colors.OrangeRed);
        }
    }
}
