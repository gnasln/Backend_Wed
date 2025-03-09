using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wed.Domain.Entities;

public class Event
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string EventType { get; set; } = string.Empty;
    [ForeignKey(nameof(ApplicationUser))]
    public string OwnerId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    
}