namespace WebApi.Services.Abstract;

public interface IPassHashService
{
    void Create(string password, out byte[] PassHash, out byte[] PassSalt);
    bool ConfirmPasswordHash(string password, byte[] PassHash, byte[] PassSalt);
    string EncodeToBase64(byte[] data);
    byte[] DecodeFromBase64(string base64Data);
}
