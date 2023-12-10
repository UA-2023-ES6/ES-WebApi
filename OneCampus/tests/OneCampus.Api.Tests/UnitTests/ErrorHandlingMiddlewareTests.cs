using Microsoft.AspNetCore.Http;
using OneCampus.Api.Middlewares;
using OneCampus.Domain.Exceptions;

namespace OneCampus.Api.Tests.UnitTests;

[TestFixture]
public class ErrorHandlingMiddlewareTests
{
    [Test]
    public async Task InvokeAsync_WithArgumentException_ReturnsStatus400BadRequest()
    {
        var errorHandlingMiddleware = new ErrorHandlingMiddleware(innerHttpContent => throw new ArgumentException());

        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await errorHandlingMiddleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Test]
    public async Task InvokeAsync_WithNotFoundException_ReturnsStatus404NotFound()
    {
        var errorHandlingMiddleware = new ErrorHandlingMiddleware(innerHttpContent => throw new NotFoundException());

        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await errorHandlingMiddleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Test]
    public async Task InvokeAsync_WithException_ReturnsStatus500InternalServerError()
    {
        var errorHandlingMiddleware = new ErrorHandlingMiddleware(innerHttpContent => throw new Exception());

        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await errorHandlingMiddleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}
