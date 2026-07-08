namespace Microsoft.eShopWeb.PublicApi;

/// <summary>
/// Marker type used by integration tests as the WebApplicationFactory&lt;T&gt; entry point.
/// Resolves CS0433 ambiguity: both PublicApi and Web define a top-level <c>Program</c> class
/// in the global namespace; using this named type avoids the conflict when both assemblies
/// are referenced by the same test project.
/// </summary>
public sealed class TestAnchor { }
