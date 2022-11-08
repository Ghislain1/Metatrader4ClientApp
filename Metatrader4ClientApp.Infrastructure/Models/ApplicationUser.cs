

namespace Metatrader4ClientApp.Infrastructure.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The user data and profile for our application
    /// </summary>
    public class ApplicationUser
    {
        public ConnectionParameter[] ConnectionParameter { get; set; } = new ConnectionParameter[AppConstants.MaxNumberOfConnection];

    }
}
