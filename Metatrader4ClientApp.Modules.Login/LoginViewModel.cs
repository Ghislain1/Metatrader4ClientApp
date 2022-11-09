

namespace Metatrader4ClientApp.Modules.Login
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Metatrader4ClientApp.Infrastructure;
    using Metatrader4ClientApp.Infrastructure.Events;
    using Metatrader4ClientApp.Infrastructure.Interfaces;
    using Metatrader4ClientApp.Infrastructure.Models;
    using Prism.Commands;
    using Prism.Events;

    public class LoginViewModel : PluginBindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IConnectionParameterService connectionParameterService;
        private readonly IMarketFeedService marketFeedService;
        private string? loginMessage;
        private string? accountNumberString;
        private string? host;
        private int port  ;
        public LoginViewModel(IEventAggregator eventAggregator, IMarketFeedService marketFeedService, IConnectionParameterService connectionParameterService)
        {
            this.eventAggregator = eventAggregator;
            this.marketFeedService = marketFeedService;
            this.connectionParameterService = connectionParameterService;
            this.PackIcon = PackIconNames.Login;
            this.Label = "LOGIN";
            this.LoginCommand = new DelegateCommand<object>(async (argument) => await this.LoginAsync(argument), (_) => !this.LoginIsRunning);
            this.Command = new DelegateCommand(() =>
             {

             });
            this.Port = 443;
            this.AccountNumberString = "500476959";
           // this.Password = "ywh3ejc";
            this.Host = "mt4-demo.roboforex.com";
 
        }
        public int AccountNumber { get; set; }
        public string? AccountNumberString
        {
            get => this.accountNumberString;
            set => this.SetProperty(ref this.accountNumberString, value);
        }
        public string? Host
        {
            get => this.host;
            set => this.SetProperty(ref this.host, value);
        }
        public int Port
        {
            get => this.port;
            set => this.SetProperty(ref this.port, value);
        }

        public string? LoginMessage
        {
            get => this.loginMessage;
            set => this.SetProperty(ref this.loginMessage, value);
        }
        

        /// <summary>
        /// A flag indicating if the login command is running
        /// </summary>
        public bool LoginIsRunning { get; set; }

        /// <summary>
        /// The command to login
        /// </summary>
        public ICommand LoginCommand { get; set; }

        /// <summary>
        /// Attempts to log the user in
        /// </summary>
        /// <param name="parameter">The <see cref="SecureString"/> passed in from the view for the users password</param>
        /// <returns></returns>
        public async Task LoginAsync(object argument)
        {
            this.LoginMessage = string.Empty;
            if (argument is not PasswordBox passwordBox)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(passwordBox.Password) )
            {
                return;
            }
            if (!int.TryParse(this.AccountNumberString, out int accontNumber))
            {
                return;
            }
            this.AccountNumber = accontNumber;

            this.eventAggregator.GetEvent<ApplicationBusyEvent>().Publish(true);
            var cp = new ConnectionParameter() { Host = this.Host, AccountNumber= this.AccountNumber, Password=passwordBox.Password, Port = this.Port };   
            var isOkay = await this.marketFeedService.CheckConnectionParameterAsync(cp);
            if (!isOkay)
            {
                this.LoginMessage = this.marketFeedService.ErrorMessage;
            }
            else
            {
                this.LoginMessage = string.Empty;
                // Store it for later
                this.connectionParameterService.StoreConnectionParameter(cp);
            }
            this.eventAggregator.GetEvent<ApplicationBusyEvent>().Publish(false);
        }
    }
}

