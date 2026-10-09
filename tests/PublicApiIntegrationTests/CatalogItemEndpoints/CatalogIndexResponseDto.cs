using System.Collections.Generic;

namespace PublicApiIntegrationTests.CatalogItemEndpoints;

/// <summary>
/// Minimal, test-local mirror of Microsoft.eShopWeb.Web.ViewModels.CatalogIndexViewModel's JSON shape.
/// Deliberately not referencing the Web project here: Web's ProjectReference pulls in BlazorAdmin's
/// Blazor WebAssembly runtime assemblies (e.g. Microsoft.JSInterop.WebAssembly), which are built for the
/// mono/wasm runtime and crash with a ReflectionTypeLoadException when the ASP.NET Core MVC
/// ApplicationPartManager (via AddControllers) enumerates all loaded assemblies' types on a normal
/// CoreCLR host such as this test's WebApplicationFactory.
/// </summary>
public class CatalogIndexResponseDto
{
    public List<CatalogItemDto> CatalogItems { get; set; } = new();
}

public class CatalogItemDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? PictureUri { get; set; }
    public decimal Price { get; set; }
}
