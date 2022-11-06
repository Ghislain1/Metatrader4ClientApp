using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metatrader4ClientApp.Infrastructure.Models
{
    public class ApplicationSettingInfo
    {
        public bool IsAutoUpdateEnabled { get; set; } = true;

        public bool IsDarkModeEnabled { get; set; } = true;

        public bool ShouldInjectTags { get; set; } = true; 

        public int ParallelLimit { get; set; } = 2;

        public int RefreshIntervalInMilliSeconde  { get; set; } = 1000;
    }
}
