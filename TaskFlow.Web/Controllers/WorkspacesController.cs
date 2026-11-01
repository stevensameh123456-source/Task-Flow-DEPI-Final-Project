using Microsoft.AspNetCore.Mvc;
using TaskFlow.Web.Mock;
using TaskFlow.Web.ViewModels.Workspaces;

namespace TaskFlow.Web.Controllers;

public class WorkspacesController : Controller
{
    private readonly IWorkspaceUiService _service;

    public WorkspacesController(IWorkspaceUiService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        return View(_service.GetAll());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new WorkspaceFormVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(WorkspaceFormVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _service.Create(model);
        TempData["Success"] = "Workspace created successfully";
        return RedirectToAction(nameof(Index));
    }
}