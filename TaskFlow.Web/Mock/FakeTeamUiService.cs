using TaskFlow.Web.ViewModels.Teams;

namespace TaskFlow.Web.Mock;

public class FakeTeamUiService : ITeamUiService
{
    private class MockTeam
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public List<TeamMemberVM> Members { get; set; } = new();
    }

    private static int _nextTeamId = 3;
    private static int _nextMemberId = 6;

    private static readonly List<MockTeam> _teams = new()
    {
        new MockTeam
        {
            Id = 1, WorkspaceId = 1, Name = "Content Team", Description = "Blog and social media",
            Members = new()
            {
                new() { Id = 1, FullName = "Ahmed Ali", Email = "ahmed@test.com", Role = MockRole.TeamLeader, JoinedAt = DateTime.Now.AddDays(-18), LastActiveAt = DateTime.Now.AddHours(-2) },
                new() { Id = 2, FullName = "Mona Samy", Email = "mona@test.com", Role = MockRole.Employee, JoinedAt = DateTime.Now.AddDays(-15), LastActiveAt = DateTime.Now.AddDays(-1) },
                new() { Id = 3, FullName = "Omar Hassan", Email = "omar@test.com", Role = MockRole.HR, JoinedAt = DateTime.Now.AddDays(-10), LastActiveAt = DateTime.Now.AddDays(-4) },
            }
        },
        new MockTeam
        {
            Id = 2, WorkspaceId = 2, Name = "Backend Team", Description = "APIs and database",
            Members = new()
            {
                new() { Id = 4, FullName = "Sara Adel", Email = "sara@test.com", Role = MockRole.ProjectManager, JoinedAt = DateTime.Now.AddDays(-5), LastActiveAt = DateTime.Now.AddHours(-5) },
                new() { Id = 5, FullName = "Khaled Nabil", Email = "khaled@test.com", Role = MockRole.Employee, JoinedAt = DateTime.Now.AddDays(-4), LastActiveAt = DateTime.Now.AddDays(-2) },
            }
        }
    };

    public List<TeamListItemVM> GetByWorkspace(int workspaceId) =>
        _teams.Where(t => t.WorkspaceId == workspaceId)
              .Select(t => new TeamListItemVM { Id = t.Id, Name = t.Name, Description = t.Description, MembersCount = t.Members.Count })
              .ToList();

    public TeamDetailsVM? GetById(int id)
    {
        var t = _teams.FirstOrDefault(x => x.Id == id);
        if (t == null) return null;
        return new TeamDetailsVM
        {
            Id = t.Id,
            WorkspaceId = t.WorkspaceId,
            Name = t.Name,
            Description = t.Description,
            Members = t.Members.ToList()
        };
    }

    public void Create(TeamFormVM model)
    {
        _teams.Add(new MockTeam
        {
            Id = _nextTeamId++,
            WorkspaceId = model.WorkspaceId,
            Name = model.Name,
            Description = model.Description
        });
    }

    public bool AddMember(int teamId, string email, MockRole role)
    {
        var team = _teams.FirstOrDefault(t => t.Id == teamId);
        if (team == null) return false;
        if (team.Members.Any(m => m.Email.Equals(email, StringComparison.OrdinalIgnoreCase))) return false;

        team.Members.Add(new TeamMemberVM
        {
            Id = _nextMemberId++,
            FullName = email.Split('@')[0],
            Email = email,
            Role = role,
            JoinedAt = DateTime.Now,
            LastActiveAt = DateTime.Now
        });
        return true;
    }

    public void ChangeRole(int teamId, int memberId, MockRole role)
    {
        var m = _teams.FirstOrDefault(t => t.Id == teamId)?.Members.FirstOrDefault(x => x.Id == memberId);
        if (m != null) m.Role = role;
    }

    public void RemoveMember(int teamId, int memberId)
    {
        _teams.FirstOrDefault(t => t.Id == teamId)?.Members.RemoveAll(x => x.Id == memberId);
    }
}