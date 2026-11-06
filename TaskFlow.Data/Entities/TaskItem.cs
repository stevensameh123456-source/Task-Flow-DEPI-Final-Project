
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? CompletedAt { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;

        [Required]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        // Team Relationship
        [Required]
        public int TeamId { get; set; }

        public Team Team { get; set; } = null!;

        // Assigned Employee Relationship
        [Required]
        public int AssignedEmployeeId { get; set; }

        public ApplicationUser AssignedEmployee { get; set; } = null!;

        // Task Creator Relationship
        [Required]
        public int CreatedByLeaderId { get; set; }

        public ApplicationUser CreatedByLeader { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Related Records
        public ICollection<TaskComment> Comments { get; set; }
            = new List<TaskComment>();

        public ICollection<TaskAttachment> Attachments { get; set; }
            = new List<TaskAttachment>();

        public ICollection<TaskSubmission> Submissions { get; set; }
            = new List<TaskSubmission>();

        public ICollection<TaskApproval> Approvals { get; set; }
            = new List<TaskApproval>();
    }
}
