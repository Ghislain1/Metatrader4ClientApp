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

namespace Metatrader4ClientApp.Modules.Login
{

    public class LoginViewModel : PluginBindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IApplicationUserService applicationUserService;
        private string password;
        private string name;
        public LoginViewModel(IEventAggregator eventAggregator, IApplicationUserService applicationUserService)
        {
            this.eventAggregator = eventAggregator;
            this.applicationUserService = applicationUserService;
            this.Glyph = GlyphNames.LoginGlyph;
            this.Label = "LOGIN";

           this. LoginCommand = new DelegateCommand<object>(async (argument) => await this.LoginAsync(argument), (_)=> !this.LoginIsRunning);
        }

        public string Name
        {
            get => this.name;
            set => this.SetProperty(ref this.name, value);
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
            if(argument is not PasswordBox passwordBox)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(passwordBox.Password)|| string.IsNullOrWhiteSpace(this.Name))
            {
                return;
            }

            this.LoginIsRunning = true;
            await Task.Run(   () =>
            {
                // Call the server and attempt to login with credentials
                //  var result = await WebRequests.PostAsync<ApiResponse<UserProfileDetailsApiModel>>(
                // Set URL
                //RouteHelpers.GetAbsoluteRoute(ApiRoutes.Login),
                //// Create api model
                //new LoginCredentialsApiModel
                //{
                //    UsernameOrEmail = Email,
                //    Password = (parameter as IHavePassword).SecurePassword.Unsecure()
                //});

                // If the response has an error...
                //  if (await result.HandleErrorIfFailedAsync("Login Failed"))
                // We are done
                //    return;

                // OK successfully logged in... now get users data
                //  var loginResult = result.ServerResponse.Response;

                // Let the application view model handle what happens
                // with the successful login
                // await ViewModelApplication.HandleSuccessfulLoginAsync(loginResult);
                this.applicationUserService.LogIn(this.Name, passwordBox.Password);


            });
            this.LoginIsRunning = false;
        }
    }
}

