namespace OneCampus.Domain.Entities.Users;

public sealed record User
{
    public Guid Id { get; }
    public string Name { get; }
    public string Email { get; }

    public User(Guid id, string name, string email)
    {
        Id = id.Throw()
            .IfDefault()
            .Value;

        Name = name.Throw()
            .IfEmpty()
            .IfWhiteSpace()
            .Value;

        Email = email.Throw()
            .IfEmpty()
            .IfWhiteSpace()
            .Value;
    }
}
