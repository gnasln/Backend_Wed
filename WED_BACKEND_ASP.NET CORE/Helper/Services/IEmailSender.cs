using Wed.Domain.Entities;

namespace WED_BACKEND_ASP.Helper.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string name, string otp);
    Task SendEmailRegisterAsync(string email, string fullname, string userName, string password);
}