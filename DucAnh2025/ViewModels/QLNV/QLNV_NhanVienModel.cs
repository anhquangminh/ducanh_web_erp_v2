using DucAnh2025.SeedWork;
using DucAnh2025.Models.QLNV;
using System.ComponentModel.DataAnnotations;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Models.NhanSu;

namespace DucAnh2025.ViewModel.QLNV
{
    public class QLNV_NhanVienModel : PagingParameters
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
       
        public string TenNhanVien { get; set; } = "";
        public string TaiKhoan { get; set; } = "";

        public string CompanyId { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public string GroupId { get; set; } = "";

        public string DepartmentId { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string ChucVuId { get; set; } = "";
        public string ChucVu { get; set; } = "";
        public string ChuyenMonId { get; set; } = "";
        public string ChuyenMon { get; set; } = "";

        public int Ordinarily { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public string CreateBy { get; set; }
        public int IsActive { get; set; } = 1;

        public string ApprovalUserId { get; set; } = "";
        public DateTime? DateApproval { get; set; }
        public string ApprovalDept { get; set; } = "";
        //public string DepartmentId { get; set; } = "";
        public int DepartmentOrder { get; set; }
        public int ApprovalOrder { get; set; }
        public string ApprovalId { get; set; } = "";
        public string LastApprovalId { get; set; } = "";
        public string IsStatus { get; set; } = "";
    }

    public class NhanVienListViewModel
    {
        public List<QLNV_NhanVienModel> NhanViens { get; set; }
        public List<ChiNhanh> ChiNhanhs { get; set; }
        public QLNV_NhanVienModel Filter { get; set; }
        public MetaData MetaData { get; set; }
    }

    public class NhanVienEditViewModel
    {
        public QLNV_NhanVien NhanVien { get; set; }
        public List<ChiNhanh> ChiNhanhs { get; set; }
        public List<DepartmentModel> Departments { get; set; }
        public List<DM_ChucVu> ChucVus { get; set; }
        public List<DM_ChuyenMon> ChuyenMons { get; set; }
    }
}
