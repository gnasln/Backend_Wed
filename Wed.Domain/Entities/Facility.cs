using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Wed.Domain.Entities
{
    public class Facility
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public required string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [ForeignKey(nameof(ApplicationUser))]
        public required string OwnerId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string FacilityType { get; set; } = string.Empty;
        public required string Status { get; set; } = "Available";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
    }
}
