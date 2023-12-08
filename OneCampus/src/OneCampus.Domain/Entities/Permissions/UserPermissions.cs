namespace OneCampus.Domain.Entities.Permissions
{
    public sealed record UserPermissions
    {
        public IList<Permissions> Permissions { get; }

        public Guid UserID { get; }

        public int GroupID { get; }

        public UserPermissions(Guid userID,int groupID) : this(userID, groupID, new List<Permissions>(0)) 
        { }

        public UserPermissions(Guid userID,int groupID,IList<Permissions> permissions) 
        {
            UserID = userID.Throw().IfDefault().Value;
            GroupID = groupID.Throw().IfNegativeOrZero().Value;
            Permissions = permissions.ThrowIfNull().Value;
        }
    }
}
