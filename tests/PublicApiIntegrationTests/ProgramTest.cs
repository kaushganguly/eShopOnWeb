extern alias WebAlias;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

// Now we can use Program from PublicApi without ambiguity
namespace PublicApiIntegrationTests;

[TestClass]
public class ProgramTest
{
    // Reference the PublicApi Program class - no ambiguity now
    private static WebApplicationFactory<global::Program>? _application;

    public static HttpClient NewClient
    {
        get
        {
            _application ??= new WebApplicationFactory<global::Program>();
            return _application.CreateClient();
        }
    }

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext _)
    {
        _application = new WebApplicationFactory<global::Program>();
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        _application?.Dispose();
    }
}
