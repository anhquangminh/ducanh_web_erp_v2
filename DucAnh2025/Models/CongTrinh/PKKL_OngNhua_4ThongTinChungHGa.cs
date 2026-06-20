namespace DucAnh2025.Models.CongTrinh
{
    public class PKKL_OngNhua_4ThongTinChungHGa : Chung
    {
        public string Id_ChiNhanh { get; set; } = "";
        //Thông tin lý trình
        public string Id_TuyenDuong { get; set; } = "";
        public string Id_LyTrinh { get; set; } = "";
        public string Id_HangMucCongViec { get; set; } = "";
        public string TenHoVan { get; set; } = "";
        //Cao độ (m)
        public double HTrangTruocKhiDao { get; set; } = 0;
        public double DayDao { get; set; } = 0;
        public double ChieuSauDao { get; set; } = 0;
        public double CDayLMongHg { get; set; } = 0;
        public double DinhLotMong { get; set; } = 0;
        public double CDayMongHg { get; set; } = 0;
        public double DayHgDongChay { get; set; } = 0;
        public double CCaoTuong { get; set; } = 0;
        public double DinhTuong { get; set; } = 0;
        public double CDayMMoThotDuoi { get; set; } = 0;
        public double MMoThotDuoi { get; set; } = 0;
        public double CDayMMoThotTrenHoacTamDan { get; set; } = 0;
        public double DinhHg { get; set; } = 0;
        //Thông tin cao độ đắp (m)
        public string Id_HinhThucDapTra { get; set; } = "";
        public double CDoDayDapCat { get; set; } = 0;
        public double CDoDinhDapCat { get; set; } = 0;
        public double ChieuDayDapCat { get; set; } = 0;
        public double CDoDayDapDat { get; set; } = 0;
        public double CDoDinhDapDat { get; set; } = 0;
        public double ChieuDayDapDat { get; set; } = 0;
        //Thông tin hố đào biện pháp (m)
        public double TyLeMoMai { get; set; } = 0;
        public double SoMaiTrai { get; set; } = 0;
        public double SoMaiPhai { get; set; } = 0;
        public double CDaiDaoHoMong { get; set; } = 0;
        public double CRongDaoDayNho { get; set; } = 0;
        public double CRongDayLon { get; set; } = 0;
        //KL đào biện pháp
        public string HangMucKlDao { get; set; } = "";
        public string LoaiKlDao { get; set; } = "";
        public double DienTichDao { get; set; } = 0;
        public double KlDao { get; set; } = 0;
        public string Id_TrangThaiThiCongDao { get; set; } = "";
        //KL đắp trả
        public double CRongDayNhoDapCatDapTraDapCat { get; set; } = 0;
        public double CRongDayLonDapCatDapTraDapCat { get; set; } = 0;
        public double DienTichDapCatDapTraDapCat { get; set; } = 0;
        public double KlDapCatDapTraDapCat { get; set; } = 0;
        public string HangMucKlDapTraDapCat { get; set; } = "";
        public string LoaiKlDapTraDapCat { get; set; } = "";
        public double KlLotMongDapTraDapCat { get; set; } = 0;
        public double KlMongDapTraDapCat { get; set; } = 0;
        public double KlTuongBaoGomTuongVaMuMoDapTraDapCat { get; set; } = 0;
        public double KlCChoDapTraDapCat { get; set; } = 0;
        public double KlDapCatSauCChoDapTraDapCat { get; set; } = 0;
        public string Id_TrangThaiThiCongDapTraDapCat { get; set; } = "";

        public double CRongDayNhoDapDatDapTraDapDat { get; set; } = 0;
        public double CRongDayLonDapDatDapTraDapDat { get; set; } = 0;
        public double DienTichDapDatDapTraDapDat { get; set; } = 0;
        public double KlDapDatDapTraDapDat { get; set; } = 0;
        public string HangMucKlDapTraDapDat { get; set; } = "";
        public string LoaiKlDapTraDapDat { get; set; } = "";
        public double KlLotMongDapTraDapDat { get; set; } = 0;
        public double KlMongDapTraDapDat { get; set; } = 0;
        public double KlTuongBaoGomTuongVaMuMoDapTraDapDat { get; set; } = 0;
        public double KlCChoDapTraDapDat { get; set; } = 0;
        public double KlDapDatSauCChoDapTraDapDat { get; set; } = 0;
        public string HangMucKlDapTraDapDatThua { get; set; } = "";
        public string LoaiKlDapTraDapDatThua { get; set; } = "";
        public double KlDatThuaDapTraDapDat { get; set; } = 0;
        public string Id_TrangThaiThiCongDapTraDapDat { get; set; } = "";
        //Thông tin lót móng hố ga
        public double DaiKTHH { get; set; } = 0;
        public double RongKTHH { get; set; } = 0;

        public string Id_HangMucKhoiLuongXayDungLotMong { get; set; } = "";
        public string Id_LoaiKhoiLuongXayDungLotMong { get; set; } = "";
        public double DaiXayDungLotMong { get; set; } = 0;
        public double RongXayDungLotMong { get; set; } = 0;
        public double CaoXayDungLotMong { get; set; } = 0;
        public double DienTichXayDungLotMong { get; set; } = 0;
        public string GhiChuDienTichXayDungLotMong { get; set; } = "";
        public double KlXayDungLotMong { get; set; } = 0;
        public string Id_TrangThaiThiCongXayDungLotMong { get; set; } = "";

        public string Id_LoaiKhoiLuongVanKhuanLotMong { get; set; } = "";
        public double DaiVanKhuanLotMong { get; set; } = 0;
        public double SlCDaiVanKhuanLotMong { get; set; } = 0;
        public double CCaoCanhDaiVanKhuanLotMong { get; set; } = 0;
        public double DTichCDaiVanKhuanLotMong { get; set; } = 0;
        public double KlCDaiVanKhuanLotMong { get; set; } = 0;
        public string Id_TrangThaiThiCongVanKhuanLotMong { get; set; } = "";
        public double RongVanKhuanLotMong { get; set; } = 0;
        public double SlCRongVanKhuanLotMong { get; set; } = 0;
        public double CCaoCanhRongVanKhuanLotMong { get; set; } = 0;
        public double DTichCRongVanKhuanLotMong { get; set; } = 0;
        public double KlCRongVanKhuanLotMong { get; set; } = 0;
        public string Id_TrangThaiThiCongVanKhuanLotMong1 { get; set; } = "";
        public double TongKlVanKhuanLotMongVanKhuanLotMong { get; set; } = 0;
        //Thông tin móng hố ga
        public string Id_HangMucKhoiLuongXayDungMong { get; set; } = "";
        public string Id_LoaiKhoiLuongXayDungMong { get; set; } = "";
        public double DaiXayDungMong { get; set; } = 0;
        public double RongXayDungMong { get; set; } = 0;
        public double CaoXayDungMong { get; set; } = 0;
        public double DienTichXayDungMong { get; set; } = 0;
        public string GhiChuDienTichXayDungMong { get; set; } = "";
        public double KlXayDungMong { get; set; } = 0;
        public string Id_TrangThaiThiCongXayDungMong { get; set; } = "";

        public string Id_LoaiKhoiLuongVanKhuanMong { get; set; } = "";
        public double DaiVanKhuanMong { get; set; } = 0;
        public double SlCDaiVanKhuanMong { get; set; } = 0;
        public double CCaoCanhDaiVanKhuanMong { get; set; } = 0;
        public double DTichCDaiVanKhuanMong { get; set; } = 0;
        public double KlCDaiVanKhuanMong { get; set; } = 0;
        public string Id_TrangThaiThiCongVanKhuanMong { get; set; } = "";
        public double RongVanKhuanMong { get; set; } = 0;
        public double SlCRongVanKhuanMong { get; set; } = 0;
        public double CCaoCanhRongVanKhuanMong { get; set; } = 0;
        public double DTichCRongVanKhuanMong { get; set; } = 0;
        public double KlCRongVanKhuanMong { get; set; } = 0;
        public string Id_TrangThaiThiCongVanKhuanMong1 { get; set; } = "";
        public double TongKlVanKhuanMongVanKhuanMong { get; set; } = 0;
        //Thông tin tường hố ga
        //Khối lượng xây dựng tường
        //KL xây dựng cạnh dài
        public string Id_HangMucKhoiLuongXayTuong { get; set; } = "";
        public string Id_LoaiKhoiLuongXayTuong { get; set; } = "";
        public double DaiXayTuong { get; set; } = 0;
        public double SlCDaiXayTuong { get; set; } = 0;
        public double CCaoCanhDaiXayTuong { get; set; } = 0;
        public double CDayTuongCanhDaiXayTuong { get; set; } = 0;
        public double DienTichCanhDaiXayTuong { get; set; } = 0;
        public string GhiChuDienTichXayTuongDai { get; set; } = "";
        public double KlTruocCChoCDaiXayTuong { get; set; } = 0;
        public double KlChiemChoCanhDaiXayTuong { get; set; } = 0;
        public double KlXayDungSauChiemChoCanhDaiXayTuong { get; set; } = 0;
        public string Id_TrangThaiThiCongXayTuongDai { get; set; } = "";

        public double RongXayTuong { get; set; } = 0;
        public double SlCRongXayTuong { get; set; } = 0;
        public double CCaoCanhRongXayTuong { get; set; } = 0;
        public double CDayTuongCanhRongXayTuong { get; set; } = 0;
        public double DienTichCanhRongXayTuong { get; set; } = 0;
        public string GhiChuDienTichXayTuongRong { get; set; } = "";
        public double KlTruocCChoCRongXayTuong { get; set; } = 0;
        public double KlChiemChoCanhRongXayTuong { get; set; } = 0;
        public double KlXayDungSauChiemChoCanhRong { get; set; } = 0;
        public string Id_TrangThaiThiCongXayTuongRong { get; set; } = "";
        public double TongKlTruocChiemChoXayTuong { get; set; } = 0;
        public double KlChiemChoXayTuong { get; set; } = 0;
        public double KlSauChiemChoXayTuong { get; set; } = 0;

        public string Id_LoaiKhoiLuongChatNgoai { get; set; } = "";
        public double DaiChatNgoai { get; set; } = 0;
        public double SlCDaiChatNgoai { get; set; } = 0;
        public double CCaoCanhDaiChatNgoai { get; set; } = 0;
        public double DienTichCanhDaiChatNgoai { get; set; } = 0;
        public double KlTruocCChoCDaiChatNgoai { get; set; } = 0;
        public double KlChiemChoCanhDaiChatNgoai { get; set; } = 0;
        public double KlXayDungSauChiemChoCanhDaiChatNgoai { get; set; } = 0;
        public string Id_TrangThaiThiCongChatNgoaiDai { get; set; } = "";

        public double RongChatNgoai { get; set; } = 0;
        public double SlCanhRongChatNgoai { get; set; } = 0;
        public double CCaoCanhRongChatNgoai { get; set; } = 0;
        public double DTichCRongChatNgoai { get; set; } = 0;
        public double KlTruocCChoCRongChatNgoai { get; set; } = 0;
        public double KlChiemChoCanhRongChatNgoai { get; set; } = 0;
        public double KlXayDungSauChiemChoCanhRongChatNgoai { get; set; } = 0;
        public string Id_TrangThaiThiCongChatNgoaiRong { get; set; } = "";

        public double TongKlTruocChiemChoChatNgoai { get; set; } = 0;
        public double KlChiemChoChatNgoai { get; set; } = 0;
        public double KlSauChiemChoChatNgoai { get; set; } = 0;

        public string Id_LoaiKhoiLuongChatTrong { get; set; } = "";
        public double DaiChatTrong { get; set; } = 0;
        public double SlCDaiChatTrong { get; set; } = 0;
        public double CCaoCanhDaiChatTrong { get; set; } = 0;
        public double DienTichCanhDaiChatTrong { get; set; } = 0;
        public double KlTruocCChoCDaiChatTrong { get; set; } = 0;
        public double KlChiemChoCanhDaiChatTrong { get; set; } = 0;
        public double KlXayDungSauChiemChoCanhDaiChatTrong { get; set; } = 0;
        public string Id_TrangThaiThiCongChatTrongDai { get; set; } = "";

        public double RongChatTrong { get; set; } = 0;
        public double SlCanhRongChatTrong { get; set; } = 0;
        public double CCaoCanhRongChatTrong { get; set; } = 0;
        public double DTichCRongChatTrong { get; set; } = 0;
        public double KlTruocCChoCRongChatTrong { get; set; } = 0;
        public double KlChiemChoCanhRongChatTrong { get; set; } = 0;
        public double KlXayDungSauChiemChoCanhRongChatTrong { get; set; } = 0;
        public string Id_TrangThaiThiCongChatTrongRong { get; set; } = "";

        public double TongKlTruocChiemChoChatTrong { get; set; } = 0;
        public double KlChiemChoChatTrong { get; set; } = 0;
        public double KlSauChiemChoChatTrong { get; set; } = 0;
        //Thông tin mũ mố thớt dưới hố ga
        public double KlVKhuanChatCDaiTruocCCho { get; set; } = 0;
        public double KlCChoVKhuanChatCDai { get; set; } = 0;
        public double KlVKhuanChatCDaiSauCCho { get; set; } = 0;
        public double KlVKhuanChatCRongTruocCCho { get; set; } = 0;
        public double KlCChoVKhuanChatCRong { get; set; } = 0;
        public double KlVKhuanChatCRongSauCCho { get; set; } = 0;

        public string Id_HangMucKhoiLuongMuMoDuoi { get; set; } = "";
        public string Id_LoaiKhoiLuongMuMoDuoi { get; set; } = "";
        public double DaiMuMoDuoi { get; set; } = 0;
        public double SlCDaiMuMoDuoi { get; set; } = 0;
        public double CCaoCanhDaiMuMoDuoi { get; set; } = 0;
        public double CDayTuongCanhDaiMuMoDuoi { get; set; } = 0;
        public double DienTichCanhDaiMuMoDuoi { get; set; } = 0;
        public string GhiChuDienTichMuMoDuoiDai { get; set; } = "";
        public double KlCDaiMuMoDuoi { get; set; } = 0;
        public string Id_TrangThaiThiCongMuMoDuoiDai { get; set; } = "";

        public double RongMuMoDuoi { get; set; } = 0;
        public double SlCRongMuMoDuoi { get; set; } = 0;
        public double CCaoCanhRongMuMoDuoi { get; set; } = 0;
        public double CDayTuongCanhRongMuMoDuoi { get; set; } = 0;
        public double DienTichCanhRongMuMoDuoi { get; set; } = 0;
        public string GhiChuDienTichMuMoDuoiRong { get; set; } = "";
        public double KlCRongMuMoDuoi { get; set; } = 0;
        public string Id_TrangThaiThiCongMuMoDuoiRong { get; set; } = "";
        public double TongKlMuMoDuoi { get; set; } = 0;

        public string Id_LoaiKhoiLuongChatDuoiNgoai { get; set; } = "";
        public double DaiChatDuoiNgoai { get; set; } = 0;
        public double SlCDaiChatDuoiNgoai { get; set; } = 0;
        public double CCaoCanhDaiChatDuoiNgoai { get; set; } = 0;
        public double DienTichCanhDaiChatDuoiNgoai { get; set; } = 0;
        public double KlCDaiChatDuoiNgoai { get; set; } = 0;
        public string Id_TrangThaiThiCongChatDuoiNgoaiDai { get; set; } = "";

        public double RongChatDuoiNgoai { get; set; } = 0;
        public double SlCanhRongChatDuoiNgoai { get; set; } = 0;
        public double CCaoCanhRongChatDuoiNgoai { get; set; } = 0;
        public double DTichCRongChatDuoiNgoai { get; set; } = 0;
        public double KlCRongChatDuoiNgoai { get; set; } = 0;
        public string Id_TrangThaiThiCongChatDuoiNgoaiRong { get; set; } = "";
        public double TongKlChatDuoiNgoai { get; set; } = 0;

        public string Id_LoaiKhoiLuongChatDuoiTrong { get; set; } = "";
        public double DaiChatDuoiTrong { get; set; } = 0;
        public double SlCDaiChatDuoiTrong { get; set; } = 0;
        public double CCaoCanhDaiChatDuoiTrong { get; set; } = 0;
        public double DienTichCanhDaiChatDuoiTrong { get; set; } = 0;
        public double KlCDaiChatDuoiTrong { get; set; } = 0;
        public string Id_TrangThaiThiCongChatDuoiTrongDai { get; set; } = "";

        public double RongChatDuoiTrong { get; set; } = 0;
        public double SlCanhRongChatDuoiTrong { get; set; } = 0;
        public double CCaoCanhRongChatDuoiTrong { get; set; } = 0;
        public double DTichCRongChatDuoiTrong { get; set; } = 0;
        public double KlCRongChatDuoiTrong { get; set; } = 0;
        public string Id_TrangThaiThiCongChatDuoiTrongRong { get; set; } = "";
        public double TongKlChatDuoiTrong { get; set; } = 0;

        public double CanhDaiMuMoDuoiDoGa { get; set; } = 0;
        public double CanhRongMuMoDuoiDoGa { get; set; } = 0;
        //Thông tin mũ mố thớt trên hố ga
        public string Id_HangMucKhoiLuongXayTren { get; set; } = "";
        public string Id_LoaiKhoiLuongXayTren { get; set; } = "";
        public double DaiXayTren { get; set; } = 0;
        public double SlCDaiXayTren { get; set; } = 0;
        public double CCaoCanhDaiXayTren { get; set; } = 0;
        public double CDayTuongCanhDaiXayTren { get; set; } = 0;
        public double DienTichCanhDaiXayTren { get; set; } = 0;
        public string GhiChuDienTichXayTrenDai { get; set; } = "";
        public double KlCDaiXayTren { get; set; } = 0;
        public string Id_TrangThaiThiCongXayTrenDai { get; set; } = "";

        public double RongXayTren { get; set; } = 0;
        public double SlCRongXayTren { get; set; } = 0;
        public double CCaoCanhRongXayTren { get; set; } = 0;
        public double CDayTuongCanhRongXayTren { get; set; } = 0;
        public double DienTichCanhRongXayTren { get; set; } = 0;
        public string GhiChuDienTichXayTrenRong { get; set; } = "";
        public double KlCRongXayTren { get; set; } = 0;
        public string Id_TrangThaiThiCongXayTrenRong { get; set; } = "";
        public double TongKlXayTren { get; set; } = 0;

        public string Id_LoaiKhoiLuongChatTren { get; set; } = "";
        public double DaiChatTren { get; set; } = 0;
        public double SlCDaiChatTren { get; set; } = 0;
        public double CCaoCanhDaiChatTren { get; set; } = 0;
        public double DienTichCanhDaiChatTren { get; set; } = 0;
        public double KlCDaiChatTren { get; set; } = 0;
        public string Id_TrangThaiThiCongChatTrenDai { get; set; } = "";

        public double RongChatTren { get; set; } = 0;
        public double SlCanhRongChatTren { get; set; } = 0;
        public double CCaoCanhRongChatTren { get; set; } = 0;
        public double DTichCRongChatTren { get; set; } = 0;
        public double KlCRongChatTren { get; set; } = 0;
        public string Id_TrangThaiThiCongChatTrenRong { get; set; } = "";
        public double TongKlChatTren { get; set; } = 0;

        public string Id_LoaiKhoiLuongChatTrenTrong { get; set; } = "";
        public double DaiChatTrenTrong { get; set; } = 0;
        public double SlCDaiChatTrenTrong { get; set; } = 0;
        public double CCaoCanhDaiChatTrenTrong { get; set; } = 0;
        public double DienTichCanhDaiChatTrenTrong { get; set; } = 0;
        public double KlCDaiChatTrenTrong { get; set; } = 0;
        public string Id_TrangThaiThiCongChatTrenTrongDai { get; set; } = "";

        public double RongChatTrenTrong { get; set; } = 0;
        public double SlCanhRongChatTrenTrong { get; set; } = 0;
        public double CCaoCanhRongChatTrenTrong { get; set; } = 0;
        public double DTichCRongChatTrenTrong { get; set; } = 0;
        public double KlCRongChatTrenTrong { get; set; } = 0;
        public string Id_TrangThaiThiCongChatTrenTrongRong { get; set; } = "";
        public double TongKlChatTrenTrong { get; set; } = 0;

        public double CanhDaiMuMoTrenHoGa { get; set; } = 0;
        public double CanhRongMuMoTrenHoGa { get; set; } = 0;
        //KL chiếm chỗ
        public double KlTuongCanhDai { get; set; } = 0;
        public double KlVanKhuanChatMatNgoaiCanhDai { get; set; } = 0;
        public double KlVanKhuanChatMatTrongCanhDai { get; set; } = 0;
        public double KlTuongCanhRong { get; set; } = 0;
        public double KlVanKhuanChatMatNgoaiCanhRong { get; set; } = 0;
        public double KlVanKhuanChatMatTrongCanhRong { get; set; } = 0;
        //Đấu nối cạnh dài
        public string Id_LoaiDauNoiXayTuongHoGa { get; set; } = "";
        public string HinhDangChiemChoXayTuongHoGa { get; set; } = "";
        public double SoLuongXayTuongHoGa { get; set; } = 0;
        public double DaiXayTuongHoGa { get; set; } = 0;
        public double RongXayTuongHoGa { get; set; } = 0;
        public double CaoXayTuongHoGa { get; set; } = 0;
        public double DienXayTuongHoGa { get; set; } = 0;
        public string GhiChuDienTichXayTuongHoGa { get; set; } = "";
        public double KlTuongXayTuongHoGa { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai{ get; set; } = 0;
        public double SlVanKhuanChatMatTrong { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai { get; set; } = 0;
        public double KlVanKhuanChatMatTrong { get; set; } = 0;

        public string Id_LoaiDauNoiXayTuongHoGa1 { get; set; } = "";
        public string HinhDangChiemChoXayTuongHoGa1 { get; set; } = "";
        public double SoLuongXayTuongHoGa1 { get; set; } = 0;
        public double DaiXayTuongHoGa1 { get; set; } = 0;
        public double RongXayTuongHoGa1 { get; set; } = 0;
        public double CaoXayTuongHoGa1 { get; set; } = 0;
        public double DienTichXayTuongHoGa1 { get; set; } = 0;
        public string GhiChuDienTichXayTuongHoGa1 { get; set; } = "";
        public double KlTuongXayTuongHoGa1 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai1 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong1 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai1 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong1 { get; set; } = 0;

        public string Id_LoaiDauNoiXayTuongHoGa2 { get; set; } = "";
        public string HinhDangChiemChoXayTuongHoGa2 { get; set; } = "";
        public double SoLuongXayTuongHoGa2 { get; set; } = 0;
        public double DaiXayTuongHoGa2 { get; set; } = 0;
        public double RongXayTuongHoGa2 { get; set; } = 0;
        public double CaoXayTuongHoGa2 { get; set; } = 0;
        public double DienTichXayTuongHoGa2 { get; set; } = 0;
        public string GhiChuDienTichXayTuongHoGa2 { get; set; } = "";
        public double KlTuongXayTuongHoGa2 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai2 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong2 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai2 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong2 { get; set; } = 0;

        public string Id_LoaiDauNoiXayTuongHoGa3 { get; set; } = "";
        public string HinhDangChiemChoXayTuongHoGa3 { get; set; } = "";
        public double SoLuongXayTuongHoGa3 { get; set; } = 0;
        public double DaiXayTuongHoGa3 { get; set; } = 0;
        public double RongXayTuongHoGa3 { get; set; } = 0;
        public double CaoXayTuongHoGa3 { get; set; } = 0;
        public double DienTichXayTuongHoGa3 { get; set; } = 0;
        public string GhiChuDienTichXayTuongHoGa3 { get; set; } = "";
        public double KlTuongXayTuongHoGa3 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai3 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong3 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai3 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong3 { get; set; } = 0;

        public string Id_LoaiDauNoiXayTuongHoGa4 { get; set; } = "";
        public string HinhDangChiemChoXayTuongHoGa4 { get; set; } = "";
        public double SoLuongXayTuongHoGa4 { get; set; } = 0;
        public double DaiXayTuongHoGa4 { get; set; } = 0;
        public double RongXayTuongHoGa4 { get; set; } = 0;
        public double CaoXayTuongHoGa4 { get; set; } = 0;
        public double DienTichXayTuongHoGa4 { get; set; } = 0;
        public string GhiChuDienTichXayTuongHoGa4 { get; set; } = "";
        public double KlTuongXayTuongHoGa4 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai4 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong4 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai4 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong4 { get; set; } = 0;

        public string Id_LoaiDauNoiTuongHoGa5 { get; set; } = "";
        public string HinhDangChiemChoTuongHoGa5 { get; set; } = "";
        public double SoLuongTuongHoGa5 { get; set; } = 0;
        public double DaiTuongHoGa5 { get; set; } = 0;
        public double RongTuongHoGa5 { get; set; } = 0;
        public double CaoTuongHoGa5 { get; set; } = 0;
        public double DienTichTuongHoGa5 { get; set; } = 0;
        public string GhiChuDienTichTuongHoGa5 { get; set; } = "";
        public double KlTuongTuongHoGa5 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai5 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong5 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai5 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong5 { get; set; } = 0;

        public string Id_LoaiDauNoiTuongHoGa6 { get; set; } = "";
        public string HinhDangChiemChoTuongHoGa6 { get; set; } = "";
        public double SoLuongTuongHoGa6 { get; set; } = 0;
        public double DaiTuongHoGa6 { get; set; } = 0;
        public double RongTuongHoGa6 { get; set; } = 0;
        public double CaoTuongHoGa6 { get; set; } = 0;
        public double DienTichTuongHoGa6 { get; set; } = 0;
        public string GhiChuDienTichTuongHoGa6 { get; set; } = "";
        public double KlTuongTuongHoGa6 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai6 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong6 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai6 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong6 { get; set; } = 0;

        public string Id_LoaiDauNoiTuongHoGa7 { get; set; } = "";
        public string HinhDangChiemChoTuongHoGa7 { get; set; } = "";
        public double SoLuongTuongHoGa7 { get; set; } = 0;
        public double DaiTuongHoGa7 { get; set; } = 0;
        public double RongTuongHoGa7 { get; set; } = 0;
        public double CaoTuongHoGa7 { get; set; } = 0;
        public double DienTichTuongHoGa7 { get; set; } = 0;
        public string GhiChuDienTichTuongHoGa7 { get; set; } = "";
        public double KlTuongTuongHoGa7 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai7 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong7 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai7 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong7 { get; set; } = 0;

        public string Id_LoaiDauNoiTuongHoGa8 { get; set; } = "";
        public string HinhDangChiemChoTuongHoGa8 { get; set; } = "";
        public double SoLuongTuongHoGa8 { get; set; } = 0;
        public double DaiTuongHoGa8 { get; set; } = 0;
        public double RongTuongHoGa8 { get; set; } = 0;
        public double CaoTuongHoGa8 { get; set; } = 0;
        public double DienTichTuongHoGa8 { get; set; } = 0;
        public string GhiChuDienTichTuongHoGa8 { get; set; } = "";
        public double KlTuongTuongHoGa8 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai8 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong8 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai8 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong8 { get; set; } = 0;

        public string Id_LoaiDauNoiTuongHoGa9 { get; set; } = "";
        public string HinhDangChiemChoTuongHoGa9 { get; set; } = "";
        public double SoLuongTuongHoGa9 { get; set; } = 0;
        public double DaiTuongHoGa9 { get; set; } = 0;
        public double RongTuongHoGa9 { get; set; } = 0;
        public double CaoTuongHoGa9 { get; set; } = 0;
        public double DienTichTuongHoGa9 { get; set; } = 0;
        public string GhiChuDienTichTuongHoGa9 { get; set; } = "";
        public double KlTuongTuongHoGa9 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai9 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong9 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai9 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong9 { get; set; } = 0;
        //Thông tin tấm đan
        //Khối lượng lắp đặt
        public string Id_HangMucCongViecTamDan { get; set; } = "";
        public string Id_LoaiCauKien { get; set; } = "";
        public string Id_HangMucKhoiLuongTamDan { get; set; } = "";
        public string Id_LoaiKhoiLuongTamDan { get; set; } = "";
        public double SoLuong { get; set; } = 0;
        public string Id_TrangThaiThiCongLapDat { get; set; } = "";
        //Khối lượng xây dựng tấm đan
        public string Id_HangMucKhoiLuongXayTamDan { get; set; } = "";
        public string Id_LoaiKhoiLuongXayTamDan { get; set; } = "";
        public double DaiXayTamDan { get; set; } = 0;
        public double RongXayTamDan { get; set; } = 0;
        public double CaoXayTamDan { get; set; } = 0;
        public double DienTichXayTamDan { get; set; } = 0;
        public string GhiChuDienTichXayTamDan { get; set; } = "";
        public double Kl01TDanXayTamDan { get; set; } = 0;
        public double TongKlTDanXayTamDan { get; set; } = 0;
        public string Id_TrangThaiThiCongXayTamDan { get; set; } = "";
        //Khối lượng ván khuân tấm đan
        public string Id_LoaiKhoiLuongVanKhuan { get; set; } = "";
        public double DaiVanKhuan { get; set; } = 0;
        public double SlCDaiVanKhuan { get; set; } = 0;
        public double CCaoCanhDaiVanKhuan { get; set; } = 0;
        public double DienTichCanhDaiVanKhuan { get; set; } = 0;
        public double KlVanKhuan { get; set; } = 0;
        public double RongVanKhuan { get; set; } = 0;
        public double SlCanhRongVanKhuan { get; set; } = 0;
        public double CCaoCanhRongVanKhuan { get; set; } = 0;
        public double DTichCRongVanKhuan { get; set; } = 0;
        public double KlCRongVanKhuan { get; set; } = 0;
        public double Kl01CauKienVanKhuan { get; set; } = 0;
        public double TongKlVanKhuanTDanVanKhuan { get; set; } = 0;
        public string Id_TrangThaiThiCongVanKhuan { get; set; } = "";
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
    }

    public class PKKL_OngNhua_4ThongTinChungHGa_Log : Chung_Log
    {
        public string Id_ChiNhanh { get; set; } = "";
        //Thông tin lý trình
        public string Id_TuyenDuong { get; set; } = "";
        public string Id_LyTrinh { get; set; } = "";
        public string Id_HangMucCongViec { get; set; } = "";
        public string TenHoVan { get; set; } = "";
        //Cao độ (m)
        public double HTrangTruocKhiDao { get; set; } = 0;
        public double DayDao { get; set; } = 0;
        public double ChieuSauDao { get; set; } = 0;
        public double CDayLMongHg { get; set; } = 0;
        public double DinhLotMong { get; set; } = 0;
        public double CDayMongHg { get; set; } = 0;
        public double DayHgDongChay { get; set; } = 0;
        public double CCaoTuong { get; set; } = 0;
        public double DinhTuong { get; set; } = 0;
        public double CDayMMoThotDuoi { get; set; } = 0;
        public double MMoThotDuoi { get; set; } = 0;
        public double CDayMMoThotTrenHoacTamDan { get; set; } = 0;
        public double DinhHg { get; set; } = 0;
        //Thông tin cao độ đắp (m)
        public string Id_HinhThucDapTra { get; set; } = "";
        public double CDoDayDapCat { get; set; } = 0;
        public double CDoDinhDapCat { get; set; } = 0;
        public double ChieuDayDapCat { get; set; } = 0;
        public double CDoDayDapDat { get; set; } = 0;
        public double CDoDinhDapDat { get; set; } = 0;
        public double ChieuDayDapDat { get; set; } = 0;
        //Thông tin hố đào biện pháp (m)
        public double TyLeMoMai { get; set; } = 0;
        public double SoMaiTrai { get; set; } = 0;
        public double SoMaiPhai { get; set; } = 0;
        public double CDaiDaoHoMong { get; set; } = 0;
        public double CRongDaoDayNho { get; set; } = 0;
        public double CRongDayLon { get; set; } = 0;
        //KL đào biện pháp
        public string HangMucKlDao { get; set; } = "";
        public string LoaiKlDao { get; set; } = "";
        public double DienTichDao { get; set; } = 0;
        public double KlDao { get; set; } = 0;
        public string Id_TrangThaiThiCongDao { get; set; } = "";
        //KL đắp trả
        public double CRongDayNhoDapCatDapTraDapCat { get; set; } = 0;
        public double CRongDayLonDapCatDapTraDapCat { get; set; } = 0;
        public double DienTichDapCatDapTraDapCat { get; set; } = 0;
        public double KlDapCatDapTraDapCat { get; set; } = 0;
        public string HangMucKlDapTraDapCat { get; set; } = "";
        public string LoaiKlDapTraDapCat { get; set; } = "";
        public double KlLotMongDapTraDapCat { get; set; } = 0;
        public double KlMongDapTraDapCat { get; set; } = 0;
        public double KlTuongBaoGomTuongVaMuMoDapTraDapCat { get; set; } = 0;
        public double KlCChoDapTraDapCat { get; set; } = 0;
        public double KlDapCatSauCChoDapTraDapCat { get; set; } = 0;
        public string Id_TrangThaiThiCongDapTraDapCat { get; set; } = "";

        public double CRongDayNhoDapDatDapTraDapDat { get; set; } = 0;
        public double CRongDayLonDapDatDapTraDapDat { get; set; } = 0;
        public double DienTichDapDatDapTraDapDat { get; set; } = 0;
        public double KlDapDatDapTraDapDat { get; set; } = 0;
        public string HangMucKlDapTraDapDat { get; set; } = "";
        public string LoaiKlDapTraDapDat { get; set; } = "";
        public double KlLotMongDapTraDapDat { get; set; } = 0;
        public double KlMongDapTraDapDat { get; set; } = 0;
        public double KlTuongBaoGomTuongVaMuMoDapTraDapDat { get; set; } = 0;
        public double KlCChoDapTraDapDat { get; set; } = 0;
        public double KlDapDatSauCChoDapTraDapDat { get; set; } = 0;
        public string HangMucKlDapTraDapDatThua { get; set; } = "";
        public string LoaiKlDapTraDapDatThua { get; set; } = "";
        public double KlDatThuaDapTraDapDat { get; set; } =0;
        public string Id_TrangThaiThiCongDapTraDapDat { get; set; } = "";
        //Thông tin lót móng hố ga
        public double DaiKTHH { get; set; } = 0;
        public double RongKTHH { get; set; } = 0;

        public string Id_HangMucKhoiLuongXayDungLotMong { get; set; } = "";
        public string Id_LoaiKhoiLuongXayDungLotMong { get; set; } = "";
        public double DaiXayDungLotMong { get; set; } = 0;
        public double RongXayDungLotMong { get; set; } = 0;
        public double CaoXayDungLotMong { get; set; } = 0;
        public double DienTichXayDungLotMong { get; set; } = 0;
        public string GhiChuDienTichXayDungLotMong { get; set; } = "";
        public double KlXayDungLotMong { get; set; } = 0;
        public string Id_TrangThaiThiCongXayDungLotMong { get; set; } = "";

        public string Id_LoaiKhoiLuongVanKhuanLotMong { get; set; } = "";
        public double DaiVanKhuanLotMong { get; set; } = 0;
        public double SlCDaiVanKhuanLotMong { get; set; } = 0;
        public double CCaoCanhDaiVanKhuanLotMong { get; set; } = 0;
        public double DTichCDaiVanKhuanLotMong { get; set; } = 0;
        public double KlCDaiVanKhuanLotMong { get; set; } = 0;
        public string Id_TrangThaiThiCongVanKhuanLotMong { get; set; } = "";
        public double RongVanKhuanLotMong { get; set; } = 0;
        public double SlCRongVanKhuanLotMong { get; set; } = 0;
        public double CCaoCanhRongVanKhuanLotMong { get; set; } = 0;
        public double DTichCRongVanKhuanLotMong { get; set; } = 0;
        public double KlCRongVanKhuanLotMong { get; set; } = 0;
        public string Id_TrangThaiThiCongVanKhuanLotMong1 { get; set; } = "";
        public double TongKlVanKhuanLotMongVanKhuanLotMong { get; set; } = 0;
        //Thông tin móng hố ga
        public string Id_HangMucKhoiLuongXayDungMong { get; set; } = "";
        public string Id_LoaiKhoiLuongXayDungMong { get; set; } = "";
        public double DaiXayDungMong { get; set; } = 0;
        public double RongXayDungMong { get; set; } = 0;
        public double CaoXayDungMong { get; set; } = 0;
        public double DienTichXayDungMong { get; set; } = 0;
        public string GhiChuDienTichXayDungMong { get; set; } = "";
        public double KlXayDungMong { get; set; } = 0;
        public string Id_TrangThaiThiCongXayDungMong { get; set; } = "";

        public string Id_LoaiKhoiLuongVanKhuanMong { get; set; } = "";
        public double DaiVanKhuanMong { get; set; } = 0;
        public double SlCDaiVanKhuanMong { get; set; } = 0;
        public double CCaoCanhDaiVanKhuanMong { get; set; } = 0;
        public double DTichCDaiVanKhuanMong { get; set; } = 0;
        public double KlCDaiVanKhuanMong { get; set; } = 0;
        public string Id_TrangThaiThiCongVanKhuanMong { get; set; } = "";
        public double RongVanKhuanMong { get; set; } = 0;
        public double SlCRongVanKhuanMong { get; set; } = 0;
        public double CCaoCanhRongVanKhuanMong { get; set; } = 0;
        public double DTichCRongVanKhuanMong { get; set; } = 0;
        public double KlCRongVanKhuanMong { get; set; } = 0;
        public string Id_TrangThaiThiCongVanKhuanMong1 { get; set; } = "";
        public double TongKlVanKhuanMongVanKhuanMong { get; set; } = 0;
        //Thông tin tường hố ga
        //Khối lượng xây dựng tường
        //KL xây dựng cạnh dài
        public string Id_HangMucKhoiLuongXayTuong { get; set; } = "";
        public string Id_LoaiKhoiLuongXayTuong { get; set; } = "";
        public double DaiXayTuong { get; set; } = 0;
        public double SlCDaiXayTuong { get; set; } = 0;
        public double CCaoCanhDaiXayTuong { get; set; } = 0;
        public double CDayTuongCanhDaiXayTuong { get; set; } = 0;
        public double DienTichCanhDaiXayTuong { get; set; } = 0;
        public string GhiChuDienTichXayTuongDai { get; set; } = "";
        public double KlTruocCChoCDaiXayTuong { get; set; } = 0;
        public double KlChiemChoCanhDaiXayTuong { get; set; } = 0;
        public double KlXayDungSauChiemChoCanhDaiXayTuong { get; set; } = 0;
        public string Id_TrangThaiThiCongXayTuongDai { get; set; } = "";

        public double RongXayTuong { get; set; } = 0;
        public double SlCRongXayTuong { get; set; } = 0;
        public double CCaoCanhRongXayTuong { get; set; } = 0;
        public double CDayTuongCanhRongXayTuong { get; set; } = 0;
        public double DienTichCanhRongXayTuong { get; set; } = 0;
        public string GhiChuDienTichXayTuongRong { get; set; } = "";
        public double KlTruocCChoCRongXayTuong { get; set; } = 0;
        public double KlChiemChoCanhRongXayTuong { get; set; } = 0;
        public double KlXayDungSauChiemChoCanhRong { get; set; } = 0;
        public string Id_TrangThaiThiCongXayTuongRong { get; set; } = "";
        public double TongKlTruocChiemChoXayTuong { get; set; } = 0;
        public double KlChiemChoXayTuong { get; set; } = 0;
        public double KlSauChiemChoXayTuong { get; set; } = 0;

        public string Id_LoaiKhoiLuongChatNgoai { get; set; } = "";
        public double DaiChatNgoai { get; set; } = 0;
        public double SlCDaiChatNgoai { get; set; } = 0;
        public double CCaoCanhDaiChatNgoai { get; set; } = 0;
        public double DienTichCanhDaiChatNgoai { get; set; } = 0;
        public double KlTruocCChoCDaiChatNgoai { get; set; } = 0;
        public double KlChiemChoCanhDaiChatNgoai { get; set; } = 0;
        public double KlXayDungSauChiemChoCanhDaiChatNgoai { get; set; } = 0;
        public string Id_TrangThaiThiCongChatNgoaiDai { get; set; } = "";

        public double RongChatNgoai { get; set; } = 0;
        public double SlCanhRongChatNgoai { get; set; } = 0;
        public double CCaoCanhRongChatNgoai { get; set; } = 0;
        public double DTichCRongChatNgoai { get; set; } = 0;
        public double KlTruocCChoCRongChatNgoai { get; set; } = 0;
        public double KlChiemChoCanhRongChatNgoai { get; set; } = 0;
        public double KlXayDungSauChiemChoCanhRongChatNgoai { get; set; } = 0;
        public string Id_TrangThaiThiCongChatNgoaiRong { get; set; } = "";

        public double TongKlTruocChiemChoChatNgoai { get; set; } = 0;
        public double KlChiemChoChatNgoai { get; set; } = 0;
        public double KlSauChiemChoChatNgoai { get; set; } = 0;

        public string Id_LoaiKhoiLuongChatTrong { get; set; } = "";
        public double DaiChatTrong { get; set; } = 0;
        public double SlCDaiChatTrong { get; set; } = 0;
        public double CCaoCanhDaiChatTrong { get; set; } = 0;
        public double DienTichCanhDaiChatTrong { get; set; } = 0;
        public double KlTruocCChoCDaiChatTrong { get; set; } = 0;
        public double KlChiemChoCanhDaiChatTrong { get; set; } = 0;
        public double KlXayDungSauChiemChoCanhDaiChatTrong { get; set; } = 0;
        public string Id_TrangThaiThiCongChatTrongDai { get; set; } = "";

        public double RongChatTrong { get; set; } = 0;
        public double SlCanhRongChatTrong { get; set; } = 0;
        public double CCaoCanhRongChatTrong { get; set; } = 0;
        public double DTichCRongChatTrong { get; set; } = 0;
        public double KlTruocCChoCRongChatTrong { get; set; } = 0;
        public double KlChiemChoCanhRongChatTrong { get; set; } = 0;
        public double KlXayDungSauChiemChoCanhRongChatTrong { get; set; } = 0;
        public string Id_TrangThaiThiCongChatTrongRong { get; set; } = "";

        public double TongKlTruocChiemChoChatTrong { get; set; } = 0;
        public double KlChiemChoChatTrong { get; set; } = 0;
        public double KlSauChiemChoChatTrong { get; set; } = 0;
        //Thông tin mũ mố thớt dưới hố ga
        public double KlVKhuanChatCDaiTruocCCho { get; set; } = 0;
        public double KlCChoVKhuanChatCDai { get; set; } = 0;
        public double KlVKhuanChatCDaiSauCCho { get; set; } = 0;
        public double KlVKhuanChatCRongTruocCCho { get; set; } = 0;
        public double KlCChoVKhuanChatCRong { get; set; } = 0;
        public double KlVKhuanChatCRongSauCCho { get; set; } = 0;

        public string Id_HangMucKhoiLuongMuMoDuoi { get; set; } = "";
        public string Id_LoaiKhoiLuongMuMoDuoi { get; set; } = "";
        public double DaiMuMoDuoi { get; set; } = 0;
        public double SlCDaiMuMoDuoi { get; set; } = 0;
        public double CCaoCanhDaiMuMoDuoi { get; set; } = 0;
        public double CDayTuongCanhDaiMuMoDuoi { get; set; } = 0;
        public double DienTichCanhDaiMuMoDuoi { get; set; } = 0;
        public string GhiChuDienTichMuMoDuoiDai { get; set; } = "";
        public double KlCDaiMuMoDuoi { get; set; } = 0;
        public string Id_TrangThaiThiCongMuMoDuoiDai { get; set; } = "";

        public double RongMuMoDuoi { get; set; } = 0;
        public double SlCRongMuMoDuoi { get; set; } = 0;
        public double CCaoCanhRongMuMoDuoi { get; set; } = 0;
        public double CDayTuongCanhRongMuMoDuoi { get; set; } = 0;
        public double DienTichCanhRongMuMoDuoi { get; set; } = 0;
        public string GhiChuDienTichMuMoDuoiRong { get; set; } = "";
        public double KlCRongMuMoDuoi { get; set; } = 0;
        public string Id_TrangThaiThiCongMuMoDuoiRong { get; set; } = "";
        public double TongKlMuMoDuoi { get; set; } = 0;

        public string Id_LoaiKhoiLuongChatDuoiNgoai { get; set; } = "";
        public double DaiChatDuoiNgoai { get; set; } = 0;
        public double SlCDaiChatDuoiNgoai { get; set; } = 0;
        public double CCaoCanhDaiChatDuoiNgoai { get; set; } = 0;
        public double DienTichCanhDaiChatDuoiNgoai { get; set; } = 0;
        public double KlCDaiChatDuoiNgoai { get; set; } = 0;
        public string Id_TrangThaiThiCongChatDuoiNgoaiDai { get; set; } = "";

        public double RongChatDuoiNgoai { get; set; } = 0;
        public double SlCanhRongChatDuoiNgoai { get; set; } = 0;
        public double CCaoCanhRongChatDuoiNgoai { get; set; } = 0;
        public double DTichCRongChatDuoiNgoai { get; set; } = 0;
        public double KlCRongChatDuoiNgoai { get; set; } = 0;
        public string Id_TrangThaiThiCongChatDuoiNgoaiRong { get; set; } = "";
        public double TongKlChatDuoiNgoai { get; set; } = 0;

        public string Id_LoaiKhoiLuongChatDuoiTrong { get; set; } = "";
        public double DaiChatDuoiTrong { get; set; } = 0;
        public double SlCDaiChatDuoiTrong { get; set; } = 0;
        public double CCaoCanhDaiChatDuoiTrong { get; set; } = 0;
        public double DienTichCanhDaiChatDuoiTrong { get; set; } = 0;
        public double KlCDaiChatDuoiTrong { get; set; } = 0;
        public string Id_TrangThaiThiCongChatDuoiTrongDai { get; set; } = "";

        public double RongChatDuoiTrong { get; set; } = 0;
        public double SlCanhRongChatDuoiTrong { get; set; } = 0;
        public double CCaoCanhRongChatDuoiTrong { get; set; } = 0;
        public double DTichCRongChatDuoiTrong { get; set; } = 0;
        public double KlCRongChatDuoiTrong { get; set; } = 0;
        public string Id_TrangThaiThiCongChatDuoiTrongRong { get; set; } = "";
        public double TongKlChatDuoiTrong { get; set; } = 0;

        public double CanhDaiMuMoDuoiDoGa { get; set; } = 0;
        public double CanhRongMuMoDuoiDoGa { get; set; } = 0;
        //Thông tin mũ mố thớt trên hố ga
        public string Id_HangMucKhoiLuongXayTren { get; set; } = "";
        public string Id_LoaiKhoiLuongXayTren { get; set; } = "";
        public double DaiXayTren { get; set; } = 0;
        public double SlCDaiXayTren { get; set; } = 0;
        public double CCaoCanhDaiXayTren { get; set; } = 0;
        public double CDayTuongCanhDaiXayTren { get; set; } = 0;
        public double DienTichCanhDaiXayTren { get; set; } = 0;
        public string GhiChuDienTichXayTrenDai { get; set; } = "";
        public double KlCDaiXayTren { get; set; } = 0;
        public string Id_TrangThaiThiCongXayTrenDai { get; set; } = "";

        public double RongXayTren { get; set; } = 0;
        public double SlCRongXayTren { get; set; } = 0;
        public double CCaoCanhRongXayTren { get; set; } = 0;
        public double CDayTuongCanhRongXayTren { get; set; } = 0;
        public double DienTichCanhRongXayTren { get; set; } = 0;
        public string GhiChuDienTichXayTrenRong { get; set; } = "";
        public double KlCRongXayTren { get; set; } = 0;
        public string Id_TrangThaiThiCongXayTrenRong { get; set; } = "";
        public double TongKlXayTren { get; set; } = 0;

        public string Id_LoaiKhoiLuongChatTren { get; set; } = "";
        public double DaiChatTren { get; set; } = 0;
        public double SlCDaiChatTren { get; set; } = 0;
        public double CCaoCanhDaiChatTren { get; set; } = 0;
        public double DienTichCanhDaiChatTren { get; set; } = 0;
        public double KlCDaiChatTren { get; set; } = 0;
        public string Id_TrangThaiThiCongChatTrenDai { get; set; } = "";

        public double RongChatTren { get; set; } = 0;
        public double SlCanhRongChatTren { get; set; } = 0;
        public double CCaoCanhRongChatTren { get; set; } = 0;
        public double DTichCRongChatTren { get; set; } = 0;
        public double KlCRongChatTren { get; set; } = 0;
        public string Id_TrangThaiThiCongChatTrenRong { get; set; } = "";
        public double TongKlChatTren { get; set; } = 0;

        public string Id_LoaiKhoiLuongChatTrenTrong { get; set; } = "";
        public double DaiChatTrenTrong { get; set; } = 0;
        public double SlCDaiChatTrenTrong { get; set; } = 0;
        public double CCaoCanhDaiChatTrenTrong { get; set; } = 0;
        public double DienTichCanhDaiChatTrenTrong { get; set; } = 0;
        public double KlCDaiChatTrenTrong { get; set; } = 0;
        public string Id_TrangThaiThiCongChatTrenTrongDai { get; set; } = "";

        public double RongChatTrenTrong { get; set; } = 0;
        public double SlCanhRongChatTrenTrong { get; set; } = 0;
        public double CCaoCanhRongChatTrenTrong { get; set; } = 0;
        public double DTichCRongChatTrenTrong { get; set; } = 0;
        public double KlCRongChatTrenTrong { get; set; } = 0;
        public string Id_TrangThaiThiCongChatTrenTrongRong { get; set; } = "";
        public double TongKlChatTrenTrong { get; set; } = 0;

        public double CanhDaiMuMoTrenHoGa { get; set; } = 0;
        public double CanhRongMuMoTrenHoGa { get; set; } = 0;
        //KL chiếm chỗ
        public double KlTuongCanhDai { get; set; } = 0;
        public double KlVanKhuanChatMatNgoaiCanhDai { get; set; } = 0;
        public double KlVanKhuanChatMatTrongCanhDai { get; set; } = 0;
        public double KlTuongCanhRong { get; set; } = 0;
        public double KlVanKhuanChatMatNgoaiCanhRong { get; set; } = 0;
        public double KlVanKhuanChatMatTrongCanhRong { get; set; } = 0;
        //Đấu nối cạnh dài
        public string Id_LoaiDauNoiXayTuongHoGa { get; set; } = "";
        public string HinhDangChiemChoXayTuongHoGa { get; set; } = "";
        public double SoLuongXayTuongHoGa { get; set; } = 0;
        public double DaiXayTuongHoGa { get; set; } = 0;
        public double RongXayTuongHoGa { get; set; } = 0;
        public double CaoXayTuongHoGa { get; set; } = 0;
        public double DienXayTuongHoGa { get; set; } = 0;
        public string GhiChuDienTichXayTuongHoGa { get; set; } = "";
        public double KlTuongXayTuongHoGa { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai { get; set; } = 0;
        public double SlVanKhuanChatMatTrong { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai { get; set; } = 0;
        public double KlVanKhuanChatMatTrong { get; set; } = 0;

        public string Id_LoaiDauNoiXayTuongHoGa1 { get; set; } = "";
        public string HinhDangChiemChoXayTuongHoGa1 { get; set; } = "";
        public double SoLuongXayTuongHoGa1 { get; set; } = 0;
        public double DaiXayTuongHoGa1 { get; set; } = 0;
        public double RongXayTuongHoGa1 { get; set; } = 0;
        public double CaoXayTuongHoGa1 { get; set; } = 0;
        public double DienTichXayTuongHoGa1 { get; set; } = 0;
        public string GhiChuDienTichXayTuongHoGa1 { get; set; } = "";
        public double KlTuongXayTuongHoGa1 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai1 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong1 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai1 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong1 { get; set; } = 0;

        public string Id_LoaiDauNoiXayTuongHoGa2 { get; set; } = "";
        public string HinhDangChiemChoXayTuongHoGa2 { get; set; } = "";
        public double SoLuongXayTuongHoGa2 { get; set; } = 0;
        public double DaiXayTuongHoGa2 { get; set; } = 0;
        public double RongXayTuongHoGa2 { get; set; } = 0;
        public double CaoXayTuongHoGa2 { get; set; } = 0;
        public double DienTichXayTuongHoGa2 { get; set; } = 0;
        public string GhiChuDienTichXayTuongHoGa2 { get; set; } = "";
        public double KlTuongXayTuongHoGa2 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai2 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong2 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai2 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong2 { get; set; } = 0;

        public string Id_LoaiDauNoiXayTuongHoGa3 { get; set; } = "";
        public string HinhDangChiemChoXayTuongHoGa3 { get; set; } = "";
        public double SoLuongXayTuongHoGa3 { get; set; } = 0;
        public double DaiXayTuongHoGa3 { get; set; } = 0;
        public double RongXayTuongHoGa3 { get; set; } = 0;
        public double CaoXayTuongHoGa3 { get; set; } = 0;
        public double DienTichXayTuongHoGa3 { get; set; } = 0;
        public string GhiChuDienTichXayTuongHoGa3 { get; set; } = "";
        public double KlTuongXayTuongHoGa3 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai3 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong3 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai3 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong3 { get; set; } = 0;

        public string Id_LoaiDauNoiXayTuongHoGa4 { get; set; } = "";
        public string HinhDangChiemChoXayTuongHoGa4 { get; set; } = "";
        public double SoLuongXayTuongHoGa4 { get; set; } = 0;
        public double DaiXayTuongHoGa4 { get; set; } = 0;
        public double RongXayTuongHoGa4 { get; set; } = 0;
        public double CaoXayTuongHoGa4 { get; set; } = 0;
        public double DienTichXayTuongHoGa4 { get; set; } = 0;
        public string GhiChuDienTichXayTuongHoGa4 { get; set; } = "";
        public double KlTuongXayTuongHoGa4 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai4 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong4 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai4 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong4 { get; set; } = 0;

        public string Id_LoaiDauNoiTuongHoGa5 { get; set; } = "";
        public string HinhDangChiemChoTuongHoGa5 { get; set; } = "";
        public double SoLuongTuongHoGa5 { get; set; } = 0;
        public double DaiTuongHoGa5 { get; set; } = 0;
        public double RongTuongHoGa5 { get; set; } = 0;
        public double CaoTuongHoGa5 { get; set; } = 0;
        public double DienTichTuongHoGa5 { get; set; } = 0;
        public string GhiChuDienTichTuongHoGa5 { get; set; } = "";
        public double KlTuongTuongHoGa5 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai5 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong5 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai5 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong5 { get; set; } = 0;

        public string Id_LoaiDauNoiTuongHoGa6 { get; set; } = "";
        public string HinhDangChiemChoTuongHoGa6 { get; set; } = "";
        public double SoLuongTuongHoGa6 { get; set; } = 0;
        public double DaiTuongHoGa6 { get; set; } = 0;
        public double RongTuongHoGa6 { get; set; } = 0;
        public double CaoTuongHoGa6 { get; set; } = 0;
        public double DienTichTuongHoGa6 { get; set; } = 0;
        public string GhiChuDienTichTuongHoGa6 { get; set; } = "";
        public double KlTuongTuongHoGa6 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai6 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong6 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai6 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong6 { get; set; } = 0;

        public string Id_LoaiDauNoiTuongHoGa7 { get; set; } = "";
        public string HinhDangChiemChoTuongHoGa7 { get; set; } = "";
        public double SoLuongTuongHoGa7 { get; set; } = 0;
        public double DaiTuongHoGa7 { get; set; } = 0;
        public double RongTuongHoGa7 { get; set; } = 0;
        public double CaoTuongHoGa7 { get; set; } = 0;
        public double DienTichTuongHoGa7 { get; set; } = 0;
        public string GhiChuDienTichTuongHoGa7 { get; set; } = "";
        public double KlTuongTuongHoGa7 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai7 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong7 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai7 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong7 { get; set; } = 0;

        public string Id_LoaiDauNoiTuongHoGa8 { get; set; } = "";
        public string HinhDangChiemChoTuongHoGa8 { get; set; } = "";
        public double SoLuongTuongHoGa8 { get; set; } = 0;
        public double DaiTuongHoGa8 { get; set; } = 0;
        public double RongTuongHoGa8 { get; set; } = 0;
        public double CaoTuongHoGa8 { get; set; } = 0;
        public double DienTichTuongHoGa8 { get; set; } = 0;
        public string GhiChuDienTichTuongHoGa8 { get; set; } = "";
        public double KlTuongTuongHoGa8 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai8 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong8 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai8 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong8 { get; set; } = 0;

        public string Id_LoaiDauNoiTuongHoGa9 { get; set; } = "";
        public string HinhDangChiemChoTuongHoGa9 { get; set; } = "";
        public double SoLuongTuongHoGa9 { get; set; } = 0;
        public double DaiTuongHoGa9 { get; set; } = 0;
        public double RongTuongHoGa9 { get; set; } = 0;
        public double CaoTuongHoGa9 { get; set; } = 0;
        public double DienTichTuongHoGa9 { get; set; } = 0;
        public string GhiChuDienTichTuongHoGa9 { get; set; } = "";
        public double KlTuongTuongHoGa9 { get; set; } = 0;

        public double SlVanKhuanChatMatNgoai9 { get; set; } = 0;
        public double SlVanKhuanChatMatTrong9 { get; set; } = 0;
        public double KlVanKhuanChatMatNgoai9 { get; set; } = 0;
        public double KlVanKhuanChatMatTrong9 { get; set; } = 0;
        //Thông tin tấm đan
        //Khối lượng lắp đặt
        public string Id_HangMucCongViecTamDan { get; set; } = "";
        public string Id_LoaiCauKien { get; set; } = "";
        public string Id_HangMucKhoiLuongTamDan { get; set; } = "";
        public string Id_LoaiKhoiLuongTamDan { get; set; } = "";
        public double SoLuong { get; set; } = 0;
        public string Id_TrangThaiThiCongLapDat { get; set; } = "";
        //Khối lượng xây dựng tấm đan
        public string Id_HangMucKhoiLuongXayTamDan { get; set; } = "";
        public string Id_LoaiKhoiLuongXayTamDan { get; set; } = "";
        public double DaiXayTamDan { get; set; } = 0;
        public double RongXayTamDan { get; set; } = 0;
        public double CaoXayTamDan { get; set; } = 0;
        public double DienTichXayTamDan { get; set; } = 0;
        public string GhiChuDienTichXayTamDan { get; set; } = "";
        public double Kl01TDanXayTamDan { get; set; } = 0;
        public double TongKlTDanXayTamDan { get; set; } = 0;
        public string Id_TrangThaiThiCongXayTamDan { get; set; } = "";
        //Khối lượng ván khuân tấm đan
        public string Id_LoaiKhoiLuongVanKhuan { get; set; } = "";
        public double DaiVanKhuan { get; set; } = 0;
        public double SlCDaiVanKhuan { get; set; } = 0;
        public double CCaoCanhDaiVanKhuan { get; set; } = 0;
        public double DienTichCanhDaiVanKhuan { get; set; } = 0;
        public double KlVanKhuan { get; set; } = 0;
        public double RongVanKhuan { get; set; } = 0;
        public double SlCanhRongVanKhuan { get; set; } = 0;
        public double CCaoCanhRongVanKhuan { get; set; } = 0;
        public double DTichCRongVanKhuan { get; set; } = 0;
        public double KlCRongVanKhuan { get; set; } = 0;
        public double Kl01CauKienVanKhuan { get; set; } = 0;
        public double TongKlVanKhuanTDanVanKhuan { get; set; } = 0;
        public string Id_TrangThaiThiCongVanKhuan { get; set; } = "";
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
    }
}

