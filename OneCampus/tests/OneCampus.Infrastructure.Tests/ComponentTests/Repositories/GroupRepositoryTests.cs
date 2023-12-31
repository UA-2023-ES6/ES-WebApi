﻿namespace OneCampus.Infrastructure.Tests.ComponentTests.Repositories;

[TestFixture]
public class GroupRepositoryTests
{
    private readonly Fixture _fixture = new();
    private Mock<IDbContextFactory<OneCampusDbContext>> _mockDbContextFactory;
    private SqliteConnection _connection;

    private GroupRepository _groupRepository;

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

        _groupRepository = new GroupRepository(_mockDbContextFactory.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Close();
        _connection.Dispose();
    }

    #region CreateAsync

    [Test]
    public async Task CreateAsync_CreateGroup_ReturnsTheGroup()
    {
        var dbGroupWithInstitution = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);

        const string Name = "group name";

        var group = await _groupRepository.CreateAsync(Name, dbGroupWithInstitution.Id);

        group.Should().NotBeNull();
        group!.Id.Should().BePositive();
        group.Name.Should().Be(Name);
    }

    #endregion

    #region UpdateAsync

    [Test]
    public async Task UpdateAsync_CreateGroup_ReturnsTheGroup()
    {
        var dbGroupWithInstitution = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var dbGroup = await GroupHelper.AddGroupAsync(_dbContextFactory, dbGroupWithInstitution.Id);

        const string Name = "new group name";

        var group = await _groupRepository.UpdateAsync(dbGroup.Id, Name);

        group.Should().NotBeNull();
        group!.Id.Should().Be(dbGroup.Id);
        group.Name.Should().Be(Name);
    }

    #endregion

    #region FindAsync

    [Test]
    public async Task FindAsync_WithGroup_ReturnsTheGroup()
    {
        var dbGroup = _fixture.Build<Database.Group>()
            .Without(item => item.ParentId)
            .Without(item => item.Institution)
            .Without(item => item.DeleteDate)
            .Create();

        using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            var result = await dbContext.Groups.AddAsync(dbGroup);

            await dbContext.SaveChangesAsync();

            dbGroup = result.Entity;
        }

        var group = await _groupRepository.FindAsync(dbGroup.Id);

        group.Should().NotBeNull();
        group!.Id.Should().Be(dbGroup.Id);
        group.Name.Should().Be(dbGroup.Name);
    }

    [Test]
    public async Task FindAsync_WithDeletedGroup_ReturnsNull()
    {
        var dbGroup = await GroupHelper.AddGroupWithInstitutionAsync(
           _dbContextFactory,
            builder => builder.With(item => item.DeleteDate));

        var group = await _groupRepository.FindAsync(dbGroup.Id);

        group.Should().BeNull();
    }

    #endregion

    #region FindByInstitutionIdAsync

    [Test]
    public async Task FindByInstitutionIdAsync_WithInstitution_ReturnsTheInstitution()
    {
        var dbGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);

        var group = await _groupRepository.FindByInstitutionIdAsync(dbGroup.Institution!.Id);

        group.Should().NotBeNull();
        group!.Id.Should().Be(dbGroup.Id);
        group.Name.Should().Be(dbGroup.Name);
    }

    [Test]
    public async Task FindByInstitutionIdAsync_WithDeletedInstitution_ReturnsNull()
    {
        var dbGroup = await GroupHelper.AddGroupWithInstitutionAsync(
            _dbContextFactory,
            builder => builder.With(item => item.DeleteDate));

        var group = await _groupRepository.FindByInstitutionIdAsync(dbGroup.Institution!.Id);

        group.Should().BeNull();
    }

    #endregion

    #region GetSubGroupsAsync

    [Test]
    public async Task GetSubGroupsAsync_GetSubGroups_ReturnsSubGroups()
    {
        var user = await UserHelper.AddUserAsync(_dbContextFactory);

        var dbGroupWithInstitution = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var dbGroup = await GroupHelper.AddGroupAsync(_dbContextFactory, dbGroupWithInstitution.Id);
        var dbGroup2 = await GroupHelper.AddGroupAsync(
            _dbContextFactory,
            builder => builder
                .With(item => item.ParentId,  dbGroupWithInstitution.Id)
                .With(item => item.DeleteDate));
        var dbGroup3 = await GroupHelper.AddGroupAsync(_dbContextFactory, dbGroupWithInstitution.Id);

        await GroupHelper.AddUsersToGroupAsync(_dbContextFactory, dbGroupWithInstitution.Id, user.Id);
        await GroupHelper.AddUsersToGroupAsync(_dbContextFactory, dbGroup.Id, user.Id);
        await GroupHelper.AddUsersToGroupAsync(_dbContextFactory, dbGroup2.Id, user.Id);
        await GroupHelper.AddUsersToGroupAsync(_dbContextFactory, dbGroup3.Id, user.Id);

        var groups = await _groupRepository.GetSubGroupsAsync(user.Id, dbGroupWithInstitution.Id);

        groups.Should().NotBeNullOrEmpty()
            .And.Contain(item => item.Id == dbGroup.Id)
            .And.NotContain(item => item.Id == dbGroup2.Id)
            .And.Contain(item => item.Id == dbGroup3.Id);
    }

    [Test]
    public async Task GetSubGroupsAsync_GetSubGroupsWithUser_ReturnsSubGroupsThatUserHasAccess()
    {
        var user = await UserHelper.AddUserAsync(_dbContextFactory);

        var dbGroupWithInstitution = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var dbGroup = await GroupHelper.AddGroupAsync(_dbContextFactory, dbGroupWithInstitution.Id);
        var dbGroup2 = await GroupHelper.AddGroupAsync(_dbContextFactory, dbGroupWithInstitution.Id);
        var dbGroup3 = await GroupHelper.AddGroupAsync(_dbContextFactory, dbGroupWithInstitution.Id);

        await GroupHelper.AddUsersToGroupAsync(_dbContextFactory, dbGroupWithInstitution.Id, user.Id);
        await GroupHelper.AddUsersToGroupAsync(_dbContextFactory, dbGroup.Id, user.Id);
        await GroupHelper.AddUsersToGroupAsync(_dbContextFactory, dbGroup3.Id, user.Id);

        var groups = await _groupRepository.GetSubGroupsAsync(user.Id, dbGroupWithInstitution.Id);

        groups.Should().NotBeNullOrEmpty()
            .And.Contain(item => item.Id == dbGroup.Id)
            .And.NotContain(item => item.Id == dbGroup2.Id)
            .And.Contain(item => item.Id == dbGroup3.Id);
    }

    #endregion

    #region DeleteAsync

    [Test]
    public async Task DeleteAsync_CreateGroup_ReturnsTheGroup()
    {
        var dbGroupWithInstitution = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var dbGroup = await GroupHelper.AddGroupAsync(_dbContextFactory, dbGroupWithInstitution.Id);

        var group = await _groupRepository.DeleteAsync(dbGroup.Id);

        group.Should().NotBeNull();

        using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            var result = await dbContext.Groups.FindAsync(dbGroup.Id);
            result.Should().NotBeNull();
            result!.DeleteDate.Should().NotBeNull();
        }
    }

    #endregion

    #region AddUserAsync

    [Test]
    public async Task AddUserAsync_AddUser_ReturnsTheGroupWithTheNewUser()
    {
        var dbGroupWithInstitution = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var dbGroup = await GroupHelper.AddGroupAsync(_dbContextFactory, dbGroupWithInstitution.Id);
        var bdUser = await UserHelper.AddUserAsync(_dbContextFactory);

        var group = await _groupRepository.AddUserAsync(dbGroup.Id, bdUser.Id);

        group.Should().NotBeNull();
        group!.Id.Should().Be(dbGroup.Id);
        group.Users.Should().NotBeNullOrEmpty()
            .And.Contain(user => user.Id == bdUser.Id);
    }

    #endregion

    #region RemoveUserAsync

    [Test]
    public async Task RemoveUserAsync_RemovesUser_ReturnsTheGroupWithoutTheUser()
    {
        var dbGroupWithInstitution = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var dbGroup = await GroupHelper.AddGroupAsync(_dbContextFactory, dbGroupWithInstitution.Id);
        var bdUser = await UserHelper.AddUserAsync(_dbContextFactory);
        var bdUser2 = await UserHelper.AddUserAsync(_dbContextFactory);

        using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            var result = await dbContext.Groups.FindAsync(dbGroup.Id);
            result!.UserGroups.Add(new Data.Entities.UserGroup
            {
                User = bdUser
            });
            result!.UserGroups.Add(new Data.Entities.UserGroup
            {
                User = bdUser2
            });

            await dbContext.SaveChangesAsync();
        }

        var group = await _groupRepository.RemoveUserAsync(dbGroup.Id, bdUser.Id);

        group.Should().NotBeNull();
        group!.Id.Should().Be(dbGroup.Id);
        group.Users.Should().NotBeNullOrEmpty()
            .And.Contain(user => user.Id == bdUser2.Id)
            .And.NotContain(user => user.Id == bdUser.Id);
    }

    #endregion

    #region GetUsersAsync

    [Test]
    public async Task GetUsersAsync_UserWithAccess_ReturnsTrue()
    {
        var user1 = await UserHelper.AddUserAsync(_dbContextFactory);
        var user2 = await UserHelper.AddUserAsync(_dbContextFactory);
        var user3 = await UserHelper.AddUserAsync(_dbContextFactory);

        var dbGroup = _fixture.Build<Database.Group>()
            .Without(item => item.ParentId)
            .Without(item => item.Institution)
            .Without(item => item.DeleteDate)
            .Create();

        using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            var result = await dbContext.Groups.AddAsync(dbGroup);

            await dbContext.SaveChangesAsync();

            dbGroup = result.Entity;
        }

        await GroupHelper.AddUsersToGroupAsync(_dbContextFactory, dbGroup.Id, user1.Id, user2.Id, user3.Id);

        var (results, totalResults) = await _groupRepository.GetUsersAsync(dbGroup.Id, 10, 1);

        results.Should().NotBeNullOrEmpty()
            .And.HaveCount(2);
        totalResults.Should().Be(3);
    }

    #endregion

    #region IsUserInTheGroupAsync

    [Test]
    public async Task IsUserInTheGroupAsync_UserWithAccess_ReturnsTrue()
    {
        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);

        await GroupHelper.AddUsersToGroupAsync(_dbContextFactory, group.Id, user.Id);

        var result = await _groupRepository.HasAccessAsync(user.Id, group.Id);

        result.Should().BeTrue();
    }

    [Test]
    public async Task IsUserInTheGroupAsync_UserWithoutAccess_ReturnsFalse()
    {
        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);

        var result = await _groupRepository.HasAccessAsync(user.Id, group.Id);

        result.Should().BeFalse();
    }

    #endregion
}
