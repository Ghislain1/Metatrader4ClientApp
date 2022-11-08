using Metatrader4ClientApp.Infrastructure;
using Metatrader4ClientApp.Infrastructure.Interfaces;
using Metatrader4ClientApp.Infrastructure.Models;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metatrader4ClientApp.Modules.UserManagement.Users
{
    public class UserListViewModel : PluginBindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private ApplicationUser selectedApplicationUser;
        private string headerInfo;
        private ObservableCollection<ConnectionParameter> connectionParameterCollection = new ObservableCollection<ConnectionParameter>();
        private IApplicationUserService applicationUserService;
        public UserListViewModel(IEventAggregator eventAggregator, IApplicationUserService applicationUserService)
        {
            this.eventAggregator = eventAggregator;
            this.applicationUserService = applicationUserService;
            this.PackIcon = PackIconNames.UserManagement;
            this.Label = "USERS";
            // this.PopulateUsers();
            this.Command = new DelegateCommand(() =>
            {
                this.PopulateUsers();
            });
        }
        public string HeaderInfo
        {
            get => this.headerInfo;
            set => this.SetProperty(ref this.headerInfo, value);
        }

        public ObservableCollection<ConnectionParameter> ConnectionParameterCollection
        {
            get => this.connectionParameterCollection;
            set => SetProperty(ref this.connectionParameterCollection, value);
        }

        private async void PopulateUsers()
        {
            var items = await this.applicationUserService.GetConnectionParametersAsync();
            this.ConnectionParameterCollection = new ObservableCollection<ConnectionParameter>(items);

        }

        public ApplicationUser SelectedApplicationUser
        {
            get { return selectedApplicationUser; }
            set
            {
                if (SetProperty(ref selectedApplicationUser, value))
                {
                    if (selectedApplicationUser != null)
                    {
                        //eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Publish(
                        //    CurrentPositionSummaryItem.TickerSymbol);
                    }
                }
            }
        }
    }
}
