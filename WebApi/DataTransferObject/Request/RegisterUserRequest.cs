namespace WebApi.DataTransferObject.Request;

public class RegisterUserRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int Age { get; set; }
}
