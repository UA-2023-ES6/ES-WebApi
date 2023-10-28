namespace OneCampus.Domain.Providers;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
