using OneCampus.Application.Services;
using OneCampus.Domain.Entities.Forums;
using OneCampus.Domain.Exceptions;
using OneCampus.Domain.Repositories;

namespace OneCampus.Application.Tests.ComponentTests.Services;

[TestFixture]
public class AnswerServiceTests
{
    private readonly Fixture Fixture = new();

    private Mock<IAnswerRepository> _mockAnswerRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IQuestionRepository> _mockQuestionRepository;

    private AnswerService _service;

    [SetUp]
    public void Setup()
    {
        _mockAnswerRepository = new Mock<IAnswerRepository>(MockBehavior.Strict);
        _mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        _mockQuestionRepository = new Mock<IQuestionRepository>(MockBehavior.Strict);

        _service = new AnswerService(_mockAnswerRepository.Object, _mockUserRepository.Object, _mockQuestionRepository.Object);
    }

    #region CreateAnswerAsync

    [Test]
    public async Task CreateAnswerAsync_CreateAnswer_ReturnTheNewAnswer()
    {
        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Users.User>());

        _mockQuestionRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Forums.Question>());

        _mockAnswerRepository.Setup(item => item.CreateAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Answer>());

        var result = await _service.CreateAnswerAsync(1, "Test Answer", Guid.NewGuid());

        result.Should().NotBeNull();
    }

    [Test]
    public async Task CreateAnswerAsync_WithNullUser_ThrowsNotFoundException()
    {
        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Domain.Entities.Users.User?)null);

        _mockQuestionRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Forums.Question>());

        _mockAnswerRepository.Setup(item => item.CreateAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Answer>());

        await _service.Invoking(s => s.CreateAnswerAsync(1, "Test Answer", Guid.NewGuid()))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task CreateAnswerAsync_WithNullQuestion_ThrowsNotFoundException()
    {
        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Users.User>());

        _mockQuestionRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync((Domain.Entities.Forums.Question?)null);

        _mockAnswerRepository.Setup(item => item.CreateAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Answer>());

        await _service.Invoking(s => s.CreateAnswerAsync(1, "Test Answer", Guid.NewGuid()))
            .Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region FindAnswersByQuestionAsync

    [Test]
    public async Task FindAnswersByQuestionAsync_FindAnswers_ReturnAnswers()
    {
        var Answers = Fixture
            .CreateMany<Answer>(3)
            .ToList();

        _mockQuestionRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Forums.Question>());

        _mockAnswerRepository.Setup(item => item.GetAnswersByQuestionAsync(It.IsAny<int>()))
            .ReturnsAsync(Answers);

        var result = await _service.FindAnswersByQuestionAsync(1);

        result.Should().NotBeNullOrEmpty()
            .And.BeEquivalentTo(Answers);
    }

    [Test]
    public async Task FindAnswersByQuestionAsync_WithNullQuestion_ThrowsNotFoundException()
    {
        _mockQuestionRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync((Domain.Entities.Forums.Question?)null);

        await _service.Invoking(s => s.FindAnswersByQuestionAsync(1))
            .Should().ThrowAsync<NotFoundException>();
    }

    #endregion


}


