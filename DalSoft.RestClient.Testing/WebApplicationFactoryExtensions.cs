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
                webApplicationFactory.CreateRestClient(restClientConfig, defaultRequestHeaders);; // Creates server ironically 

            restClientConfig ??= new Config();
            restClientConfig.UseHandler(webApplicationFactory.Server.CreateHandler());
            
            return webApplicationFactory.Server.CreateRestClient(restClientConfig, defaultRequestHeaders);
        }
    }
}