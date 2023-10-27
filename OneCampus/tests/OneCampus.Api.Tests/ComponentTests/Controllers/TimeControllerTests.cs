using Microsoft.AspNetCore.Mvc;
using OneCampus.Application.Providers;
using OneCampus.Controllers;
using OneCampus.Models.Responses;

namespace OneCampus.Tests.ComponentTests.Controllers;

[TestFixture]
public class TimeControllerTests
{
    private Mock<IDateTimeProvider> _mockDateTimeProvider;

    private TimeController _controller;

    [SetUp]
    public void Setup()
    {
        _mockDateTimeProvider = new Mock<IDateTimeProvider>(MockBehavior.Strict);

        _controller = new TimeController(_mockDateTimeProvider.Object);
    }

    [Test]
    public void GetServerTime_GetServerTime_ReturnsUtcNow()
    {
        var date = DateTime.UtcNow;

        _mockDateTimeProvider.SetupGet(item => item.UtcNow)
            .Returns(date);

        var result =  _controller.GetServerTime();
        result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();

        var response = result as OkObjectResult;

        response!.Value.Should().NotBeNull()
            .And.BeOfType<ServerTimeResponse>();

        var serverTimeResponse = response.Value as ServerTimeResponse;

        serverTimeResponse!.ServerTime.Should().Be(date);
    }
}
