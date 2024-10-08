﻿using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; } // ??
    public bool TwoFactorAuthentication { get; set; }
    public string TwoFactorAuthenticationType { get; set; }
    public string VerificationCode { get; set; }
    public DateTimeOffset VerificationCodeExpire { get; set; }
    public byte[] PassHash { get; set; }
    public byte[] PassSalt { get; set; }
    public string Role { get; set; }
    public string RefreshToken { get; set; }
    public DateTime TokenExpireDate { get; set; }
    public string SecretKey { get; set; }
}
