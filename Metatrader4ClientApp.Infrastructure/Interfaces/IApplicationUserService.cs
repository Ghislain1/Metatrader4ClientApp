using Metatrader4ClientApp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metatrader4ClientApp.Infrastructure.Interfaces
{
    public interface IApplicationUserService
    {
        IList<ApplicationUser> GetUsers();
        Task<IList<ApplicationUser>> GetUsersAsync();
    }
}
