using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Reflection;

namespace PublicApiIntegrationTests;

[TestClass]
public class ProgramTest
{
    private static dynamic _application = null!;

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
        // Load the Program type from PublicApi assembly
        var publicApiAssembly = Assembly.Load("PublicApi");
        var programType = publicApiAssembly.GetType("Program") ?? throw new InvalidOperationException("Cannot find Program type in PublicApi");
        
        var factoryType = typeof(WebApplicationFactory<>).MakeGenericType(programType);
        _application = Activator.CreateInstance(factoryType)!;
    }
}
