using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using Wed.Domain.Entities;
using WED_BACKEND_ASP.Helper.Services;

namespace WED_BACKEND_ASP.Services;

public class EmailSender : IEmailSender
{
    private readonly SmtpClient _client;
    private readonly string _fromAddress;
    private readonly IFluentEmail _fluentEmail;
    private readonly IFluentEmailFactory _fluentEmailFactory;

    public EmailSender(IConfiguration configuration, IFluentEmail fluentEmail, IFluentEmailFactory fluentEmailFactory)
    {
        _client = new SmtpClient(configuration["EmailSettings:Host"])
        {
            Port = int.Parse(configuration["EmailSettings:Port"]),
            Credentials = new NetworkCredential(configuration["EmailSettings:Username"], configuration["EmailSettings:Password"]),
            EnableSsl = bool.Parse(configuration["EmailSettings:EnableSsl"])
        };
        _fromAddress = configuration["EmailSettings:FromAddress"];
        _fluentEmail = fluentEmail;
        _fluentEmailFactory = fluentEmailFactory;
    }

    public async Task SendEmailAsync(string email, string name, string otp)
    {
        var result = await _fluentEmail
            .To(email, name)
            .Subject("Xác Minh Email")
            .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Resources/Templates/Send_OTP.cshtml",
                new { Name = name, OtpCode = otp })
            .SendAsync();

        if (!result.Successful)
        {
            throw new Exception($"Failed to send email: {result.ErrorMessages.FirstOrDefault()}");
        }
    }

}
