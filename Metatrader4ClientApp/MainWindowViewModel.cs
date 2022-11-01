using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metatrader4ClientApp
{
    
        // See Dopamine
        public class MainWindowViewModel : BindableBase
        {
            private IDialogService dialogService;

            public MainWindowViewModel(IDialogService dialogService)
            {
                this.dialogService = dialogService;
            }
            private string _title = "Prism Application";
            public string Title
            {
                get { return _title; }
                set { SetProperty(ref _title, value); }
            }

            private DelegateCommand showDialogCommand;
            public DelegateCommand ShowDialogCommand =>
                showDialogCommand ?? (showDialogCommand = new DelegateCommand(ShowDialog));

            private void ShowDialog()
            {
                var message = "This is a message that should be shown in the dialog.";
                //using the dialog service as-is
                this.dialogService.ShowDialog("NotificationDialog", new DialogParameters($"message={message}"), r =>
                {
                    if (r.Result == ButtonResult.None)
                        Title = "Result is None";
                    else if (r.Result == ButtonResult.OK)
                        Title = "Result is OK";
                    else if (r.Result == ButtonResult.Cancel)
                        Title = "Result is Cancel";
                    else
                        Title = "I Don't know what you did!?";
                });
            }
        }
    }