﻿using WebApi.DataTransferObject.Request;
using WebApi.DataTransferObject.Responses;

namespace WebApi.Services.Abstract;

public interface IUserService
{
    Task<bool> Register(RegisterUserRequest request);
    Task<AuthTokenInfoResponse> Login(LoginUserRequest request);
    Task<bool> EnableTwoFactorAuth(EnableTwoFactorAuthRequest request);
    Task<bool> SetUserPassword(SetUserPasswordRequest request);
    Task<User2FaSettingsInfoResponse> GetUser2FaSettings(GetUser2FaSettingsRequest reqeust);

    // New method to refresh access token
    Task<string> RefreshTokenAsync(string refreshToken);
}
