using System.Net.Http;
using DalSoft.RestClient.DependencyInjection;

namespace DalSoft.RestClient.Test.Integration
{
    public class MockRestClientFactory : IRestClientFactory
    {
        public RestClient CreateClient()
        {
            return new RestClient
            (
                "http://NotUsedAsMockResponseReturned",
                new Config()
                    .UseUnitTestHandler(request => new HttpResponseMessage()
                    {
                        Content = new StringContent("[{ \"name\": \"Hello World\" }]")
                    })
            );
        }

        public RestClient CreateClient(string name)
        {
            return CreateClient();
        }
    }
}