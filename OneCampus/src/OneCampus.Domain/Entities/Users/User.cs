namespace OneCampus.Domain.Entities.Users;

public sealed record User
{
    public Guid Id { get; }
    public string Username { get; }
    public string Email { get; }

    public User(Guid id, string username, string email)
    {
        Id = id.Throw()
            .IfDefault()
            .Value;

        Username = username.Throw()
            .IfEmpty()
            .IfWhiteSpace()
            .Value;

        Email = email.Throw()
            .IfEmpty()
            .IfWhiteSpace()
            .Value;
    }
}
