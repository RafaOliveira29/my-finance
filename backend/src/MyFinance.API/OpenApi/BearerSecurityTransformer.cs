using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace MyFinance.API.OpenApi;

/// <summary>
/// Declara o esquema JWT no documento OpenAPI. Sem isso a interface (Scalar) não oferece onde
/// colar o token e toda rota <c>[Authorize]</c> responderia 401 — o documento descreve as rotas,
/// mas não diria como se autenticar nelas.
/// </summary>
public sealed class BearerSecurityTransformer : IOpenApiDocumentTransformer
{
    private const string SchemeName = "Bearer";

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Cole o accessToken devolvido por /api/auth/login (só o token, sem o prefixo 'Bearer').",
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SchemeName, document)] = [],
        });

        return Task.CompletedTask;
    }
}
