using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.eShopWeb.PublicApi.Middleware;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace PublicApiIntegrationTests;

[TestClass]
public class ProgramTest
{
    // Use ExceptionMiddleware (a PublicApi-specific type) to anchor WebApplicationFactory
    // to the PublicApi assembly, avoiding the CS0433 ambiguity with Web's Program type.
    private static WebApplicationFactory<ExceptionMiddleware> _application = new();

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
        _application = new WebApplicationFactory<ExceptionMiddleware>();

    }
}
