using OneCampus.Domain;
using OneCampus.Domain.Entities;
using OneCampus.Domain.Entities.Permissions;
using OneCampus.Domain.Exceptions;
using OneCampus.Domain.Repositories;
using OneCampus.Domain.Services;

namespace OneCampus.Application.Services;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;

    private readonly UserInfo _userInfo;

    public PermissionService(
        IPermissionRepository permissionRepository,
        IUserRepository userRepository,
        IGroupRepository groupRepository,
        UserInfo userInfo)
    {
        _permissionRepository = permissionRepository.ThrowIfNull().Value;
        _userRepository = userRepository.ThrowIfNull().Value;
        _groupRepository = groupRepository.ThrowIfNull().Value;
        _userInfo = userInfo.ThrowIfNull().Value;
        _userInfo.Id.Throw().IfDefault();
    }

    public async Task<UserPermissions> AllowPermissionsAsync(Guid userId, int groupId, IList<PermissionType> permissions)
    {
        userId.Throw().IfDefault();
        groupId.Throw().IfNegativeOrZero();

        await ValidatePermissionAsync(_userInfo.Id, groupId, PermissionType.ManageUsersPermission);
        await ValidateGroupAccessAsync(groupId);

        if (userId == _userInfo.Id)
        {
            throw new ForbiddenException("the user cannot change their own permissions");
        }

        var user = await _userRepository.FindAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var group = await _groupRepository.FindAsync(groupId);
        if (group == null)
        {
            throw new NotFoundException("Group not found");
        }

        var result = await _permissionRepository.AllowPermissionsAsync(userId, groupId, permissions);
        if (result == null)
        {
            throw new NotFoundException("permissions not found");
        }

        return result;
    }

    public async Task<UserPermissions> DenyPermissionsAsync(Guid userId, int groupId, IList<PermissionType> permissions)
    {
        userId.Throw().IfDefault();
        groupId.Throw().IfNegativeOrZero();

        await ValidatePermissionAsync(_userInfo.Id, groupId, PermissionType.ManageUsersPermission);
        await ValidateGroupAccessAsync(groupId);

        if (userId == _userInfo.Id)
        {
            throw new ForbiddenException("the user cannot change their own permissions");
        }

        var user = await _userRepository.FindAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var group = await _groupRepository.FindAsync(groupId);
        if (group == null)
        {
            throw new NotFoundException("Group not found");
        }

        var result = await _permissionRepository.DenyPermissionsAsync(userId, groupId, permissions);
        if (result == null)
        {
            throw new NotFoundException("permissions not found");
        }

        return result;
    }

    public async Task<UserPermissions> GetPermissionsAsync(int groupId)
    {
        groupId.Throw().IfNegativeOrZero();

        await ValidatePermissionAsync(_userInfo.Id, groupId, PermissionType.ManageUsersPermission);
        await ValidateGroupAccessAsync(groupId);

        var group = await _groupRepository.FindAsync(groupId);
        if (group == null)
        {
            throw new NotFoundException("Group not found");
        }

        var permissions = await _permissionRepository.GetPermissionsAsync(_userInfo.Id, groupId);
        if (permissions == null)
        {
            throw new NotFoundException("No permissions found");
        }

        return permissions;
    }

    public async Task<UserPermissions> GetPermissionsAsync(Guid userId, int groupId)
    {
        userId.Throw().IfDefault();
        groupId.Throw().IfNegativeOrZero();

        await ValidateGroupAccessAsync(groupId);

        var user = await _userRepository.FindAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var group = await _groupRepository.FindAsync(groupId);
        if (group == null)
        {
            throw new NotFoundException("Group not found");
        }

        var permissions = await _permissionRepository.GetPermissionsAsync(userId, groupId);
        if (permissions == null)
        {
            throw new NotFoundException("No permissions found");
        }

        return permissions;
    }

    public async Task ValidatePermissionAsync(Guid userId, int groupId, PermissionType type)
    {
        userId.Throw().IfDefault();

        var hasPermission = await _permissionRepository.UserHasPermissionAsync(_userInfo.Id, groupId, type);
        if (!hasPermission)
        {
            throw new ForbiddenException("the user does not have the permission: " + type);
        }
    }

    private async Task ValidateGroupAccessAsync(int groupId)
    {
        groupId.Throw()
            .IfNegativeOrZero();

        var hasAccess = await _groupRepository.HasAccessAsync(_userInfo.Id, groupId);
        if (!hasAccess)
        {
            throw new ForbiddenException("the user does not have access to the group");
        }
    }
}
