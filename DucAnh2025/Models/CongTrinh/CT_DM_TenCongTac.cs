using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.CongTrinh
{
    public class CT_DM_TenCongTac : Chung
    {
        [Required(ErrorMessage = "Bạn phải chọn hạng mục công việc")]
        public string Id_HangMucCongViec { get; set; } = "";
        public string Id_LoaiCauKien { get; set; } = "";
        [Required(ErrorMessage = "Bạn phải chọn hạng mục khối lượng")]
        public string Id_HangMucKhoiLuong { get; set; } = "";
        [Required(ErrorMessage = "Bạn phải chọn loại khối lượng")]
        public string Id_LoaiKhoiLuong { get; set; } = "";
        [Required(ErrorMessage = "Bạn phải nhập Tên công tác thực tế")]
        public string TenCongTac { get; set; } = "";
        [Required(ErrorMessage = "Bạn phải nhập đơn vị")]
        public string DonVi { get; set; } = "";
    }
    public class CT_DM_TenCongTac_Log : Chung_Log
    {
        public string Id_HangMucCongViec { get; set; } = "";
        public string Id_LoaiCauKien { get; set; } = "";
        public string Id_HangMucKhoiLuong { get; set; } = "";
        public string Id_LoaiKhoiLuong { get; set; } = "";
        public string TenCongTac { get; set; } = "";
        public string DonVi { get; set; } = "";
    }
}
