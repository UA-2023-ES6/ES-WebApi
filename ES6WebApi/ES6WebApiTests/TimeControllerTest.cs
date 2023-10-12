namespace ES6WebApiTests;

using ES6WebApi.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

[TestClass]
public class TimeControllerTest
{
    [TestMethod]
    public async Task TestMethod1()
    {
        // Arrange
        var controller = new TimeController();

        // Act
        // ActionResult result = controller.GetServDerTimeAsync().Result;

        var result = await controller.GetServDerTimeAsync();

        var response = result as OkObjectResult;

        Console.Write(response.Value);

        response.Value.Should().NotBeNull()
            .And.BeOfType<DateTime>();

    }
}
