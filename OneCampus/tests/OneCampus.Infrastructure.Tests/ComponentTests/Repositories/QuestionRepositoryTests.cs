namespace OneCampus.Infrastructure.Tests.ComponentTests.Repositories;

public class QuestionRepositoryTests
{
    private readonly Fixture _fixture = new();
    private Mock<IDbContextFactory<OneCampusDbContext>> _mockDbContextFactory;
    private SqliteConnection _connection;

    private QuestionRepository _questionRepository;

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

        _questionRepository = new QuestionRepository(_mockDbContextFactory.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Close();
        _connection.Dispose();
    }

    #region CreateAsync

    [Test]
    public async Task CreateAsync_CreateQuestion_ReturnsTheQuestion()
    {
        const string Content = "Test Question";

        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);

        var user = await UserHelper.AddUserAsync(_dbContextFactory);
        var Question = await _questionRepository.CreateAsync(Content, group.Id, user.Id);

        Question.Should().NotBeNull();
        Question!.Id.Should().BePositive();
        Question.Content.Should().Be(Content);
        Question.GroupId.Should().Be(group.Id);
    }

    [Test]
    public async Task CreateAsync_WithUserNotFound_ReturnsNull()
    {
        const string Content = "Test Question";

        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);

        var Question = await _questionRepository.CreateAsync(Content, group.Id, Guid.NewGuid());

        Question.Should().BeNull();
    }

    #endregion

    #region GetQuestionsByGroupAsync

    [Test]
    public async Task GetQuestionsByGroupAsync_GroupWithQuestions_ReturnsTheGroupQuestion()
    {
        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);
        var Question1 = await QuestionHelper.AddQuestionAsync(_dbContextFactory, group.Id, user.Id);
        var Question2 = await QuestionHelper.AddQuestionAsync(_dbContextFactory, group.Id, user.Id);

        var Questions = await _questionRepository.GetQuestionsByGroupAsync(group.Id);

        Questions.Should().NotBeNullOrEmpty()
            .And.HaveCount(2)
            .And.Contain(item => item.Content == Question1.Content)
            .And.Contain(item => item.Content == Question2.Content)
            .And.BeInAscendingOrder(item => item.CreateDate);
    }

    #endregion

    #region FindAsync

    [Test]
    public async Task FindAsync_WithQuestion_ReturnsTheQuestion()
    {
        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);
        var question = await QuestionHelper.AddQuestionAsync(_dbContextFactory, group.Id, user.Id);

        var result = await _questionRepository.FindAsync(question.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(question.Id);
        result.SenderName.Should().Be(result.SenderName);
        result.Content.Should().Be(result.Content);
    }

    [Test]
    public async Task FindAsync_WithQuestionNotFound_ReturnsNull()
    {
        var result = await _questionRepository.FindAsync(1);

        result.Should().BeNull();
    }

    #endregion

    #region IsUserInTheGroupAsync

    [Test]
    public async Task IsUserInTheGroupAsync_UserWithAccess_ReturnsTrue()
    {
        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);
        var question = await QuestionHelper.AddQuestionAsync(_dbContextFactory, group.Id, user.Id);

        await GroupHelper.AddUsersToGroupAsync(_dbContextFactory, group.Id, user.Id);

        var result = await _questionRepository.HasAccessAsync(user.Id, question.Id);

        result.Should().BeTrue();
    }

    [Test]
    public async Task IsUserInTheGroupAsync_UserWithoutAccess_ReturnsFalse()
    {
        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);

        var result = await _questionRepository.HasAccessAsync(user.Id, group.Id);

        result.Should().BeFalse();
    }

    #endregion
}
