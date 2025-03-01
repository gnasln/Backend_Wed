namespace WED_BACKEND_ASP.Dtos
{
    public class UserDto
    {
        public required string Id { get; set; }
        public string? Fullname { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool? Gender { get; set; }
        public string? CellPhone { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Address { get; set; }
        public string? status { get; set; }
        public string? Role { get; set; }


    }
}
