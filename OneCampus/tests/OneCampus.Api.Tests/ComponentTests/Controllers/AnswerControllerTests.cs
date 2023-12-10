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
public class AnswerControllerTests
{
    private readonly Fixture _fixture = new();

    private Mock<IAnswerService> _mockIAnswerService;

    private AnswerController _controller;

    private UserInfo _user;

    private readonly CreateAnswerRequest request = new CreateAnswerRequest
    {
        Content = "Test Answer",
        QuestionId = 1,
    };

    private readonly Answer expectedAnswer = new Answer(1, 1, "Test Answer", "Manel", DateTime.UtcNow);

    [SetUp]
    public void Setup()
    {
        _mockIAnswerService = new Mock<IAnswerService>(MockBehavior.Strict);

        _user = _fixture.Create<UserInfo>();

        _controller = new AnswerController(_mockIAnswerService.Object, _user);
    }

    [Test]
    public async Task CreateAnswerAsyncTest()
    {
        _mockIAnswerService.Setup(s => s.CreateAnswerAsync(_user.Id, request.QuestionId, request.Content))
            .ReturnsAsync(expectedAnswer);

        var result = await _controller.CreateAnswerAsync(request);

        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var response = result as OkObjectResult;

        response!.Value.Should().NotBeNull()
            .And.BeOfType<BaseResponse<CreateAnswerRequest, Answer>>();
    }

    [Test]
    public async Task FindQuestionAnswersAsyncTest()
    {
        // Arrange
        int validId = 1;  // Assuming 1 is a valid ID for this test
        var expected = new List<Answer>
        {
            new Answer(1, 1, "Test Answer 1", "Manel", DateTime.UtcNow.AddMinutes(-2)),
            new Answer(2, 1, "Test Answer 2", "Manel", DateTime.UtcNow.AddMinutes(-1)),
            new Answer(3, 1, "Test Answer 3", "Manel", DateTime.UtcNow)
        };

        _mockIAnswerService.Setup(s => s.FindAnswersByQuestionAsync(_user.Id, validId))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.FindAnswersAsync(validId);

        // Assert
        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;

        okResult!.Value.Should().NotBeNull()
            .And.BeOfType<EnumerableResponse<AnswersByQuestionRequest, Answer>>();

        var response = okResult!.Value as EnumerableResponse<AnswersByQuestionRequest, Answer>;

        response.Should().NotBeNull();
        response!.Request.QuestionId.Should().Be(validId);
        response.Data.Should().BeEquivalentTo(expected);
    }
}
