

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


        /// <summary>
        /// the user's full name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  Password, which is shased
        /// </summary>
        /// 
        public string? Password { get; set; }


        /// <summary>
        /// The email address.
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// contact telephone number
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// account type to be selected from the list defined by the brokerage company.
        /// </summary>
        public string? AccountType { get; set; }
        /// <summary>
        /// the amount of the initial deposit in terms of the basic currency.The minimum amount is 10 units of the specified currency.
        /// </summary>

        public string? Deposit { get; set; }
        /// <summary>
        ///      Currency – the basic currency of the deposit to be set automatically depending on the account type selected.
        /// </summary>
        /// 

        public string? Currency { get; set; }
        /// <summary>
        ///  – the ratio between the borrowed and owned funds for trading.
        /// </summary>  
        public string? Leverage { get; set; }
    }
}
