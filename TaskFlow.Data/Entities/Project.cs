
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

        [Required]
        public int ProjectManagerId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Project Manager
        public ApplicationUser ProjectManager { get; set; } = null!;

        // Project Teams
        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}
