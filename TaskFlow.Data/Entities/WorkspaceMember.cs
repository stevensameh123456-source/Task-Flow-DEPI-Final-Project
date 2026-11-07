using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class WorkspaceMember
    {
        public int Id { get; set; }

        [Required]
        public int WorkspaceId { get; set; }

        public Workspace Workspace { get; set; } = null!;

        [Required]
        public int UserId { get; set; }

        public ApplicationUser User { get; set; } = null!;

        [Required]
        public WorkspaceRole Role { get; set; }

        [Required]
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}