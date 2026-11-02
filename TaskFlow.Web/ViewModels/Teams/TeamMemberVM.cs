using TaskFlow.Web.Mock;

namespace TaskFlow.Web.ViewModels.Teams;

public class TeamMemberVM
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public MockRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime LastActiveAt { get; set; }
}