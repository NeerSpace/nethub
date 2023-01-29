﻿using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetHub.Api.Shared;
using NetHub.Api.Shared.Abstractions;
using NetHub.Application.Models.Users;
using NetHub.Data.SqlServer.Entities.Identity;

namespace NetHub.Api.Endpoints.Users;

[Tags(TagNames.Users)]
[ApiVersion(Versions.V1)]
public sealed class UserSearchEndpoint : Endpoint<SearchUsersRequest, PrivateUserDto[]>
{
    [HttpGet("users/search")]
    public override Task<PrivateUserDto[]> HandleAsync(SearchUsersRequest request, CancellationToken ct)
    {
        var result = Database.Set<AppUser>()
            .Where(u => u.NormalizedUserName.Contains(request.Username.ToUpper()))
            .ProjectToType<PrivateUserDto>()
            .ToArrayAsync(ct);

        return result;
    }
}