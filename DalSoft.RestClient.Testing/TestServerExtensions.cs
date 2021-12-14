using Microsoft.AspNetCore.TestHost;
namespace DalSoft.RestClient.Testing
{
    public static class TestServerExtensions
    {
        public static RestClient CreateRestClient(this TestServer server, Config restClientConfig = null, Headers defaultRequestHeaders = null)
        {
            restClientConfig ??= new Config();
            restClientConfig.UseHandler(server.CreateHandler());
            return new RestClient(server.BaseAddress.ToString(), defaultRequestHeaders, restClientConfig);
        }
    }
}
