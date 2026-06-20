public class RolePermission
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RoleId { get; set; }
    public string PermissionId { get; set; }
}