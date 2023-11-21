﻿using OneCampus.Application.Services;
using OneCampus.Domain.Entities.Forums;
using OneCampus.Domain.Exceptions;
using OneCampus.Domain.Repositories;

namespace OneCampus.Application.Tests.ComponentTests.Services;

[TestFixture]
public class QuestionServiceTests
{
    private readonly Fixture Fixture = new();

    private Mock<IQuestionRepository> _mockQuestionRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IGroupRepository> _mockGroupRepository;

    private QuestionService _service;

    [SetUp]
    public void Setup()
    {
        _mockQuestionRepository = new Mock<IQuestionRepository>(MockBehavior.Strict);
        _mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        _mockGroupRepository = new Mock<IGroupRepository>(MockBehavior.Strict);

        _service = new QuestionService(_mockQuestionRepository.Object, _mockUserRepository.Object, _mockGroupRepository.Object);
    }

    #region CreateQuestionAsync

    [Test]
    public async Task CreateQuestionAsync_CreateQuestion_ReturnTheNewQuestion()
    {
        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Users.User>());

        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Groups.Group>());

        _mockQuestionRepository.Setup(item => item.CreateAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Question>());

        var result = await _service.CreateQuestionAsync(1, "Test Question", Guid.NewGuid());

        result.Should().NotBeNull();
    }

    [Test]
    public async Task CreateQuestionAsync_WithNullUser_ThrowsNotFoundException()
    {
        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Domain.Entities.Users.User?)null);

        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Groups.Group>());

        _mockQuestionRepository.Setup(item => item.CreateAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Question>());

        await _service.Invoking(s => s.CreateQuestionAsync(1, "Test Question", Guid.NewGuid()))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task CreateQuestionAsync_WithNullGroup_ThrowsNotFoundException()
    {
        _mockUserRepository.Setup(item => item.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Users.User>());

        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync((Domain.Entities.Groups.Group?)null);

        _mockQuestionRepository.Setup(item => item.CreateAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync(Fixture.Create<Question>());

        await _service.Invoking(s => s.CreateQuestionAsync(1, "Test Question", Guid.NewGuid()))
            .Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region FindQuestionsByGroupAsync

    [Test]
    public async Task FindQuestionsByGroupAsync_FindQuestions_ReturnQuestions()
    {
        var Questions = Fixture
            .CreateMany<Question>(3)
            .ToList();

        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync(Fixture.Create<Domain.Entities.Groups.Group>());

        _mockQuestionRepository.Setup(item => item.GetQuestionsByGroupAsync(It.IsAny<int>()))
            .ReturnsAsync(Questions);

        var result = await _service.FindQuestionsByGroupAsync(1);

        result.Should().NotBeNullOrEmpty()
            .And.BeEquivalentTo(Questions);
    }

    [Test]
    public async Task FindQuestionsByGroupAsync_WithNullGroup_ThrowsNotFoundException()
    {
        _mockGroupRepository.Setup(item => item.FindAsync(It.IsAny<int>()))
            .ReturnsAsync((Domain.Entities.Groups.Group?)null);

        await _service.Invoking(s => s.FindQuestionsByGroupAsync(1))
            .Should().ThrowAsync<NotFoundException>();
    }

    #endregion


}


