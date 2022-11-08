namespace Metatrader4ClientApp
{
    using MahApps.Metro.Controls;

    using MaterialDesignThemes.Wpf;
    using Metatrader4ClientApp.Infrastructure.Events;
    using Metatrader4ClientApp.Infrastructure.Interfaces;

    using Prism.Commands;
    using Prism.Events;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    // See Dopamine
    public class ShellViewModel : BindableBase
    {
        private IDialogService dialogService;
        private readonly IEventAggregator eventAggregator;
        private object activatedItem;
        private bool isPaneOpen;
        private bool isBusy;
        private DelegateCommand showDialogCommand;
        public SnackbarMessageQueue Notifications { get; } = new(TimeSpan.FromSeconds(5));
        public ShellViewModel(IEventAggregator eventAggregator,IDialogService dialogService, ISettingsService settingsService)
        {
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;
            this.IsPaneOpen = true;
            this.eventAggregator.GetEvent<ApplicationBusyEvent>().Subscribe(value => this.IsBusy = value);
        }
        public bool IsPaneOpen
        {
            get => this.isPaneOpen;
            set => SetProperty(ref this.isPaneOpen, value);
        }

        public bool IsBusy
        {
            get => this.isBusy;
            set => SetProperty(ref this.isBusy, value);
        }

        
        public object ActivatedItem
        {
            get { return activatedItem; }
            set
            {
                if (this.SetProperty(ref this.activatedItem, value))
                {
                    if (value is not HamburgerMenuGlyphItem hamburgerMenuGlyphItem)
                    {
                        return;
                    }
                    hamburgerMenuGlyphItem.RaiseCommand();
                   
                }
            }
        }


        public DelegateCommand ShowDialogCommand =>
            showDialogCommand ?? (showDialogCommand = new DelegateCommand(ShowDialog));

        private void ShowDialog()
        {
            var message = "This is a message that should be shown in the dialog.";
            //using the dialog service as-is
            this.dialogService.ShowDialog("NotificationDialog", new DialogParameters($"message={message}"), r =>
            {
                //if (r.Result == ButtonResult.None)
                //    Title = "Result is None";
                //else if (r.Result == ButtonResult.OK)
                //    Title = "Result is OK";
                //else if (r.Result == ButtonResult.Cancel)
                //    Title = "Result is Cancel";
                //else
                //    Title = "I Don't know what you did!?";
            });
        }
    }
}