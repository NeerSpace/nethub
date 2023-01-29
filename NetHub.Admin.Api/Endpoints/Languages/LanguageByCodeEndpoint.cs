using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeerCore.Data.EntityFramework.Extensions;
using NetHub.Admin.Infrastructure.Models.Languages;
using NetHub.Api.Shared;
using NetHub.Api.Shared.Abstractions;
using NetHub.Api.Shared.Swagger;
using NetHub.Data.SqlServer.Context;
using NetHub.Data.SqlServer.Entities;

namespace NetHub.Admin.Api.Endpoints.Languages;

[ApiVersion(Versions.V1)]
[Tags(TagNames.Languages)]
[Authorize(Policy = Policies.HasReadLanguagesPermission)]
public sealed class LanguageByCodeEndpoint : Endpoint<string, LanguageModel>
{
    private readonly ISqlServerDatabase _database;
    public LanguageByCodeEndpoint(ISqlServerDatabase database) => _database = database;


    [HttpGet("languages/{code:alpha}"), ClientSide(ActionName = "getByCode")]
    public override async Task<LanguageModel> HandleAsync([FromRoute] string code, CancellationToken ct = default)
    {
        var language = await _database.Set<Language>().AsNoTracking().FirstOr404Async(l => l.Code == code, ct);
        return language.Adapt<LanguageModel>();
    }
}