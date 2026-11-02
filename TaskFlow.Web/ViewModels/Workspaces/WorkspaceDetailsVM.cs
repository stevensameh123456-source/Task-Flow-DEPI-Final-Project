using TaskFlow.Web.ViewModels.Teams;

namespace TaskFlow.Web.ViewModels.Workspaces;

public class WorkspaceDetailsVM
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TeamListItemVM> Teams { get; set; } = new();
}