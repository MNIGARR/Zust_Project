using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZustSN.Entities;

namespace ZustSN.Business.Abstract
{
    public interface IUserService
    {
        Task<List<ZustIdentityUser>> GetAll();
        Task Add(ZustIdentityUser user);
        Task Update(ZustIdentityUser user);
        Task Delete(string id);
        Task<ZustIdentityUser> GetById(string id);
        Task<ZustIdentityUser> GetByUserName(string username);
    }
}
