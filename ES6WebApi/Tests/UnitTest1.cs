namespace Tests;

using ES6WebApi.Controllers.TimeController

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void GetTime()
    {

        TimeController

        controller.Request = new HttpRequestMessage();
        controller.Configuration = new HttpConfiguration();

        // Act
        var response = controller.Get(10);

        // Assert
        Assert.IsTrue(response.TryGetContentValue<Product>(out product));
        Assert.AreEqual(10, product.Id);
    }
}
