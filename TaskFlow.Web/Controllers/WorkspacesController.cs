using Microsoft.AspNetCore.Mvc;
using TaskFlow.Web.Mock;
using TaskFlow.Web.ViewModels.Workspaces;

namespace TaskFlow.Web.Controllers;

public class WorkspacesController : Controller
{
    private readonly IWorkspaceUiService _workspaces;
    private readonly ITeamUiService _teams;

    public WorkspacesController(IWorkspaceUiService workspaces, ITeamUiService teams)
    {
        _workspaces = workspaces;
        _teams = teams;
    }

    public IActionResult Index() => View(_workspaces.GetAll());

    public IActionResult Details(int id)
    {
        var w = _workspaces.GetById(id);
        if (w == null) return NotFound();

        return View(new WorkspaceDetailsVM
        {
            Id = w.Id,
            Name = w.Name,
            Description = w.Description,
            CreatedAt = w.CreatedAt,
            Teams = _teams.GetByWorkspace(id)
        });
    }

    [HttpGet]
    public IActionResult Create() => View(new WorkspaceFormVM());

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(WorkspaceFormVM model)
    {
        if (!ModelState.IsValid) return View(model);
        _workspaces.Create(model);
        TempData["Success"] = "Workspace created successfully";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var w = _workspaces.GetById(id);
        if (w == null) return NotFound();
        return View(new WorkspaceFormVM { Id = w.Id, Name = w.Name, Description = w.Description });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(WorkspaceFormVM model)
    {
        if (!ModelState.IsValid) return View(model);
        if (!_workspaces.Update(model)) return NotFound();
        TempData["Success"] = "Workspace updated";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }
}