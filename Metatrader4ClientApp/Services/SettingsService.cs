namespace Metatrader4ClientApp.Services
{
    using Metatrader4ClientApp.Infrastructure.Interfaces;
    using Metatrader4ClientApp.Infrastructure.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SettingsService : ISettingsService
    {
        private readonly IExportService exportService;
        public SettingsService(IExportService exportService)
        {
            //Configuration.StorageSpace = StorageSpace.Instance;
            //Configuration.SubDirectoryPath = "";
            //Configuration.FileName = "Settings.dat";
            this.exportService = exportService;
        }

        public ApplicationSettingInfo Get()
        {
            return new ApplicationSettingInfo();
        }

        public async Task<ApplicationSettingInfo> GetAsync()
        {
            return await Task.Run(this.Get);
        }
    }
}