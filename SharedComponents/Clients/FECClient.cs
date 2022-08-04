using System.Threading.Tasks;
using MDWatch.Client;

namespace MDWatch.SolutionClients
{
    public abstract class FECClient
    {
        protected abstract void ConfigureEndPoint();

        public abstract Task SubmitAsync();

        public Configuration Config => _config;
        public string APIKey => _apiKey;
        protected Configuration _config;
        protected string _apiKey;
    }
}