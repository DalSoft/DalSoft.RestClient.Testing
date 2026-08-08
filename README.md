# DalSoft C# RestClient Testing

### `If you find this repo / package useful all I ask is you please star it ⭐`
> ### Do you or the company you work for benefit from the tools I build? <br /> If so please consider [Becoming a Sponsor](https://github.com/sponsors/dalsoft) it would be greatly appreciated ❤️

* Extends DalSoft RestClient to Make testing Rest API's or anything HTTP trivial.
* Use DalSoft RestClient with [ASP.NET Core In-Memory Test Server for integration tests](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.2).


## Getting Started

## Install via .NET CLI

```bash
> dotnet add package DalSoft.RestClient.Testing
```

## Install via NuGet

```bash
PM> Install-Package DalSoft.RestClient.Testing
```

## DalSoft.RestClient 5.0

From version 5.0 DalSoft.RestClient uses [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/overview) to deserialize responses - `[JsonPropertyName]` attributes are honoured and property matching is case sensitive. If your models rely on the legacy Json.NET behaviour (`[JsonProperty]` attributes, case insensitive matching etc.) opt in to the fallback with `new Config().UseNewtonsoftJson()` - see the [DalSoft.RestClient readme](https://github.com/DalSoft/DalSoft.RestClient) for the breaking changes.

## Using the Verify extension method to fluently test anything HTTP.


Just pass an type and expression returning a boolean to Verify.

```cs
[Fact]
public async Task GetUser_ProvidingAValidUserId_ReturnsExpectedResponse()
{
	var client = new RestClient("https://jsonplaceholder.typicode.com/");

	await client.Resource("users/1").Get()
		.Verify<HttpResponseMessage>(response => response.IsSuccessStatusCode) // Verify using HttpResponseMessage
		.Verify<string>(s => s.Contains("Leanne Graham")) // Verify string response body
		.Verify<User>(user => user.Username == "Bret") // Verify by casting to your model
		.Verify(o => o.username == "Bret") // Verify dynamically
		.Verify(o => o.HttpResponseMessage.IsSuccessStatusCode); // Verify dynamically
}

public class User
{
	// From 5.0 RestClient uses System.Text.Json which matches case sensitively, so map the property names
	[JsonPropertyName("username")]
	public string Username { get; set; }
}
```

If you change the test to fail for example change "Leanne Graham" to "XXX Leanne Graham" the test will fail. 

A Test will fail on the first Verify failure, and won't carry on Verifying inline with how your would expect Assert to work.

When Verify fails it throws a meaningful exception which can be read in the test output, for example in the test above:

```bash
s => s.Contains("XXX Leanne Graham") was not verified
```

## Integration Testing using ASP.NET Core In-Memory Test Server

If you haven't used ASP.NET Core's In-Memory Test Server for integration testing, it's worth heading over to ASP.NET Core [Testing documentation](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.2).

DalSoft RestClient Testing extends both TestServer and WebApplicationFactory, you just call the `CreateRestClient` extension method of instead of `CreateClient`

Using TestServer:

```cs
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
}    	
```

Using WebApplicationFactory:

```cs
public class WebApplicationFactoryTests : IClassFixture<WebApplicationFactory<Startup>>
{
	private readonly WebApplicationFactory<Startup> _factory;

	public WebApplicationFactoryTests(WebApplicationFactory<Startup> factory)
	{
	    _factory = factory;
	}

	[Fact]
	public async Task TestServer_VerifyingResponseUsingCreateRestClient_ShouldVerifyResponseAsExpected()
	{
	    var client = _factory.WithWebHostBuilder(builder =>
	    {
		builder.ConfigureServices(services =>
		    {
			services.AddSingleton<IRestClientFactory>(provider => new MockRestClientFactory()); // Return Mock Response
		    });
	    }).CreateRestClient(new Config());

	    var result = await client
		.Resource("examples/createclient")
		.Get()
		    .Verify<HttpResponseMessage>(response => response.IsSuccessStatusCode)
		    .Verify<List<Repository>>(repositories => repositories.FirstOrDefault().name == "Hello World"); // Test Mock was used
	}
}


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

```

## Supported Platforms

RestClient targets .NET Standard 2.0 and .NET 8.0 therefore **supports Windows, Linux, Mac and Xamarin (iOS, Android and UWP)**.

DalSoft.RestClient.Testing targets .NET 8.0 - TestServer and WebApplicationFactory works with .NET 8.0 onwards

