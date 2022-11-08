// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Metatrader4ClientApp.Modules.Trade
{
    using Metatrader4ClientApp.Infrastructure.Models;
    using Prism.Mvvm;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ConnectionParameterViewModel : BindableBase
    {
        private DateTime connectionTime;
        private bool isConnectionSuccess;
        public ConnectionParameterViewModel(ConnectionParameter model)
        {
            this.AccountNumber = model.AccountNumber;
            this.Port = model.Port;
            this.Host = model.Host;
            this.AccountNumber = model.AccountNumber;
        }
        public int AccountNumber { get; }

        public int Port { get; }

        public bool IsConnectionSuccess
        {
            get => this.isConnectionSuccess;
            set => this.SetProperty(ref this.isConnectionSuccess, value);
        }

        public string? Host { get; }

        public DateTime ConnectionTime
        {
            get => this.connectionTime;
            set => this.SetProperty(ref this.connectionTime, value);
        }
    }
}
