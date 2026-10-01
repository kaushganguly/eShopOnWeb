using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.eShopWeb.PublicApi;

public class CustomSchemaFilters : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema is not OpenApiSchema concreteSchema || concreteSchema.Properties is null)
            return;

        var excludeProperties = new[] { "CorrelationId" };

        foreach (var prop in excludeProperties)
            if (concreteSchema.Properties.ContainsKey(prop))
                concreteSchema.Properties.Remove(prop);
    }
}
