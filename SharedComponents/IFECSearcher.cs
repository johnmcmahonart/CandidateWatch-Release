using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FECIngest.Model;
using FECIngest.Client;
namespace FECIngest
{
    public interface IFECSearcher
    {
        Configuration Configuration { get; }
        string APIKey { get; }
        Task<bool> Submit();
    }
}
