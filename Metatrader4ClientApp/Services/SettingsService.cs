using Metatrader4ClientApp.Infrastructure.Interfaces;
using Metatrader4ClientApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metatrader4ClientApp.Services
{
    public class SettingsService : ISettingsService
    {
        public SettingsService()
        {
            //Configuration.StorageSpace = StorageSpace.Instance;
            //Configuration.SubDirectoryPath = "";
            //Configuration.FileName = "Settings.dat";
        }

        public ApplicationSettingInfo Get()
        {
           return new ApplicationSettingInfo();
        }

        public  async Task<ApplicationSettingInfo> GetAsync()
        {
            return await Task.Run(this.Get);
        }
    }
}
