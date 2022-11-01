using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Metatrader4ClientApp.Infrastructure.Models;

using Newtonsoft.Json;

namespace Metatrader4ClientApp.Infrastructure.Services
{
    internal static class ApplicationUserRepository
    {
        // C:\ProgramData\
        public static string CommonApplicationData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);
        public static string LoginPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "Metatrader4ClientApp");
        private static readonly string ApplicationUserFilePath = Path.Combine(LoginPath, "ApplicationUser.json");
        static ApplicationUserRepository()
        {
            if (!Directory.Exists(LoginPath))
            {
                Directory.CreateDirectory(LoginPath);

            }
            
            if (!File.Exists(ApplicationUserFilePath))
            {
               
                using (File.Create(ApplicationUserFilePath));
            }




        }
        public static void Save(ApplicationUser applicationUser)
        {
            // Read existing json data
            var jsonData = System.IO.File.ReadAllText(ApplicationUserFilePath);
            // De-serialize to object or create new list
            var applicationUserList = JsonConvert.DeserializeObject<List<ApplicationUser>>(jsonData)
                                  ?? new List<ApplicationUser>();

            // Add any new employees
            applicationUserList.Add(applicationUser);

            // Update json data string           

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(ApplicationUserFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, applicationUserList);
            }

        }

        public static List<ApplicationUser> LoadAll() => JsonConvert.DeserializeObject<List<ApplicationUser>>(System.IO.File.ReadAllText(ApplicationUserFilePath)) ?? new List<ApplicationUser>();

    }
}
