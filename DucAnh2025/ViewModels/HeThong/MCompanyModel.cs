using DucAnh2025.SeedWork;

namespace DucAnh2025.ViewModels.HeThong
{
    public partial class MCompanyModel : PagingParameters
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? ParentId { get; set; }
        public string? CompanyName { get; set; }
        public int? CompanyType { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? GroupId { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? CreateBy { get; set; }
        public int? IsActive { get; set; }
        public string? UserId { get; set; }
        public DateTime? DateApproval { get; set; }
        public string? IdDepartment { get; set; }
        public int? ApprovalOrder { get; set; }
    }

}
