using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.eShopWeb.PublicApi.AuthEndpoints;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace PublicApiIntegrationTests;

[TestClass]
public class ProgramTest
{
    // Use AuthenticateEndpoint (a public type from the PublicApi assembly) to avoid ambiguity
    // with Web's Program class — WebApplicationFactory<T> works with any type in the entry assembly
    private static WebApplicationFactory<AuthenticateEndpoint> _application = new();

    public static HttpClient NewClient
    {
        get
        {
            return _application.CreateClient();
        }
    }

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext _)
    {
        _application = new WebApplicationFactory<AuthenticateEndpoint>();

    }
}
