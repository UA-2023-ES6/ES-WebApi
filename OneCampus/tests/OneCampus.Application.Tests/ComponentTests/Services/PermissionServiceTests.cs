using OneCampus.Application.Services;
using OneCampus.Domain;
using OneCampus.Domain.Entities;
using OneCampus.Domain.Entities.Groups;
using OneCampus.Domain.Entities.Permissions;
using OneCampus.Domain.Exceptions;
using OneCampus.Domain.Repositories;

namespace OneCampus.Application.Tests.ComponentTests.Services;

[TestFixture]
public class PermissionServiceTests
{
    private readonly Fixture _fixture = new();

    private Mock<IPermissionRepository> _mockPermissionRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IGroupRepository> _mockGroupRepository;

    private PermissionService _service;

    private UserInfo _userInfo;

    [SetUp]
    public void Setup()
    {
        _mockPermissionRepository = new Mock<IPermissionRepository>(MockBehavior.Strict);
        _mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        _mockGroupRepository = new Mock<IGroupRepository>(MockBehavior.Strict);
        _userInfo = _fixture.Create<UserInfo>();

        _service = new PermissionService(
            _mockPermissionRepository.Object,
            _mockUserRepository.Object,
            _mockGroupRepository.Object,
            _userInfo);

        _mockGroupRepository.Setup(item => item.HasAccessAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        _mockPermissionRepository.Setup(item => item.UserHasPermissionAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<PermissionType>()))
            .ReturnsAsync(true);
    }


    #region AllowPermissionsAsync

