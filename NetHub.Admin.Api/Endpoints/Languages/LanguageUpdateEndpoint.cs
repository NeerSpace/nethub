using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeerCore.Data.EntityFramework.Extensions;
using NetHub.Admin.Infrastructure.Models.Languages;
using NetHub.Api.Shared;
using NetHub.Api.Shared.Abstractions;
using NetHub.Data.SqlServer.Context;
using NetHub.Data.SqlServer.Entities;

namespace NetHub.Admin.Api.Endpoints.Languages;

[ApiVersion(Versions.V1)]
[Tags(TagNames.Languages)]
[Authorize(Policy = Policies.HasManageLanguagesPermission)]
public sealed class LanguageUpdateEndpoint : ActionEndpoint<LanguageModel>
{
    private readonly ISqlServerDatabase _database;
    public LanguageUpdateEndpoint(ISqlServerDatabase database) => _database = database;


    [HttpPut("languages")]
    public override async Task HandleAsync([FromBody] LanguageModel request, CancellationToken ct = default)
    {
        var language = await _database.Set<Language>().FirstOr404Async(l => l.Code == request.Code, ct);
        request.Adapt(language);
        await _database.SaveChangesAsync(ct);
    }
}