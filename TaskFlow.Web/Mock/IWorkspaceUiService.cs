using TaskFlow.Web.ViewModels.Workspaces;

namespace TaskFlow.Web.Mock;

public interface IWorkspaceUiService
{
    List<WorkspaceListItemVM> GetAll();
    WorkspaceListItemVM? GetById(int id);
    void Create(WorkspaceFormVM model);
    bool Update(WorkspaceFormVM model);
}