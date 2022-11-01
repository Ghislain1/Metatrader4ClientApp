using Metatrader4ClientApp.Infrastructure.Models;
using System;


namespace Metatrader4ClientApp.Infrastructure.Models
{
    public class AccountPositionModelEventArgs : EventArgs
    {
        public AccountPositionModelEventArgs(AccountPosition position)
        {
            AcctPosition = position;
        }

        public AccountPosition AcctPosition { get; set; }
    }
}