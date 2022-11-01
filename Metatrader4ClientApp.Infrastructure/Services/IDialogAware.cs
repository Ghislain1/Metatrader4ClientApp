using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metatrader4ClientApp.Infrastructure.Services
{
    public interface IDialogAware
    {
        void OnDialogClosed();

        void OnDialogOpened(dynamic parameters);

        event Action<dynamic> RequestClose;
    }
}
