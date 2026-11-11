using FluentValidation;
using TaskFlow.Data.Entities;
using TaskFlow.Data.Interfaces;
using TaskFlow.Services.Common.Abstractions;
using TaskFlow.Services.Common.Constants;
using TaskFlow.Services.Common.Exceptions;
using TaskFlow.Services.Common.Results;
using TaskFlow.Services.Features.Permissions.Interfaces;
using TaskFlow.Services.Features.Permissions.Policies;
using TaskFlow.Services.Features.Projects.DTOs;
using TaskFlow.Services.Features.Projects.Interfaces;
using TaskFlow.Services.Features.Projects.Mappers;
using TaskStatus = TaskFlow.Data.Entities.TaskStatus;

namespace TaskFlow.Services.Features.Projects.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _uow;
    private readonly IPermissionService _permissions;
    private readonly ICurrentUser _currentUser;
    private readonly IAuditLogger _audit;
    private readonly IValidator<CreateProjectDto> _createValidator;

    private IGenericRepository<Project> Projects
        => _uow.Repository<Project>();

    public ProjectService(
        IUnitOfWork uow,
        IPermissionService permissions,
        ICurrentUser currentUser,
        IAuditLogger audit,
        IValidator<CreateProjectDto> createValidator)
    {
        _uow = uow;
        _permissions = permissions;
        _currentUser = currentUser;
        _audit = audit;
        _createValidator = createValidator;
    }

    // 1. Create Project
    public async Task<ServiceResult<ProjectResponseDto>> CreateAsync(
        CreateProjectDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);

        if (!validation.IsValid)
        {
            return ServiceResult<ProjectResponseDto>.Failure(
                validation.Errors[0].ErrorMessage);
        }

        await EnsureCanManageAsync();

        var workspaceExists = await _uow
            .Repository<Workspace>()
            .ExistsAsync(w => w.Id == dto.WorkspaceId);

        if (!workspaceExists)
        {
            return ServiceResult<ProjectResponseDto>.Failure(
                "Workspace not found.");
        }

        var managerExists = await _uow
            .Repository<ApplicationUser>()
            .ExistsAsync(u => u.Id == dto.ProjectManagerId && u.IsActive);

        if (!managerExists)
        {
            return ServiceResult<ProjectResponseDto>.Failure(
                "The selected Project Manager does not exist or is inactive.");
        }

        var hasManagerRole = await _permissions.HasRoleAsync(
            dto.ProjectManagerId,
            RoleNames.ProjectManager);

        if (!hasManagerRole)
        {
            return ServiceResult<ProjectResponseDto>.Failure(
                "The selected user is not a Project Manager.");
        }

        var project = new Project
        {
            WorkspaceId = dto.WorkspaceId,
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = ProjectStatus.Planning,
            ProjectManagerId = dto.ProjectManagerId
        };

        await Projects.AddAsync(project);
        await _uow.SaveChangesAsync();

        // Create a default Board for the new Project.
        var board = new Board
        {
            Name = project.Name,
            ProjectId = project.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<Board>().AddAsync(board);
        await _uow.SaveChangesAsync();

        await _audit.LogAsync(
            "ProjectCreated",
            nameof(Project),
            project.Id);

        return ServiceResult<ProjectResponseDto>.Success(
            await MapAsync(project));
    }

    // 2. Update Project
    public async Task<ServiceResult<ProjectResponseDto>> UpdateAsync(
        int id,
        UpdateProjectDto dto)
    {
        await EnsureCanManageAsync();

        var project = await Projects.GetByIdAsync(id);

        if (project == null)
        {
            throw new NotFoundException(nameof(Project), id);
        }

        if (string.IsNullOrWhiteSpace(dto.Name) ||
            dto.Name.Trim().Length < 3)
        {
            return ServiceResult<ProjectResponseDto>.Failure(
                "Project name must be at least 3 characters.");
        }

        if (dto.Name.Trim().Length > 150)
        {
            return ServiceResult<ProjectResponseDto>.Failure(
                "Project name cannot exceed 150 characters.");
        }

        if (dto.Description?.Length > 1000)
        {
            return ServiceResult<ProjectResponseDto>.Failure(
                "Project description cannot exceed 1000 characters.");
        }

        if (dto.EndDate.HasValue && dto.EndDate.Value <= dto.StartDate)
        {
            return ServiceResult<ProjectResponseDto>.Failure(
                "End date must be after the start date.");
        }

        // Completed and Cancelled projects cannot change status.
        if ((project.Status == ProjectStatus.Completed ||
             project.Status == ProjectStatus.Cancelled) &&
            dto.Status != project.Status)
        {
            return ServiceResult<ProjectResponseDto>.Failure(
                $"A {project.Status} project cannot change status.");
        }

        project.Name = dto.Name.Trim();
        project.Description = dto.Description?.Trim();
        project.StartDate = dto.StartDate;
        project.EndDate = dto.EndDate;
        project.Status = dto.Status;

        Projects.Update(project);

        await _uow.SaveChangesAsync();

        await _audit.LogAsync(
            "ProjectUpdated",
            nameof(Project),
            project.Id);

        return ServiceResult<ProjectResponseDto>.Success(
            await MapAsync(project));
    }

    // 3. Get Project by ID
    public async Task<ServiceResult<ProjectResponseDto>> GetByIdAsync(
        int id)
    {
        var project = await Projects.GetByIdAsync(id);

        if (project == null)
        {
            return ServiceResult<ProjectResponseDto>.Failure(
                "Project not found.");
        }

        return ServiceResult<ProjectResponseDto>.Success(
            await MapAsync(project));
    }

    // 4. Get Projects by Workspace
    public async Task<ServiceResult<IReadOnlyList<ProjectResponseDto>>>
        GetByWorkspaceAsync(int workspaceId)
    {
        var workspaceExists = await _uow
            .Repository<Workspace>()
            .ExistsAsync(w => w.Id == workspaceId);

        if (!workspaceExists)
        {
            return ServiceResult<IReadOnlyList<ProjectResponseDto>>.Failure(
                "Workspace not found.");
        }

        var projects = (await Projects.GetAllNoTrackingAsync())
            .Where(p => p.WorkspaceId == workspaceId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        var result = new List<ProjectResponseDto>();

        foreach (var project in projects)
        {
            result.Add(await MapAsync(project));
        }

        return ServiceResult<IReadOnlyList<ProjectResponseDto>>.Success(
            result);
    }

    // 5. Calculate Project Progress
    // Progress = Completed Tasks / Total Tasks * 100
    public async Task<double> GetProgressAsync(int projectId)
    {
        var projectExists = await Projects.ExistsAsync(
            p => p.Id == projectId);

        if (!projectExists)
        {
            throw new NotFoundException(nameof(Project), projectId);
        }

        var teams = await _uow
            .Repository<Team>()
            .GetAllNoTrackingAsync();

        var teamIds = teams
            .Where(t => t.ProjectId == projectId)
            .Select(t => t.Id)
            .ToHashSet();

        if (teamIds.Count == 0)
        {
            return 0;
        }

        var tasks = await _uow
            .Repository<TaskItem>()
            .GetAllNoTrackingAsync();

        var projectTasks = tasks
            .Where(t => teamIds.Contains(t.TeamId))
            .ToList();

        int totalTasks = projectTasks.Count;

        if (totalTasks == 0)
        {
            return 0;
        }

        int completedTasks = projectTasks.Count(
            t => t.Status == TaskStatus.Completed);

        return Math.Round(
            completedTasks * 100.0 / totalTasks,
            1);
    }

    // 6. Get current user
    private int RequireUser()
    {
        return _currentUser.UserId
            ?? throw new UnauthorizedOperationException(
                "You must be signed in.");
    }

    // 7. Check Manage Projects Permission
    private Task EnsureCanManageAsync()
    {
        return _permissions.EnsurePermissionAsync(
            RequireUser(),
            AppPermissions.ManageProjects);
    }

    // 8. Map Project to Response DTO
    private async Task<ProjectResponseDto> MapAsync(Project project)
    {
        var teams = await _uow
            .Repository<Team>()
            .GetAllNoTrackingAsync();

        int teamsCount = teams.Count(
            t => t.ProjectId == project.Id);

        double progress = await GetProgressAsync(project.Id);

        return ProjectMapper.ToDto(
            project,
            teamsCount,
            progress);
    }
}