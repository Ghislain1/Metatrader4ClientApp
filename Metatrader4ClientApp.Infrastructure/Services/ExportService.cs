// <copyright company="ROSEN Swiss AG">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

namespace Metatrader4ClientApp.Infrastructure.Services
{
    using Metatrader4ClientApp.Infrastructure.Interfaces;
    using Metatrader4ClientApp.Infrastructure.Models;
    using Microsoft.VisualBasic;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public  class ExportService: IExportService
    {

        /// <summary>
        /// Default declaration for XML documents.
        /// </summary>
        private static readonly XDeclaration DefaultXDeclaration = new XDeclaration("1.0", "utf-8", "yes");
        private bool CreateCsv(IEnumerable<TradeItem> tradeItems, string filePath)
        {
            try
            {
                var stringBuilder = new StringBuilder();

               // stringBuilder.AppendLine($"{nameof(AccountPosition.TickerSymbol)},{nameof(AccountPosition.Shares)}");

                foreach (var info in tradeItems)
                {
                    stringBuilder.AppendLine($"{info.TickerSymbol},{info.Shares},{info.CostBasis}");
                }
                System.IO.File.WriteAllText(filePath, stringBuilder.ToString());
            }
            catch (Exception)
            {

                return false;
            }


            return true;
        }
       /// <summary>
       /// Other nice methdo to export data
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="data"></param>
       /// <param name="fileName"></param>
       /// <param name="columnSeperator"></param>
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
        private bool CreateTxt(IEnumerable<TradeItem> accountPositions, string filePath)
        {
            try
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine($"{nameof(TradeItem.TickerSymbol)},{nameof(TradeItem.Shares)}");

                foreach (var info in accountPositions)
                {
                    stringBuilder.AppendLine($"{info.TickerSymbol},{info.Shares},{info.CostBasis}");
                }
                System.IO.File.WriteAllText(filePath, stringBuilder.ToString());
            }
            catch (Exception)
            {

                return false;
            }


            return true;
        }
        private bool CreateJson(IEnumerable<TradeItem> accountPositions, string filePath)
        {
            try
            {
                var collection = accountPositions.ToList();
                var jsonData = new object[collection.Count];

                for (var i = 0; i < collection.Count; i++)
                {
                    jsonData[i] = new
                    {
                        collection[i].TickerSymbol,
                        ChannelCenterFrequencyInKilohertz = collection[i].TickerSymbol.ToString(),
                        Shares = collection[i].Shares.ToString(),
                        CostBasis = collection[i].CostBasis.ToString()
                       
                    };
                }

                System.IO.File.WriteAllText(filePath, JsonConvert.SerializeObject(jsonData, Formatting.Indented));
            }
            catch (Exception)
            {

                return false;
            }


            return true;
        }
        private bool CreateXml(IEnumerable<TradeItem> accountPositions, string filePath)
        {
            try
            {
        

                var document = new XDocument(DefaultXDeclaration,

           new XElement("ApplicationName",
               new XElement(nameof(TradeItem) + "s",

               from info in accountPositions
               select
                   new XElement(nameof(TradeItem),
                       new XElement(nameof(TradeItem.TickerSymbol), info.TickerSymbol),
                       new XElement(nameof(TradeItem.Shares), info.Shares),
                       new XElement(nameof(TradeItem.CostBasis), info.CostBasis)
                 ))));

                document.Save(filePath);
            }
            catch (Exception)
            {

                return false;
            }


            return true;
        }

       

        public   bool Export(IEnumerable<TradeItem> accountPositions, string filePath, ExportFileType fileType)
        {
            return fileType switch
            {
                ExportFileType.CSV => this.CreateCsv(accountPositions, filePath),
                ExportFileType.XML => this.CreateXml(accountPositions, filePath),
                ExportFileType.JSON => this.CreateJson(accountPositions, filePath),
                ExportFileType.TXT => this.CreateTxt(accountPositions, filePath),
                _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
            };
          
        }
    }
}
