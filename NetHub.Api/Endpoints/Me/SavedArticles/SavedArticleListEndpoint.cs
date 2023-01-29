﻿using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetHub.Api.Shared;
using NetHub.Api.Shared.Abstractions;
using NetHub.Application.Models.Articles.Localizations;
using NetHub.Data.SqlServer.Entities.Views;

namespace NetHub.Api.Endpoints.Me.SavedArticles;

[Authorize]
[Tags(TagNames.MySavedArticles)]
[ApiVersion(Versions.V1)]
public sealed class SavedArticleListEndpoint : Endpoint<ArticleLocalizationQuery, ExtendedArticleModel[]>
{
    [HttpGet("me/saved-articles")]
    public override async Task<ExtendedArticleModel[]> HandleAsync(ArticleLocalizationQuery request, CancellationToken ct)
    {
        var userId = UserProvider.UserId;

        var saved = await Database.Set<ExtendedUserArticle>()
            .Where(ea => ea.UserId == userId
                    && ea.IsSaved == true
                //TODO: Remove comments in release (please...)
                // && ea.Status == ContentStatus.Published
            )
            .ProjectToType<ExtendedArticleModel>()
            .ToArrayAsync(ct);

        return saved.DistinctBy(s => s.LocalizationId).ToArray();
    }
}