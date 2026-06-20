using DucAnh2025.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.ViewModel.QLNV
{
    public class QLNV_CongViecModel : PagingParameters
    {
        public string Id { get; set; } = "";
        public string Id_NguoiGiaoViec { get; set; } = "";
        public string NguoiThucHien { get; set; } = "";
        public string NhomCongViec { get; set; } = "";
        public string TenNhom { get; set; } = "";
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string MucDoUuTien { get; set; } = "";
        public string TuDanhGia { get; set; } = "";
        public Double DuocDanhGia { get; set; } = 0;
        public Double TienDo { get; set; } = 0;
        public string LapLai { get; set; } = "";
        public string TenCongViec { get; set; } = "";
        public string NoiDungCongViec { get; set; } = "";
        public string FileDinhKem { get; set; } = "";

        public string CompanyId { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public string GroupId { get; set; } = "";
        public int Ordinarily { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public string CreateBy { get; set; }
        public int IsActive { get; set; } = 1;

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
