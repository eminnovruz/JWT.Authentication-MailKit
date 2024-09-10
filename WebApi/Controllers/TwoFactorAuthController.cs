using Microsoft.AspNetCore.Mvc;
using WebApi.HelperServices.Abstract;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TwoFactorAuthController : ControllerBase
{
    private readonly ITwoFactorAuthService _twoFactorAuthService;

    public TwoFactorAuthController(ITwoFactorAuthService twoFactorAuthService)
    {
        _twoFactorAuthService = twoFactorAuthService;
    }
}
