namespace TaskFlow.Web.ViewModels.Teams;

public class TeamListItemVM
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int MembersCount { get; set; }
}