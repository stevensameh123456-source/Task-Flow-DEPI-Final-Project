using TaskFlow.Web.ViewModels.Workspaces;

namespace TaskFlow.Web.Mock;

public class FakeWorkspaceUiService : IWorkspaceUiService
{
    private static int _nextId = 3;
    private static readonly List<WorkspaceListItemVM> _items = new()
    {
        new() { Id = 1, Name = "Marketing", Description = "Campaigns and content", TeamsCount = 1, MembersCount = 3, CreatedAt = DateTime.Now.AddDays(-20) },
        new() { Id = 2, Name = "Development", Description = "Product engineering", TeamsCount = 1, MembersCount = 2, CreatedAt = DateTime.Now.AddDays(-5) },
    };

    public List<WorkspaceListItemVM> GetAll() => _items.OrderByDescending(x => x.CreatedAt).ToList();

    public WorkspaceListItemVM? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

    public void Create(WorkspaceFormVM model)
    {
        _items.Add(new WorkspaceListItemVM
        {
            Id = _nextId++,
            Name = model.Name,
            Description = model.Description,
            CreatedAt = DateTime.Now
        });
    }

    public bool Update(WorkspaceFormVM model)
    {
        var item = GetById(model.Id);
        if (item == null) return false;
        item.Name = model.Name;
        item.Description = model.Description;
        return true;
    }
}