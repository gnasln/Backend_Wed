namespace WED_BACKEND_ASP.Identity
{
    public record ForgotPassword
    {
        public required  string UserName { get; init; }
        public required string Email { get; init; }

    }
}
