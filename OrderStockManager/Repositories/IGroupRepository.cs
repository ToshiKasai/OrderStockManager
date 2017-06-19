using OrderStockManager.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderStockManager.Repositories
{
    public interface IGroupRepository
    {
        IEnumerable<GroupInterfaceModel> GetGroupsForInterface(CustomParameterModel parameter);
        RepositoryResult<GroupInterfaceModel> GetGroupByIdForInterface(int groupId);
        RepositoryResult<GroupInterfaceModel> GetGroupByCodeForInterface(string groupCode);
        RepositoryResult<GroupInterfaceModel> CreateGroup(GroupInterfaceModel createGroup);
        RepositoryResult<GroupInterfaceModel> ModifyGroup(int groupId, GroupInterfaceModel modifiedGroup);
        object GetGroupsPages(CustomParameterModel parameter);
        RepositoryResult<IEnumerable<ProductInterfaceModel>> GroupModifyProducts(int groupId, List<ProductInterfaceModel> ProductList);
    }
}
