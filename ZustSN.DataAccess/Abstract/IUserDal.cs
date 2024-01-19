using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZustSN.Core.DataAccess;
using ZustSN.Entities;

namespace ZustSN.DataAccess.Abstract
{
    public interface IUserDal: IEntityRepository<ZustIdentityUser>
    {
    }
}
