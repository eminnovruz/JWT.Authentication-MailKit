using MongoDB.Driver;
using Org.BouncyCastle.Asn1.Ocsp;
using WebApi.Context;
using WebApi.Models;
using WebApi.Services.Abstract;
using ZstdSharp;
using static QRCoder.PayloadGenerator;

namespace WebApi.Services;

public class SecretKeyService : ISecretKeyService
{
    private readonly MongoDbContext _context;

    public SecretKeyService(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<string> GetUserSecret(string email)
    {
        var user = await _context.Users
            .Find(user => user.Email == email)
            .FirstOrDefaultAsync();

        if (user == null)
            throw new Exception("Cannot find user related with given email");

        return user.SecretKey ?? throw new Exception("Something went wrong.");
    }

    public async Task<bool> SaveUserSecret(string secret, string email)
    {
        var user = await _context.Users
                    .Find(user => user.Email == email)
                    .FirstOrDefaultAsync();

        if( user == null)
            throw new Exception("Cannot find user related with given email");

        var update = Builders<User>.Update
            .Set(user => user.SecretKey, secret);

        var result = await _context.Users.UpdateOneAsync(u => u.Email == email, update);

        return result.IsAcknowledged;
    }
}
