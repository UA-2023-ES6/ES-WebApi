using OneCampus.Domain;

namespace OneCampus.Infrastructure.Tests.ComponentTests.Repositories;

[TestFixture]
public class PermissionsRepositoryTests
{
    private readonly Fixture _fixture = new();
    private Mock<IDbContextFactory<OneCampusDbContext>> _mockDbContextFactory;
    private SqliteConnection _connection;

    private PermissionsRepository _permissionsRepository;

    private IDbContextFactory<OneCampusDbContext> _dbContextFactory => _mockDbContextFactory.Object;

    [SetUp]
    public void SetUp()
    {
        _fixture.Customizations.Add(new IgnoreVirtualMembers());

        _mockDbContextFactory = new Mock<IDbContextFactory<OneCampusDbContext>>();

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<OneCampusDbContext>()
            .UseSqlite(_connection)
            .Options;
        using (var dbContext = new OneCampusDbContext(options))
        {
            dbContext.Database.EnsureCreated();
        }

        _mockDbContextFactory.Setup(item => item.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new OneCampusDbContext(options));

        _permissionsRepository = new PermissionsRepository(_mockDbContextFactory.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Close();
        _connection.Dispose();
    }

    #region AllowPermissionsAsync

    [Test]
    public async Task AllowPermissionsAsync_WithNewPermissions_ReturnsNewPermissions()
    {
        var permissionsTypes = new PermissionType[]
        {
            PermissionType.CreateAnswer,
            PermissionType.CreateQuestion
        };

        await PermissionHelper.AddPermissions(_dbContextFactory);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        await GroupHelper.AddUsersToGroupAsync(_dbContextFactory, group.Id, user.Id);

        var result = await _permissionsRepository.AllowPermissionsAsync(user.Id, group.Id,permissionsTypes);

        result.Should().NotBeNull();
        result!.Permissions.Should().NotBeNullOrEmpty()
            .And.BeEquivalentTo(permissionsTypes);
    }

    #endregion

    #region DenyPermissionsAsync

    [Test]
    public async Task DenyPermissionsAsync_WithPermissionsToRemove_ReturnsWithoutPermissionsToRemove()
    {
        var startPermissionsTypes = new PermissionType[]
        {
            PermissionType.CreateAnswer,
            PermissionType.CreateQuestion,
            PermissionType.CreateSubGroup,
            PermissionType.ManageUsersPermission
        };

        var permissionsTypesToRemove = new PermissionType[]
        {
            PermissionType.CreateSubGroup,
            PermissionType.ManageUsersPermission
        };

        var expretedPermissionsTypes = new PermissionType[]
        {
            PermissionType.CreateAnswer,
            PermissionType.CreateQuestion
        };

        await PermissionHelper.AddPermissions(_dbContextFactory);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        await GroupHelper.AddUsersToGroupAsync(_dbContextFactory, group.Id, user.Id);
        await PermissionHelper.AddPermissionsAsync(_dbContextFactory, group.Id, user.Id, startPermissionsTypes);

        var result = await _permissionsRepository.DenyPermissionsAsync(user.Id, group.Id, permissionsTypesToRemove);

        result.Should().NotBeNull();
        result!.Permissions.Should().NotBeNullOrEmpty()
            .And.BeEquivalentTo(expretedPermissionsTypes);
    }

    #endregion

    #region GetPermissionsAsync

    [Test]
    public async Task GetPermissionsAsync_WithPermission_ReturnsPermissions()
    {
        var permissionsTypes = new PermissionType[]
        {
            PermissionType.CreateAnswer,
            PermissionType.CreateQuestion
        };

        await PermissionHelper.AddPermissions(_dbContextFactory);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        await GroupHelper.AddUsersToGroupAsync(_dbContextFactory, group.Id, user.Id);
        await PermissionHelper.AddPermissionsAsync(_dbContextFactory, group.Id, user.Id, permissionsTypes);

        var result = await _permissionsRepository.GetPermissionsAsync(user.Id, group.Id);

        result.Should().NotBeNull();
        result!.Permissions.Should().NotBeNullOrEmpty()
            .And.BeEquivalentTo(permissionsTypes);
    }

    #endregion

    #region UserHasPermissionAsync

    [Test]
    public async Task UserHasPermissionAsync_WithPermission_ReturnsTrue()
    {
        var permissionType = PermissionType.CreateAnswer;
        await PermissionHelper.AddPermissions(_dbContextFactory);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        await GroupHelper.AddUsersToGroupAsync(_dbContextFactory, group.Id, user.Id);
        await PermissionHelper.AddPermissionsAsync(_dbContextFactory, group.Id, user.Id, permissionType);

        var result = await _permissionsRepository.UserHasPermissionAsync(user.Id, group.Id, permissionType);

        result.Should().BeTrue();
    }

    [Test]
    public async Task UserHasPermissionAsync_WithoutPermission_ReturnsFalse()
    {
        var permissionType = PermissionType.CreateAnswer;
        await PermissionHelper.AddPermissions(_dbContextFactory);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        await GroupHelper.AddUsersToGroupAsync(_dbContextFactory, group.Id, user.Id);


        var result = await _permissionsRepository.UserHasPermissionAsync(user.Id, group.Id, permissionType);

        result.Should().BeFalse();
    }

    #endregion

}