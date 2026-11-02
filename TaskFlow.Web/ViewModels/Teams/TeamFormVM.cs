using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Web.ViewModels.Teams;

public class TeamFormVM
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }

    [Required(ErrorMessage = "Team name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = "";

    [StringLength(500)]
    public string? Description { get; set; }
}