using MongoDB.Driver;
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

    public Task<bool> EnableTwoFactorAuth(EnableTwoFactorAuthRequest flag)
    {
        throw new NotImplementedException();
    }

    public AuthTokenInfoResponse GenerateToken(User user)
    {
        throw new NotImplementedException();
    }

    public Task<AuthTokenInfoResponse> Login(LoginUserRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<AuthTokenInfoResponse> RefreshToken(RefreshTokenRequest request)
    {
        throw new NotImplementedException();
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

    public Task<bool> SetUserPassword(SetUserPasswordRequest request)
    {
        throw new NotImplementedException();
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
        };

        await _context.Users.InsertOneAsync(newUser);
    }
}
