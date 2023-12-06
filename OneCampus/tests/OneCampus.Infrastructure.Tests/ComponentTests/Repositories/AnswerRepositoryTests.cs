namespace OneCampus.Infrastructure.Tests.ComponentTests.Repositories;

public class AnswerRepositoryTests
{
    private readonly Fixture _fixture = new();
    private Mock<IDbContextFactory<OneCampusDbContext>> _mockDbContextFactory;
    private SqliteConnection _connection;

    private AnswerRepository _AnswerRepository;

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

        _AnswerRepository = new AnswerRepository(_mockDbContextFactory.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Close();
        _connection.Dispose();
    }

    #region CreateAsync

    [Test]
    public async Task CreateAsync_CreateAnswer_ReturnsTheAnswer()
    {
        const string Content = "Test Question";

        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);
        var question = await QuestionHelper.AddQuestionAsync(_dbContextFactory, institutionGroup.Id, user.Id);

        var Answer = await _AnswerRepository.CreateAsync(Content, question.Id, user.Id);

        Answer.Should().NotBeNull();
        Answer!.Id.Should().BePositive();
        Answer.Content.Should().Be(Content);
        Answer.QuestionId.Should().Be(question.Id);
        /*Question.UserId.Should().NotBeNullOrWhiteSpace()
            .And.Be(user.Name);*/
    }

    [Test]
    public async Task CreateAsync_WithUserNotFound_ReturnsNull()
    {
        const string Content = "Test Answer";

        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);

        var question = await QuestionHelper.AddQuestionAsync(_dbContextFactory, institutionGroup.Id, user.Id);

        var answer = await _AnswerRepository.CreateAsync(Content, question.Id, Guid.NewGuid());

        answer.Should().BeNull();
    }

    #endregion

    #region GetQuestionsByGroupAsync

    [Test]
    public async Task GetAnswersByQuestionAsync_QuestionWithAnswers_ReturnsTheQuestionAnswers()
    {
        var institutionGroup = await GroupHelper.AddGroupWithInstitutionAsync(_dbContextFactory);
        var group = await GroupHelper.AddGroupAsync(_dbContextFactory, institutionGroup.Id);
        var user = await UserHelper.AddUserAsync(_dbContextFactory);
        var question = await QuestionHelper.AddQuestionAsync(_dbContextFactory, institutionGroup.Id, user.Id);

        var Answer1 = await AnswerHelper.AddAnswerAsync(_dbContextFactory, question.Id, user.Id);
        var Answer2 = await AnswerHelper.AddAnswerAsync(_dbContextFactory, question.Id, user.Id);

        var Answers = await _AnswerRepository.GetAnswersByQuestionAsync(question.Id);

        Answers.Should().NotBeNullOrEmpty()
            .And.HaveCount(2)
            .And.Contain(item => item.Content == Answer1.Content)
            .And.Contain(item => item.Content == Answer2.Content)
            .And.BeInAscendingOrder(item => item.CreateDate);
    }

    #endregion
}
