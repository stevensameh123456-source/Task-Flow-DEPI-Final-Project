
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // Required Project Relationship
        [Required]
        public int ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        // Team Leader Relationship
        [Required]
        public int LeaderId { get; set; }

        public ApplicationUser Leader { get; set; } = null!;

        // Team Employees
        public ICollection<ApplicationUser> Employees { get; set; }
            = new List<ApplicationUser>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
