﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetHub.Api.Abstractions;
using NetHub.Application.Features.Public.Users;
using NetHub.Application.Features.Public.Users.Dto;
using NetHub.Application.Features.Public.Users.Login;
using NetHub.Application.Features.Public.Users.Me;
using NetHub.Application.Features.Public.Users.RefreshTokens;
using NetHub.Application.Features.Public.Users.Register;

namespace NetHub.Api.Areas.Public.Controllers;

public class UserController : ApiController
{
	[HttpPost("register")]
	[AllowAnonymous]
	public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
	{
		var user = await Mediator.Send(request);
		return CreatedAtAction(nameof(GetMe), user);
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<AuthModel> LoginUser([FromBody] LoginUserRequest request)
	{
		var (tokenDto, refreshToken) = await Mediator.Send(request);

		Response.Cookies.Append("NetHub-Refresh-Token", refreshToken,
			new CookieOptions {HttpOnly = true, SameSite = SameSiteMode.Strict});

		return tokenDto;
	}
	//
	// [HttpPost("sso")]
	// [AllowAnonymous]
	// public async Task<IActionResult> SsoAuthorization(SsoEnterRequest request)
	// {
	// 	var (tokenDto, refreshToken) = await Mediator.Send(request);
	//
	// 	Response.Cookies.Append("NetHub-Refresh-Token", refreshToken,
	// 		new CookieOptions {HttpOnly = true, SameSite = SameSiteMode.Strict});
	//
	// 	return Ok(tokenDto);
	// }

	[HttpPost("refresh-tokens")]
	[AllowAnonymous]
	public async Task<AuthModel> RefreshTokens([FromBody] RefreshTokensRequest request)
	{
		var (tokenDto, refreshToken) = await Mediator.Send(request);

		Response.Cookies.Append("NetHub-Refresh-Token", refreshToken,
			new CookieOptions {HttpOnly = true, SameSite = SameSiteMode.Strict});

		return tokenDto;
	}

	[HttpGet("me")]
	public async Task<UserProfileDto> GetMe()
	{
		var user = await Mediator.Send(new GetUserRequest());
		return user;
	}
}