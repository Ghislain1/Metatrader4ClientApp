using Metatrader4ClientApp.Infrastructure.Interfaces;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading;

namespace Metatrader4ClientApp
{
    public class DashboardViewModel:BindableBase, IDisposable
    {
        private readonly IDialogService dialogService;
        private readonly ISettingsService settingsService;

        private readonly Semaphore semaphore = new(0,2);
        public DashboardViewModel(IDialogService dialogService,   ISettingsService settingsService)
        {

            this.dialogService = dialogService;
            this.settingsService = settingsService;

            // _progressMuxer = Progress.CreateMuxer().WithAutoReset();

           // this.settingsService.BindAndInvoke(o => o.ParallelLimit, (_, e) => _downloadSemaphore.MaxCount = e.NewValue);
          //  Progress.Bind(o => o.Current, (_, _) => NotifyOfPropertyChange(() => IsProgressIndeterminate));
        }

        public void Dispose()
        {
            this.semaphore.Dispose();
        }
    }
}