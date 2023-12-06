using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using OneCampus.Api.Controllers;
using OneCampus.Api.Models;
using OneCampus.Api.Models.Requests;
using OneCampus.Api.Models.Responses;
using OneCampus.Domain.Entities.Forums;
using OneCampus.Domain.Services;

namespace OneCampus.Tests.ComponentTests.Controllers;

[TestFixture]
public class QuestionControllerTests
{
    private readonly Fixture _fixture = new();

    private Mock<IQuestionService> _mockIQuestionService;

    private QuestionController _controller;

    private UserInfo _user;

    private readonly CreateQuestionRequest request = new CreateQuestionRequest
    {
        Content = "Test Question",
        GroupId = 1,
    };

    private readonly Question expectedQuestion = new Question(1, 1, "Test Question", "Manel", DateTime.UtcNow);


    [SetUp]
    public void Setup()
    {
        _mockIQuestionService = new Mock<IQuestionService>(MockBehavior.Strict);

        _user = _fixture.Create<UserInfo>();

        _controller = new QuestionController(_mockIQuestionService.Object, _user);
    }

    [Test]
    public async Task CreateQuestionAsyncTest()
    {

        _mockIQuestionService.Setup(s => s.CreateQuestionAsync(request.GroupId, request.Content, _user.Id))
            .ReturnsAsync(expectedQuestion);

        var result = await _controller.CreateQuestionAsync(request);

        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var response = result as OkObjectResult;

        response!.Value.Should().NotBeNull()
            .And.BeOfType<BaseResponse<CreateQuestionRequest, Question>>();
    }

    [Test]
    public async Task FindGroupQuestionsAsyncTest()
    {
        // Arrange
        int validId = 1;  // Assuming 1 is a valid ID for this test
        var expected = new List<Question>
        {
            new Question(1, 1, "Test Question 1", "Manel", DateTime.UtcNow.AddMinutes(-2)),
            new Question(2, 1, "Test Question 2", "Manel", DateTime.UtcNow.AddMinutes(-1)),
            new Question(3, 1, "Test Question 3", "Manel", DateTime.UtcNow)
        };

        _mockIQuestionService.Setup(s => s.FindQuestionsByGroupAsync(validId))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.FindGroupAsync(validId);

        // Assert
        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;

        okResult!.Value.Should().NotBeNull()
            .And.BeOfType<EnumerableResponse<QuestionsByGroupRequest, Question>>();

        var response = okResult!.Value as EnumerableResponse<QuestionsByGroupRequest, Question>;

        response.Should().NotBeNull();
        response!.Request.GroupId.Should().Be(validId);
        response.Data.Should().BeEquivalentTo(expected);
    }

}

