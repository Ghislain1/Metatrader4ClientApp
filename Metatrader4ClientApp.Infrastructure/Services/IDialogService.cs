using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metatrader4ClientApp.Infrastructure.Services
{
    public interface IDialogService
    {
        void Show(string name, dynamic parameters, Action<dynamic> callback);

        void ShowDialog(string name, dynamic parameters, Action<dynamic> callback);
    }
}
