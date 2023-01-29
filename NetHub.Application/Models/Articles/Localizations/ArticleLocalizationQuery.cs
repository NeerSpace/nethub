﻿using Microsoft.AspNetCore.Mvc;

namespace NetHub.Application.Models.Articles.Localizations;

public record ArticleLocalizationQuery
{
    [FromRoute] public long Id { get; init; }
    [FromRoute(Name = "lang")] public required string LanguageCode { get; init; }
}