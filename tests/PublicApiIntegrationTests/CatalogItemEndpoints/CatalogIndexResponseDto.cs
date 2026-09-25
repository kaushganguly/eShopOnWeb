using System.Collections.Generic;
using System.Text.Json;

namespace PublicApiIntegrationTests.CatalogItemEndpoints;

/// <summary>
/// Minimal, test-local shape used to deserialize the "/api/catalog-items" response.
/// Deliberately independent of Microsoft.eShopWeb.Web.ViewModels.CatalogIndexViewModel so this
/// test project does not need a project reference to the Web project (which pulls in the
/// Blazor WebAssembly hosting stack and its dependencies, unrelated to this API test).
/// </summary>
public class CatalogIndexResponseDto
{
    public List<JsonElement> CatalogItems { get; set; } = new();
}
