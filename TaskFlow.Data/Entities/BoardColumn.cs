using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class BoardColumn
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int BoardId { get; set; }

        public Board Board { get; set; } = null!;

        [Required]
        public int Position { get; set; }

        public int? WipLimit { get; set; }

        public ICollection<TaskItem> Tasks { get; set; }
            = new List<TaskItem>();
    }
}