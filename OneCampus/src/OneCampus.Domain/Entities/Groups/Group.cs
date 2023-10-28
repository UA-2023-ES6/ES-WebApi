namespace OneCampus.Domain.Entities.Groups;

public sealed record Group
{
    public int Id { get; }
    public string Name { get; }

    public IList<User> Users { get; }

    public IList<Group> SubGroup { get; } = new List<Group>();

    public Group(int id, string name)
        : this(id, name, new List<User>(0))
    {
    }

    public Group(int id, string name, List<User> users)
    {
        Id = id.Throw()
            .IfLessThan(0)
            .Value;

        Name = name.Throw()
            .IfEmpty()
            .IfWhiteSpace()
            .Value;

        Users = users.ThrowIfNull().Value;
    }
}
