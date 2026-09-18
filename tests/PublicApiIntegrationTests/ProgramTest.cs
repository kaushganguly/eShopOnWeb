using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Reflection;

namespace PublicApiIntegrationTests;

[TestClass]
public class ProgramTest
{
    private static object? _application;

    public static HttpClient NewClient
    {
        get
        {
            if (_application == null)
            {
                InitializeApplication();
            }

            // Use reflection to call CreateClient on the WebApplicationFactory instance
            var createClientMethod = _application!.GetType().GetMethod("CreateClient", Type.EmptyTypes);
            return (HttpClient)createClientMethod!.Invoke(_application, null)!;
        }
    }

    private static void InitializeApplication()
    {
        // Get the Program type from the PublicApi assembly
        var publicApiAssembly = typeof(Microsoft.eShopWeb.PublicApi.Middleware.ExceptionMiddleware).Assembly;
        var programType = publicApiAssembly.GetType("Program", throwOnError: true)!;

        // Create WebApplicationFactory<Program> dynamically to avoid type ambiguity
        var factoryType = typeof(WebApplicationFactory<>).MakeGenericType(programType);
        _application = Activator.CreateInstance(factoryType);
    }

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext _)
    {
        InitializeApplication();
    }
}
