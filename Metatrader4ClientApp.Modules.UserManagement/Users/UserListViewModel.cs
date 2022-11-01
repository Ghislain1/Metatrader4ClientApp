using Metatrader4ClientApp.Infrastructure;
using Metatrader4ClientApp.Infrastructure.Interfaces;
using Metatrader4ClientApp.Infrastructure.Models;
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
        private ObservableCollection<ApplicationUser> applicationUserCollection = new ObservableCollection<ApplicationUser>();      
        private IApplicationUserService applicationUserService;

        public UserListViewModel(IEventAggregator eventAggregator, IApplicationUserService applicationUserService )
        {
            this.eventAggregator = eventAggregator;
            this.applicationUserService = applicationUserService;
            this.Glyph = GlyphNames.UserManagementGlyph;
            this.Label = "USERS";
            this.PopulateUsers();
        }
        public string HeaderInfo
        {
            get =>this.headerInfo;            

            set =>     this.           SetProperty(ref this.headerInfo, value);
            
        }

        public ObservableCollection<ApplicationUser> ApplicationUserCollection
        {
            get => this.applicationUserCollection;
            set => SetProperty(ref this.applicationUserCollection, value);
        }

        private async void PopulateUsers()
        {
            var items = await this.applicationUserService.GetUsersAsync();
            this.ApplicationUserCollection= new ObservableCollection<ApplicationUser>(items);
            
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
