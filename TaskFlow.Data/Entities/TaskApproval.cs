
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class TaskApproval
    {
        public int Id { get; set; }

        // Task Relationship
        [Required]
        public int TaskId { get; set; }

        public TaskItem Task { get; set; } = null!;

        // Reviewer Relationship
        [Required]
        public int ReviewerId { get; set; }

        public ApplicationUser Reviewer { get; set; } = null!;

        [Required]
        public ApprovalStatus Status { get; set; }
            = ApprovalStatus.Pending;

        [StringLength(2000)]
        public string? ReviewComment { get; set; }

        public DateTime? ReviewedAt { get; set; }
    }
}
