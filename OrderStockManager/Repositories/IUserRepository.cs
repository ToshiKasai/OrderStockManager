using Microsoft.AspNet.Identity;
using OrderStockManager.Models;
using OrderStockManager.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OrderStockManager.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<UserInterfaceModel> GetUsersForInterface(BaseParameterModel parameter);
        Task<RepositoryResult<UserInterfaceModel>> GetUserByIdForInterfaceAsync(int id);
        Task<RepositoryResult<UserInterfaceModel>> GetUserByNameForInterfaceAsync(string userName);
        Task<RepositoryResult<UserInterfaceModel>> CreateUserAsync(UserInterfaceModel createUser);
        Task<RepositoryResult<UserInterfaceModel>> ModifyUserAsync(int id, UserInterfaceModel modifiedUser);
        Task<RepositoryResult<UserInterfaceModel>> DeleteUserAsync(int id);
        object GetUsersPages(BaseParameterModel parameter);

        Task<RepositoryResult<IList<string>>> GetRoleByUserIdAsync(int userId, bool isAdmin = false);
        Task<RepositoryResult<string>> GetRoleByUserIdAndRoleNameAsync(int userId, string roleName, bool isAdmin = false);
        Task<RepositoryResult<IList<string>>> SetRoleByUserIdAsync(int userId, IList<string> roleList, bool isAdmin = false);
    }
}
