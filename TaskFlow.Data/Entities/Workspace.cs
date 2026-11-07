using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class Workspace
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Workspace Owner
        [Required]
        public int OwnerId { get; set; }

        public ApplicationUser Owner { get; set; } = null!;

        // Navigation Properties
        public ICollection<Project> Projects { get; set; }
            = new List<Project>();

        public ICollection<WorkspaceMember> Members { get; set; }
            = new List<WorkspaceMember>();
    }
}