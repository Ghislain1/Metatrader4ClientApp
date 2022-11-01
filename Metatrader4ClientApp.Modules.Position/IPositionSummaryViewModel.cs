using Metatrader4ClientApp.Infrastructure.Interfaces;
using Metatrader4ClientApp.Modules.Position.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Metatrader4ClientApp.Modules.Position
{
    public interface IPositionSummaryViewModel : IHeaderInfoProvider<string>
    {
       // IObservablePosition Position { get; }

        //ICommand BuyCommand { get; }

        //ICommand SellCommand { get; }
    }
}
