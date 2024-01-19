using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZustSN.Business.Abstract;
using ZustSN.Entities;

namespace ZustSN.Business.Concrete
{
    public class UserService : IUserService
    {
        public Task Add(ZustIdentityUser user)
        {
            throw new NotImplementedException();
        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ZustIdentityUser>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ZustIdentityUser> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ZustIdentityUser> GetByUserName(string username)
        {
            throw new NotImplementedException();
        }

        public Task Update(ZustIdentityUser user)
        {
            throw new NotImplementedException();
        }
    }
}
