using FECIngest.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace FECIngest
{
    public abstract class FECClient
    {
        protected abstract void ConfigureEndPoint();
        public abstract Task<bool> Submit();
        
        
        public Configuration Config => _config;
        public string APIKey => _apiKey;
        private protected Configuration _config;
        private protected string _apiKey;
        
    }
}