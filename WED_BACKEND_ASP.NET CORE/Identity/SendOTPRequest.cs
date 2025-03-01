namespace WED_BACKEND_ASP.Identity;

public record SendOTPRequest
{
    public required string Email { get; set; }
}