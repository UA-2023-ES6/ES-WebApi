﻿using Microsoft.AspNetCore.Mvc;
using OneCampus.Api.Controllers;
using OneCampus.Api.Models.Requests;
using OneCampus.Api.Models.Responses;
using OneCampus.Domain.Entities.Forums;
using OneCampus.Domain.Services;

namespace OneCampus.Tests.ComponentTests.Controllers;

[TestFixture]
public class AnswerControllerTests
{
    private Mock<IAnswerService> _mockIAnswerService;

    private AnswerController _controller;

    private readonly CreateAnswerRequest request = new CreateAnswerRequest
    {
        Content = "Test Answer",
        QuestionId = 1,
        UserId = new Guid("b8e7f65a-f6ca-4211-a562-1fb022636e87")
    };

    private readonly Answer expectedAnswer = new Answer(1, 1, "Test Answer", new Guid("b8e7f65a-f6ca-4211-a562-1fb022636e87"), DateTime.UtcNow);


    [SetUp]
    public void Setup()
    {
        _mockIAnswerService = new Mock<IAnswerService>(MockBehavior.Strict);

        _controller = new AnswerController(_mockIAnswerService.Object);
    }

    [Test]
    public async Task CreateAnswerAsyncTest()
    {

        _mockIAnswerService.Setup(s => s.CreateAnswerAsync(request.QuestionId, request.Content, request.UserId))
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
        Guid UserId = new Guid("b8e7f65a-f6ca-4211-a562-1fb022636e87");
        var expected = new List<Answer>
        {
            new Answer(1, 1, "Test Answer 1", UserId, DateTime.UtcNow.AddMinutes(-2)),
            new Answer(2, 1, "Test Answer 2", UserId, DateTime.UtcNow.AddMinutes(-1)),
            new Answer(3, 1, "Test Answer 3", UserId, DateTime.UtcNow)
        };

        _mockIAnswerService.Setup(s => s.FindAnswersByQuestionAsync(validId))
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
