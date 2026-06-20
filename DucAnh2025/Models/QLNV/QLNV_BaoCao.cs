namespace DucAnh2025.Models.QLNV
{
    public class QLNV_BaoCao
    {
    }
    public class CongViecStatusReport
    {
        public int TongSo { get; set; }
        public int DangThucHien { get; set; }
        public int HoanThanh { get; set; }
        public int Cho { get; set; }
        public int ChuaLam { get; set; }
        public int QuaHan { get; set; }
    }

    public class CongViecByNhomReport
    {
        public string NhomCongViec { get; set; }
        public string TenNhom { get; set; }
        public string IconName { get; set; }
        public int SoLuong { get; set; }
    }
    public class TienDoTrungBinhReport
    {
        public double TienDoTrungBinh { get; set; }
    }
    
    public class SoLuongTheoUuTienReport
    {
        public string MucDoUuTien { get; set; }
        public int SoLuong { get; set; }
    }

    public class SoLuongTheoThoiGianReport
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public int SoLuong { get; set; }
    }
    //
    public class CongViecTheoUuTienReport
    {
        public string MucDoUuTien { get; set; }
        public int SoLuong { get; set; }
    }
    public class CongViecDanhGiaReport
    {
        public int DaDanhGia { get; set; }
        public int ChuaDanhGia { get; set; }
    }
    public class CongViecThoiHanReport
    {
        public int DungHan { get; set; }
        public int QuaHan { get; set; }
        public int SapQuaHan { get; set; }
    }
    public class TienDoTrungBinhNVTHReport
    {
        public double TienDoTrungBinh { get; set; }
    }
    public class DashboardNVTHReport
    {
        public CongViecStatusReport TrangThai { get; set; }
        public CongViecDanhGiaReport DanhGia { get; set; }
        public CongViecThoiHanReport ThoiHan { get; set; }
        public List<CongViecTheoUuTienReport> UuTien { get; set; }
        public double TienDoTrungBinh { get; set; }
    }

}
