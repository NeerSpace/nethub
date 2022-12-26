﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NeerCore.Exceptions;
using NetHub.Application.Features.Public.Users.Dto;
using NetHub.Application.Interfaces;
using NetHub.Application.Tools;
using NetHub.Data.SqlServer.Entities.Identity;

namespace NetHub.Application.Features.Public.Users.Sso;

public sealed class SsoEnterHandler : DbHandler<SsoEnterRequest, (AuthResult, string)>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IAuthValidator _validator;
    private readonly IJwtService _jwtService;

    public SsoEnterHandler(IServiceProvider serviceProvider, UserManager<AppUser> userManager,
        IAuthValidator validator, IJwtService jwtService) : base(serviceProvider)
    {
        _userManager = userManager;
        _validator = validator;
        _jwtService = jwtService;
    }

    // protected override async Task<(AuthModel, string)> Handle(SsoEnterRequest request)
    public override async Task<(AuthResult, string)> Handle(SsoEnterRequest request, CancellationToken ct)
    {
        var loginInfo = await GetUserLoginInfoAsync(request.ProviderKey, request.Provider, ct);
        if (loginInfo is null) throw new ValidationFailedException("Login info is invalid");

        var user = await _userManager.FindByIdAsync(loginInfo.UserId.ToString());

        var validated = false;

        if (user is null)
        {
            user = await RegisterUserAsync(request, ct);
            validated = true;
        }

        await LoginUserAsync(request, validated, ct);

        var dto = await _jwtService.GenerateAsync(user, ct)
            with
            {
                Id = user.Id,
                ProfilePhotoLink = user.ProfilePhotoLink,
                FirstName = user.FirstName
            };

        // dto.ProfilePhotoLink = user.ProfilePhotoLink;
        // dto.FirstName = user.FirstName;

        // return (dto.Adapt<AuthModel>(), dto.RefreshToken);
        return (dto, dto.RefreshToken);
    }

    private async Task LoginUserAsync(SsoEnterRequest request, bool validated, CancellationToken ct)
    {
        // var loginInfo = await GetUserLoginInfo(request.ProviderKey, request.Provider);
        // user ??= await _userManager.FindByIdAsync(loginInfo!.UserId.ToString());

        // var userProviders = await _userManager.GetLoginsAsync(user);
        //
        // if (userProviders.All(up => up.ProviderDisplayName.ToEnum<ProviderType>() != request.Provider))
        // 	throw new ValidationFailedException($"Login by {request.Provider} not supported for this user");

        if (!validated)
            await ValidateUserAsync(request, ct);

        // if (user is null)
        // throw new ValidationFailedException("Username", "No such User with provided username");

        // return user;
    }


    private async Task<AppUser> RegisterUserAsync(SsoEnterRequest request, CancellationToken ct)
    {
        await ValidateUserAsync(request, ct);

        var user = new AppUser
        {
            UserName = request.Username,
            FirstName = request.FirstName!,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Email = request.Email!,
            ProfilePhotoLink = request.ProfilePhotoLink,
            EmailConfirmed = request.Provider is not ProviderType.Telegram
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
            throw new ValidationFailedException(result.Errors.First().Description);

        await _userManager.AddLoginAsync(user,
            new UserLoginInfo(request.Provider.ToString().ToLower(),
                request.ProviderKey,
                null));

        return user;
    }

    private async Task ValidateUserAsync(SsoEnterRequest request, CancellationToken ct)
    {
        if (request.ProviderMetadata is not { Count: > 0 })
            throw new ValidationFailedException("Metadata not provided");

        var isValid = await _validator.ValidateAsync(request, SsoType.Login, ct);

        if (!isValid)
            throw new ValidationFailedException("Provided invalid data");
    }

    private async Task<IdentityUserLogin<long>?> GetUserLoginInfoAsync(string key, ProviderType provider, CancellationToken ct)
    {
        return await Database.Set<AppUserLogin>()
            .SingleOrDefaultAsync(info =>
                info.ProviderKey == key &&
                info.LoginProvider == provider.ToString().ToLower(), ct);
    }
}