﻿using Microsoft.EntityFrameworkCore;
using NetHub.Application.Tools;
using NetHub.Data.SqlServer.Entities.Identity;

namespace NetHub.Application.Features.Public.Users.CheckUserExists;

internal sealed class CheckUserExistsHandler : DbHandler<CheckUserExistsRequest, CheckUserExistsResult>
{
    public CheckUserExistsHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }


    public override async Task<CheckUserExistsResult> Handle(CheckUserExistsRequest request, CancellationToken ct)
    {
        string requestLoginProvider = request.Provider.ToString().ToLower();
        var loginInfo = await Database.Set<AppUserLogin>()
            .SingleOrDefaultAsync(l =>
                l.ProviderKey == request.Key
                && l.LoginProvider == requestLoginProvider, ct);

        return new(loginInfo is not null);
    }
}