using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FECIngest
{
    public interface IFECSearcher
    {
        string APIKey { get; }
        Task<bool> Submit();
    }
}
