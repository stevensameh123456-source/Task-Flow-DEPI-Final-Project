using TaskFlow.Web.ViewModels.Workspaces;

namespace TaskFlow.Web.Mock;

public interface IWorkspaceUiService
{
    List<WorkspaceListItemVM> GetAll();
    void Create(WorkspaceFormVM model);
}