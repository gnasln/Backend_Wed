namespace Wed.Application.Dtos
{
    public class FacilityResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public required string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public required string OwnerId { get; set; }
        public string FacilityType { get; set; } = string.Empty;
        public required string Status { get; set; } = "Available";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    }
}
