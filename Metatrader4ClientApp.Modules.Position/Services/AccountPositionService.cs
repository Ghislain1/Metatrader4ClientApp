﻿using Metatrader4ClientApp.Infrastructure.Interfaces;
using Metatrader4ClientApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metatrader4ClientApp.Modules.Position.Services
{
    public class AccountPositionService : IAccountPositionService
    {
        List<AccountPosition> _positions = new List<AccountPosition>();

        public AccountPositionService()
        {
            InitializePositions();
        }

        #region IAccountPositionService Members

        public event EventHandler<AccountPositionModelEventArgs> Updated = delegate { };

        public IList<AccountPosition> GetAccountPositions()
        {
            _positions=   Enumerable.Range(1, 8).Select(ite => new AccountPosition() { CostBasis =  decimal.Parse("1.5", CultureInfo.InvariantCulture), Shares =ite, TickerSymbol="STOCK "+ite }).ToList();
            return _positions;
        }

        public async Task<IList<AccountPosition>> GetAccountPositionsAsync()
        {
            return await Task.Run(this.GetAccountPositions);
        }
        #endregion

        private void InitializePositions()
        {
            //var resourcesAccountPositions = XDocument.Load(DataLocationNames.AccountPositionsXml);
            //using (var sr = new StringReader(resourcesAccountPositions.ToString()))
            //{
            //    XDocument document = XDocument.Load(sr);
            //    _positions = document.Descendants("AccountPosition")
            //        .Select(
            //        x => new AccountPosition(x.Element("TickerSymbol").Value,
            //                                 decimal.Parse(x.Element("CostBasis").Value, CultureInfo.InvariantCulture),
            //                                 long.Parse(x.Element("Shares").Value, CultureInfo.InvariantCulture)))
            //        .ToList();
            //}
        }

    }
}