namespace WED_BACKEND_ASP.NET_CORE.Dtos
{
    public class CreateFacilityDto
    {
        public string? Name { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? FacilityType { get; set; } = string.Empty;
        public int Status { get; set; }
    }
}
