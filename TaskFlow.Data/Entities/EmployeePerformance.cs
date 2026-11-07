
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class EmployeePerformance
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public ApplicationUser Employee { get; set; } = null!;

        [Required]
        public int CompletedTasks { get; set; }

        [Required]
        public int PendingTasks { get; set; }

        [Required]
        public int RejectedTasks { get; set; }

        [Required]
        public double CompletionRate { get; set; }

        [Required]
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    }
}
