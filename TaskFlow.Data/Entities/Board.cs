using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Data.Entities
{
    public class Board
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<BoardColumn> Columns { get; set; }
            = new List<BoardColumn>();
    }
}