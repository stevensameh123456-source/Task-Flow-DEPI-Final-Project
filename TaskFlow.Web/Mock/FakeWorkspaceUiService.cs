using TaskFlow.Web.ViewModels.Workspaces;

namespace TaskFlow.Web.Mock;

public class FakeWorkspaceUiService : IWorkspaceUiService
{
    private static readonly List<WorkspaceListItemVM> _items = new()
    {
        new() { Id = 1, Name = "Marketing", Description = "Campaigns and content", TeamsCount = 2, MembersCount = 8, CreatedAt = DateTime.Now.AddDays(-20) },
        new() { Id = 2, Name = "Development", Description = "Product engineering", TeamsCount = 3, MembersCount = 12, CreatedAt = DateTime.Now.AddDays(-5) },
    };

    public List<WorkspaceListItemVM> GetAll() => _items.OrderByDescending(x => x.CreatedAt).ToList();

    public void Create(WorkspaceFormVM model)
    {
        _items.Add(new WorkspaceListItemVM
        {
            Id = _items.Count + 1,
            Name = model.Name,
            Description = model.Description,
            CreatedAt = DateTime.Now
        });
    }
}