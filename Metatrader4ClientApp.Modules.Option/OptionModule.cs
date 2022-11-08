

namespace Metatrader4ClientApp.Modules.Option
{
    using Metatrader4ClientApp.Infrastructure.Interfaces;
    using Metatrader4ClientApp.Infrastructure;
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;

    public class OptionModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.OptionsRegion,
                                                        () => containerProvider.Resolve<OptionListView>());
          


        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }

}
