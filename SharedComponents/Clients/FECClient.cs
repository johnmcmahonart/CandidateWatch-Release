using System.Threading.Tasks;
using MDWatch.Client;

namespace MDWatch.SolutionClients
{
    public abstract class FECClient
    {
        private protected abstract void ConfigureEndPoint();

        public abstract Task SubmitAsync();

        public Configuration Config => _config;
        public string APIKey => _apiKey;
        private protected Configuration _config;
        private protected string _apiKey;
    }
}