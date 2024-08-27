using MongoDB.Driver;
using WebApi.Context;
using WebApi.CustomExceptions;
using WebApi.DataTransferObject.Request;
using WebApi.DataTransferObject.Responses;
using WebApi.HelperServices.Abstract;
using WebApi.Models;
using WebApi.Services.Abstract;

namespace WebApi.Services;

public class AuthService : IAuthService
{
    private readonly IPassHashService _passHashService;
    private readonly IJwtService _jwtService;

    private readonly MongoDbContext _context;

    public AuthService(IPassHashService passHashService, IJwtService jwtService, MongoDbContext context)
    {
        _passHashService = passHashService;
        _jwtService = jwtService;
        _context = context;
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

        _passHashService.Create(request.Password, out byte[] passHash, out byte[] passSalt);

        var newUser = new User()
        {
            Name = request.Name,
            PassHash = passHash,
            PassSalt = passSalt,
            Email = request.Email,
            Id = Guid.NewGuid().ToString(),
            Role = "User",
            RefreshToken = "",
            TokenExpireDate = default,
            IsEmailConfirmed = false
        };

        await userCollection.InsertOneAsync(newUser);

        return true;
    }

}
