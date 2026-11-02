namespace TaskFlow.Web.ViewModels.Teams;

public class TeamDetailsVM
{
    public int Id { get; set; }
    public int WorkspaceId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public List<TeamMemberVM> Members { get; set; } = new();
}