﻿using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeerCore.Data.EntityFramework.Extensions;
using NetHub.Shared.Api;
using NetHub.Shared.Api.Abstractions;
using NetHub.Data.SqlServer.Entities.Articles;
using NetHub.Data.SqlServer.Enums;
using NetHub.Models.Articles;
using NetHub.Shared.Api.Constants;
using NetHub.Shared.Api.Swagger;

namespace NetHub.Api.Endpoints.Articles;

[Tags(TagNames.Articles)]
[ApiVersion(Versions.V1)]
public sealed class ArticleGetByIdEndpoint : Endpoint<long, (ArticleModelExtended, Guid[]?)>
{
    [HttpGet("articles/{id:long}"), ClientSide(ActionName = "getById")]
    public override async Task<(ArticleModelExtended, Guid[]?)> HandleAsync([FromRoute] long id, CancellationToken ct)
    {
        var article = await Database.Set<Article>()
            .Include(a => a.Localizations!.Where(l => l.Status == ContentStatus.Published))
            .Include(a => a.Tags)!.ThenInclude(at => at.Tag)
            .Include(a => a.Images)
            .FirstOr404Async(a => a.Id == id, ct);

        var model = article.Adapt<ArticleModelExtended>();
        var imageIds = article.Images?.Select(i => i.ResourceId).ToArray();

        return (model, imageIds);
    }
}