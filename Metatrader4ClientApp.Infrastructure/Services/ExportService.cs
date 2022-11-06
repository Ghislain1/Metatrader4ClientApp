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
        private bool CreateCsv(IEnumerable<AccountPosition> accountPositions, string filePath)
        {
            try
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine($"{nameof(AccountPosition.TickerSymbol)},{nameof(AccountPosition.Shares)}");

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

        private bool CreateTxt(IEnumerable<AccountPosition> accountPositions, string filePath)
        {
            try
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine($"{nameof(AccountPosition.TickerSymbol)},{nameof(AccountPosition.Shares)}");

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
        private bool CreateJson(IEnumerable<AccountPosition> accountPositions, string filePath)
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
        private bool CreateXml(IEnumerable<AccountPosition> accountPositions, string filePath)
        {
            try
            {
        

                var document = new XDocument(DefaultXDeclaration,

           new XElement("ApplicationName",
               new XElement(nameof(AccountPosition) + "s",

               from info in accountPositions
               select
                   new XElement(nameof(AccountPosition),
                       new XElement(nameof(AccountPosition.TickerSymbol), info.TickerSymbol),
                       new XElement(nameof(AccountPosition.Shares), info.Shares),
                       new XElement(nameof(AccountPosition.CostBasis), info.CostBasis)
                 ))));

                document.Save(filePath);
            }
            catch (Exception)
            {

                return false;
            }


            return true;
        }

       

        public   bool Export(IEnumerable<AccountPosition> accountPositions, string filePath, ExportFileType fileType)
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
