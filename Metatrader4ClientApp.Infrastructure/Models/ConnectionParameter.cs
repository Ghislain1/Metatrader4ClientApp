﻿// <copyright company="GhislainOne Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Metatrader4ClientApp.Infrastructure.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// See http://mtapi.online/ford/form.html
    /// </summary>
    public class ConnectionParameter
    {
        /// <summary>
        /// the user
        /// </summary>
        public int AccountNumber { get; set; } = 500478235;

        public int Port { get; set; } = 443;

        public string? Password { get; set; } = "ywh3ejc";

        public string? Host { get; set; } = "mt4-demo.roboforex.com";

      
    }
}