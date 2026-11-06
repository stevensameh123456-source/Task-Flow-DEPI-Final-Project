using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Data.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "Full name must be between 3 and 100 characters.")]
        [RegularExpression(@"^[\p{L}\p{M}\s'-]+$",
            ErrorMessage = "Full name contains invalid characters.")]
        public string FullName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional Team Membership
        public int? TeamId { get; set; }

        public Team? Team { get; set; }

        // Teams managed by this user as Team Leader
        public ICollection<Team> LedTeams { get; set; }
            = new List<Team>();

        // Projects managed by this user
        public ICollection<Project> ManagedProjects { get; set; }
            = new List<Project>();

        // Workspaces memberships
        public ICollection<WorkspaceMember> WorkspaceMembers { get; set; }
            = new List<WorkspaceMember>();
    }
}