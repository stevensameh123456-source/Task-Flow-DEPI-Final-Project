// Features/Teams/Services/TeamService.cs
using FluentValidation;
using TaskFlow.Data.Entities;
using TaskFlow.Data.Interfaces;
using TaskFlow.Services.Common.Abstractions;
using TaskFlow.Services.Common.Constants;
using TaskFlow.Services.Common.Exceptions;
using TaskFlow.Services.Common.Results;
using TaskFlow.Services.Features.Permissions.Interfaces;
using TaskFlow.Services.Features.Permissions.Policies;
using TaskFlow.Services.Features.Teams.DTOs;
using TaskFlow.Services.Features.Teams.Interfaces;
using TaskFlow.Services.Features.Teams.Mappers;
using TaskStatus = TaskFlow.Data.Entities.TaskStatus;

namespace TaskFlow.Services.Features.Teams.Services;

public class TeamService : ITeamService
{
    private readonly IUnitOfWork _uow;
    private readonly IPermissionService _permissions;
    private readonly ICurrentUser _currentUser;
    private readonly IAuditLogger _audit;
    private readonly IValidator<CreateTeamDto> _createValidator;

    public TeamService(IUnitOfWork uow, IPermissionService permissions,
        ICurrentUser currentUser, IAuditLogger audit, IValidator<CreateTeamDto> createValidator)
    {
        _uow = uow;
        _permissions = permissions;
        _currentUser = currentUser;
        _audit = audit;
        _createValidator = createValidator;
    }

    private IGenericRepository<Team> Teams => _uow.Repository<Team>();
    private IGenericRepository<ApplicationUser> Users => _uow.Repository<ApplicationUser>();

    public async Task<ServiceResult<TeamResponseDto>> CreateAsync(CreateTeamDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ServiceResult<TeamResponseDto>.Failure(validation.Errors[0].ErrorMessage);

        await EnsureCanManageAsync();

        var project = await _uow.Repository<Project>().GetByIdAsync(dto.ProjectId);
        if (project is null)
            return ServiceResult<TeamResponseDto>.Failure("Project not found.");
        if (project.Status is ProjectStatus.Completed or ProjectStatus.Cancelled)
            return ServiceResult<TeamResponseDto>.Failure($"Cannot add a team to a {project.Status} project.");

        if (!await _permissions.HasRoleAsync(dto.LeaderId, RoleNames.TeamLeader))
            return ServiceResult<TeamResponseDto>.Failure("The selected user is not a Team Leader.");

        var leader = await Users.GetByIdAsync(dto.LeaderId)
            ?? throw new NotFoundException(nameof(ApplicationUser), dto.LeaderId);

        // Team Rule 1: الموظف (والقائد) في فريق واحد فقط
        if (leader.TeamId is not null)
            return ServiceResult<TeamResponseDto>.Failure("This leader already belongs to a team.");

        var team = new Team
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            ProjectId = dto.ProjectId,
            LeaderId = dto.LeaderId
        };
        await Teams.AddAsync(team);
        await _uow.SaveChangesAsync();

        leader.TeamId = team.Id;
        Users.Update(leader);
        await _uow.SaveChangesAsync();
        await _audit.LogAsync("TeamCreated", nameof(Team), team.Id);

