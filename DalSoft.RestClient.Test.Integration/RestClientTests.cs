using System.Net.Http;
using System.Threading.Tasks;
using DalSoft.RestClient.Examples;
using DalSoft.RestClient.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DalSoft.RestClient.Test.Integration
{
    public class RestClientTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        [Fact]
        public async Task Verify_VerifyingResponseUsingRestClient_ShouldVerifyResponseAsExpected()
        {
            var client = new RestClient("https://jsonplaceholder.typicode.com/");

            await client.Resource("users/1").Get()
                .Verify<HttpResponseMessage>(response => response.IsSuccessStatusCode) // Verify using HttpResponseMessage
                .Verify<string>(s => s.Contains("Leanne Graham")) // Verify string response body
                .Verify<User>(user => user.Username == "Bret") // Verify model
                .Verify(o => o.username == "Bret") // Verify dynamically
                .Verify(o => o.HttpResponseMessage.IsSuccessStatusCode); // Verify dynamically
        }

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
        }
    }
}
