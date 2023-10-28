namespace OneCampus.Application.Providers;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
