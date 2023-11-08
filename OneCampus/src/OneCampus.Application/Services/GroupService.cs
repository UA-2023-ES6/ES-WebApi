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

    public async Task<Group> CreateGroupAsync(string name, int parentGroupId)
    {
        name.Throw()
            .IfEmpty()
            .IfWhiteSpace();

        parentGroupId.Throw()
            .IfNegativeOrZero();

        var parentGroup = await _groupRepository.FindAsync(parentGroupId);
        if (parentGroup is null)
        {
            throw new NotFoundException("parent group not found.");
        }

        return await _groupRepository.CreateAsync(name, parentGroupId);
    }

    public async Task<Group> UpdateGroupAsync(int id, string name)
    {
        id.Throw()
            .IfNegativeOrZero();

        name.Throw()
            .IfEmpty()
            .IfWhiteSpace();

        var group = await _groupRepository.UpdateAsync(id, name);
        if (group is null)
        {
            throw new NotFoundException("group not found.");
        }

        return group;
    }

    public async Task<IEnumerable<Group>> GetGroupsAsync()
    {
        var institutions = await _institutionRepository.GetAsync();

        var groups = new List<Group>(institutions.Count());
        foreach (var institution in institutions)
        {
            var group = await _groupRepository.FindByInstitutionIdAsync(institution.Id);
            if (group is null)
            {
                throw new NotFoundException("institution group not found.");
            }

            group = await GetGroupWithSubGroupsAsync(group);

            groups.Add(group!);
        }

        return groups;
    }

    public async Task<Group> FindGroupAsync(int id)
    {
        id.Throw()
            .IfNegativeOrZero();

        var group = await _groupRepository.FindAsync(id);
        if (group is null)
        {
            throw new NotFoundException("group not found.");
        }

        return group;
    }

    public async Task<Group?> DeleteGroupAsync(int id)
    {
        id.Throw()
            .IfNegativeOrZero();

        return await _groupRepository.DeleteAsync(id);
    }

    public async Task<Group> AddUserAsync(int groupId, Guid userId)
    {
        groupId.Throw()
            .IfNegativeOrZero();
        userId.Throw()
            .IfDefault();

        var group = await _groupRepository.FindAsync(groupId);
        if (group is null)
        {
            throw new NotFoundException("group not found.");
        }

        if (group.Users.Any(item => item.Id == userId))
        {
            return group;
        }

        var user = await _userRepository.FindAsync(userId);
        if (user is null)
        {
            throw new NotFoundException("user not found.");
        }

        group = await _groupRepository.AddUserAsync(groupId, userId);
        if (group is null)
        {
            throw new NotFoundException("group not found.");
        }

        return group;
    }

    public async Task<Group> RemoveUserAsync(int groupId, Guid userId)
    {
        groupId.Throw()
           .IfNegativeOrZero();
        userId.Throw()
            .IfDefault();

        var group = await _groupRepository.FindAsync(groupId);
        if (group is null)
        {
            throw new NotFoundException("group not found.");
        }

        if (!group.Users.Any(item => item.Id == userId))
        {
            return group;
        }

        group = await _groupRepository.RemoveUserAsync(groupId, userId);
        if (group is null)
        {
            throw new NotFoundException("group not found.");
        }

        return group;
    }

    private async Task<Group> GetGroupWithSubGroupsAsync(Group group)
    {
        var subGroups = await _groupRepository.GetSubGroupsAsync(group.Id);

        foreach (var subGroup in subGroups)
        {
            var subGroupWithGroups = await GetGroupWithSubGroupsAsync(subGroup);

            group.SubGroup.Add(subGroupWithGroups);
        }

        return group;
    }
}
