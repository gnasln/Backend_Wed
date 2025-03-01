using System.ComponentModel.DataAnnotations;

namespace WED_BACKEND_ASP.Identity
{
    public record RegisterForm
    {
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? CellPhone { get; set; }
        public string? Password { get; set; }
        public string? RePassword { get; set; }
	}
}
