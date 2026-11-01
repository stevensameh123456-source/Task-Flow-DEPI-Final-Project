using TaskFlow.Web.ViewModels.Teams;

namespace TaskFlow.Web.Mock;

public interface ITeamUiService
{
    List<TeamListItemVM> GetByWorkspace(int workspaceId);
    TeamDetailsVM? GetById(int id);
    void Create(TeamFormVM model);
    bool AddMember(int teamId, string email, MockRole role);
    void ChangeRole(int teamId, int memberId, MockRole role);
    void RemoveMember(int teamId, int memberId);
}