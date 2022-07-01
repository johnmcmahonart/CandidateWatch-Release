using System;
using System.Collections.Generic;
using System.Text;

namespace FECIngest
{
    public interface IFECQueryParms
    {
        public void SetQuery(Dictionary<string, string> parms);
    }
}
