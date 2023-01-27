﻿using Microsoft.AspNetCore.Mvc;
using NetHub.Api.Abstractions;
using NetHub.Api.Shared;

namespace NetHub.Api.Areas.Public.Controllers;

[ApiVersion(Versions.V1)]
[Route("/v{version:apiVersion}")]
public class ResourcesController : ApiController
{
	// [HttpPost("articles/{articleId:long}/images")]
	// public async Task<ArticleImageLinkModel> AddImageToArticle(IFormFile file, long articleId)
	// {
	// 	var resourceId = await Mediator.Send(new AddArticleImageRequest(file, articleId));
	// 	return new(Request.GetResourceUrl(resourceId));
	// }
	//
	// [HttpPost("user/profile-photo")]
	// public async Task<SetUserPhotoResult> SetUserPhoto(IFormFile? file, string? link)
	// {
	// 	var request = new SetUserPhotoRequest(file, link);
	// 	var result = await Mediator.Send(request);
	// 	return result;
	// }
	//
	// [HttpGet("resources/{id:guid}")]
	// [AllowAnonymous]
	// public async Task<FileResult> GetResource([FromRoute] Guid id)
	// {
	// 	var file = await Mediator.Send(new GetResourceRequest(id));
	// 	return File(file.Bytes, file.ContentType);
	// }
}