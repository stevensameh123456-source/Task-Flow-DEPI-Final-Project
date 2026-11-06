using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class ChecklistItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public bool IsCompleted { get; set; } = false;

        [Required]
        public int Position { get; set; }

        [Required]
        public int TaskId { get; set; }

        public TaskItem Task { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}