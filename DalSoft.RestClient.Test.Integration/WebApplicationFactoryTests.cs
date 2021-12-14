using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DalSoft.RestClient.DependencyInjection;
using DalSoft.RestClient.Examples;
using DalSoft.RestClient.Examples.Models;
using DalSoft.RestClient.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DalSoft.RestClient.Test.Integration
{
    public class WebApplicationFactoryTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public WebApplicationFactoryTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task WebApplicationFactory_VerifyingResponseUsingCreateRestClient_ShouldVerifyResponseAsExpected()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                    {
                        services.AddSingleton<IRestClientFactory>(provider => new MockRestClientFactory());
                    });
            }).CreateRestClient(new Config());

            var result = await client
                .Resource("examples/createclient")
                .Get()
                    .Verify<HttpResponseMessage>(response => response.IsSuccessStatusCode)
                    .Verify<List<Repository>>(repositories => repositories.FirstOrDefault().name == "Hello World")
                    .Verify<HttpResponseMessage>(message => message.RequestMessage.RequestUri.ToString() == "http://localhost/examples/createclient");
        }
        
        [Fact]
        public async Task WebApplicationFactory_VerifyingResponseUsingCreateRestClientWithCustomHandler_ShouldVerifyResponseAsExpected()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IRestClientFactory>(provider => new MockRestClientFactory());
                });
            }).CreateRestClient(new Config()
                .UseNoDefaultHandlers()
                .UseUnitTestHandler(_ => new HttpResponseMessage { Content = new StringContent("Handler has been called") }));

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
