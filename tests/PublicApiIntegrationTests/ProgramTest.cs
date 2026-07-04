using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.eShopWeb.PublicApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace PublicApiIntegrationTests;

[TestClass]
public class ProgramTest
{
    // Use MappingProfile as the anchor type to avoid CS0433 ambiguity:
    // both PublicApi and Web expose a top-level `Program` class; using a
    // named type from Microsoft.eShopWeb.PublicApi unambiguously targets
    // the PublicApi entry-point assembly.
    private static WebApplicationFactory<MappingProfile> _application = new();

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
        _application = new WebApplicationFactory<MappingProfile>();

    }
}
