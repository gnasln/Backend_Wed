using Wed.Domain.Constants;

namespace WED_BACKEND_ASP.NET_CORE.Dtos
{
    public class UpdateFacilityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string FacilityType { get; set; } = string.Empty;
        public FacilityStatus Status { get; set; } = 0;
        
    }
}
