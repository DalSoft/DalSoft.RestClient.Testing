using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace DalSoft.RestClient.Testing
{
    public static class WebApplicationFactoryExtensions
    {
        public static RestClient CreateRestClient<TEntryPoint>(this WebApplicationFactory<TEntryPoint> webApplicationFactory, Config restClientConfig = null, Headers defaultRequestHeaders = null) where TEntryPoint : class
        {
            if (webApplicationFactory.Server == null) // wouldn't need to do this if base.EnsureServerCreated() was protected
                webApplicationFactory.CreateDefaultClient(); // Creates server ironically 

            restClientConfig = restClientConfig ?? new Config();
            restClientConfig.UseHandler(webApplicationFactory.Server.CreateHandler());

            // Create the HttpClient using WebApplicationFactory so that it deals with disposing 
            var httpClient = webApplicationFactory.CreateDefaultClient(webApplicationFactory.ClientOptions.BaseAddress, (DelegatingHandler)restClientConfig.CreatePipeline());

            return webApplicationFactory.Server.CreateRestClient(httpClient, defaultRequestHeaders);
        }

        private static RestClient CreateRestClient(this TestServer server, HttpClient httpClient, Headers defaultRequestHeaders = null)
        {
            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient));

            return new RestClient(new HttpClientWrapper(httpClient, defaultRequestHeaders), server.BaseAddress.ToString());
        }
    }
}