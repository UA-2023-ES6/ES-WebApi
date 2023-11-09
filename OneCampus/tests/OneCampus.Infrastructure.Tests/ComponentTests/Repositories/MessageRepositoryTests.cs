using System;
namespace OneCampus.Infrastructure.Tests.ComponentTests.Repositories;

public class MessageRepositoryTests
{
    private readonly Fixture _fixture = new();
    private Mock<IDbContextFactory<OneCampusDbContext>> _mockDbContextFactory;
    private SqliteConnection _connection;

    private MessageRepository _messageRepository;
    private GroupRepository _groupRepository;
    private UserRepository _userRepository;


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

        _messageRepository = new MessageRepository(_mockDbContextFactory.Object);
        _userRepository = new UserRepository(_mockDbContextFactory.Object);
        _groupRepository = new GroupRepository(_mockDbContextFactory.Object);

    }

    [TearDown]
    public void TearDown()
    {
        _connection.Close();
        _connection.Dispose();
    }


    /*
    [Test]
    public async Task CreateAsync_CreateMessage_ReturnsTheMessage()
    {
        const string Content = "Test message";
        const int GroupId = 2;
        const string Name1 = "Group1";
        const string Name2 = "Group2";

        Guid UserId = new Guid("b8e7f65a-f6ca-4211-a562-1fb022636e87");

        var group1 = await _groupRepository.CreateAsync(Name1, 1);
        var group2 = await _groupRepository.CreateAsync(Name2, 2);

        var user = await _groupRepository.AddUserAsync(GroupId, UserId);
        var message = await _messageRepository.CreateAsync(Content, GroupId, UserId);

        message.Should().NotBeNull();
        message!.Id.Should().BePositive();
        message.Content.Should().Be(Content);
        message.GroupId.Should().Be(GroupId);

    }
    */
}


