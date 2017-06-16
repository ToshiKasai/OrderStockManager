using OrderStockManager.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderStockManager.Repositories
{
    public interface IRoleRepository
    {
        IEnumerable<RoleInterfaceModel> GetRolesForInterface(BaseParameterModel parameter, bool isAdmin = false);
        Task<RepositoryResult<RoleInterfaceModel>> GetRoleByIdForInterfaceAsync(int roleId);
    }
}
