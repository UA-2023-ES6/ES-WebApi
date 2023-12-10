namespace OneCampus.Domain.Entities.Institutions;

public sealed record Institution
{
    public int Id { get; }
    public string Name { get; }

    public Institution(int id, string name)
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
