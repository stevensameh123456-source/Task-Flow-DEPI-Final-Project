
using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class ActivityLog
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public ApplicationUser User { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string EntityName { get; set; } = string.Empty;

        public int EntityId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
