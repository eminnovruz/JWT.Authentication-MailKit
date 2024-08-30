using System.Security.Cryptography;
using System.Text;
using WebApi.HelperServices.Abstract;

namespace WebApi.HelperServices;

public class PassHashService : IPassHashService
{
    public void Create(string password, out byte[] PassHash, out byte[] PassSalt)
    {
        using var hmac = new HMACSHA512();
        PassSalt = hmac.Key; 
        PassHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)); 
    }

    public bool ConfirmPasswordHash(string password, byte[] PassHash, byte[] PassSalt)
    {
        using var hmac = new HMACSHA512(PassSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)); 
        return computedHash.SequenceEqual(PassHash); 
    }

    public string EncodeToBase64(byte[] data)
    {
        return Convert.ToBase64String(data);
    }

    public byte[] DecodeFromBase64(string base64Data)
    {
        return Convert.FromBase64String(base64Data);
    }
}
