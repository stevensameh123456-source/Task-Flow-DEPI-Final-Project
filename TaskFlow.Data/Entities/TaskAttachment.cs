
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class TaskAttachment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ContentType { get; set; } = string.Empty;

        [Required]
        public long FileSize { get; set; }

        // Task Relationship
        [Required]
        public int TaskId { get; set; }

        public TaskItem Task { get; set; } = null!;

        // Uploader Relationship
        [Required]
        public int UploadedById { get; set; }

        public ApplicationUser UploadedBy { get; set; } = null!;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
