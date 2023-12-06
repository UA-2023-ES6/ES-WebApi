namespace OneCampus.Infrastructure.Tests.ComponentTests.Repositories;

[TestFixture]
public class UserRepositoryTests
{
    private readonly Fixture _fixture = new();
    private Mock<IDbContextFactory<OneCampusDbContext>> _mockDbContextFactory;
    private SqliteConnection _connection;

    private UserRepository _userRepository;

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

        _userRepository = new UserRepository(_mockDbContextFactory.Object);
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
        Guid id = Guid.NewGuid();
        const string UserName = "username";
        const string Email = "email";

        var user = await _userRepository.CreateAsync(id,UserName, Email);

        user.Should().NotBeNull();
        user!.Id.Should().Be(id);
        user.Username.Should().Be(UserName);
        user.Email.Should().Be(Email);
    }

    #endregion

    #region FindAsync

    [Test]
    public async Task FindAsync_WithUser_ReturnsTheUser()
    {
        var dbUser = await UserHelper.AddUserAsync(_dbContextFactory);

        var user = await _userRepository.FindAsync(dbUser.Id);

        user.Should().NotBeNull();
        user!.Id.Should().Be(dbUser.Id);
        user.Username.Should().Be(dbUser.Username);
    }

    [Test]
    public async Task FindAsync_WithDeletedUser_ReturnsNull()
    {
        var dbUser = await UserHelper.AddUserAsync(_dbContextFactory, builder => builder.With(item => item.DeleteDate));

        var user = await _userRepository.FindAsync(dbUser.Id);

        user.Should().BeNull();
    }

    #endregion

    #region FindByEmailAsync

    [Test]
    public async Task FindByEmailAsync_WithUser_ReturnsTheUser()
    {
        var dbUser = _fixture.Build<Database.User>()
            .Without(item => item.DeleteDate)
            .Create();

        using (var dbContext = await _dbContextFactory.CreateDbContextAsync())
        {
            var result = await dbContext.Users.AddAsync(dbUser);

            await dbContext.SaveChangesAsync();

            dbUser = result.Entity;
        }

        var user = await _userRepository.FindByEmailAsync(dbUser.Email);

        user.Should().NotBeNull();
        user!.Id.Should().Be(dbUser.Id);
        user.Username.Should().Be(dbUser.Username);
        user.Email.Should().Be(dbUser.Email);
    }

    [Test]
    public async Task FindByEmailAsync_WithDeletedUser_ReturnsNull()
    {
        var dbUser = await UserHelper.AddUserAsync(
           _dbContextFactory,
            builder => builder.With(item => item.DeleteDate));

        var user = await _userRepository.FindByEmailAsync(dbUser.Email);

        user.Should().BeNull();
    }

    #endregion
}
