namespace WED_BACKEND_ASP.Dtos
{
    public record UpdateUser
    {
        public string? FullName { get; set; }
        public bool? Gender { get; set; } = true;
        public string? Address { set; get; }
        public DateTime? Birthday { set; get; } = DateTime.MinValue;
        public new string? Email { get; set; }
        public string? CellPhone { get; set; }
        public string? Status { get; set; } = "Active";
    
    
    }
}