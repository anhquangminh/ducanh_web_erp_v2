namespace DucAnh2025.Models.CongTrinh
{
    public class PKKL_OngNhua_3TTinLDatVanTruCHoa :Chung
    {
        public string Id_ChiNhanh { get; set; } = "";
        public string Id_TuyenDuong { get; set; } = "";
        public string Id_TuLyTrinh { get; set; } = "";
        public string Id_HangMucCongViec { get; set; } = "";
        public string Id_LoaiCauKien { get; set; } = "";
        public string Id_HangMucKhoiLuong { get; set; } = "";
        public string Id_LoaiKhoiLuong { get; set; } = "";
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public string Id_TrangThaiThiCong { get; set; } = "";
    }

    public class PKKL_OngNhua_3TTinLDatVanTruCHoa_Log : Chung_Log
    {
        public string Id_ChiNhanh { get; set; } = "";
        public string Id_TuyenDuong { get; set; } = "";
        public string Id_TuLyTrinh { get; set; } = "";
        public string Id_HangMucCongViec { get; set; } = "";
        public string Id_LoaiCauKien { get; set; } = "";
        public string Id_HangMucKhoiLuong { get; set; } = "";
        public string Id_LoaiKhoiLuong { get; set; } = "";
        public double X { get; set; } =0;
        public double Y { get; set; } =0;
        public string Id_TrangThaiThiCong { get; set; } = "";
    }
}
