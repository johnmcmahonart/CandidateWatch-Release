using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace MDWatch
{
    public interface IFECPagination
    {
        Task<IFECResultPage> GetNextPage();
    }
}
