namespace DucAnh2025.Models.HeThong
{
    public class AddRolePermissionByMajorDto
    {
        public string RoleId { get; set; } = string.Empty;
        public string MajorId { get; set; } = string.Empty;
        public List<string> PermissionIds { get; set; } = new();
        public string GroupId { get; set; } = string.Empty;
    }
    public class RolePermissionViewDto
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public string MajorId { get; set; }
        public string MajorName { get; set; }

        public string PermissionId { get; set; }
        public string PermissionName { get; set; }
        public int PermissionType { get; set; }
    }


}
