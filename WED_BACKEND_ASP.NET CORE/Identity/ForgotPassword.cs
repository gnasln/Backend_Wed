namespace WED_BACKEND_ASP.Identity
{
    public record ForgotPassword
    {
        public required string Email { get; init; }
        public string? Password { get; init; }
        public string? ConfirmPassword { get; init; }

    }
}
