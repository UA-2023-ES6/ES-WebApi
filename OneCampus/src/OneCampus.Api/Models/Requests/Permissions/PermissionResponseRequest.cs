namespace OneCampus.Api.Models.Requests.Permissions
{
    public class PermissionResponseRequest : PermissionRequest
    {
        public Guid UserId { get; set; }
        public int GroupId { get; set; }
    }
}
