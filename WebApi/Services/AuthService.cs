using MongoDB.Driver;
using Org.BouncyCastle.Asn1.Ocsp;
using WebApi.Context;
using WebApi.CustomExceptions;
using WebApi.DataTransferObject.Request;
using WebApi.DataTransferObject.Responses;
using WebApi.HelperServices.Abstract;
using WebApi.Models;
using WebApi.Models.Enums;
using WebApi.Services.Abstract;

namespace WebApi.Services;

public class AuthService : IAuthService
{
    private readonly IPassHashService _passHashService;
    private readonly IJwtService _jwtService;
    private readonly IVerificationService _verifyService;

    private readonly MongoDbContext _context;

    public AuthService(IPassHashService passHashService, IJwtService jwtService, MongoDbContext context, IVerificationService verifyService)
    {
        _passHashService = passHashService;
        _jwtService = jwtService;
        _context = context;
        _verifyService = verifyService;
    }

    public async Task<bool> EnableTwoFactorAuth(EnableTwoFactorAuthRequest request)
    {
        var user = await _context.Users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();

        if (user == null)
            throw new UserNotFoundException();

        user.TwoFactorAuthentication = request.Flag;

        var updateResult = await _context.Users.ReplaceOneAsync(
            u => u.Email == user.Email,
            user
        );

        return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }

    public AuthTokenInfoResponse GenerateToken(User user)
    {
        var token = _jwtService.GenerateSecurityToken(user.Id, user.Email, user.Role);
        user.RefreshToken = token.RefreshToken;
        user.TokenExpireDate = token.ExpireDate;
        return token;
    }

    public async Task<AuthTokenInfoResponse> Login(LoginUserRequest request)
    {
        var userCollection = _context.Users;
        var user = await userCollection.Find(u => u.Email == request.Email).FirstOrDefaultAsync();

        if (user == null)
            throw new UserNotFoundException(); 
        
        if (!_passHashService.ConfirmPasswordHash(request.Password, user.PassHash, user.PassSalt))
            throw new WrongPasswordException();

        var token = GenerateToken(new User()
        {
            PassSalt = user.PassSalt,
            Email = user.Email,
            Id = user.Id,
            Name = user.Name,
            PassHash = user.PassHash,
            RefreshToken = user.RefreshToken,
            Role = user.Role,
            TokenExpireDate = user.TokenExpireDate,
        });

        var updateDefinition = Builders<User>.Update.Set(u => u.RefreshToken, token.RefreshToken)
                                                    .Set(u => u.TokenExpireDate, token.ExpireDate);

        await userCollection.UpdateOneAsync(u => u.Id == user.Id, updateDefinition);

        await _verifyService.SendEmailVerificationCode(new SendEmailVerificationCodeRequest()
        {
            Email = user.Email
        });

        return token;
    }

    public async Task<AuthTokenInfoResponse> RefreshToken(RefreshTokenRequest request)
    {
        if (request.ExpireDate >= DateTime.Now)
        {
            var userCollection = _context.Users;
            var user = await userCollection.Find(u => u.RefreshToken == request.RefreshToken).FirstOrDefaultAsync();

            if (user is not null)
            {
                var token = GenerateToken(new User()
                {
                    PassSalt = user.PassSalt,
                    Email = user.Email,
                    Id = user.Id,
                    Name = user.Name,
                    PassHash = user.PassHash,
                    RefreshToken = user.RefreshToken,
                    Role = user.Role,
                    TokenExpireDate = user.TokenExpireDate
                });

                var updateDefinition = Builders<User>.Update.Set(u => u.RefreshToken, token.RefreshToken)
                                                            .Set(u => u.TokenExpireDate, token.ExpireDate);

                await userCollection.UpdateOneAsync(u => u.Id == user.Id, updateDefinition);

                return token;
            }
        }

        throw new Exception();
    }


    public async Task<bool> Register(RegisterUserRequest request)
    {
        var userCollection = _context.Users;
        var existingUser = await userCollection.Find(u => u.Email == request.Email).FirstOrDefaultAsync();

        if (existingUser != null)
            throw new EmailUsedException();

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
            TwoFactor = TwoFactorAuthTypes.Email,
        };

        await userCollection.InsertOneAsync(newUser);

        await _verifyService.RequireSetPassword(newUser.Email);

        return true;
    }
}
