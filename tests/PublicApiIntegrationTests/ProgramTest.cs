using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.eShopWeb.PublicApi.Middleware;
using System.Net.Http;

namespace PublicApiIntegrationTests;

// Use ExceptionMiddleware (unique to PublicApi assembly) instead of Program
// to avoid CS0433 ambiguity with Web's auto-generated public partial Program class
[TestClass]
public class ProgramTest
{
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
