using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metatrader4ClientApp.Modules.Position.Controllers
{
    public interface IOrdersController
    {
        DelegateCommand<string> BuyCommand { get; }
        DelegateCommand<string> SellCommand { get; }
    }
}
