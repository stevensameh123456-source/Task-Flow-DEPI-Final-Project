
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class TaskSubmission
    {
        public int Id { get; set; }

        // Task Relationship
        [Required]
        public int TaskId { get; set; }

        public TaskItem Task { get; set; } = null!;

        // Employee Relationship
        [Required]
        public int EmployeeId { get; set; }

        public ApplicationUser Employee { get; set; } = null!;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public SubmissionStatus Status { get; set; }
            = SubmissionStatus.Submitted;
    }
}
