
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class Report
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ReportType { get; set; } = string.Empty;

        [Required]
        public int GeneratedById { get; set; }

        public ApplicationUser GeneratedBy { get; set; } = null!;

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
