﻿using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Abstract;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApplicationController : ControllerBase
{
    private readonly IMailService _mailService;

    string emailBody = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Email Template</title>
    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
        }
        .email-container {
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .header {
            background-color: #007bff;
            color: #ffffff;
            padding: 20px;
            text-align: center;
        }
        .header img {
            max-width: 150px;
            height: auto;
        }
        .content {
            padding: 20px;
            text-align: left;
        }
        .content h1 {
            font-size: 24px;
            color: #333333;
            margin-top: 0;
        }
        .content p {
            font-size: 16px;
            color: #666666;
            line-height: 1.5;
            margin: 0 0 20px;
        }
        .button {
            display: inline-block;
            background-color: #007bff;
            color: #ffffff;
            padding: 12px 20px;
            text-decoration: none;
            border-radius: 4px;
            font-size: 16px;
            font-weight: bold;
            margin: 10px 0;
        }
        .footer {
            background-color: #f4f4f4;
            padding: 10px;
            text-align: center;
            font-size: 14px;
            color: #999999;
        }
        @media only screen and (max-width: 600px) {
            .email-container {
                width: 100% !important;
                border-radius: 0;
                box-shadow: none;
            }
        }
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">
            <img src=""https://via.placeholder.com/150x50?text=Logo"" alt=""Company Logo"">
            <h1>Welcome to Our Service!</h1>
        </div>
        <div class=""content"">
            <h1>Hello [Recipient's Name],</h1>
            <p>Thank you for signing up for our service. We're excited to have you on board and can't wait for you to start exploring all the features we have to offer.</p>
            <p>If you have any questions or need assistance, feel free to reach out to our support team.</p>
            <a href=""#"" class=""button"">Get Started</a>
        </div>
        <div class=""footer"">
            <p>&copy; 2024 Company Name. All rights reserved.</p>
            <p>1234 Street Name, City, Country</p>
            <p><a href=""#"" style=""color: #007bff;"">Unsubscribe</a></p>
        </div>
    </div>
</body>
</html>";


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
