public class Role
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } // "Admin", "NhanSu", "KeToan", ...
    public string? Description { get; set; }


}