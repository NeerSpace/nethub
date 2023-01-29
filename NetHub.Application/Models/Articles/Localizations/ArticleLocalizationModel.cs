﻿using NetHub.Data.SqlServer.Enums;

namespace NetHub.Application.Models.Articles.Localizations;

public sealed class ArticleLocalizationModel
{
    public long Id { get; set; }
    public long ArticleId { get; set; }
    public string LanguageCode { get; set; } = default!;
    public ArticleContributorModel[] Contributors { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Html { get; set; } = default!;
    public ContentStatus Status { get; set; }
    public int Views { get; set; }
    public int Rate { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? Updated { get; set; }
    public DateTimeOffset? Published { get; set; }
    public DateTimeOffset? Banned { get; set; }

    public bool IsSaved { get; set; }
    public DateTimeOffset? SavedDate { get; set; }
    public Vote? Vote { get; set; }
}