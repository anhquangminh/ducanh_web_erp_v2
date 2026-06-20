
namespace DucAnh2025.ViewModels.CongTrinh
{
    public class DM_TenCongTacModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Id_HangMucCongViec { get; set; } = "";
        public string HangMucCongViec { get; set; } = "";
        public string Id_LoaiCauKien { get; set; } = "";
        public string LoaiCauKien { get; set; } = "";
        public string Id_HangMucKhoiLuong { get; set; } = "";
        public string HangMucKhoiLuong { get; set; } = "";
        public string Id_LoaiKhoiLuong { get; set; } = "";
        public string LoaiKhoiLuong { get; set; } = "";
        public string TenCongTac { get; set; } = "";
        public string DonVi { get; set; } = "";
    

        public string GroupId { get; set; } = "";
        public int Ordinarily { get; set; }
        public DateTime? CreateAt { get; set; }
        public string CreateBy { get; set; } = "";
        public int IsActive { get; set; }
        public string ApprovalUserId { get; set; } = "";
        public DateTime? DateApproval { get; set; }
        public string ApprovalDept { get; set; } = "";
        public string DepartmentId { get; set; } = "";
        public int DepartmentOrder { get; set; }
        public int ApprovalOrder { get; set; }
        public string ApprovalId { get; set; } = "";
        public string LastApprovalId { get; set; } = "";
        public string IsStatus { get; set; } = "";
    }
}
