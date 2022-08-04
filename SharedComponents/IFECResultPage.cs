using System;
using System.Collections.Generic;
using System.Text;

namespace MDWatch
{
    public interface IFECResultPage
    {
        public bool IsLastPage { get; }
        public object PageData { get; }
        
    }
}
