// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Metatrader4ClientApp.Modules.Option
{
    using Metatrader4ClientApp.Infrastructure;
    using Metatrader4ClientApp.Infrastructure.Interfaces;
    using Prism.Commands;
    using Prism.Events;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class OptionListViewModel : PluginBindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly ISettingsService settingsService;
        private bool isInitialized = false;

        public OptionListViewModel(IEventAggregator eventAggregator, ISettingsService settingsService)
        {
            this.eventAggregator = eventAggregator;
            this.settingsService = settingsService;
            this.PackIcon = PackIconNames.Option;
            this.Label = "Settings";
            this.Command = new DelegateCommand(() => this.ExecuteSynchronize());
            this.isInitialized = true;
        }

        private void ExecuteSynchronize()
        {
            if (this.isInitialized)
            {
                return;
            }
        }
    }
}
