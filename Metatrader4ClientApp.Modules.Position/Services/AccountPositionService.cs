

namespace Metatrader4ClientApp.Modules.Position.Services
{
    using Metatrader4ClientApp.Infrastructure.Interfaces;
    using Metatrader4ClientApp.Infrastructure.Models;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class AccountPositionService : IAccountPositionService
    {
        

        public AccountPositionService()
        {
         
        }

 

        public event EventHandler<AccountPositionModelEventArgs> Updated = delegate { };

        public IList<AccountPosition> GetAccountPositions()
        {
           
            return Enumerable.Range(1, 8).Select(ite => new AccountPosition() { CostBasis = decimal.Parse("1.5", CultureInfo.InvariantCulture), Shares = ite, TickerSymbol = "STOCK " + ite }).ToList(); ;
        }

        public async Task<IList<AccountPosition>> GetAccountPositionsAsync()
        {
            return await Task.Run(this.GetAccountPositions);
        }
 
        // TODO : Columnseparator in Setting page 
        public void ExportToTextFile<T>(IEnumerable<T> data, string fileName, char columnSeperator = ';')
        {
            using (var sw = File.CreateText(fileName))
            {
                var plist = typeof(T).GetProperties().Where(p => p.CanRead && (p.PropertyType.IsValueType || p.PropertyType == typeof(string)) && p.GetIndexParameters().Length == 0).ToList();
                if (plist.Count > 0)
                {
                    var seperator = columnSeperator.ToString();
                    sw.WriteLine(string.Join(seperator, plist.Select(p => p.Name)));
                    foreach (var item in data)
                    {
                        var values = new List<object>();
                        foreach (var p in plist) values.Add(p.GetValue(item, null));
                        sw.WriteLine(string.Join(seperator, values));
                    }
                }
            }
        }

        public async Task ExportToTextFileAsync<T>(IEnumerable<T> data, string fileName, char columnSeperator = ';')
        {
            await Task.Run(() => this.ExportToTextFile(data, fileName, columnSeperator));
        }

    }
}