    [Test]
    public async Task AllowPermissionsAsync_FindGroup_ReturnsUserPermissions()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(_fixture.Create<GroupDetails>());

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_fixture.Create<Domain.Entities.Users.User>());

        _mockPermissionRepository.Setup(item => item.AllowPermissionsAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<IList<PermissionType>>()))
            .ReturnsAsync(_fixture.Create<UserPermissions>());

        var result = await _service.AllowPermissionsAsync(
            Guid.NewGuid(),
            1,
            new List<PermissionType>{PermissionType.CreateSubGroup});

        result.Should().NotBeNull();
    }

    [Test]
    public async Task AllowPermissionsAsync_WithUserNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(_fixture.Create<GroupDetails>());

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Domain.Entities.Users.User?)null);

        _mockPermissionRepository.Setup(item => item.AllowPermissionsAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<IList<PermissionType>>()))
            .ReturnsAsync(_fixture.Create<UserPermissions>());

        await _service.Invoking(s => s.AllowPermissionsAsync(
            Guid.NewGuid(),
            1,
            new List<PermissionType> { PermissionType.CreateSubGroup }))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task AllowPermissionsAsync_WithGroupNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync((GroupDetails?)null);

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_fixture.Create<Domain.Entities.Users.User>());

        _mockPermissionRepository.Setup(item => item.AllowPermissionsAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<IList<PermissionType>>()))
            .ReturnsAsync(_fixture.Create<UserPermissions>());

        await _service.Invoking(s => s.AllowPermissionsAsync(
            Guid.NewGuid(),
            1,
            new List<PermissionType> { PermissionType.CreateSubGroup }))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task AllowPermissionsAsync_WithPermissionsNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(_fixture.Create<GroupDetails>());

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_fixture.Create<Domain.Entities.Users.User>());

        _mockPermissionRepository.Setup(item => item.AllowPermissionsAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<IList<PermissionType>>()))
            .ReturnsAsync((UserPermissions?)null);

        await _service.Invoking(s => s.AllowPermissionsAsync(
            Guid.NewGuid(),
            1,
            new List<PermissionType> { PermissionType.CreateSubGroup }))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task AllowPermissionsAsync_WithoutAccessFound_ThrowsForbiddenException()
    {
        _mockGroupRepository.Setup(item => item.HasAccessAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        await _service.Invoking(s => s.AllowPermissionsAsync(
                Guid.NewGuid(),
                1,
                new List<PermissionType> { PermissionType.CreateSubGroup }))
            .Should().ThrowAsync<ForbiddenException>();
    }

    #endregion

    #region DenyPermissionsAsync

    [Test]
    public async Task DenyPermissionsAsync_FindGroup_ReturnsUserPermissions()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(_fixture.Create<GroupDetails>());

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_fixture.Create<Domain.Entities.Users.User>());

        _mockPermissionRepository.Setup(item => item.DenyPermissionsAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<IList<PermissionType>>()))
            .ReturnsAsync(_fixture.Create<UserPermissions>());

        var result = await _service.DenyPermissionsAsync(
            Guid.NewGuid(),
            1,
            new List<PermissionType>{PermissionType.CreateSubGroup});

        result.Should().NotBeNull();
    }

    [Test]
    public async Task DenyPermissionsAsync_WithUserNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(_fixture.Create<GroupDetails>());

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Domain.Entities.Users.User?)null);

        _mockPermissionRepository.Setup(item => item.DenyPermissionsAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<IList<PermissionType>>()))
            .ReturnsAsync(_fixture.Create<UserPermissions>());

        await _service.Invoking(s => s.DenyPermissionsAsync(
            Guid.NewGuid(),
            1,
            new List<PermissionType> { PermissionType.CreateSubGroup }))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task DenyPermissionsAsync_WithGroupNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync((GroupDetails?)null);

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_fixture.Create<Domain.Entities.Users.User>());

        _mockPermissionRepository.Setup(item => item.DenyPermissionsAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<IList<PermissionType>>()))
            .ReturnsAsync(_fixture.Create<UserPermissions>());

        await _service.Invoking(s => s.DenyPermissionsAsync(
            Guid.NewGuid(),
            1,
            new List<PermissionType> { PermissionType.CreateSubGroup }))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task DenyPermissionsAsync_WithPermissionsNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(_fixture.Create<GroupDetails>());

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_fixture.Create<Domain.Entities.Users.User>());

        _mockPermissionRepository.Setup(item => item.DenyPermissionsAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<IList<PermissionType>>()))
            .ReturnsAsync((UserPermissions?)null);

        await _service.Invoking(s => s.DenyPermissionsAsync(
            Guid.NewGuid(),
            1,
            new List<PermissionType> { PermissionType.CreateSubGroup }))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task DenyPermissionsAsync_WithoutAccessFound_ThrowsForbiddenException()
    {
        _mockGroupRepository.Setup(item => item.HasAccessAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        await _service.Invoking(s => s.DenyPermissionsAsync(
                Guid.NewGuid(),
                1,
                new List<PermissionType> { PermissionType.CreateSubGroup }))
            .Should().ThrowAsync<ForbiddenException>();
    }

    #endregion

    #region GetMyPermissionsAsync

    [Test]
    public async Task GetMyPermissionsAsync_FindGroup_ReturnsUserPermissions()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(_fixture.Create<GroupDetails>());

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_fixture.Create<Domain.Entities.Users.User>());

        _mockPermissionRepository.Setup(item => item.GetPermissionsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(_fixture.Create<UserPermissions>());

        var result = await _service.GetMyPermissionsAsync(1);

        result.Should().NotBeNull();
    }

    [Test]
    public async Task GetMyPermissionsAsync_WithGroupNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
                   .ReturnsAsync((GroupDetails?)null);

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_fixture.Create<Domain.Entities.Users.User>());

        _mockPermissionRepository.Setup(item => item.GetPermissionsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(_fixture.Create<UserPermissions>());

        await _service.Invoking(s => s.GetMyPermissionsAsync(1))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GetMyPermissionsAsync_WithPermissionsNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
                   .ReturnsAsync(_fixture.Create<GroupDetails>());

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_fixture.Create<Domain.Entities.Users.User>());

        _mockPermissionRepository.Setup(item => item.GetPermissionsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync((UserPermissions?)null);

        await _service.Invoking(s => s.GetMyPermissionsAsync(1))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GetMyPermissionsAsync_WithoutAccessFound_ThrowsForbiddenException()
    {
        _mockGroupRepository.Setup(item => item.HasAccessAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        await _service.Invoking(s => s.GetMyPermissionsAsync(1))
            .Should().ThrowAsync<ForbiddenException>();
    }

    #endregion

    #region GetPermissionsAsync

    [Test]
    public async Task GetPermissionsAsync_FindGroup_ReturnsUserPermissions()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(_fixture.Create<GroupDetails>());

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_fixture.Create<Domain.Entities.Users.User>());

        _mockPermissionRepository.Setup(item => item.GetPermissionsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(_fixture.Create<UserPermissions>());

        var result = await _service.GetPermissionsAsync(Guid.NewGuid(), 1);

        result.Should().NotBeNull();
    }

    [Test]
    public async Task GetPermissionsAsync_WithUserNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
           .ReturnsAsync(_fixture.Create<GroupDetails>());

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Domain.Entities.Users.User?)null);

        _mockPermissionRepository.Setup(item => item.GetPermissionsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(_fixture.Create<UserPermissions>());

        await _service.Invoking(s => s.GetPermissionsAsync(Guid.NewGuid(), 1))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GetPermissionsAsync_WithGroupNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
                   .ReturnsAsync((GroupDetails?)null);

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_fixture.Create<Domain.Entities.Users.User>());

        _mockPermissionRepository.Setup(item => item.GetPermissionsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(_fixture.Create<UserPermissions>());

        await _service.Invoking(s => s.GetPermissionsAsync(Guid.NewGuid(), 1))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GetPermissionsAsync_WithPermissionsNotFound_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
                   .ReturnsAsync(_fixture.Create<GroupDetails>());

        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(_fixture.Create<Domain.Entities.Users.User>());

        _mockPermissionRepository.Setup(item => item.GetPermissionsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync((UserPermissions?)null);

        await _service.Invoking(s => s.GetPermissionsAsync(Guid.NewGuid(), 1))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GetPermissionsAsync_WithoutAccessFound_ThrowsForbiddenException()
    {
        _mockGroupRepository.Setup(item => item.HasAccessAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        await _service.Invoking(s => s.GetPermissionsAsync(Guid.NewGuid(), 1))
            .Should().ThrowAsync<ForbiddenException>();
    }

    #endregion

    #region ValidatePermissionAsync

    [Test]
    public async Task ValidatePermissionAsync_WithPermissions_NotThrow()
    {
        _mockPermissionRepository.Setup(item => item.UserHasPermissionAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<PermissionType>()))
            .ReturnsAsync(true);

        await _service.Invoking(service => service.ValidatePermissionAsync(Guid.NewGuid(), 1, PermissionType.CreateSubGroup))
            .Should().NotThrowAsync();
    }

    [Test]
    public async Task ValidatePermissionAsync_WithoutPermissions_ThrowsForbiddenException()
    {
        _mockPermissionRepository.Setup(item => item.UserHasPermissionAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<PermissionType>()))
            .ReturnsAsync(false);

        await _service.Invoking(service => service.ValidatePermissionAsync(Guid.NewGuid(), 1, PermissionType.CreateSubGroup))
            .Should().ThrowAsync<ForbiddenException>();
    }

    #endregion
}