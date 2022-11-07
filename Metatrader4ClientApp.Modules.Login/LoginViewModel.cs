

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
    using Metatrader4ClientApp.Infrastructure.Interfaces;
    using Prism.Commands;
    using Prism.Events;

    public class LoginViewModel : PluginBindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IApplicationUserService applicationUserService;
        private bool savePassword;
        private string name;
        public LoginViewModel(IEventAggregator eventAggregator, IApplicationUserService applicationUserService)
        {
            this.eventAggregator = eventAggregator;
            this.applicationUserService = applicationUserService;
            this.PackIcon = PackIconNames.Login;
            this.Label = "LOGIN";

            this.LoginCommand = new DelegateCommand<object>(async (argument) => await this.LoginAsync(argument), (_) => !this.LoginIsRunning);

            this.Command = new DelegateCommand(() =>
             {

             });
        }

        public string Name
        {
            get => this.name;
            set => this.SetProperty(ref this.name, value);
        }

        public bool SavePassword
        {
            get => this.savePassword;
            set => this.SetProperty(ref this.savePassword, value);
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
            if (argument is not PasswordBox passwordBox)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(passwordBox.Password) || string.IsNullOrWhiteSpace(this.Name))
            {
                return;
            }

            this.LoginIsRunning = true;
            await Task.Run(() =>
            {
              

                if (this.SavePassword)
                {
                    var alreadyStored = this.applicationUserService.CheckUser(this.Name, passwordBox.Password);
                    if (alreadyStored)
                    {
                        this.Name = String.Empty;
                    }
                    else
                    {
                        this.applicationUserService.StoreUser(this.Name, passwordBox.Password);
                    }

                }



            });
            this.LoginIsRunning = false;
        }
    }
}

