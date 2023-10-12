using ES6WebApi.Providers.Interfaces;

namespace ES6WebApi.Providers;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
