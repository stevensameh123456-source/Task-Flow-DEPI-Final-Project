using System.Linq;
using FluentValidation;
using TaskFlow.Data.Entities;
using TaskFlow.Data.Interfaces;
using TaskFlow.Services.Common.Abstractions;
using TaskFlow.Services.Common.Exceptions;
using TaskFlow.Services.Common.Results;
using TaskFlow.Services.Features.Workspaces.DTOs;
using TaskFlow.Services.Features.Workspaces.Interfaces;
using TaskFlow.Services.Features.Workspaces.Mappers;

namespace TaskFlow.Services.Features.Workspaces.Services;

public class WorkspaceService : IWorkspaceService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;
    private readonly IAuditLogger _audit;
    private readonly IValidator<CreateWorkspaceDto> _createValidator;

    public WorkspaceService(
        IUnitOfWork uow,
        ICurrentUser currentUser,
        IAuditLogger audit,
        IValidator<CreateWorkspaceDto> createValidator)
    {
        _uow = uow;
        _currentUser = currentUser;
        _audit = audit;
        _createValidator = createValidator;
    }

    private IGenericRepository<Workspace> Workspaces =>
        _uow.Repository<Workspace>();

    private IGenericRepository<WorkspaceMember> Members =>
        _uow.Repository<WorkspaceMember>();

    public async Task<ServiceResult<WorkspaceResponseDto>> CreateAsync(CreateWorkspaceDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);

        if (!validation.IsValid)
            return ServiceResult<WorkspaceResponseDto>.Failure(validation.Errors[0].ErrorMessage);

        var userId = RequireUser();

        var workspace = new Workspace
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            OwnerId = userId
        };

        workspace.Members.Add(new WorkspaceMember
        {
            UserId = userId,
            Role = WorkspaceRole.Owner
        });

        await Workspaces.AddAsync(workspace);
        await _uow.SaveChangesAsync();

        await _audit.LogAsync("WorkspaceCreated", nameof(Workspace), workspace.Id);

        return ServiceResult<WorkspaceResponseDto>.Success(await MapAsync(workspace));
    }

    public async Task<ServiceResult<WorkspaceResponseDto>> UpdateAsync(int id, UpdateWorkspaceDto dto)
    {
        await EnsureWorkspaceAdminAsync(id);

        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length < 3)
            return ServiceResult<WorkspaceResponseDto>.Failure("Workspace name must be at least 3 characters.");

        var workspace = await Workspaces.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Workspace), id);

        workspace.Name = dto.Name.Trim();
        workspace.Description = dto.Description?.Trim();

        Workspaces.Update(workspace);
        await _uow.SaveChangesAsync();

        await _audit.LogAsync("WorkspaceUpdated", nameof(Workspace), id);

        return ServiceResult<WorkspaceResponseDto>.Success(await MapAsync(workspace));
    }

    public async Task<ServiceResult<WorkspaceResponseDto>> GetByIdAsync(int id)
    {
        await EnsureMemberAsync(id);

        var workspace = await Workspaces.GetByIdAsync(id);

        return workspace is null
            ? ServiceResult<WorkspaceResponseDto>.Failure("Workspace not found.")
            : ServiceResult<WorkspaceResponseDto>.Success(await MapAsync(workspace));
    }

    public async Task<ServiceResult<IReadOnlyList<WorkspaceResponseDto>>> GetMineAsync()
    {
        var userId = RequireUser();

        var allMemberships = await Members.GetAllAsync();

        var memberships = allMemberships
            .Where(m => m.UserId == userId)
            .ToList();

        var result = new List<WorkspaceResponseDto>();

        foreach (var membership in memberships)
        {
            var workspace = await Workspaces.GetByIdAsync(membership.WorkspaceId);

            if (workspace is not null)
                result.Add(await MapAsync(workspace));
        }

        return ServiceResult<IReadOnlyList<WorkspaceResponseDto>>.Success(result);
    }

    public async Task<ServiceResult<IReadOnlyList<WorkspaceMemberDto>>> GetMembersAsync(int workspaceId)
    {
        await EnsureMemberAsync(workspaceId);

        var allMembers = await Members.GetAllAsync();

        var members = allMembers
            .Where(m => m.WorkspaceId == workspaceId)
            .ToList();

        var ids = members.Select(m => m.UserId).ToList();

        var allUsers = await _uow.Repository<ApplicationUser>().GetAllAsync();

        var users = allUsers
            .Where(u => ids.Contains(u.Id))
            .ToList();

        var names = users.ToDictionary(u => u.Id, u => u.FullName);

        var result = members
            .Select(m => WorkspaceMapper.ToDto(
                m,
                names.GetValueOrDefault(m.UserId, "Unknown")))
            .ToList();

        return ServiceResult<IReadOnlyList<WorkspaceMemberDto>>.Success(result);
    }

    public async Task<ServiceResult> AddMemberAsync(int workspaceId, AddWorkspaceMemberDto dto)
    {
        await EnsureWorkspaceAdminAsync(workspaceId);

        if (dto.Role == WorkspaceRole.Owner)
            return ServiceResult.Failure("A workspace has a single owner.");

        var user = await _uow.Repository<ApplicationUser>().GetByIdAsync(dto.UserId);

        if (user is null || !user.IsActive)
            return ServiceResult.Failure("User not found or inactive.");

        var allMembers = await Members.GetAllAsync();

        if (allMembers.Any(m =>
            m.WorkspaceId == workspaceId &&
            m.UserId == dto.UserId))
        {
            return ServiceResult.Failure("The user is already a member.");
        }

        await Members.AddAsync(new WorkspaceMember
        {
            WorkspaceId = workspaceId,
            UserId = dto.UserId,
            Role = dto.Role
        });

        await _uow.SaveChangesAsync();

        await _audit.LogAsync("WorkspaceMemberAdded", nameof(Workspace), workspaceId);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> ChangeMemberRoleAsync(int workspaceId, int userId, WorkspaceRole role)
    {
        await EnsureWorkspaceAdminAsync(workspaceId);

        if (role == WorkspaceRole.Owner)
            return ServiceResult.Failure("A workspace has a single owner.");

        var member = await FindMemberAsync(workspaceId, userId);

        if (member is null)
            return ServiceResult.Failure("Member not found.");

        if (member.Role == WorkspaceRole.Owner)
            return ServiceResult.Failure("The owner's role cannot be changed.");

        member.Role = role;

        Members.Update(member);
        await _uow.SaveChangesAsync();

        await _audit.LogAsync("WorkspaceRoleChanged", nameof(Workspace), workspaceId);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RemoveMemberAsync(int workspaceId, int userId)
    {
        await EnsureWorkspaceAdminAsync(workspaceId);

        var member = await FindMemberAsync(workspaceId, userId);

        if (member is null)
            return ServiceResult.Failure("Member not found.");

        if (member.Role == WorkspaceRole.Owner)
            return ServiceResult.Failure("The owner cannot be removed.");

        Members.Delete(member);
        await _uow.SaveChangesAsync();

        await _audit.LogAsync("WorkspaceMemberRemoved", nameof(Workspace), workspaceId);

        return ServiceResult.Success();
    }

    private int RequireUser()
    {
        return _currentUser.UserId
            ?? throw new UnauthorizedOperationException("You must be signed in.");
    }

    private async Task<WorkspaceMember?> FindMemberAsync(int workspaceId, int userId)
    {
        var allMembers = await Members.GetAllAsync();

        return allMembers.FirstOrDefault(m =>
            m.WorkspaceId == workspaceId &&
            m.UserId == userId);
    }

    private async Task EnsureMemberAsync(int workspaceId)
    {
        var member = await FindMemberAsync(workspaceId, RequireUser());

        if (member is null)
            throw new UnauthorizedOperationException("You are not a member of this workspace.");
    }

    private async Task EnsureWorkspaceAdminAsync(int workspaceId)
    {
        var member = await FindMemberAsync(workspaceId, RequireUser());

        if (member is null ||
            member.Role is not (WorkspaceRole.Owner or WorkspaceRole.Admin))
        {
            throw new UnauthorizedOperationException(
                "Only a workspace Owner or Admin can do this.");
        }
    }

    private async Task<WorkspaceResponseDto> MapAsync(Workspace workspace)
    {
        var allMembers = await Members.GetAllAsync();

        var membersCount = allMembers.Count(m => m.WorkspaceId == workspace.Id);

        var allProjects = await _uow.Repository<Project>().GetAllAsync();

        var projectsCount = allProjects.Count(p => p.WorkspaceId == workspace.Id);

        return WorkspaceMapper.ToDto(workspace, membersCount, projectsCount);
    }
}