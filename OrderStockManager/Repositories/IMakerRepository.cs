using OrderStockManager.Models.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderStockManager.Repositories
{
    public interface IMakerRepository
    {
        IEnumerable<MakerInterfaceModel> GetMakersForInterface(BaseParameterModel parameter);
        RepositoryResult<MakerInterfaceModel> GetMakerByIdForInterface(int makerId);
        RepositoryResult<MakerInterfaceModel> ModifyMaker(int makerId, MakerInterfaceModel modifiedMaker);
        object GetMakersPages(BaseParameterModel parameter);
        IEnumerable<MakerInterfaceModel> GetMakersByUserIdForInterface(int userId);
    }
}
