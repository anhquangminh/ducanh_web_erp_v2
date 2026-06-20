using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.CongTrinh
{
    public class CT_DM_ThongTinVatTu: Chung
    {
        [Required(ErrorMessage = "Bạn phải nhập Loại trụ cứu hỏa")]
        public string LoaiTruCuuHoa { get; set; } = "";
        [Required(ErrorMessage = "Bạn phải nhập Đơn vị")]
        public string DonVi { get; set; } = "";
        [Required(ErrorMessage = "Bạn phải nhập vật tư")]
        public string VatTu { get; set; } = "";
    }
    public class CT_DM_ThongTinVatTu_Log : Chung_Log
    {
        public string LoaiTruCuuHoa { get; set; } = "";
        public string DonVi { get; set; } = "";
        public string VatTu { get; set; } = "";
    }
}
