using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Web.ViewModels.Workspaces;

public class WorkspaceFormVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Workspace name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}