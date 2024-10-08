﻿using MongoDB.Driver;
using WebApi.Context;
using WebApi.DataTransferObject.Request;
using WebApi.DataTransferObject.Responses;
using WebApi.Models;
using WebApi.Services.Abstract;

namespace WebApi.Services;

public class UserService : IUserService
{
    private readonly IPassHashService _passHashService;
    private readonly IJwtService _jwtService;
    private readonly IVerificationService _verificationService;
    private readonly MongoDbContext _context;

    public UserService(IPassHashService passHashService, IJwtService jwtService, MongoDbContext context, IVerificationService verificationService)
    {
        _passHashService = passHashService;
        _jwtService = jwtService;
        _context = context;
        _verificationService = verificationService;
    }

    public async Task<bool> EnableTwoFactorAuth(EnableTwoFactorAuthRequest request)
    {
        // Find the user by email
        var user = await _context.Users
            .Find(u => u.Email == request.Email)
            .FirstOrDefaultAsync();

        // If the user doesn't exist, return false
        if (user == null)
            return false;

        // Update the user's two-factor authentication status
        var update = Builders<User>.Update
            .Set(u => u.TwoFactorAuthentication, true)
            .Set(u => u.TwoFactorAuthenticationType, request.Type);
        ;

        var result = await _context.Users.UpdateOneAsync(u => u.Email == request.Email, update);

        // Return true if the update was successful
        return result.ModifiedCount > 0;
    }

    public async Task<AuthTokenInfoResponse> Login(LoginUserRequest request)
    {
        // Find the user by email
        var user = await _context.Users
            .Find(u => u.Email == request.Email)
            .FirstOrDefaultAsync();

        // If the user doesn't exist, return an unauthorized response
        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        // Verify the password using your password hashing service
        var passwordMatch = _passHashService.ConfirmPasswordHash(request.Password, user.PassHash, user.PassSalt);
        if (!passwordMatch)
            throw new UnauthorizedAccessException("Invalid credentials.");

        if (user.TwoFactorAuthentication is true)
        {
            if (user.TwoFactorAuthenticationType == "google authenticator")
            {
                return new AuthTokenInfoResponse()
                {
                    TwoFactorAuthenticationType = user.TwoFactorAuthenticationType,
                };
            }
            else if (user.TwoFactorAuthenticationType == "gmail")
            {
                await HandleGmail2FA(user);

                return null;
            }
        }

        // Generate JWT access token using the JwtService
        var accessTokenResponse = _jwtService.GenerateSecurityToken(user.Id.ToString(), user.Email, user.Role);

        // Optionally save the refresh token to the database
        user.RefreshToken = accessTokenResponse.RefreshToken;
        user.TokenExpireDate = DateTime.UtcNow.AddDays(7); // Assuming 7-day refresh token validity
        accessTokenResponse.TwoFactorAuthenticationType = user.TwoFactorAuthenticationType;

        await _context.Users.ReplaceOneAsync(u => u.Email == user.Email, user);
        // Return the access token, refresh token, and expiration date
        return accessTokenResponse;
    }

    public async Task<bool> Register(RegisterUserRequest request)
    {
        var userCollection = _context.Users;

        var existingUser = await userCollection.Find(u => u.Email == request.Email).FirstOrDefaultAsync();

        if (existingUser != null)
            throw new Exception("This email is used before");

        await CreateUser(request);

        return await _verificationService.SendVerificationEmail(request.Email);
    }

    public async Task<bool> SetUserPassword(SetUserPasswordRequest request)
    {
        var user = await _context.Users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();

        if (user == null)
            throw new Exception("Cannot find user related with given email address");

        byte[] newPassHash;
        byte[] newPassSalt;
        _passHashService.Create(request.Password, out newPassHash, out newPassSalt);

        var updateDefinition = Builders<User>.Update
            .Set(u => u.PassHash, newPassHash)
            .Set(u => u.PassSalt, newPassSalt);

        var updateResult = await _context.Users.UpdateOneAsync(u => u.Email == request.Email, updateDefinition);

        return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }

    public async Task<User2FaSettingsInfoResponse> GetUser2FaSettings(GetUser2FaSettingsRequest request)
    {
        var user = await _context.Users.Find(user => user.Email == request.Email).FirstOrDefaultAsync();

        if (user is null)
            throw new Exception("Cannot find user related with given email");

        return new User2FaSettingsInfoResponse()
        {
            TwoFactorAuthentication = user.TwoFactorAuthentication,
            TwoFactorAuthenticationType = user.TwoFactorAuthenticationType,
        };
    }

    private async Task CreateUser(RegisterUserRequest request)
    {
        var newUser = new User()
        {
            Name = request.Name,
            Email = request.Email,
            Id = Guid.NewGuid().ToString(),
            Role = "User",
            RefreshToken = "",
            TokenExpireDate = default,
            IsEmailConfirmed = false,
            TwoFactorAuthentication = false,
            VerificationCode = "",
            VerificationCodeExpire = default,
            TwoFactorAuthenticationType = ""
        };

        await _context.Users.InsertOneAsync(newUser);
    }


    private async Task HandleGmail2FA(User user)
    {
        await _verificationService.SendVerificationEmail(user.Email);
    }

    public async Task<string> RefreshTokenAsync(string refreshToken)
    {
        // Step 1: Validate the refresh token
        var principal = _jwtService.GetPrincipalFromExpiredToken(refreshToken);
        if (principal == null)
        {
            throw new Exception("Invalid refresh token");
        }

        // Step 2: Retrieve the user's email from the claims
        var email = principal.Identity?.Name;

        // Step 3: Retrieve the user by email
        var user =  await _context.Users.Find(x => x.Email == email).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new Exception("User not found");
        }

        // Step 4: Verify the refresh token against the stored refresh token (if applicable)
        if (user.RefreshToken != refreshToken)
        {
            throw new Exception("Invalid refresh token");
        }

        // Step 5: Generate a new access token
        var newAccessToken = _jwtService.GenerateSecurityToken(user.Id, user.Email, user.Role);

        // Optionally, generate a new refresh token and save it to the database
        // user.RefreshToken = _jwtService.GenerateRefreshToken();
        // await _userRepository.UpdateAsync(user);

        // Return the new access token (and optionally a new refresh token)
        return newAccessToken.AccessToken;
    }
}
