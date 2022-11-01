using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Metatrader4ClientApp.Infrastructure;

using Prism.Events;

namespace Metatrader4ClientApp.Modules.Login
{

    public class LoginViewModel : PluginBindableBase
    {
        private readonly IEventAggregator eventAggregator;



        public LoginViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.Glyph = "\uE80F";


        }
    }
}

