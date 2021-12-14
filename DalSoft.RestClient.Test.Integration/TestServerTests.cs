using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DalSoft.RestClient.Examples;
using DalSoft.RestClient.Examples.Models;
using DalSoft.RestClient.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace DalSoft.RestClient.Test.Integration
{
    public class TestServerTests
    {
        [Fact]
        public async Task TestServer_VerifyingResponseUsingCreateRestClient_ShouldVerifyResponseAsExpected()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>(); // just an example for real use a Fixture

            var testServer = new TestServer(builder);

            var client = testServer.CreateRestClient();

            await client.Resource("examples/createclient")
                .Get()
                .Verify<HttpResponseMessage>(response => response.IsSuccessStatusCode)
                .Verify<List<Repository>>(r => r.FirstOrDefault().name != null);

        }
        
        [Fact]
        public async Task TestServer_VerifyingResponseUsingCreateRestClientWithCustomHandler_ShouldVerifyResponseAsExpected()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>(); // just an example for real use a Fixture

            var testServer = new TestServer(builder);

            var client = testServer.CreateRestClient(new Config().UseNoDefaultHandlers().UseUnitTestHandler(_ => new HttpResponseMessage { Content = new StringContent("Handler has been called") }));

            var result = await client
                .Resource("examples/createclient")
                .Get()
                .Act<string>(x =>
                {
                    Assert.Equal("Handler has been called", x);
                });
        }
    }
}