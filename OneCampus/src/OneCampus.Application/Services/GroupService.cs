using OneCampus.Domain.Entities.Groups;
using OneCampus.Domain.Exceptions;
using OneCampus.Domain.Repositories;
using OneCampus.Domain.Services;

namespace OneCampus.Application.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IInstitutionRepository _institutionRepository;

    public GroupService(
        IGroupRepository groupRepository,
        IUserRepository userRepository,
        IInstitutionRepository institutionRepository)
    {
        _groupRepository = groupRepository.ThrowIfNull().Value;
        _userRepository = userRepository.ThrowIfNull().Value;
        _institutionRepository = institutionRepository.ThrowIfNull().Value;
    }

    public async Task<Group> CreateGroupAsync(Guid userId, string name, int parentGroupId)
    {
        name.Throw()
            .IfEmpty()
            .IfWhiteSpace();

        await ValidateGroupAccessAsync(userId, parentGroupId);

        var group = await _groupRepository.CreateAsync(name, parentGroupId);

        await _groupRepository.AddUserAsync(group.Id, userId);

        return group;
    }

    public async Task<Group> UpdateGroupAsync(Guid userId, int id, string name)
    {
        name.Throw()
            .IfEmpty()
            .IfWhiteSpace();

        await ValidateGroupAccessAsync(userId, id);

        var group = await _groupRepository.UpdateAsync(id, name);
        if (group is null)
        {
            throw new NotFoundException("group not found.");
        }

        return group;
    }

    public async Task<IEnumerable<Group>> GetGroupsAsync(Guid userId)
    {
        var institutions = await _institutionRepository.GetAsync(userId);

        var groups = new List<Group>(institutions.Count());
        foreach (var institution in institutions)
        {
            var group = await _groupRepository.FindByInstitutionIdAsync(institution.Id);
            if (group is null)
            {
                throw new NotFoundException("institution group not found.");
            }

            group = await GetGroupWithSubGroupsAsync(userId, group);

            groups.Add(group!);
        }

        return groups;
    }

    public async Task<GroupDetails> FindGroupAsync(Guid userId, int id)
    {
        await ValidateGroupAccessAsync(userId, id);

        var group = await _groupRepository.FindAsync(id);
        if (group is null)
        {
            throw new NotFoundException("group not found.");
        }

        return group;
    }

    public async Task<Group?> DeleteGroupAsync(Guid userId, int id)
    {
        await ValidateGroupAccessAsync(userId, id);

        return await _groupRepository.DeleteAsync(id);
    }

    public async Task<GroupDetails> AddUserAsync(Guid userId, int groupId, Guid userIdToAdd)
    {
        await ValidateGroupAccessAsync(userId, groupId);

        userIdToAdd.Throw()
            .IfDefault();

        var group = await _groupRepository.FindAsync(groupId);
        if (group is null)
        {
            throw new NotFoundException("group not found.");
        }

        if (group.Users.Any(item => item.Id == userIdToAdd))
        {
            return group;
        }

        var user = await _userRepository.FindAsync(userIdToAdd);
        if (user is null)
        {
            throw new NotFoundException("user not found.");
        }

        group = await _groupRepository.AddUserAsync(groupId, userIdToAdd);
        if (group is null)
        {
            throw new NotFoundException("group not found.");
        }

        return group;
    }

    public async Task<GroupDetails> RemoveUserAsync(Guid userId, int groupId, Guid userIdToRemove)
    {
        await ValidateGroupAccessAsync(userId, groupId);

        userIdToRemove.Throw()
            .IfDefault();

        var  group = await _groupRepository.RemoveUserAsync(groupId, userIdToRemove);
        if (group is null)
        {
            throw new NotFoundException("group not found.");
        }

        return group;
    }

    private async Task<Group> GetGroupWithSubGroupsAsync(Guid userId, Group group)
    {
        var subGroups = await _groupRepository.GetSubGroupsAsync(userId, group.Id);

        foreach (var subGroup in subGroups)
        {
            var subGroupWithGroups = await GetGroupWithSubGroupsAsync(userId, subGroup);

            group.SubGroup.Add(subGroupWithGroups);
        }

        return group;
    }

    public async Task<(IEnumerable<User> Results, int TotalResults)> GetUsersAsync(Guid userId, int id, int take, int skip)
    {
        await ValidateGroupAccessAsync(userId, id);

        take.Throw()
            .IfNegativeOrZero();

        skip.Throw()
            .IfNegative();

        var group = await _groupRepository.FindAsync(id);
        if (group is null)
        {
            throw new NotFoundException("group not found.");
        }

        return await _groupRepository.GetUsersAsync(id, take, skip);
    }

    private async Task ValidateGroupAccessAsync(Guid userId, int groupId)
    {
        userId.Throw()
            .IfDefault();

        groupId.Throw()
            .IfNegativeOrZero();

        var isUserInTheGroup = await _groupRepository.HasAccessAsync(userId, groupId);
        if (!isUserInTheGroup)
        {
            throw new ForbiddenException("the user does not have access to the group");
        }
    }
}
