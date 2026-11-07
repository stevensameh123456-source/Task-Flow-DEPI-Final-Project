using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(7)]
        public string Color { get; set; } = "#808080";

        [Required]
        public int ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        public ICollection<TaskTag> TaskTags { get; set; }
            = new List<TaskTag>();
    }
}