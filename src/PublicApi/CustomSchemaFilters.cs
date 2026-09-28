using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.eShopWeb.PublicApi;

public class CustomSchemaFilters : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema is not OpenApiSchema openApiSchema)
        {
            return;
        }

        var excludeProperties = new[] { "CorrelationId" };

        if (openApiSchema.Properties is null)
        {
            return;
        }

        foreach (var prop in excludeProperties)
            if (openApiSchema.Properties.ContainsKey(prop))
                openApiSchema.Properties.Remove(prop);
    }
}
