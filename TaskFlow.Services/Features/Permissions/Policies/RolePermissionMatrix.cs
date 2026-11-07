// Features/Permissions/Policies/RolePermissionMatrix.cs
using TaskFlow.Services.Common.Constants;

namespace TaskFlow.Services.Features.Permissions.Policies;

public static class AppPermissions
{
    public const string ManageProjects = "projects.manage";
    public const string ManageTeams = "teams.manage";
    public const string CreateTasks = "tasks.create";
    public const string SubmitWork = "tasks.submit";
    public const string ReviewTasks = "tasks.review";
    public const string ViewPortfolio = "portfolio.view";
    public const string ViewReports = "reports.view";
    public const string ViewAllActivity = "activity.viewAll";
    public const string ManageEmployees = "employees.manage";
}

// بيحوّل أدوار Identity إلى صلاحيات. مش بيكرر Identity، بس بيسمّي القواعد في مكان واحد.
public static class RolePermissionMatrix
{
    private static readonly IReadOnlyDictionary<string, HashSet<string>> Matrix =
        new Dictionary<string, HashSet<string>>
        {
            [RoleNames.Employee] = new()
            {
                AppPermissions.SubmitWork
            },
            [RoleNames.TeamLeader] = new()
            {
                AppPermissions.CreateTasks, AppPermissions.ReviewTasks
            },
            [RoleNames.ProjectManager] = new()
            {
                AppPermissions.ManageProjects, AppPermissions.ManageTeams,
                AppPermissions.ReviewTasks, AppPermissions.ViewPortfolio,
                AppPermissions.ViewReports, AppPermissions.ViewAllActivity
            },
            [RoleNames.HR] = new()
            {
                AppPermissions.ManageEmployees, AppPermissions.ViewAllActivity
            },
            [RoleNames.CEO] = new()
            {
                AppPermissions.ViewPortfolio, AppPermissions.ViewReports,
                AppPermissions.ViewAllActivity
            }
        };

    public static IReadOnlySet<string> GetPermissions(IEnumerable<string> roles)
    {
        var result = new HashSet<string>();
        foreach (var role in roles)
            if (Matrix.TryGetValue(role, out var permissions))
                result.UnionWith(permissions);
        return result;
    }

    public static bool HasPermission(IEnumerable<string> roles, string permission)
        => GetPermissions(roles).Contains(permission);
}