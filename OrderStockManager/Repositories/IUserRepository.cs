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
        Task<UserInterfaceModel> GetUserByIdForInterfaceAsync(int id);
        Task<UserInterfaceModel> GetUserByNameForInterfaceAsync(string userName);
        Task<IdentityResult> CreateUserAsync(UserInterfaceModel createUser);
        Task<IdentityResult> ModifyUserAsync(int id, UserInterfaceModel modifiedUser);
        Task<IdentityResult> DeleteUserAsync(int id);
        object GetUsersPages(BaseParameterModel parameter);
    }
}
