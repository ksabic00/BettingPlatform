using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BettingPlatform.Api.Swagger;

public class StringEnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var t = Nullable.GetUnderlyingType(context.Type) ?? context.Type;
        if (!t.IsEnum) return;

        schema.Type = "string";
        schema.Format = null;
        schema.Enum = Enum.GetNames(t)
            .Select(n => (IOpenApiAny)new OpenApiString(n))
            .ToList();
    }
}
