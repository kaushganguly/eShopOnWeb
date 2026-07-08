using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.eShopWeb.PublicApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace PublicApiIntegrationTests;

[TestClass]
public class ProgramTest
{
    // Use TestAnchor (a named type from the PublicApi assembly) instead of Program to avoid
    // CS0433: both PublicApi and Web define a top-level Program class in the global namespace.
    private static WebApplicationFactory<TestAnchor> _application = new();

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
        _application = new WebApplicationFactory<TestAnchor>();

    }
}
