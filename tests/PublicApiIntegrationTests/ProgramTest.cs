extern alias PublicApiAssembly;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace PublicApiIntegrationTests;

[TestClass]
public class ProgramTest
{
    // Note: Program class exists in both PublicApi and Web assemblies.
    // We're using an extern alias to reference the PublicApi version.
    private static WebApplicationFactory<PublicApiAssembly::Program> _application = new();

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
        _application = new WebApplicationFactory<PublicApiAssembly::Program>();

    }
}
