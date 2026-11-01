using Microsoft.AspNetCore.Mvc;
using TaskFlow.Web.Mock;
using TaskFlow.Web.ViewModels.Teams;

namespace TaskFlow.Web.Controllers;

public class TeamsController : Controller
{
    private readonly ITeamUiService _teams;
    private readonly IWorkspaceUiService _workspaces;

    public TeamsController(ITeamUiService teams, IWorkspaceUiService workspaces)
    {
        _teams = teams;
        _workspaces = workspaces;
    }

    [HttpGet]
    public IActionResult Create(int workspaceId)
    {
        if (_workspaces.GetById(workspaceId) == null) return NotFound();
        return View(new TeamFormVM { WorkspaceId = workspaceId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(TeamFormVM model)
    {
        if (!ModelState.IsValid) return View(model);
        _teams.Create(model);
        TempData["Success"] = "Team created";
        return RedirectToAction("Details", "Workspaces", new { id = model.WorkspaceId });
    }

    public IActionResult Details(int id)
    {
        var team = _teams.GetById(id);
        if (team == null) return NotFound();
        return View(team);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult InviteMember(InviteMemberVM model)
    {
        if (!ModelState.IsValid)
            TempData["Error"] = "Please enter a valid email";
        else if (_teams.AddMember(model.TeamId, model.Email, model.Role))
            TempData["Success"] = "Member added";
        else
            TempData["Error"] = "This email is already in the team";

        return RedirectToAction(nameof(Details), new { id = model.TeamId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult ChangeRole(int teamId, int memberId, MockRole role)
    {
        _teams.ChangeRole(teamId, memberId, role);
        TempData["Success"] = "Role updated";
        return RedirectToAction(nameof(Details), new { id = teamId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult RemoveMember(int teamId, int memberId)
    {
        _teams.RemoveMember(teamId, memberId);
        TempData["Success"] = "Member removed";
        return RedirectToAction(nameof(Details), new { id = teamId });
    }
}