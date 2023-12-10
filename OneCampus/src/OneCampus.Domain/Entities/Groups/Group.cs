namespace OneCampus.Domain.Entities.Groups;

public sealed record Group
{
    public int Id { get; }
    public string Name { get; }

    public IList<Group> SubGroup { get; } = new List<Group>();

    public Group(int id, string name)
    {
        Id = id.Throw()
            .IfLessThan(0)
            .Value;

        Name = name.Throw()
            .IfEmpty()
            .IfWhiteSpace()
            .Value;
    }
}
