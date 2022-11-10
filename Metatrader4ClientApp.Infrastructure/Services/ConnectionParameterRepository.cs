

namespace Metatrader4ClientApp.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Metatrader4ClientApp.Infrastructure.Models;
    using Newtonsoft.Json;
    internal static class ConnectionParameterRepository
    {
        private static readonly string ConnectionParameterFilePath = Path.Combine(KnownFolders.Mt4Path, "ConnectionParameters.json");
        static ConnectionParameterRepository()
        {

            if (!File.Exists(ConnectionParameterFilePath))
            {
                try
                {

                    Directory.CreateDirectory(KnownFolders.Mt4Path);
                    using (File.Create(ConnectionParameterFilePath)) ;
                }
                catch (Exception s)
                {
                    // Add Log4net to log it
                    Console.WriteLine(s.Message);
                }

                // CreateDefaultConnectionParameter();
            }




        }
        public static void CreateDefaultConnectionParameter()
        {
            //var connectionParameter1 = new ConnectionParameter() { Host = "mt4-demo.roboforex.com", AccountNumber = 500478235, Password = "ywh3ejc", Port = 443 };
            var connectionParameter2 = new ConnectionParameter() { Host = "mt4-demo.roboforex.com", AccountNumber = 500476959, Password = "ehj4bod", Port = 443 };
            // connectionParameter1.Password = HashManager.HashPassword(connectionParameter1.Password);
            connectionParameter2.Password = HashManager.HashPassword(connectionParameter2.Password);
            //Save(connectionParameter1);
            Save(connectionParameter2);
        }
        public static void Save(ConnectionParameter connectionParameter)
        {
            // Read existing json data
            var jsonData = System.IO.File.ReadAllText(ConnectionParameterFilePath);
            // De-serialize to object or create new list
            var applicationUserList = JsonConvert.DeserializeObject<List<ConnectionParameter>>(jsonData)
                                  ?? new List<ConnectionParameter>();

            // Add any new employees
            applicationUserList.Add(connectionParameter);

            // Update json data string           

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(ConnectionParameterFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, applicationUserList);
            }

        }

        public static List<ConnectionParameter> LoadAll() => JsonConvert.DeserializeObject<List<ConnectionParameter>>(System.IO.File.ReadAllText(ConnectionParameterFilePath)) ?? new List<ConnectionParameter>();

    }
}
