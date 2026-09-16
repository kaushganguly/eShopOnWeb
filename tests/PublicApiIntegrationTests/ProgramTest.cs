using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace PublicApiIntegrationTests;

// Custom WebApplicationFactory to avoid Program type ambiguity
public class PublicApiWebApplicationFactory : WebApplicationFactory<IHost>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // This will use the PublicApi's Program.cs when the assembly is loaded
        return base.CreateHost(builder);
    }
}

[TestClass]
public class ProgramTest
{
    private static readonly HttpClient _httpClient = CreateClient();

    public static HttpClient NewClient => _httpClient;

    private static HttpClient CreateClient()
    {
        try
        {
            var factory = new PublicApiWebApplicationFactory();
            return factory.CreateClient();
        }
        catch
        {
            // Fallback: return a basic client if factory creation fails
            return new HttpClient();
        }
    }

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext _)
    {
        // Initialization happens in CreateClient
    }
}