        return ServiceResult<TeamResponseDto>.Success(await MapAsync(team));
    }

    public async Task<ServiceResult<TeamResponseDto>> UpdateAsync(int id, string name, string? description)
    {
        await EnsureCanManageAsync();

        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length < 2)
            return ServiceResult<TeamResponseDto>.Failure("Team name must be at least 2 characters.");

        var team = await Teams.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Team), id);

        team.Name = name.Trim();
        team.Description = description?.Trim();
        Teams.Update(team);
        await _uow.SaveChangesAsync();
        await _audit.LogAsync("TeamUpdated", nameof(Team), id);

        return ServiceResult<TeamResponseDto>.Success(await MapAsync(team));
    }

    public async Task<ServiceResult<TeamResponseDto>> GetByIdAsync(int id)
    {
        var team = await Teams.GetByIdAsync(id);
        return team is null
            ? ServiceResult<TeamResponseDto>.Failure("Team not found.")
            : ServiceResult<TeamResponseDto>.Success(await MapAsync(team));
    }

    public async Task<ServiceResult<IReadOnlyList<TeamResponseDto>>> GetByProjectAsync(int projectId)
    {
        var teams = await Teams.FindAsync(t => t.ProjectId == projectId);
        var result = new List<TeamResponseDto>();
        foreach (var t in teams)
            result.Add(await MapAsync(t));

        return ServiceResult<IReadOnlyList<TeamResponseDto>>.Success(result);
    }

    public async Task<ServiceResult> AssignLeaderAsync(int teamId, int leaderId)
    {
        await EnsureCanManageAsync();

        var team = await Teams.GetByIdAsync(teamId)
            ?? throw new NotFoundException(nameof(Team), teamId);

        if (!await _permissions.HasRoleAsync(leaderId, RoleNames.TeamLeader))
            return ServiceResult.Failure("The selected user is not a Team Leader.");

        var newLeader = await Users.GetByIdAsync(leaderId)
            ?? throw new NotFoundException(nameof(ApplicationUser), leaderId);

        if (newLeader.TeamId is not null && newLeader.TeamId != teamId)
            return ServiceResult.Failure("This leader already belongs to another team.");

        team.LeaderId = leaderId;
        newLeader.TeamId = teamId;
        Teams.Update(team);
        Users.Update(newLeader);
        await _uow.SaveChangesAsync();
        await _audit.LogAsync("TeamLeaderAssigned", nameof(Team), teamId);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> AddEmployeeAsync(int teamId, int employeeId)
    {
        await EnsureCanManageAsync();

        if (!await Teams.ExistsAsync(t => t.Id == teamId))
            throw new NotFoundException(nameof(Team), teamId);

        var employee = await Users.GetByIdAsync(employeeId)
            ?? throw new NotFoundException(nameof(ApplicationUser), employeeId);

        if (!employee.IsActive)
            return ServiceResult.Failure("This user account is inactive.");
        if (employee.TeamId == teamId)
            return ServiceResult.Success();
        if (employee.TeamId is not null)
            return ServiceResult.Failure("This employee already belongs to another team.");

        employee.TeamId = teamId;
        Users.Update(employee);
        await _uow.SaveChangesAsync();
        await _audit.LogAsync("EmployeeAddedToTeam", nameof(Team), teamId);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RemoveEmployeeAsync(int teamId, int employeeId)
    {
        await EnsureCanManageAsync();

        var team = await Teams.GetByIdAsync(teamId)
            ?? throw new NotFoundException(nameof(Team), teamId);
        var employee = await Users.GetByIdAsync(employeeId)
            ?? throw new NotFoundException(nameof(ApplicationUser), employeeId);

        if (employee.TeamId != teamId)
            return ServiceResult.Failure("This employee is not in the team.");
        if (team.LeaderId == employeeId)
            return ServiceResult.Failure("Assign another leader before removing the current one.");

        var hasOpenTasks = await _uow.Repository<TaskItem>()
            .ExistsAsync(t => t.AssignedEmployeeId == employeeId && t.Status != TaskStatus.Completed);
        if (hasOpenTasks)
            return ServiceResult.Failure("The employee still has unfinished tasks. Reassign them first.");

        employee.TeamId = null;
        Users.Update(employee);
        await _uow.SaveChangesAsync();
        await _audit.LogAsync("EmployeeRemovedFromTeam", nameof(Team), teamId);

        return ServiceResult.Success();
    }

    private int RequireUser()
        => _currentUser.UserId ?? throw new UnauthorizedOperationException("You must be signed in.");

    private Task EnsureCanManageAsync()
        => _permissions.EnsurePermissionAsync(RequireUser(), AppPermissions.ManageTeams);

    private async Task<TeamResponseDto> MapAsync(Team t)
        => TeamMapper.ToDto(t, await Users.CountAsync(u => u.TeamId == t.Id));
}