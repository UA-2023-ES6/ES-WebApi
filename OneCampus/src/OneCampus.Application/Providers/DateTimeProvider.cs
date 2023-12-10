using OneCampus.Domain.Providers;

namespace OneCampus.Application.Providers;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
