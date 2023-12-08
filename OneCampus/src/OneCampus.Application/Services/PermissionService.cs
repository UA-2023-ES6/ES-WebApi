using OneCampus.Domain.Entities.Permissions;
using OneCampus.Domain.Exceptions;
using OneCampus.Domain.Repositories;
using OneCampus.Domain.Services;

namespace OneCampus.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;

        public PermissionService(IPermissionRepository permissionRepository,IUserRepository userRepository,IGroupRepository groupRepository) 
        {
            _permissionRepository = permissionRepository.ThrowIfNull().Value;
            _userRepository = userRepository.ThrowIfNull().Value;
            _groupRepository = groupRepository.ThrowIfNull().Value;
        }

        public async Task<UserPermissions> AllowPermissionAsync(Guid userID, int groupID, Permissions permission)
        {
            userID.Throw().IfDefault();
            var user = await _userRepository.FindAsync(userID);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            groupID.Throw().IfNegativeOrZero();
            var group = await _groupRepository.FindAsync(groupID);
            if(group == null)
            {
                throw new NotFoundException("Group not found");
            }

            return await _permissionRepository.AllowPermissionAsync(userID, groupID, permission);
        }

        public async Task<UserPermissions> DeleteAsync(Guid userID, int groupID)
        {
            userID.Throw().IfDefault();
            groupID.Throw().IfNegativeOrZero();
            var user = await _userRepository.FindAsync(userID);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            groupID.Throw().IfNegativeOrZero();
            var group = await _groupRepository.FindAsync(groupID);
            if (group == null)
            {
                throw new NotFoundException("Group not found");
            }

            return await _permissionRepository.DeleteAsync(userID, groupID);
        }

        public async Task<UserPermissions> DenyPermissionAsync(Guid userID, int groupID, Permissions permission)
        {
            userID.Throw().IfDefault();
            groupID.Throw().IfNegativeOrZero();
            var user = await _userRepository.FindAsync(userID);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            groupID.Throw().IfNegativeOrZero();
            var group = await _groupRepository.FindAsync(groupID);
            if (group == null)
            {
                throw new NotFoundException("Group not found");
            }

            return await _permissionRepository.DenyPermissionAsync(userID, groupID, permission);
        }

        public async Task<UserPermissions> GetPermissions(Guid userID, int groupID)
        {
            userID.Throw().IfDefault();
            groupID.Throw().IfNegativeOrZero();
            var user = await _userRepository.FindAsync(userID);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            groupID.Throw().IfNegativeOrZero();
            var group = await _groupRepository.FindAsync(groupID);
            if (group == null)
            {
                throw new NotFoundException("Group not found");
            }

            var permissions = await _permissionRepository.GetPermissions(userID, groupID);
            if(permissions == null) 
            {
                throw new NotFoundException("No permissions found");
            }

            return permissions;
        }
    }
}
