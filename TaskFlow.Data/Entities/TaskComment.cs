
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class TaskComment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string Content { get; set; } = string.Empty;

        // Task Relationship
        [Required]
        public int TaskId { get; set; }

        public TaskItem Task { get; set; } = null!;

        // User Relationship
        [Required]
        public int UserId { get; set; }

        public ApplicationUser User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
