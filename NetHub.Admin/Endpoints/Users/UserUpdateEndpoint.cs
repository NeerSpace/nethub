using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NeerCore.Exceptions;
using NetHub.Admin.Abstractions;
using NetHub.Admin.Infrastructure.Models.Users;
using NetHub.Application.Extensions;
using NetHub.Data.SqlServer.Entities.Identity;

namespace NetHub.Admin.Endpoints.Users;

[ApiVersion(Versions.V1)]
[Tags(TagNames.Users)]
// [Authorize(Policy = Policies.HasManageUsersPermission)]
[AllowAnonymous]
public sealed class UserUpdateEndpoint : ActionEndpoint<UserUpdate>
{
    private readonly UserManager<AppUser> _userManager;
    public UserUpdateEndpoint(UserManager<AppUser> userManager) => _userManager = userManager;


    [HttpPut("users")]
    public override async Task HandleAsync([FromBody] UserUpdate request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user is null)
            throw new NotFoundException($"User with Id '{request.Id}' does not exist.");

        request.Adapt(user);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new ValidationFailedException("User not updated.", result.ToErrorDetails());

        if (!string.IsNullOrEmpty(request.Password))
            await _userManager.AddPasswordAsync(user, request.Password);
    }
}