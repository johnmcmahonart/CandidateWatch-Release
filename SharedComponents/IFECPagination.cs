using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace FECIngest
{
    public interface IFECPagination
    {
        Task<IFECResultPage> GetNextPage();
    }
}
