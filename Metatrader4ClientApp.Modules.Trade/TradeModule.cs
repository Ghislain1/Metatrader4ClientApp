// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Metatrader4ClientApp.Modules.Trade
{
    using Metatrader4ClientApp.Infrastructure.Interfaces;
    using Metatrader4ClientApp.Infrastructure;
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;
    using System;

   
        public class TradeModule : IModule
        {
            public void OnInitialized(IContainerProvider containerProvider)
            {
                var regionManager = containerProvider.Resolve<IRegionManager>();

                regionManager.RegisterViewWithRegion(RegionNames.MainRegion,
                                                            () => containerProvider.Resolve<TradeView>());


            }

            public void RegisterTypes(IContainerRegistry containerRegistry)
            {


                // containerRegistry.Register<IAccountPositionService, AccountPositionService>();

            }
        }

    }
