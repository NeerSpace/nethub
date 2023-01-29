using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeerCore.Data.EntityFramework.Extensions;
using NetHub.Api.Shared;
using NetHub.Api.Shared.Abstractions;
using NetHub.Application.Extensions;
using NetHub.Application.Models.Articles;
using NetHub.Application.Services;
using NetHub.Core.Exceptions;
using NetHub.Data.SqlServer.Entities.Articles;

namespace NetHub.Api.Endpoints.Articles;

[Authorize]
[Tags(TagNames.Articles)]
[ApiVersion(Versions.V1)]
public class ArticleUploadImageEndpoint : Endpoint<AddArticleImageRequest, CreatedResult>
{
    private readonly IResourceService _resourceService;
    public ArticleUploadImageEndpoint(IResourceService resourceService) => _resourceService = resourceService;


    [HttpPost("articles/{id:long}/images")]
    public override async Task<CreatedResult> HandleAsync(AddArticleImageRequest request, CancellationToken ct)
    {
        var userId = UserProvider.UserId;

        var article = await Database.Set<Article>().FirstOr404Async(a => a.Id == request.Id, ct);

        if (article.AuthorId != userId)
            throw new PermissionsException();

        var resourceId = await _resourceService.SaveResourceToDb(request.File);

        await Database.Set<ArticleResource>().AddAsync(new ArticleResource
        {
            ArticleId = request.Id,
            ResourceId = resourceId
        }, ct);

        await Database.SaveChangesAsync(ct);

        return Created(Request.GetResourceUrl(resourceId), null);
    }
}