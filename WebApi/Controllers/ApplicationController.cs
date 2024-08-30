using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Abstract;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApplicationController : ControllerBase
{
    private readonly IMailService _mailService;

    public ApplicationController(IMailService mailService)
    {
        _mailService = mailService;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterAsync()
    {
        try
        {
            return Ok(await _mailService.SendEmailAsync("softwarecrim@gmail.com", "Welcome to our service!🎉"));
        }
        catch (Exception exception)
        {
            return BadRequest(exception);
        }
    }
}
