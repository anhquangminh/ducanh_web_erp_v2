using DucAnh2025.Data;
using DucAnh2025.Models.CongTrinh;
using DucAnh2025.Repository.CongTrinh;
using DucAnh2025.ViewModels.CongTrinh;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services.CongTrinh
{
    public class PKKL_OngNhua_1TtChungNSachDocRepository : IPKKL_OngNhua_1TtChungNSachDocRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public PKKL_OngNhua_1TtChungNSachDocRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public async Task<List<PKKL_OngNhua_1TtChungNSachDocModel>> GetAllByVM(PKKL_OngNhua_1TtChungNSachDocModel dataModel, string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();

                // Lấy dữ liệu gốc
                var baseData = await context.PKKL_OngNhua_1TtChungNSachDocs
                    .AsNoTracking()
                    .Where(p => p.GroupId == groupId && p.IsActive != 100)
                    .OrderByDescending(p => p.CreateAt)
                    .ToListAsync();

                // Lấy dictionary lookup
                var chiNhanhs = await context.ChiNhanhs.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var tuyenDuongs = await context.CT_DM_TuyenDuongs.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var lyTrinhs = await context.CT_DM_LyTrinhs.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var hangMucCVs = await context.CT_DM_HangMucCongViecs.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var loaiCauKiens = await context.CT_DM_LoaiCauKiens.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var hangMucKLs = await context.CT_DM_HangMucKhoiLuongs.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var loaiKhoiLuongs = await context.CT_DM_LoaiKhoiLuongs.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var hinhThucDapTras = await context.CT_DM_HinhThucDapTras.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var trangThaiThiCongs = await context.CT_DM_TrangThaiThiCongs.AsNoTracking().ToDictionaryAsync(x => x.Id);

                // Map dữ liệu
                var data = baseData.Select(p1 => new PKKL_OngNhua_1TtChungNSachDocModel
                {
                    Id = p1.Id,
                    Id_ChiNhanh = p1.Id_ChiNhanh,
                    TenChiNhanh = chiNhanhs.TryGetValue(p1.Id_ChiNhanh, out var cn) ? cn.TenChiNhanh : "",

                    Id_TuyenDuong = p1.Id_TuyenDuong,
                    TuyenDuong = tuyenDuongs.TryGetValue(p1.Id_TuyenDuong, out var td) ? td.TenDanhMuc : "",
                    Id_TuLyTrinh = p1.Id_TuLyTrinh,
                    TuLyTrinh = lyTrinhs.TryGetValue(p1.Id_TuLyTrinh, out var tlt) ? tlt.TenDanhMuc : "",
                    Id_DenLyTrinh = p1.Id_DenLyTrinh,
                    DenLyTrinh = lyTrinhs.TryGetValue(p1.Id_DenLyTrinh, out var dlt) ? dlt.TenDanhMuc : "",

                    Id_HangMucCongViec = p1.Id_HangMucCongViec,
                    HangMucCongViec = hangMucCVs.TryGetValue(p1.Id_HangMucCongViec, out var hmcv) ? hmcv.TenDanhMuc : "",
                    Id_LoaiCauKien = p1.Id_LoaiCauKien,
                    LoaiCauKien = loaiCauKiens.TryGetValue(p1.Id_LoaiCauKien, out var lck) ? lck.TenDanhMuc : "",
                    Id_HangMucKhoiLuong = p1.Id_HangMucKhoiLuong,
                    HangMucKhoiLuong = hangMucKLs.TryGetValue(p1.Id_HangMucKhoiLuong, out var hmkl) ? hmkl.TenDanhMuc : "",
                    Id_LoaiKhoiLuong = p1.Id_LoaiKhoiLuong,
                    LoaiKhoiLuong = loaiKhoiLuongs.TryGetValue(p1.Id_LoaiKhoiLuong, out var lkl) ? lkl.TenDanhMuc : "",

                    DuongKinhNgoaiOngNhua = p1.DuongKinhNgoaiOngNhua,
                    CDayOngOngNhua = p1.CDayOngOngNhua,
                    ChieuDaiOngNhua = p1.ChieuDaiOngNhua,
                    Id_TrangThaiThiCongOngNhua = p1.Id_TrangThaiThiCongOngNhua,
                    TrangThaiThiCongOngNhua = trangThaiThiCongs.TryGetValue(p1.Id_TrangThaiThiCongOngNhua, out var tttcon) ? tttcon.TenDanhMuc : "",
                    Id_LoaiCauKienOngThep = p1.Id_LoaiCauKienOngThep,
                    LoaiCauKienOngThep = loaiCauKiens.TryGetValue(p1.Id_LoaiCauKienOngThep, out var lckot) ? lckot.TenDanhMuc : "",
                    Id_HangMucKhoiLuongOngThep = p1.Id_HangMucKhoiLuongOngThep,
                    HangMucKhoiLuongOngThep = hangMucKLs.TryGetValue(p1.Id_HangMucKhoiLuongOngThep, out var hmklot) ? hmklot.TenDanhMuc : "",
                    Id_LoaiKhoiLuongOngThep = p1.Id_LoaiKhoiLuongOngThep,
                    LoaiKhoiLuongOngThep = loaiKhoiLuongs.TryGetValue(p1.Id_LoaiKhoiLuongOngThep, out var lklot) ? lklot.TenDanhMuc : "",

                    DuongKinhNgoaiMOngThep = p1.DuongKinhNgoaiMOngThep,
                    CDayOngMOngThep = p1.CDayOngMOngThep,
                    ChieuDaiMOngThep = p1.ChieuDaiMOngThep,
                    Id_TrangThaiThiCongOngThep = p1.Id_TrangThaiThiCongOngThep,
                    TrangThaiThiCongOngThep = trangThaiThiCongs.TryGetValue(p1.Id_TrangThaiThiCongOngThep, out var tttcot) ? tttcot.TenDanhMuc : "",

                    Id_HinhThucDapTra = p1.Id_HinhThucDapTra,
                    HinhThucDapTra = hinhThucDapTras.TryGetValue(p1.Id_HinhThucDapTra, out var htdt) ? htdt.TenDanhMuc : "",

                    HTrangTruocKhiDaoThuongLuu = p1.HTrangTruocKhiDaoThuongLuu,
                    DayDaoThuongLuu = p1.DayDaoThuongLuu,
                    ChieuSauDaoThuongLuu = p1.ChieuSauDaoThuongLuu,
                    DongChayThuongLuu = p1.DongChayThuongLuu,
                    DinhDuongOngThuongLuu = p1.DinhDuongOngThuongLuu,
                    CDoDayDemCatThuongLuu = p1.CDoDayDemCatThuongLuu,
                    CDayDemCatThuongLuu = p1.CDayDemCatThuongLuu,
                    DinhDemCatThuongLuu = p1.DinhDemCatThuongLuu,
                    DayDapCatThuongLuu = p1.DayDapCatThuongLuu,
                    CDayDapCatThuongLuu = p1.CDayDapCatThuongLuu,
                    DinhDapCatThuongLuu = p1.DinhDapCatThuongLuu,
                    TongChieuDayDemDapCatThuongLuu = p1.TongChieuDayDemDapCatThuongLuu,
                    CDoDayDapDatThuongLuu = p1.CDoDayDapDatThuongLuu,
                    ChieuDayDapDatThuongLuu = p1.ChieuDayDapDatThuongLuu,
                    DinhDapDatThuongLuu = p1.DinhDapDatThuongLuu,
                    DapDatCatThuongLuu = p1.DapDatCatThuongLuu,
                    ChenhDapSoVoiDaoThuongLuu = p1.ChenhDapSoVoiDaoThuongLuu,
                    HTrangTruocKhiDaoHaLuu = p1.HTrangTruocKhiDaoHaLuu,
                    DayDaoHaLuu = p1.DayDaoHaLuu,
                    ChieuSauDaoHaLuu = p1.ChieuSauDaoHaLuu,
                    DongChayHaLuu = p1.DongChayHaLuu,
                    DinhDuongOngHaLuu = p1.DinhDuongOngHaLuu,
                    CDoDayDemCatHaLuu = p1.CDoDayDemCatHaLuu,
                    CDayDemCatHaLuu = p1.CDayDemCatHaLuu,
                    DinhDemCatHaLuu = p1.DinhDemCatHaLuu,
                    DayDapCatHaLuu = p1.DayDapCatHaLuu,
                    CDayDapCatHaLuu = p1.CDayDapCatHaLuu,
                    DinhDapCatHaLuu = p1.DinhDapCatHaLuu,
                    TongChieuDayDemDapCatHaLuu = p1.TongChieuDayDemDapCatHaLuu,
                    CDoDayDapDatHaLuu = p1.CDoDayDapDatHaLuu,
                    ChieuDayDapDatHaLuu = p1.ChieuDayDapDatHaLuu,
                    DinhDapDatHaLuu = p1.DinhDapDatHaLuu,
                    DapDatCatHaLuu = p1.DapDatCatHaLuu,
                    ChenhDapSoVoiDaoHaLuu = p1.ChenhDapSoVoiDaoHaLuu,
                    CDoDayDemCat = p1.CDoDayDemCat,
                    CDoDinhDemCat = p1.CDoDinhDemCat,
                    ChieuDayDemCat = p1.ChieuDayDemCat,
                    CDoDayDapCat = p1.CDoDayDapCat,
                    CDoDinhDapCat = p1.CDoDinhDapCat,
                    ChieuDayDapCat = p1.ChieuDayDapCat,
                    CDoDayDapDat = p1.CDoDayDapDat,
                    CDoDinhDapDat = p1.CDoDinhDapDat,
                    ChieuDayDapDat = p1.ChieuDayDapDat,
                    CRongDayNhoHLuu = p1.CRongDayNhoHLuu,
                    CRongDayNhoTLuu = p1.CRongDayNhoTLuu,
                    CRongDayNhoTBinh = p1.CRongDayNhoTBinh,
                    ChieuSauDaoTrungBinh = p1.ChieuSauDaoTrungBinh,
                    CRongDayLonTrungBinh = p1.CRongDayLonTrungBinh,
                    TyLeMoMai = p1.TyLeMoMai,
                    SoMaiTrai = p1.SoMaiTrai,
                    SoMaiPhai = p1.SoMaiPhai,
                    HangMucKlDaoDat = p1.HangMucKlDaoDat,
                    LoaiKlDaoDat = p1.LoaiKlDaoDat,
                    DienTich = p1.DienTich,
                    KlDao = p1.KlDao,

                    Id_TrangThaiThiCongDaoDat = p1.Id_TrangThaiThiCongDaoDat,
                    TrangThaiThiCongDaoDat = trangThaiThiCongs.TryGetValue(p1.Id_TrangThaiThiCongDaoDat, out var tttcdd) ? tttcdd.TenDanhMuc : "",

                    CRongDayNhoDemCat = p1.CRongDayNhoDemCat,
                    CRongDayLonDemCat = p1.CRongDayLonDemCat,
                    DienTichDapCat1 = p1.DienTichDapCat1,
                    KlDemCat = p1.KlDemCat,
                    CRongDayNhoDapCat = p1.CRongDayNhoDapCat,
                    CRongDayLonDapCat = p1.CRongDayLonDapCat,
                    DienTichDapCat2 = p1.DienTichDapCat2,
                    KlDapCat = p1.KlDapCat,
                    HangMucKlDapCat = p1.HangMucKlDapCat,
                    LoaiKlDapCat = p1.LoaiKlDapCat,
                    KLDapCat_KlOngCCho = p1.KLDapCat_KlOngCCho,
                    KlDapCatSauCCho = p1.KlDapCatSauCCho,

                    Id_TrangThaiThiCongDapCat = p1.Id_TrangThaiThiCongDapCat,
                    TrangThaiThiCongDapCat = trangThaiThiCongs.TryGetValue(p1.Id_TrangThaiThiCongDapCat, out var tttcdc) ? tttcdc.TenDanhMuc : "",

                    CRongDayNhoDapDat = p1.CRongDayNhoDapDat,
                    CRongDayLonDapDat = p1.CRongDayLonDapDat,
                    DienTichDapDat = p1.DienTichDapDat,
                    KlDapDat = p1.KlDapDat,
                    HangMucKlDapDat = p1.HangMucKlDapDat,
                    LoaiKlDapDat = p1.LoaiKlDapDat,
                    KLDapDat_KlOngCCho = p1.KLDapDat_KlOngCCho,
                    KlDapDatSauCCho = p1.KlDapDatSauCCho,

                    Id_TrangThaiThiCongDapDat = p1.Id_TrangThaiThiCongDapDat,
                    TrangThaiThiCongDapDat = trangThaiThiCongs.TryGetValue(p1.Id_TrangThaiThiCongDapDat, out var tttcdapd) ? tttcdapd.TenDanhMuc : "",

                    HangMucKlDatThua = p1.HangMucKlDatThua,
                    LoaiKlDatThua = p1.LoaiKlDatThua,
                    KlDatThua = p1.KlDatThua,

                    Id_TrangThaiThiCongDatThua = p1.Id_TrangThaiThiCongDatThua,
                    TrangThaiThiCongDatThua = trangThaiThiCongs.TryGetValue(p1.Id_TrangThaiThiCongDatThua, out var tttcdt) ? tttcdt.TenDanhMuc : "",

                    X1 = p1.X1,
                    Y1 = p1.Y1,
                    X2 = p1.X2,
                    Y2 = p1.Y2,

                    GroupId = p1.GroupId,
                    Ordinarily = p1.Ordinarily,
                    CreateAt = (DateTime)p1.CreateAt,
                    CreateBy = p1.CreateBy,
                    IsActive = p1.IsActive,
                    ApprovalUserId = p1.ApprovalUserId ?? "",
                    DateApproval = p1.DateApproval ?? DateTime.MinValue,
                    ApprovalDept = p1.ApprovalDept ?? "",
                    DepartmentId = p1.DepartmentId,
                    DepartmentOrder = p1.DepartmentOrder,
                    ApprovalOrder = p1.ApprovalOrder,
                    ApprovalId = p1.ApprovalId,
                    LastApprovalId = p1.LastApprovalId,
                    IsStatus = p1.IsStatus
                }).ToList();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy dữ liệu: " + ex.Message, ex);
            }
        }

        public async Task<List<PKKL_OngNhua_1TtChungNSachDocModel>> GetHistoryIsValidEdit(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var data = await (from p1 in context.PKKL_OngNhua_1TtChungNSachDoc_Logs
                                  join cn in context.ChiNhanhs on p1.Id_ChiNhanh equals cn.Id into cnJoin 
                                  from cn in cnJoin.DefaultIfEmpty()

                                  join hmcv in context.CT_DM_HangMucCongViecs on p1.Id_HangMucCongViec equals hmcv.Id into hmcvJoin
                                  from hmcv in hmcvJoin.DefaultIfEmpty()

                                  join td in context.CT_DM_TuyenDuongs on p1.Id_TuyenDuong equals td.Id into tdJoin
                                  from td in tdJoin.DefaultIfEmpty()
                                  join tlt in context.CT_DM_LyTrinhs on p1.Id_TuLyTrinh equals tlt.Id into tltJoin
                                  from tlt in tltJoin.DefaultIfEmpty()
                                  join dlt in context.CT_DM_LyTrinhs on p1.Id_DenLyTrinh equals dlt.Id into dltJoin
                                  from dlt in dltJoin.DefaultIfEmpty()

                                  join lck in context.CT_DM_LoaiCauKiens on p1.Id_LoaiCauKien equals lck.Id into ttvtJoin
                                  from lck in ttvtJoin.DefaultIfEmpty()
                                  join lckot in context.CT_DM_LoaiCauKiens on p1.Id_LoaiCauKienOngThep equals lckot.Id into lckotJoin
                                  from lckot in lckotJoin.DefaultIfEmpty()

                                  join hmkl in context.CT_DM_HangMucKhoiLuongs on p1.Id_HangMucKhoiLuong equals hmkl.Id into hmklJoin
                                  from hmkl in hmklJoin.DefaultIfEmpty()
                                  join hmklot in context.CT_DM_HangMucKhoiLuongs on p1.Id_HangMucKhoiLuongOngThep equals hmklot.Id into hmklotJoin
                                  from hmklot in hmklotJoin.DefaultIfEmpty()

                                  join lkl in context.CT_DM_LoaiKhoiLuongs on p1.Id_LoaiKhoiLuong equals lkl.Id into lklJoin
                                  from lkl in lklJoin.DefaultIfEmpty()
                                  join lklot in context.CT_DM_LoaiKhoiLuongs on p1.Id_LoaiKhoiLuongOngThep equals lklot.Id into lklotJoin
                                  from lklot in lklotJoin.DefaultIfEmpty()

                                  join htdt in context.CT_DM_HinhThucDapTras on p1.Id_HinhThucDapTra equals htdt.Id into htdtJoin
                                  from htdt in htdtJoin.DefaultIfEmpty()

                                  join tttcdd in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongDaoDat equals tttcdd.Id into tttcddJoin
                                  from tttcdd in tttcddJoin.DefaultIfEmpty()
                                  join tttcdc in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongDapCat equals tttcdc.Id into tttcdcJoin
                                  from tttcdc in tttcdcJoin.DefaultIfEmpty()
                                  join tttcdt in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongDatThua equals tttcdt.Id into tttcdtJoin
                                  from tttcdt in tttcdtJoin.DefaultIfEmpty()
                                  join tttcdapd in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongDapDat equals tttcdapd.Id into tttcdapdJoin
                                  from tttcdapd in tttcdapdJoin.DefaultIfEmpty()
                                  join tttcon in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongOngNhua equals tttcon.Id into tttconJoin
                                  from tttcon in tttconJoin.DefaultIfEmpty()
                                  join tttcot in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongOngThep equals tttcot.Id into tttcotJoin
                                  from tttcot in tttcotJoin.DefaultIfEmpty()

                                  join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                                  join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                                  where p1.IdChung == id && p1.IsValid == true
                                  
                                  select new PKKL_OngNhua_1TtChungNSachDocModel
                                  {
                                      Id = p1.Id,
                                      Id_ChiNhanh= p1.Id_ChiNhanh,
                                      TenChiNhanh = cn.TenChiNhanh,

                                      Id_TuyenDuong = p1.Id_TuyenDuong,
                                      TuyenDuong = td.TenDanhMuc,
                                      Id_TuLyTrinh = p1.Id_TuLyTrinh,
                                      TuLyTrinh = tlt.TenDanhMuc,
                                      Id_DenLyTrinh = p1.Id_DenLyTrinh,
                                      DenLyTrinh = dlt.TenDanhMuc,

                                      Id_HangMucCongViec = p1.Id_HangMucCongViec,
                                      HangMucCongViec = hmcv.TenDanhMuc,
                                      Id_LoaiCauKien = p1.Id_LoaiCauKien,
                                      LoaiCauKien = lck.TenDanhMuc,
                                      Id_HangMucKhoiLuong = p1.Id_HangMucKhoiLuong,
                                      HangMucKhoiLuong = hmkl.TenDanhMuc,
                                      Id_LoaiKhoiLuong = p1.Id_LoaiKhoiLuong,
                                      LoaiKhoiLuong = lkl.TenDanhMuc,

                                      DuongKinhNgoaiOngNhua = p1.DuongKinhNgoaiOngNhua,
                                      CDayOngOngNhua = p1.CDayOngOngNhua,
                                      ChieuDaiOngNhua = p1.ChieuDaiOngNhua,
                                      Id_TrangThaiThiCongOngNhua = p1.Id_TrangThaiThiCongOngNhua,
                                      Id_LoaiCauKienOngThep = p1.Id_LoaiCauKienOngThep,
                                      Id_HangMucKhoiLuongOngThep = p1.Id_HangMucKhoiLuongOngThep,
                                      Id_LoaiKhoiLuongOngThep = p1.Id_LoaiKhoiLuongOngThep,

                                      DuongKinhNgoaiMOngThep = p1.DuongKinhNgoaiMOngThep,
                                      CDayOngMOngThep = p1.CDayOngMOngThep,
                                      ChieuDaiMOngThep = p1.ChieuDaiMOngThep,
                                      Id_TrangThaiThiCongOngThep = p1.Id_TrangThaiThiCongOngThep,

                                      Id_HinhThucDapTra = p1.Id_HinhThucDapTra,
                                      HinhThucDapTra = htdt.TenDanhMuc,

                                      HTrangTruocKhiDaoThuongLuu = p1.HTrangTruocKhiDaoThuongLuu,
                                      DayDaoThuongLuu = p1.DayDaoThuongLuu,
                                      ChieuSauDaoThuongLuu = p1.ChieuSauDaoThuongLuu,
                                      DongChayThuongLuu = p1.DongChayThuongLuu,
                                      DinhDuongOngThuongLuu = p1.DinhDuongOngThuongLuu,
                                      CDoDayDemCatThuongLuu = p1.CDoDayDemCatThuongLuu,
                                      CDayDemCatThuongLuu = p1.CDayDemCatThuongLuu,
                                      DinhDemCatThuongLuu = p1.DinhDemCatThuongLuu,
                                      DayDapCatThuongLuu = p1.DayDapCatThuongLuu,
                                      CDayDapCatThuongLuu = p1.CDayDapCatThuongLuu,
                                      DinhDapCatThuongLuu = p1.DinhDapCatThuongLuu,
                                      TongChieuDayDemDapCatThuongLuu = p1.TongChieuDayDemDapCatThuongLuu,
                                      CDoDayDapDatThuongLuu = p1.CDoDayDapDatThuongLuu,
                                      ChieuDayDapDatThuongLuu = p1.ChieuDayDapDatThuongLuu,
                                      DinhDapDatThuongLuu = p1.DinhDapDatThuongLuu,
                                      DapDatCatThuongLuu = p1.DapDatCatThuongLuu,
                                      ChenhDapSoVoiDaoThuongLuu = p1.ChenhDapSoVoiDaoThuongLuu,
                                      HTrangTruocKhiDaoHaLuu = p1.HTrangTruocKhiDaoHaLuu,
                                      DayDaoHaLuu = p1.DayDaoHaLuu,
                                      ChieuSauDaoHaLuu = p1.ChieuSauDaoHaLuu,
                                      DongChayHaLuu = p1.DongChayHaLuu,
                                      DinhDuongOngHaLuu = p1.DinhDuongOngHaLuu,
                                      CDoDayDemCatHaLuu = p1.CDoDayDemCatHaLuu,
                                      CDayDemCatHaLuu = p1.CDayDemCatHaLuu,
                                      DinhDemCatHaLuu = p1.DinhDemCatHaLuu,
                                      DayDapCatHaLuu = p1.DayDapCatHaLuu,
                                      CDayDapCatHaLuu = p1.CDayDapCatHaLuu,
                                      DinhDapCatHaLuu = p1.DinhDapCatHaLuu,
                                      TongChieuDayDemDapCatHaLuu = p1.TongChieuDayDemDapCatHaLuu,
                                      CDoDayDapDatHaLuu = p1.CDoDayDapDatHaLuu,
                                      ChieuDayDapDatHaLuu = p1.ChieuDayDapDatHaLuu,
                                      DinhDapDatHaLuu = p1.DinhDapDatHaLuu,
                                      DapDatCatHaLuu = p1.DapDatCatHaLuu,
                                      ChenhDapSoVoiDaoHaLuu = p1.ChenhDapSoVoiDaoHaLuu,
                                      CDoDayDemCat = p1.CDoDayDemCat,
                                      CDoDinhDemCat = p1.CDoDinhDemCat,
                                      ChieuDayDemCat = p1.ChieuDayDemCat,
                                      CDoDayDapCat = p1.CDoDayDapCat,
                                      CDoDinhDapCat = p1.CDoDinhDapCat,
                                      ChieuDayDapCat = p1.ChieuDayDapCat,
                                      CDoDayDapDat = p1.CDoDayDapDat,
                                      CDoDinhDapDat = p1.CDoDinhDapDat,
                                      ChieuDayDapDat = p1.ChieuDayDapDat,
                                      CRongDayNhoHLuu = p1.CRongDayNhoHLuu,
                                      CRongDayNhoTLuu = p1.CRongDayNhoTLuu,
                                      CRongDayNhoTBinh = p1.CRongDayNhoTBinh,
                                      ChieuSauDaoTrungBinh = p1.ChieuSauDaoTrungBinh,
                                      CRongDayLonTrungBinh = p1.CRongDayLonTrungBinh,
                                      TyLeMoMai = p1.TyLeMoMai,
                                      SoMaiTrai = p1.SoMaiTrai,
                                      SoMaiPhai = p1.SoMaiPhai,
                                      HangMucKlDaoDat = p1.HangMucKlDaoDat,
                                      LoaiKlDaoDat = p1.LoaiKlDaoDat,
                                      DienTich = p1.DienTich,
                                      KlDao = p1.KlDao,

                                      Id_TrangThaiThiCongDaoDat = p1.Id_TrangThaiThiCongDaoDat,
                                      TrangThaiThiCongDaoDat = tttcdd.TenDanhMuc,

                                      CRongDayNhoDemCat = p1.CRongDayNhoDemCat,
                                      CRongDayLonDemCat = p1.CRongDayLonDemCat,
                                      DienTichDapCat1 = p1.DienTichDapCat1,
                                      KlDemCat = p1.KlDemCat,
                                      CRongDayNhoDapCat = p1.CRongDayNhoDapCat,
                                      CRongDayLonDapCat = p1.CRongDayLonDapCat,
                                      DienTichDapCat2 = p1.DienTichDapCat2,
                                      KlDapCat = p1.KlDapCat,
                                      HangMucKlDapCat = p1.HangMucKlDapCat,
                                      LoaiKlDapCat = p1.LoaiKlDapCat,
                                      KLDapCat_KlOngCCho = p1.KLDapCat_KlOngCCho,
                                      KlDapCatSauCCho = p1.KlDapCatSauCCho,

                                      Id_TrangThaiThiCongDapCat = p1.Id_TrangThaiThiCongDapCat,
                                      TrangThaiThiCongDapCat = tttcdc.TenDanhMuc,

                                      CRongDayNhoDapDat = p1.CRongDayNhoDapDat,
                                      CRongDayLonDapDat = p1.CRongDayLonDapDat,
                                      DienTichDapDat = p1.DienTichDapDat,
                                      KlDapDat = p1.KlDapDat,
                                      HangMucKlDapDat = p1.HangMucKlDapDat,
                                      LoaiKlDapDat = p1.LoaiKlDapDat,
                                      KLDapDat_KlOngCCho = p1.KLDapDat_KlOngCCho,
                                      KlDapDatSauCCho = p1.KlDapDatSauCCho,

                                      Id_TrangThaiThiCongDapDat = p1.Id_TrangThaiThiCongDapDat,

                                      HangMucKlDatThua = p1.HangMucKlDatThua,
                                      LoaiKlDatThua = p1.LoaiKlDatThua,
                                      KlDatThua = p1.KlDatThua,

                                      Id_TrangThaiThiCongDatThua = p1.Id_TrangThaiThiCongDatThua,
                                      TrangThaiThiCongDatThua = tttcdt.TenDanhMuc,

                                      X1 = p1.X1,
                                      Y1 = p1.Y1,
                                      X2 = p1.X2,
                                      Y2 = p1.Y2,

                                      GroupId = p1.GroupId,
                                      Ordinarily = p1.Ordinarily,
                                      CreateAt = p1.CreateAt ?? DateTime.MinValue,
                                      CreateBy = createBy.Email,
                                      IsActive = p1.IsActive,
                                      ApprovalUserId = p1.ApprovalUserId,
                                      DateApproval = p1.DateApproval,
                                      ApprovalDept = p1.ApprovalDept,
                                      DepartmentId = p1.DepartmentId,
                                      DepartmentOrder = p1.DepartmentOrder,
                                      ApprovalOrder = p1.ApprovalOrder,
                                      ApprovalId = p1.ApprovalId,
                                      LastApprovalId = p1.LastApprovalId,
                                      IsStatus = p1.IsStatus
                                  }).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<PKKL_OngNhua_1TtChungNSachDocModel> GetDetails(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.PKKL_OngNhua_1TtChungNSachDocs
                              join cn in context.ChiNhanhs on p1.Id_ChiNhanh equals cn.Id into cnJoin
                              from cn in cnJoin.DefaultIfEmpty()

                              join hmcv in context.CT_DM_HangMucCongViecs on p1.Id_HangMucCongViec equals hmcv.Id into hmcvJoin
                              from hmcv in hmcvJoin.DefaultIfEmpty()

                              join td in context.CT_DM_TuyenDuongs on p1.Id_TuyenDuong equals td.Id into tdJoin
                              from td in tdJoin.DefaultIfEmpty()
                              join tlt in context.CT_DM_LyTrinhs on p1.Id_TuLyTrinh equals tlt.Id into tltJoin
                              from tlt in tltJoin.DefaultIfEmpty()
                              join dlt in context.CT_DM_LyTrinhs on p1.Id_DenLyTrinh equals dlt.Id into dltJoin
                              from dlt in dltJoin.DefaultIfEmpty()

                              join lck in context.CT_DM_LoaiCauKiens on p1.Id_LoaiCauKien equals lck.Id into ttvtJoin
                              from lck in ttvtJoin.DefaultIfEmpty()
                              join lckot in context.CT_DM_LoaiCauKiens on p1.Id_LoaiCauKienOngThep equals lckot.Id into lckotJoin
                              from lckot in lckotJoin.DefaultIfEmpty()

                              join hmkl in context.CT_DM_HangMucKhoiLuongs on p1.Id_HangMucKhoiLuong equals hmkl.Id into hmklJoin
                              from hmkl in hmklJoin.DefaultIfEmpty()
                              join hmklot in context.CT_DM_HangMucKhoiLuongs on p1.Id_HangMucKhoiLuongOngThep equals hmklot.Id into hmklotJoin
                              from hmklot in hmklotJoin.DefaultIfEmpty()

                              join lkl in context.CT_DM_LoaiKhoiLuongs on p1.Id_LoaiKhoiLuong equals lkl.Id into lklJoin
                              from lkl in lklJoin.DefaultIfEmpty()
                              join lklot in context.CT_DM_LoaiKhoiLuongs on p1.Id_LoaiKhoiLuongOngThep equals lklot.Id into lklotJoin
                              from lklot in lklotJoin.DefaultIfEmpty()

                              join htdt in context.CT_DM_HinhThucDapTras on p1.Id_HinhThucDapTra equals htdt.Id into htdtJoin
                              from htdt in htdtJoin.DefaultIfEmpty()

                              join tttcdd in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongDaoDat equals tttcdd.Id into tttcddJoin
                              from tttcdd in tttcddJoin.DefaultIfEmpty()
                              join tttcdc in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongDapCat equals tttcdc.Id into tttcdcJoin
                              from tttcdc in tttcdcJoin.DefaultIfEmpty()
                              join tttcdt in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongDatThua equals tttcdt.Id into tttcdtJoin
                              from tttcdt in tttcdtJoin.DefaultIfEmpty()
                              join tttcdapd in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongDapDat equals tttcdapd.Id into tttcdapdJoin
                              from tttcdapd in tttcdapdJoin.DefaultIfEmpty()
                              join tttcon in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongOngNhua equals tttcon.Id into tttconJoin
                              from tttcon in tttconJoin.DefaultIfEmpty()
                              join tttcot in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongOngThep equals tttcot.Id into tttcotJoin
                              from tttcot in tttcotJoin.DefaultIfEmpty()

                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                              from approvalDept in c1.DefaultIfEmpty()
                              where p1.Id == id
                              select new PKKL_OngNhua_1TtChungNSachDocModel
                              {
                                  Id = p1.Id,
                                  Id_ChiNhanh =p1.Id_ChiNhanh,
                                  TenChiNhanh =cn.TenChiNhanh,

                                  Id_TuyenDuong = p1.Id_TuyenDuong,
                                  TuyenDuong = td.TenDanhMuc,
                                  Id_TuLyTrinh = p1.Id_TuLyTrinh,
                                  TuLyTrinh = tlt.TenDanhMuc,
                                  Id_DenLyTrinh = p1.Id_DenLyTrinh,
                                  DenLyTrinh = dlt.TenDanhMuc,

                                  Id_HangMucCongViec = p1.Id_HangMucCongViec,
                                  HangMucCongViec = hmcv.TenDanhMuc,
                                  Id_LoaiCauKien = p1.Id_LoaiCauKien,
                                  LoaiCauKien = lck.TenDanhMuc,
                                  Id_HangMucKhoiLuong = p1.Id_HangMucKhoiLuong,
                                  HangMucKhoiLuong = hmkl.TenDanhMuc,
                                  Id_LoaiKhoiLuong = p1.Id_LoaiKhoiLuong,
                                  LoaiKhoiLuong = lkl.TenDanhMuc,

                                  DuongKinhNgoaiOngNhua = p1.DuongKinhNgoaiOngNhua,
                                  CDayOngOngNhua = p1.CDayOngOngNhua,
                                  ChieuDaiOngNhua = p1.ChieuDaiOngNhua,
                                  Id_TrangThaiThiCongOngNhua = p1.Id_TrangThaiThiCongOngNhua,
                                  Id_LoaiCauKienOngThep = p1.Id_LoaiCauKienOngThep,
                                  Id_HangMucKhoiLuongOngThep = p1.Id_HangMucKhoiLuongOngThep,
                                  Id_LoaiKhoiLuongOngThep = p1.Id_LoaiKhoiLuongOngThep,

                                  DuongKinhNgoaiMOngThep = p1.DuongKinhNgoaiMOngThep,
                                  CDayOngMOngThep = p1.CDayOngMOngThep,
                                  ChieuDaiMOngThep = p1.ChieuDaiMOngThep,
                                  Id_TrangThaiThiCongOngThep = p1.Id_TrangThaiThiCongOngThep,

                                  Id_HinhThucDapTra = p1.Id_HinhThucDapTra,
                                  HinhThucDapTra = htdt.TenDanhMuc,

                                  HTrangTruocKhiDaoThuongLuu = p1.HTrangTruocKhiDaoThuongLuu,
                                  DayDaoThuongLuu = p1.DayDaoThuongLuu,
                                  ChieuSauDaoThuongLuu = p1.ChieuSauDaoThuongLuu,
                                  DongChayThuongLuu = p1.DongChayThuongLuu,
                                  DinhDuongOngThuongLuu = p1.DinhDuongOngThuongLuu,
                                  CDoDayDemCatThuongLuu = p1.CDoDayDemCatThuongLuu,
                                  CDayDemCatThuongLuu = p1.CDayDemCatThuongLuu,
                                  DinhDemCatThuongLuu = p1.DinhDemCatThuongLuu,
                                  DayDapCatThuongLuu = p1.DayDapCatThuongLuu,
                                  CDayDapCatThuongLuu = p1.CDayDapCatThuongLuu,
                                  DinhDapCatThuongLuu = p1.DinhDapCatThuongLuu,
                                  TongChieuDayDemDapCatThuongLuu = p1.TongChieuDayDemDapCatThuongLuu,
                                  CDoDayDapDatThuongLuu = p1.CDoDayDapDatThuongLuu,
                                  ChieuDayDapDatThuongLuu = p1.ChieuDayDapDatThuongLuu,
                                  DinhDapDatThuongLuu = p1.DinhDapDatThuongLuu,
                                  DapDatCatThuongLuu = p1.DapDatCatThuongLuu,
                                  ChenhDapSoVoiDaoThuongLuu = p1.ChenhDapSoVoiDaoThuongLuu,
                                  HTrangTruocKhiDaoHaLuu = p1.HTrangTruocKhiDaoHaLuu,
                                  DayDaoHaLuu = p1.DayDaoHaLuu,
                                  ChieuSauDaoHaLuu = p1.ChieuSauDaoHaLuu,
                                  DongChayHaLuu = p1.DongChayHaLuu,
                                  DinhDuongOngHaLuu = p1.DinhDuongOngHaLuu,
                                  CDoDayDemCatHaLuu = p1.CDoDayDemCatHaLuu,
                                  CDayDemCatHaLuu = p1.CDayDemCatHaLuu,
                                  DinhDemCatHaLuu = p1.DinhDemCatHaLuu,
                                  DayDapCatHaLuu = p1.DayDapCatHaLuu,
                                  CDayDapCatHaLuu = p1.CDayDapCatHaLuu,
                                  DinhDapCatHaLuu = p1.DinhDapCatHaLuu,
                                  TongChieuDayDemDapCatHaLuu = p1.TongChieuDayDemDapCatHaLuu,
                                  CDoDayDapDatHaLuu = p1.CDoDayDapDatHaLuu,
                                  ChieuDayDapDatHaLuu = p1.ChieuDayDapDatHaLuu,
                                  DinhDapDatHaLuu = p1.DinhDapDatHaLuu,
                                  DapDatCatHaLuu = p1.DapDatCatHaLuu,
                                  ChenhDapSoVoiDaoHaLuu = p1.ChenhDapSoVoiDaoHaLuu,
                                  CDoDayDemCat = p1.CDoDayDemCat,
                                  CDoDinhDemCat = p1.CDoDinhDemCat,
                                  ChieuDayDemCat = p1.ChieuDayDemCat,
                                  CDoDayDapCat = p1.CDoDayDapCat,
                                  CDoDinhDapCat = p1.CDoDinhDapCat,
                                  ChieuDayDapCat = p1.ChieuDayDapCat,
                                  CDoDayDapDat = p1.CDoDayDapDat,
                                  CDoDinhDapDat = p1.CDoDinhDapDat,
                                  ChieuDayDapDat = p1.ChieuDayDapDat,
                                  CRongDayNhoHLuu = p1.CRongDayNhoHLuu,
                                  CRongDayNhoTLuu = p1.CRongDayNhoTLuu,
                                  CRongDayNhoTBinh = p1.CRongDayNhoTBinh,
                                  ChieuSauDaoTrungBinh = p1.ChieuSauDaoTrungBinh,
                                  CRongDayLonTrungBinh = p1.CRongDayLonTrungBinh,
                                  TyLeMoMai = p1.TyLeMoMai,
                                  SoMaiTrai = p1.SoMaiTrai,
                                  SoMaiPhai = p1.SoMaiPhai,
                                  HangMucKlDaoDat = p1.HangMucKlDaoDat,
                                  LoaiKlDaoDat = p1.LoaiKlDaoDat,
                                  DienTich = p1.DienTich,
                                  KlDao = p1.KlDao,

                                  Id_TrangThaiThiCongDaoDat = p1.Id_TrangThaiThiCongDaoDat,
                                  TrangThaiThiCongDaoDat = tttcdd.TenDanhMuc,

                                  CRongDayNhoDemCat = p1.CRongDayNhoDemCat,
                                  CRongDayLonDemCat = p1.CRongDayLonDemCat,
                                  DienTichDapCat1 = p1.DienTichDapCat1,
                                  KlDemCat = p1.KlDemCat,
                                  CRongDayNhoDapCat = p1.CRongDayNhoDapCat,
                                  CRongDayLonDapCat = p1.CRongDayLonDapCat,
                                  DienTichDapCat2 = p1.DienTichDapCat2,
                                  KlDapCat = p1.KlDapCat,
                                  HangMucKlDapCat = p1.HangMucKlDapCat,
                                  LoaiKlDapCat = p1.LoaiKlDapCat,
                                  KLDapCat_KlOngCCho = p1.KLDapCat_KlOngCCho,
                                  KlDapCatSauCCho = p1.KlDapCatSauCCho,

                                  Id_TrangThaiThiCongDapCat = p1.Id_TrangThaiThiCongDapCat,
                                  TrangThaiThiCongDapCat = tttcdc.TenDanhMuc,

                                  CRongDayNhoDapDat = p1.CRongDayNhoDapDat,
                                  CRongDayLonDapDat = p1.CRongDayLonDapDat,
                                  DienTichDapDat = p1.DienTichDapDat,
                                  KlDapDat = p1.KlDapDat,
                                  HangMucKlDapDat = p1.HangMucKlDapDat,
                                  LoaiKlDapDat = p1.LoaiKlDapDat,
                                  KLDapDat_KlOngCCho = p1.KLDapDat_KlOngCCho,
                                  KlDapDatSauCCho = p1.KlDapDatSauCCho,

                                  Id_TrangThaiThiCongDapDat = p1.Id_TrangThaiThiCongDapDat,

                                  HangMucKlDatThua = p1.HangMucKlDatThua,
                                  LoaiKlDatThua = p1.LoaiKlDatThua,
                                  KlDatThua = p1.KlDatThua,

                                  Id_TrangThaiThiCongDatThua = p1.Id_TrangThaiThiCongDatThua,
                                  TrangThaiThiCongDatThua = tttcdt.TenDanhMuc,

                                  X1 = p1.X1,
                                  Y1 = p1.Y1,
                                  X2 = p1.X2,
                                  Y2 = p1.Y2,

                                  GroupId = p1.GroupId,
                                  Ordinarily = p1.Ordinarily,
                                  CreateAt = (DateTime)p1.CreateAt,
                                  CreateBy = createBy.Email,
                                  IsActive = p1.IsActive,
                                  ApprovalUserId = p1.ApprovalUserId,
                                  DateApproval = (DateTime)p1.DateApproval,
                                  ApprovalDept = p1.ApprovalDept,
                                  DepartmentId = p1.DepartmentId,
                                  DepartmentOrder = p1.DepartmentOrder,
                                  ApprovalOrder = p1.ApprovalOrder,
                                  ApprovalId = p1.ApprovalId,
                                  LastApprovalId = p1.LastApprovalId,
                                  IsStatus = p1.IsStatus
                              }).FirstOrDefaultAsync();
            return data;
        }
        public async Task<List<PKKL_OngNhua_1TtChungNSachDocModel>> GetHistory(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var data = await (from p1 in context.PKKL_OngNhua_1TtChungNSachDoc_Logs
                                  join cn in context.ChiNhanhs on p1.Id_ChiNhanh equals cn.Id into cnJoin
                                  from cn in cnJoin.DefaultIfEmpty()

                                  join hmcv in context.CT_DM_HangMucCongViecs on p1.Id_HangMucCongViec equals hmcv.Id into hmcvJoin
                                  from hmcv in hmcvJoin.DefaultIfEmpty()

                                  join td in context.CT_DM_TuyenDuongs on p1.Id_TuyenDuong equals td.Id into tdJoin
                                  from td in tdJoin.DefaultIfEmpty()
                                  join tlt in context.CT_DM_LyTrinhs on p1.Id_TuLyTrinh equals tlt.Id into tltJoin
                                  from tlt in tltJoin.DefaultIfEmpty()
                                  join dlt in context.CT_DM_LyTrinhs on p1.Id_DenLyTrinh equals dlt.Id into dltJoin
                                  from dlt in dltJoin.DefaultIfEmpty()

                                  join lck in context.CT_DM_LoaiCauKiens on p1.Id_LoaiCauKien equals lck.Id into ttvtJoin
                                  from lck in ttvtJoin.DefaultIfEmpty()
                                  join lckot in context.CT_DM_LoaiCauKiens on p1.Id_LoaiCauKienOngThep equals lckot.Id into lckotJoin
                                  from lckot in lckotJoin.DefaultIfEmpty()

                                  join hmkl in context.CT_DM_HangMucKhoiLuongs on p1.Id_HangMucKhoiLuong equals hmkl.Id into hmklJoin
                                  from hmkl in hmklJoin.DefaultIfEmpty()
                                  join hmklot in context.CT_DM_HangMucKhoiLuongs on p1.Id_HangMucKhoiLuongOngThep equals hmklot.Id into hmklotJoin
                                  from hmklot in hmklotJoin.DefaultIfEmpty()

                                  join lkl in context.CT_DM_LoaiKhoiLuongs on p1.Id_LoaiKhoiLuong equals lkl.Id into lklJoin
                                  from lkl in lklJoin.DefaultIfEmpty()
                                  join lklot in context.CT_DM_LoaiKhoiLuongs on p1.Id_LoaiKhoiLuongOngThep equals lklot.Id into lklotJoin
                                  from lklot in lklotJoin.DefaultIfEmpty()

                                  join htdt in context.CT_DM_HinhThucDapTras on p1.Id_HinhThucDapTra equals htdt.Id into htdtJoin
                                  from htdt in htdtJoin.DefaultIfEmpty()

                                  join tttcdd in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongDaoDat equals tttcdd.Id into tttcddJoin
                                  from tttcdd in tttcddJoin.DefaultIfEmpty()
                                  join tttcdc in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongDapCat equals tttcdc.Id into tttcdcJoin
                                  from tttcdc in tttcdcJoin.DefaultIfEmpty()
                                  join tttcdt in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongDatThua equals tttcdt.Id into tttcdtJoin
                                  from tttcdt in tttcdtJoin.DefaultIfEmpty()
                                  join tttcdapd in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongDapDat equals tttcdapd.Id into tttcdapdJoin
                                  from tttcdapd in tttcdapdJoin.DefaultIfEmpty()
                                  join tttcon in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongOngNhua equals tttcon.Id into tttconJoin
                                  from tttcon in tttconJoin.DefaultIfEmpty()
                                  join tttcot in context.CT_DM_TrangThaiThiCongs on p1.Id_TrangThaiThiCongOngThep equals tttcot.Id into tttcotJoin
                                  from tttcot in tttcotJoin.DefaultIfEmpty()

                                  join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                                  join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                                  from approvalUserId in a1.DefaultIfEmpty()
                                  join b in context.Departments on p1.DepartmentId equals b.Id into b1
                                  from departmentId in b1.DefaultIfEmpty()
                                  join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                                  from approvalDept in c1.DefaultIfEmpty()
                                  where p1.IdChung == id
                                  orderby p1.CreateAt
                                  select new PKKL_OngNhua_1TtChungNSachDocModel
                                  {
                                      Id = p1.Id,
                                      Id_ChiNhanh = p1.Id_ChiNhanh,
                                      TenChiNhanh = cn.TenChiNhanh,

                                      Id_TuyenDuong = p1.Id_TuyenDuong,
                                      TuyenDuong = td.TenDanhMuc,
                                      Id_TuLyTrinh = p1.Id_TuLyTrinh,
                                      TuLyTrinh = tlt.TenDanhMuc,
                                      Id_DenLyTrinh = p1.Id_DenLyTrinh,
                                      DenLyTrinh = dlt.TenDanhMuc,

                                      Id_HangMucCongViec = p1.Id_HangMucCongViec,
                                      HangMucCongViec = hmcv.TenDanhMuc,
                                      Id_LoaiCauKien = p1.Id_LoaiCauKien,
                                      LoaiCauKien = lck.TenDanhMuc,
                                      Id_HangMucKhoiLuong = p1.Id_HangMucKhoiLuong,
                                      HangMucKhoiLuong = hmkl.TenDanhMuc,
                                      Id_LoaiKhoiLuong = p1.Id_LoaiKhoiLuong,
                                      LoaiKhoiLuong = lkl.TenDanhMuc,

                                      DuongKinhNgoaiOngNhua = p1.DuongKinhNgoaiOngNhua,
                                      CDayOngOngNhua = p1.CDayOngOngNhua,
                                      ChieuDaiOngNhua = p1.ChieuDaiOngNhua,
                                      Id_TrangThaiThiCongOngNhua = p1.Id_TrangThaiThiCongOngNhua,
                                      Id_LoaiCauKienOngThep = p1.Id_LoaiCauKienOngThep,
                                      Id_HangMucKhoiLuongOngThep = p1.Id_HangMucKhoiLuongOngThep,
                                      Id_LoaiKhoiLuongOngThep = p1.Id_LoaiKhoiLuongOngThep,

                                      DuongKinhNgoaiMOngThep = p1.DuongKinhNgoaiMOngThep,
                                      CDayOngMOngThep = p1.CDayOngMOngThep,
                                      ChieuDaiMOngThep = p1.ChieuDaiMOngThep,
                                      Id_TrangThaiThiCongOngThep = p1.Id_TrangThaiThiCongOngThep,

                                      Id_HinhThucDapTra = p1.Id_HinhThucDapTra,
                                      HinhThucDapTra = htdt.TenDanhMuc,

                                      HTrangTruocKhiDaoThuongLuu = p1.HTrangTruocKhiDaoThuongLuu,
                                      DayDaoThuongLuu = p1.DayDaoThuongLuu,
                                      ChieuSauDaoThuongLuu = p1.ChieuSauDaoThuongLuu,
                                      DongChayThuongLuu = p1.DongChayThuongLuu,
                                      DinhDuongOngThuongLuu = p1.DinhDuongOngThuongLuu,
                                      CDoDayDemCatThuongLuu = p1.CDoDayDemCatThuongLuu,
                                      CDayDemCatThuongLuu = p1.CDayDemCatThuongLuu,
                                      DinhDemCatThuongLuu = p1.DinhDemCatThuongLuu,
                                      DayDapCatThuongLuu = p1.DayDapCatThuongLuu,
                                      CDayDapCatThuongLuu = p1.CDayDapCatThuongLuu,
                                      DinhDapCatThuongLuu = p1.DinhDapCatThuongLuu,
                                      TongChieuDayDemDapCatThuongLuu = p1.TongChieuDayDemDapCatThuongLuu,
                                      CDoDayDapDatThuongLuu = p1.CDoDayDapDatThuongLuu,
                                      ChieuDayDapDatThuongLuu = p1.ChieuDayDapDatThuongLuu,
                                      DinhDapDatThuongLuu = p1.DinhDapDatThuongLuu,
                                      DapDatCatThuongLuu = p1.DapDatCatThuongLuu,
                                      ChenhDapSoVoiDaoThuongLuu = p1.ChenhDapSoVoiDaoThuongLuu,
                                      HTrangTruocKhiDaoHaLuu = p1.HTrangTruocKhiDaoHaLuu,
                                      DayDaoHaLuu = p1.DayDaoHaLuu,
                                      ChieuSauDaoHaLuu = p1.ChieuSauDaoHaLuu,
                                      DongChayHaLuu = p1.DongChayHaLuu,
                                      DinhDuongOngHaLuu = p1.DinhDuongOngHaLuu,
                                      CDoDayDemCatHaLuu = p1.CDoDayDemCatHaLuu,
                                      CDayDemCatHaLuu = p1.CDayDemCatHaLuu,
                                      DinhDemCatHaLuu = p1.DinhDemCatHaLuu,
                                      DayDapCatHaLuu = p1.DayDapCatHaLuu,
                                      CDayDapCatHaLuu = p1.CDayDapCatHaLuu,
                                      DinhDapCatHaLuu = p1.DinhDapCatHaLuu,
                                      TongChieuDayDemDapCatHaLuu = p1.TongChieuDayDemDapCatHaLuu,
                                      CDoDayDapDatHaLuu = p1.CDoDayDapDatHaLuu,
                                      ChieuDayDapDatHaLuu = p1.ChieuDayDapDatHaLuu,
                                      DinhDapDatHaLuu = p1.DinhDapDatHaLuu,
                                      DapDatCatHaLuu = p1.DapDatCatHaLuu,
                                      ChenhDapSoVoiDaoHaLuu = p1.ChenhDapSoVoiDaoHaLuu,
                                      CDoDayDemCat = p1.CDoDayDemCat,
                                      CDoDinhDemCat = p1.CDoDinhDemCat,
                                      ChieuDayDemCat = p1.ChieuDayDemCat,
                                      CDoDayDapCat = p1.CDoDayDapCat,
                                      CDoDinhDapCat = p1.CDoDinhDapCat,
                                      ChieuDayDapCat = p1.ChieuDayDapCat,
                                      CDoDayDapDat = p1.CDoDayDapDat,
                                      CDoDinhDapDat = p1.CDoDinhDapDat,
                                      ChieuDayDapDat = p1.ChieuDayDapDat,
                                      CRongDayNhoHLuu = p1.CRongDayNhoHLuu,
                                      CRongDayNhoTLuu = p1.CRongDayNhoTLuu,
                                      CRongDayNhoTBinh = p1.CRongDayNhoTBinh,
                                      ChieuSauDaoTrungBinh = p1.ChieuSauDaoTrungBinh,
                                      CRongDayLonTrungBinh = p1.CRongDayLonTrungBinh,
                                      TyLeMoMai = p1.TyLeMoMai,
                                      SoMaiTrai = p1.SoMaiTrai,
                                      SoMaiPhai = p1.SoMaiPhai,
                                      HangMucKlDaoDat = p1.HangMucKlDaoDat,
                                      LoaiKlDaoDat = p1.LoaiKlDaoDat,
                                      DienTich = p1.DienTich,
                                      KlDao = p1.KlDao,

                                      Id_TrangThaiThiCongDaoDat = p1.Id_TrangThaiThiCongDaoDat,
                                      TrangThaiThiCongDaoDat = tttcdd.TenDanhMuc,

                                      CRongDayNhoDemCat = p1.CRongDayNhoDemCat,
                                      CRongDayLonDemCat = p1.CRongDayLonDemCat,
                                      DienTichDapCat1 = p1.DienTichDapCat1,
                                      KlDemCat = p1.KlDemCat,
                                      CRongDayNhoDapCat = p1.CRongDayNhoDapCat,
                                      CRongDayLonDapCat = p1.CRongDayLonDapCat,
                                      DienTichDapCat2 = p1.DienTichDapCat2,
                                      KlDapCat = p1.KlDapCat,
                                      HangMucKlDapCat = p1.HangMucKlDapCat,
                                      LoaiKlDapCat = p1.LoaiKlDapCat,
                                      KLDapCat_KlOngCCho = p1.KLDapCat_KlOngCCho,
                                      KlDapCatSauCCho = p1.KlDapCatSauCCho,

                                      Id_TrangThaiThiCongDapCat = p1.Id_TrangThaiThiCongDapCat,
                                      TrangThaiThiCongDapCat = tttcdc.TenDanhMuc,

                                      CRongDayNhoDapDat = p1.CRongDayNhoDapDat,
                                      CRongDayLonDapDat = p1.CRongDayLonDapDat,
                                      DienTichDapDat = p1.DienTichDapDat,
                                      KlDapDat = p1.KlDapDat,
                                      HangMucKlDapDat = p1.HangMucKlDapDat,
                                      LoaiKlDapDat = p1.LoaiKlDapDat,
                                      KLDapDat_KlOngCCho = p1.KLDapDat_KlOngCCho,
                                      KlDapDatSauCCho = p1.KlDapDatSauCCho,

                                      Id_TrangThaiThiCongDapDat = p1.Id_TrangThaiThiCongDapDat,

                                      HangMucKlDatThua = p1.HangMucKlDatThua,
                                      LoaiKlDatThua = p1.LoaiKlDatThua,
                                      KlDatThua = p1.KlDatThua,

                                      Id_TrangThaiThiCongDatThua = p1.Id_TrangThaiThiCongDatThua,
                                      TrangThaiThiCongDatThua = tttcdt.TenDanhMuc,

                                      X1 = p1.X1,
                                      Y1 = p1.Y1,
                                      X2 = p1.X2,
                                      Y2 = p1.Y2,

                                      GroupId = p1.GroupId,
                                      Ordinarily = p1.Ordinarily,
                                      CreateAt = (DateTime)p1.CreateAt,
                                      CreateBy = createBy.Email,
                                      IsActive = p1.IsActive,
                                      ApprovalUserId = p1.ApprovalUserId,
                                      DateApproval = (DateTime)p1.DateApproval,
                                      ApprovalDept = p1.ApprovalDept,
                                      DepartmentId = p1.DepartmentId,
                                      DepartmentOrder = p1.DepartmentOrder,
                                      ApprovalOrder = p1.ApprovalOrder,
                                      ApprovalId = p1.ApprovalId,
                                      LastApprovalId = p1.LastApprovalId,
                                      IsStatus = p1.IsStatus
                                  }).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<PKKL_OngNhua_1TtChungNSachDoc>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.PKKL_OngNhua_1TtChungNSachDocs.Where(p => p.IsActive != 100).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<PKKL_OngNhua_1TtChungNSachDoc> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.PKKL_OngNhua_1TtChungNSachDocs.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy thông tin đã chọn.");
            }
            return entity;
        }
        public async Task Insert(PKKL_OngNhua_1TtChungNSachDoc entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có thông tin nào được thêm!");
                }
                context.PKKL_OngNhua_1TtChungNSachDocs.Add(entity);
                var addLog = new PKKL_OngNhua_1TtChungNSachDoc_Log()
                {
                    Id = entity.Id,
                    Id_ChiNhanh=entity.Id_ChiNhanh,

                    Id_TuyenDuong = entity.Id_TuyenDuong,
                    Id_TuLyTrinh = entity.Id_TuLyTrinh,
                    Id_DenLyTrinh = entity.Id_DenLyTrinh,

                    Id_HangMucCongViec = entity.Id_HangMucCongViec,
                    Id_LoaiCauKien = entity.Id_LoaiCauKien,
                    Id_HangMucKhoiLuong = entity.Id_HangMucKhoiLuong,
                    Id_LoaiKhoiLuong = entity.Id_LoaiKhoiLuong,

                    DuongKinhNgoaiOngNhua = entity.DuongKinhNgoaiOngNhua,
                    CDayOngOngNhua = entity.CDayOngOngNhua,
                    ChieuDaiOngNhua = entity.ChieuDaiOngNhua,
                    Id_TrangThaiThiCongOngNhua = entity.Id_TrangThaiThiCongOngNhua,
                    Id_LoaiCauKienOngThep = entity.Id_LoaiCauKienOngThep,
                    Id_HangMucKhoiLuongOngThep = entity.Id_HangMucKhoiLuongOngThep,
                    Id_LoaiKhoiLuongOngThep = entity.Id_LoaiKhoiLuongOngThep,

                    DuongKinhNgoaiMOngThep = entity.DuongKinhNgoaiMOngThep,
                    CDayOngMOngThep = entity.CDayOngMOngThep,
                    ChieuDaiMOngThep = entity.ChieuDaiMOngThep,
                    Id_TrangThaiThiCongOngThep = entity.Id_TrangThaiThiCongOngThep,

                    Id_HinhThucDapTra = entity.Id_HinhThucDapTra,

                    HTrangTruocKhiDaoThuongLuu = entity.HTrangTruocKhiDaoThuongLuu,
                    DayDaoThuongLuu = entity.DayDaoThuongLuu,
                    ChieuSauDaoThuongLuu = entity.ChieuSauDaoThuongLuu,
                    DongChayThuongLuu = entity.DongChayThuongLuu,
                    DinhDuongOngThuongLuu = entity.DinhDuongOngThuongLuu,
                    CDoDayDemCatThuongLuu = entity.CDoDayDemCatThuongLuu,
                    CDayDemCatThuongLuu = entity.CDayDemCatThuongLuu,
                    DinhDemCatThuongLuu = entity.DinhDemCatThuongLuu,
                    DayDapCatThuongLuu = entity.DayDapCatThuongLuu,
                    CDayDapCatThuongLuu = entity.CDayDapCatThuongLuu,
                    DinhDapCatThuongLuu = entity.DinhDapCatThuongLuu,
                    TongChieuDayDemDapCatThuongLuu = entity.TongChieuDayDemDapCatThuongLuu,
                    CDoDayDapDatThuongLuu = entity.CDoDayDapDatThuongLuu,
                    ChieuDayDapDatThuongLuu = entity.ChieuDayDapDatThuongLuu,
                    DinhDapDatThuongLuu = entity.DinhDapDatThuongLuu,
                    DapDatCatThuongLuu = entity.DapDatCatThuongLuu,
                    ChenhDapSoVoiDaoThuongLuu = entity.ChenhDapSoVoiDaoThuongLuu,
                    HTrangTruocKhiDaoHaLuu = entity.HTrangTruocKhiDaoHaLuu,
                    DayDaoHaLuu = entity.DayDaoHaLuu,
                    ChieuSauDaoHaLuu = entity.ChieuSauDaoHaLuu,
                    DongChayHaLuu = entity.DongChayHaLuu,
                    DinhDuongOngHaLuu = entity.DinhDuongOngHaLuu,
                    CDoDayDemCatHaLuu = entity.CDoDayDemCatHaLuu,
                    CDayDemCatHaLuu = entity.CDayDemCatHaLuu,
                    DinhDemCatHaLuu = entity.DinhDemCatHaLuu,
                    DayDapCatHaLuu = entity.DayDapCatHaLuu,
                    CDayDapCatHaLuu = entity.CDayDapCatHaLuu,
                    DinhDapCatHaLuu = entity.DinhDapCatHaLuu,
                    TongChieuDayDemDapCatHaLuu = entity.TongChieuDayDemDapCatHaLuu,
                    CDoDayDapDatHaLuu = entity.CDoDayDapDatHaLuu,
                    ChieuDayDapDatHaLuu = entity.ChieuDayDapDatHaLuu,
                    DinhDapDatHaLuu = entity.DinhDapDatHaLuu,
                    DapDatCatHaLuu = entity.DapDatCatHaLuu,
                    ChenhDapSoVoiDaoHaLuu = entity.ChenhDapSoVoiDaoHaLuu,
                    CDoDayDemCat = entity.CDoDayDemCat,
                    CDoDinhDemCat = entity.CDoDinhDemCat,
                    ChieuDayDemCat = entity.ChieuDayDemCat,
                    CDoDayDapCat = entity.CDoDayDapCat,
                    CDoDinhDapCat = entity.CDoDinhDapCat,
                    ChieuDayDapCat = entity.ChieuDayDapCat,
                    CDoDayDapDat = entity.CDoDayDapDat,
                    CDoDinhDapDat = entity.CDoDinhDapDat,
                    ChieuDayDapDat = entity.ChieuDayDapDat,
                    CRongDayNhoHLuu = entity.CRongDayNhoHLuu,
                    CRongDayNhoTLuu = entity.CRongDayNhoTLuu,
                    CRongDayNhoTBinh = entity.CRongDayNhoTBinh,
                    ChieuSauDaoTrungBinh = entity.ChieuSauDaoTrungBinh,
                    CRongDayLonTrungBinh = entity.CRongDayLonTrungBinh,
                    TyLeMoMai = entity.TyLeMoMai,
                    SoMaiTrai = entity.SoMaiTrai,
                    SoMaiPhai = entity.SoMaiPhai,
                    HangMucKlDaoDat = entity.HangMucKlDaoDat,
                    LoaiKlDaoDat = entity.LoaiKlDaoDat,
                    DienTich = entity.DienTich,
                    KlDao = entity.KlDao,

                    Id_TrangThaiThiCongDaoDat = entity.Id_TrangThaiThiCongDaoDat,

                    CRongDayNhoDemCat = entity.CRongDayNhoDemCat,
                    CRongDayLonDemCat = entity.CRongDayLonDemCat,
                    DienTichDapCat1 = entity.DienTichDapCat1,
                    KlDemCat = entity.KlDemCat,
                    CRongDayNhoDapCat = entity.CRongDayNhoDapCat,
                    CRongDayLonDapCat = entity.CRongDayLonDapCat,
                    DienTichDapCat2 = entity.DienTichDapCat2,
                    KlDapCat = entity.KlDapCat,
                    HangMucKlDapCat = entity.HangMucKlDapCat,
                    LoaiKlDapCat = entity.LoaiKlDapCat,
                    KLDapCat_KlOngCCho = entity.KLDapCat_KlOngCCho,
                    KlDapCatSauCCho = entity.KlDapCatSauCCho,

                    Id_TrangThaiThiCongDapCat = entity.Id_TrangThaiThiCongDapCat,

                    CRongDayNhoDapDat = entity.CRongDayNhoDapDat,
                    CRongDayLonDapDat = entity.CRongDayLonDapDat,
                    DienTichDapDat = entity.DienTichDapDat,
                    KlDapDat = entity.KlDapDat,
                    HangMucKlDapDat = entity.HangMucKlDapDat,
                    LoaiKlDapDat = entity.LoaiKlDapDat,
                    KLDapDat_KlOngCCho = entity.KLDapDat_KlOngCCho,
                    KlDapDatSauCCho = entity.KlDapDatSauCCho,

                    Id_TrangThaiThiCongDapDat = entity.Id_TrangThaiThiCongDapDat,

                    HangMucKlDatThua = entity.HangMucKlDatThua,
                    LoaiKlDatThua = entity.LoaiKlDatThua,
                    KlDatThua = entity.KlDatThua,

                    Id_TrangThaiThiCongDatThua = entity.Id_TrangThaiThiCongDatThua,
                   
                    X1 = entity.X1,
                    Y1 = entity.Y1,
                    X2 = entity.X2,
                    Y2 = entity.Y2,

                    GroupId = entity.GroupId,
                    Ordinarily = entity.Ordinarily,
                    CreateAt = DateTime.Now,
                    CreateBy = entity.CreateBy,
                    IsActive = entity.IsActive,
                    ApprovalUserId = entity.ApprovalUserId,
                    DateApproval = (DateTime?)entity.DateApproval,
                    ApprovalDept = entity.ApprovalDept,
                    DepartmentId = entity.DepartmentId,
                    DepartmentOrder = entity.DepartmentOrder,
                    ApprovalOrder = entity.ApprovalOrder,
                    ApprovalId = entity.ApprovalId,
                    LastApprovalId = entity.LastApprovalId,
                    IsStatus = entity.IsStatus,
                    IdChung = entity.Id,

                };
                context.PKKL_OngNhua_1TtChungNSachDoc_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(PKKL_OngNhua_1TtChungNSachDoc data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy thông tin đã chọn");
            }
            context.PKKL_OngNhua_1TtChungNSachDocs.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.PKKL_OngNhua_1TtChungNSachDoc_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.PKKL_OngNhua_1TtChungNSachDoc_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.PKKL_OngNhua_1TtChungNSachDoc_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.PKKL_OngNhua_1TtChungNSachDoc_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.PKKL_OngNhua_1TtChungNSachDoc_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt)
                .FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.PKKL_OngNhua_1TtChungNSachDoc_Logs.Update(updateLog);
                }
            }
            var addLog = new PKKL_OngNhua_1TtChungNSachDoc_Log
            {
                Id = Guid.NewGuid().ToString(),
                Id_ChiNhanh = entity.Id_ChiNhanh,

                Id_TuyenDuong = entity.Id_TuyenDuong,
                Id_TuLyTrinh = entity.Id_TuLyTrinh,
                Id_DenLyTrinh = entity.Id_DenLyTrinh,

                Id_HangMucCongViec = entity.Id_HangMucCongViec,
                Id_LoaiCauKien = entity.Id_LoaiCauKien,
                Id_HangMucKhoiLuong = entity.Id_HangMucKhoiLuong,
                Id_LoaiKhoiLuong = entity.Id_LoaiKhoiLuong,

                DuongKinhNgoaiOngNhua = entity.DuongKinhNgoaiOngNhua,
                CDayOngOngNhua = entity.CDayOngOngNhua,
                ChieuDaiOngNhua = entity.ChieuDaiOngNhua,
                Id_TrangThaiThiCongOngNhua = entity.Id_TrangThaiThiCongOngNhua,
                Id_LoaiCauKienOngThep = entity.Id_LoaiCauKienOngThep,
                Id_HangMucKhoiLuongOngThep = entity.Id_HangMucKhoiLuongOngThep,
                Id_LoaiKhoiLuongOngThep = entity.Id_LoaiKhoiLuongOngThep,

                DuongKinhNgoaiMOngThep = entity.DuongKinhNgoaiMOngThep,
                CDayOngMOngThep = entity.CDayOngMOngThep,
                ChieuDaiMOngThep = entity.ChieuDaiMOngThep,
                Id_TrangThaiThiCongOngThep = entity.Id_TrangThaiThiCongOngThep,

                Id_HinhThucDapTra = entity.Id_HinhThucDapTra,

                HTrangTruocKhiDaoThuongLuu = entity.HTrangTruocKhiDaoThuongLuu,
                DayDaoThuongLuu = entity.DayDaoThuongLuu,
                ChieuSauDaoThuongLuu = entity.ChieuSauDaoThuongLuu,
                DongChayThuongLuu = entity.DongChayThuongLuu,
                DinhDuongOngThuongLuu = entity.DinhDuongOngThuongLuu,
                CDoDayDemCatThuongLuu = entity.CDoDayDemCatThuongLuu,
                CDayDemCatThuongLuu = entity.CDayDemCatThuongLuu,
                DinhDemCatThuongLuu = entity.DinhDemCatThuongLuu,
                DayDapCatThuongLuu = entity.DayDapCatThuongLuu,
                CDayDapCatThuongLuu = entity.CDayDapCatThuongLuu,
                DinhDapCatThuongLuu = entity.DinhDapCatThuongLuu,
                TongChieuDayDemDapCatThuongLuu = entity.TongChieuDayDemDapCatThuongLuu,
                CDoDayDapDatThuongLuu = entity.CDoDayDapDatThuongLuu,
                ChieuDayDapDatThuongLuu = entity.ChieuDayDapDatThuongLuu,
                DinhDapDatThuongLuu = entity.DinhDapDatThuongLuu,
                DapDatCatThuongLuu = entity.DapDatCatThuongLuu,
                ChenhDapSoVoiDaoThuongLuu = entity.ChenhDapSoVoiDaoThuongLuu,
                HTrangTruocKhiDaoHaLuu = entity.HTrangTruocKhiDaoHaLuu,
                DayDaoHaLuu = entity.DayDaoHaLuu,
                ChieuSauDaoHaLuu = entity.ChieuSauDaoHaLuu,
                DongChayHaLuu = entity.DongChayHaLuu,
                DinhDuongOngHaLuu = entity.DinhDuongOngHaLuu,
                CDoDayDemCatHaLuu = entity.CDoDayDemCatHaLuu,
                CDayDemCatHaLuu = entity.CDayDemCatHaLuu,
                DinhDemCatHaLuu = entity.DinhDemCatHaLuu,
                DayDapCatHaLuu = entity.DayDapCatHaLuu,
                CDayDapCatHaLuu = entity.CDayDapCatHaLuu,
                DinhDapCatHaLuu = entity.DinhDapCatHaLuu,
                TongChieuDayDemDapCatHaLuu = entity.TongChieuDayDemDapCatHaLuu,
                CDoDayDapDatHaLuu = entity.CDoDayDapDatHaLuu,
                ChieuDayDapDatHaLuu = entity.ChieuDayDapDatHaLuu,
                DinhDapDatHaLuu = entity.DinhDapDatHaLuu,
                DapDatCatHaLuu = entity.DapDatCatHaLuu,
                ChenhDapSoVoiDaoHaLuu = entity.ChenhDapSoVoiDaoHaLuu,
                CDoDayDemCat = entity.CDoDayDemCat,
                CDoDinhDemCat = entity.CDoDinhDemCat,
                ChieuDayDemCat = entity.ChieuDayDemCat,
                CDoDayDapCat = entity.CDoDayDapCat,
                CDoDinhDapCat = entity.CDoDinhDapCat,
                ChieuDayDapCat = entity.ChieuDayDapCat,
                CDoDayDapDat = entity.CDoDayDapDat,
                CDoDinhDapDat = entity.CDoDinhDapDat,
                ChieuDayDapDat = entity.ChieuDayDapDat,
                CRongDayNhoHLuu = entity.CRongDayNhoHLuu,
                CRongDayNhoTLuu = entity.CRongDayNhoTLuu,
                CRongDayNhoTBinh = entity.CRongDayNhoTBinh,
                ChieuSauDaoTrungBinh = entity.ChieuSauDaoTrungBinh,
                CRongDayLonTrungBinh = entity.CRongDayLonTrungBinh,
                TyLeMoMai = entity.TyLeMoMai,
                SoMaiTrai = entity.SoMaiTrai,
                SoMaiPhai = entity.SoMaiPhai,
                HangMucKlDaoDat = entity.HangMucKlDaoDat,
                LoaiKlDaoDat = entity.LoaiKlDaoDat,
                DienTich = entity.DienTich,
                KlDao = entity.KlDao,

                Id_TrangThaiThiCongDaoDat = entity.Id_TrangThaiThiCongDaoDat,

                CRongDayNhoDemCat = entity.CRongDayNhoDemCat,
                CRongDayLonDemCat = entity.CRongDayLonDemCat,
                DienTichDapCat1 = entity.DienTichDapCat1,
                KlDemCat = entity.KlDemCat,
                CRongDayNhoDapCat = entity.CRongDayNhoDapCat,
                CRongDayLonDapCat = entity.CRongDayLonDapCat,
                DienTichDapCat2 = entity.DienTichDapCat2,
                KlDapCat = entity.KlDapCat,
                HangMucKlDapCat = entity.HangMucKlDapCat,
                LoaiKlDapCat = entity.LoaiKlDapCat,
                KLDapCat_KlOngCCho = entity.KLDapCat_KlOngCCho,
                KlDapCatSauCCho = entity.KlDapCatSauCCho,

                Id_TrangThaiThiCongDapCat = entity.Id_TrangThaiThiCongDapCat,

                CRongDayNhoDapDat = entity.CRongDayNhoDapDat,
                CRongDayLonDapDat = entity.CRongDayLonDapDat,
                DienTichDapDat = entity.DienTichDapDat,
                KlDapDat = entity.KlDapDat,
                HangMucKlDapDat = entity.HangMucKlDapDat,
                LoaiKlDapDat = entity.LoaiKlDapDat,
                KLDapDat_KlOngCCho = entity.KLDapDat_KlOngCCho,
                KlDapDatSauCCho = entity.KlDapDatSauCCho,

                Id_TrangThaiThiCongDapDat = entity.Id_TrangThaiThiCongDapDat,

                HangMucKlDatThua = entity.HangMucKlDatThua,
                LoaiKlDatThua = entity.LoaiKlDatThua,
                KlDatThua = entity.KlDatThua,

                Id_TrangThaiThiCongDatThua = entity.Id_TrangThaiThiCongDatThua,

                X1 = entity.X1,
                Y1 = entity.Y1,
                X2 = entity.X2,
                Y2 = entity.Y2,

                GroupId = data.GroupId,
                Ordinarily = data.Ordinarily,
                CreateAt = DateTime.Now,
                CreateBy = data.CreateBy,
                IsActive = data.IsActive,
                ApprovalUserId = data.ApprovalUserId,
                DateApproval = data.DateApproval,
                ApprovalDept = data.ApprovalDept,
                DepartmentId = data.DepartmentId,
                DepartmentOrder = data.DepartmentOrder,
                ApprovalOrder = data.ApprovalOrder,
                ApprovalId = data.ApprovalId,
                LastApprovalId = data.LastApprovalId,
                IsStatus = data.IsStatus,
                IdChung = data.Id,
                IsValid = data.IsActive == 100 ? false : true
            };
            context.PKKL_OngNhua_1TtChungNSachDoc_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(PKKL_OngNhua_1TtChungNSachDoc[] PKKL_OngNhua_1TtChungNSachDocs)
        {
            using var context = _context.CreateDbContext();
            string[] ids = PKKL_OngNhua_1TtChungNSachDocs.Select(x => x.Id).ToArray();
            var listEntities = await context.PKKL_OngNhua_1TtChungNSachDocs.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.PKKL_OngNhua_1TtChungNSachDocs.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task Delete(PKKL_OngNhua_1TtChungNSachDoc data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy thông tin đã chọn");
            }
            else
            {
                if (entity.IsActive == 0)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa";
                    data.ApprovalUserId = userId;
                }
                else if (entity.IsActive == 1)
                {
                    var logdata = (from p in context.PKKL_OngNhua_1TtChungNSachDoc_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        data.Id_ChiNhanh = logdata.Id_ChiNhanh;

                        data.Id_TuyenDuong = logdata.Id_TuyenDuong;
                        data.Id_TuLyTrinh = logdata.Id_TuLyTrinh;
                        data.Id_DenLyTrinh = logdata.Id_DenLyTrinh;

                        data.Id_HangMucCongViec = logdata.Id_HangMucCongViec;
                        data.Id_LoaiCauKien = logdata.Id_LoaiCauKien;
                        data.Id_HangMucKhoiLuong = logdata.Id_HangMucKhoiLuong;
                        data.Id_LoaiKhoiLuong = logdata.Id_LoaiKhoiLuong;

                        data.DuongKinhNgoaiOngNhua = logdata.DuongKinhNgoaiOngNhua;
                        data.CDayOngOngNhua = logdata.CDayOngOngNhua;
                        data.ChieuDaiOngNhua = logdata.ChieuDaiOngNhua;
                        data.Id_TrangThaiThiCongOngNhua = logdata.Id_TrangThaiThiCongOngNhua;
                        data.Id_LoaiCauKienOngThep = logdata.Id_LoaiCauKienOngThep;
                        data.Id_HangMucKhoiLuongOngThep = logdata.Id_HangMucKhoiLuongOngThep;
                        data.Id_LoaiKhoiLuongOngThep = logdata.Id_LoaiKhoiLuongOngThep;

                        data.DuongKinhNgoaiMOngThep = logdata.DuongKinhNgoaiMOngThep;
                        data.CDayOngMOngThep = logdata.CDayOngMOngThep;
                        data.ChieuDaiMOngThep = logdata.ChieuDaiMOngThep;
                        data.Id_TrangThaiThiCongOngThep = logdata.Id_TrangThaiThiCongOngThep;

                        data.Id_HinhThucDapTra = logdata.Id_HinhThucDapTra;

                        data.HTrangTruocKhiDaoThuongLuu = logdata.HTrangTruocKhiDaoThuongLuu;
                        data.DayDaoThuongLuu = logdata.DayDaoThuongLuu;
                        data.ChieuSauDaoThuongLuu = logdata.ChieuSauDaoThuongLuu;
                        data.DongChayThuongLuu = logdata.DongChayThuongLuu;
                        data.DinhDuongOngThuongLuu = logdata.DinhDuongOngThuongLuu;
                        data.CDoDayDemCatThuongLuu = logdata.CDoDayDemCatThuongLuu;
                        data.CDayDemCatThuongLuu = logdata.CDayDemCatThuongLuu;
                        data.DinhDemCatThuongLuu = logdata.DinhDemCatThuongLuu;
                        data.DayDapCatThuongLuu = logdata.DayDapCatThuongLuu;
                        data.CDayDapCatThuongLuu = logdata.CDayDapCatThuongLuu;
                        data.DinhDapCatThuongLuu = logdata.DinhDapCatThuongLuu;
                        data.TongChieuDayDemDapCatThuongLuu = logdata.TongChieuDayDemDapCatThuongLuu;
                        data.CDoDayDapDatThuongLuu = logdata.CDoDayDapDatThuongLuu;
                        data.ChieuDayDapDatThuongLuu = logdata.ChieuDayDapDatThuongLuu;
                        data.DinhDapDatThuongLuu = logdata.DinhDapDatThuongLuu;
                        data.DapDatCatThuongLuu = logdata.DapDatCatThuongLuu;
                        data.ChenhDapSoVoiDaoThuongLuu = logdata.ChenhDapSoVoiDaoThuongLuu;
                        data.HTrangTruocKhiDaoHaLuu = logdata.HTrangTruocKhiDaoHaLuu;
                        data.DayDaoHaLuu = logdata.DayDaoHaLuu;
                        data.ChieuSauDaoHaLuu = logdata.ChieuSauDaoHaLuu;
                        data.DongChayHaLuu = logdata.DongChayHaLuu;
                        data.DinhDuongOngHaLuu = logdata.DinhDuongOngHaLuu;
                        data.CDoDayDemCatHaLuu = logdata.CDoDayDemCatHaLuu;
                        data.CDayDemCatHaLuu = logdata.CDayDemCatHaLuu;
                        data.DinhDemCatHaLuu = logdata.DinhDemCatHaLuu;
                        data.DayDapCatHaLuu = logdata.DayDapCatHaLuu;
                        data.CDayDapCatHaLuu = logdata.CDayDapCatHaLuu;
                        data.DinhDapCatHaLuu = logdata.DinhDapCatHaLuu;
                        data.TongChieuDayDemDapCatHaLuu = logdata.TongChieuDayDemDapCatHaLuu;
                        data.CDoDayDapDatHaLuu = logdata.CDoDayDapDatHaLuu;
                        data.ChieuDayDapDatHaLuu = logdata.ChieuDayDapDatHaLuu;
                        data.DinhDapDatHaLuu = logdata.DinhDapDatHaLuu;
                        data.DapDatCatHaLuu = logdata.DapDatCatHaLuu;
                        data.ChenhDapSoVoiDaoHaLuu = logdata.ChenhDapSoVoiDaoHaLuu;
                        data.CDoDayDemCat = logdata.CDoDayDemCat;
                        data.CDoDinhDemCat = logdata.CDoDinhDemCat;
                        data.ChieuDayDemCat = logdata.ChieuDayDemCat;
                        data.CDoDayDapCat = logdata.CDoDayDapCat;
                        data.CDoDinhDapCat = logdata.CDoDinhDapCat;
                        data.ChieuDayDapCat = logdata.ChieuDayDapCat;
                        data.CDoDayDapDat = logdata.CDoDayDapDat;
                        data.CDoDinhDapDat = logdata.CDoDinhDapDat;
                        data.ChieuDayDapDat = logdata.ChieuDayDapDat;
                        data.CRongDayNhoHLuu = logdata.CRongDayNhoHLuu;
                        data.CRongDayNhoTLuu = logdata.CRongDayNhoTLuu;
                        data.CRongDayNhoTBinh = logdata.CRongDayNhoTBinh;
                        data.ChieuSauDaoTrungBinh = logdata.ChieuSauDaoTrungBinh;
                        data.CRongDayLonTrungBinh = logdata.CRongDayLonTrungBinh;
                        data.TyLeMoMai = logdata.TyLeMoMai;
                        data.SoMaiTrai = logdata.SoMaiTrai;
                        data.SoMaiPhai = logdata.SoMaiPhai;
                        data.HangMucKlDaoDat = logdata.HangMucKlDaoDat;
                        data.LoaiKlDaoDat = logdata.LoaiKlDaoDat;
                        data.DienTich = logdata.DienTich;
                        data.KlDao = logdata.KlDao;

                        data.Id_TrangThaiThiCongDaoDat = logdata.Id_TrangThaiThiCongDaoDat;

                        data.CRongDayNhoDemCat = logdata.CRongDayNhoDemCat;
                        data.CRongDayLonDemCat = logdata.CRongDayLonDemCat;
                        data.DienTichDapCat1 = logdata.DienTichDapCat1;
                        data.KlDemCat = logdata.KlDemCat;
                        data.CRongDayNhoDapCat = logdata.CRongDayNhoDapCat;
                        data.CRongDayLonDapCat = logdata.CRongDayLonDapCat;
                        data.DienTichDapCat2 = logdata.DienTichDapCat2;
                        data.KlDapCat = logdata.KlDapCat;
                        data.HangMucKlDapCat = logdata.HangMucKlDapCat;
                        data.LoaiKlDapCat = logdata.LoaiKlDapCat;
                        data.KLDapCat_KlOngCCho = logdata.KLDapCat_KlOngCCho;
                        data.KlDapCatSauCCho = logdata.KlDapCatSauCCho;

                        data.Id_TrangThaiThiCongDapCat = logdata.Id_TrangThaiThiCongDapCat;

                        data.CRongDayNhoDapDat = logdata.CRongDayNhoDapDat;
                        data.CRongDayLonDapDat = logdata.CRongDayLonDapDat;
                        data.DienTichDapDat = logdata.DienTichDapDat;
                        data.KlDapDat = logdata.KlDapDat;
                        data.HangMucKlDapDat = logdata.HangMucKlDapDat;
                        data.LoaiKlDapDat = logdata.LoaiKlDapDat;
                        data.KLDapDat_KlOngCCho = logdata.KLDapDat_KlOngCCho;
                        data.KlDapDatSauCCho = logdata.KlDapDatSauCCho;

                        data.Id_TrangThaiThiCongDapDat = logdata.Id_TrangThaiThiCongDapDat;

                        data.HangMucKlDatThua = logdata.HangMucKlDatThua;
                        data.LoaiKlDatThua = logdata.LoaiKlDatThua;
                        data.KlDatThua = logdata.KlDatThua;

                        data.Id_TrangThaiThiCongDatThua = logdata.Id_TrangThaiThiCongDatThua;

                        data.X1 = logdata.X1;
                        data.Y1 = logdata.Y1;
                        data.X2 = logdata.X2;
                        data.Y2 = logdata.Y2;

                        data.Ordinarily = logdata.Ordinarily;
                        data.ApprovalUserId = "";
                        data.DateApproval = null;

                        logdata.IsValid = true;
                        context.PKKL_OngNhua_1TtChungNSachDoc_Logs.Update(logdata);
                    }

                    var logupdate = (from p in context.PKKL_OngNhua_1TtChungNSachDoc_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.PKKL_OngNhua_1TtChungNSachDoc_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new PKKL_OngNhua_1TtChungNSachDoc_Log()
                    {
                        Id = data.Id,
                        Id_ChiNhanh = data.Id_ChiNhanh,

                        Id_TuyenDuong = data.Id_TuyenDuong,
                        Id_TuLyTrinh = data.Id_TuLyTrinh,
                        Id_DenLyTrinh = data.Id_DenLyTrinh,

                        Id_HangMucCongViec = data.Id_HangMucCongViec,
                        Id_LoaiCauKien = data.Id_LoaiCauKien,
                        Id_HangMucKhoiLuong = data.Id_HangMucKhoiLuong,
                        Id_LoaiKhoiLuong = data.Id_LoaiKhoiLuong,

                        DuongKinhNgoaiOngNhua = data.DuongKinhNgoaiOngNhua,
                        CDayOngOngNhua = data.CDayOngOngNhua,
                        ChieuDaiOngNhua = data.ChieuDaiOngNhua,
                        Id_TrangThaiThiCongOngNhua = data.Id_TrangThaiThiCongOngNhua,
                        Id_LoaiCauKienOngThep = data.Id_LoaiCauKienOngThep,
                        Id_HangMucKhoiLuongOngThep = data.Id_HangMucKhoiLuongOngThep,
                        Id_LoaiKhoiLuongOngThep = data.Id_LoaiKhoiLuongOngThep,

                        DuongKinhNgoaiMOngThep = data.DuongKinhNgoaiMOngThep,
                        CDayOngMOngThep = data.CDayOngMOngThep,
                        ChieuDaiMOngThep = data.ChieuDaiMOngThep,
                        Id_TrangThaiThiCongOngThep = data.Id_TrangThaiThiCongOngThep,

                        Id_HinhThucDapTra = data.Id_HinhThucDapTra,

                        HTrangTruocKhiDaoThuongLuu = data.HTrangTruocKhiDaoThuongLuu,
                        DayDaoThuongLuu = data.DayDaoThuongLuu,
                        ChieuSauDaoThuongLuu = data.ChieuSauDaoThuongLuu,
                        DongChayThuongLuu = data.DongChayThuongLuu,
                        DinhDuongOngThuongLuu = data.DinhDuongOngThuongLuu,
                        CDoDayDemCatThuongLuu = data.CDoDayDemCatThuongLuu,
                        CDayDemCatThuongLuu = data.CDayDemCatThuongLuu,
                        DinhDemCatThuongLuu = data.DinhDemCatThuongLuu,
                        DayDapCatThuongLuu = data.DayDapCatThuongLuu,
                        CDayDapCatThuongLuu = data.CDayDapCatThuongLuu,
                        DinhDapCatThuongLuu = data.DinhDapCatThuongLuu,
                        TongChieuDayDemDapCatThuongLuu = data.TongChieuDayDemDapCatThuongLuu,
                        CDoDayDapDatThuongLuu = data.CDoDayDapDatThuongLuu,
                        ChieuDayDapDatThuongLuu = data.ChieuDayDapDatThuongLuu,
                        DinhDapDatThuongLuu = data.DinhDapDatThuongLuu,
                        DapDatCatThuongLuu = data.DapDatCatThuongLuu,
                        ChenhDapSoVoiDaoThuongLuu = data.ChenhDapSoVoiDaoThuongLuu,
                        HTrangTruocKhiDaoHaLuu = data.HTrangTruocKhiDaoHaLuu,
                        DayDaoHaLuu = data.DayDaoHaLuu,
                        ChieuSauDaoHaLuu = data.ChieuSauDaoHaLuu,
                        DongChayHaLuu = data.DongChayHaLuu,
                        DinhDuongOngHaLuu = data.DinhDuongOngHaLuu,
                        CDoDayDemCatHaLuu = data.CDoDayDemCatHaLuu,
                        CDayDemCatHaLuu = data.CDayDemCatHaLuu,
                        DinhDemCatHaLuu = data.DinhDemCatHaLuu,
                        DayDapCatHaLuu = data.DayDapCatHaLuu,
                        CDayDapCatHaLuu = data.CDayDapCatHaLuu,
                        DinhDapCatHaLuu = data.DinhDapCatHaLuu,
                        TongChieuDayDemDapCatHaLuu = data.TongChieuDayDemDapCatHaLuu,
                        CDoDayDapDatHaLuu = data.CDoDayDapDatHaLuu,
                        ChieuDayDapDatHaLuu = data.ChieuDayDapDatHaLuu,
                        DinhDapDatHaLuu = data.DinhDapDatHaLuu,
                        DapDatCatHaLuu = data.DapDatCatHaLuu,
                        ChenhDapSoVoiDaoHaLuu = data.ChenhDapSoVoiDaoHaLuu,
                        CDoDayDemCat = data.CDoDayDemCat,
                        CDoDinhDemCat = data.CDoDinhDemCat,
                        ChieuDayDemCat = data.ChieuDayDemCat,
                        CDoDayDapCat = data.CDoDayDapCat,
                        CDoDinhDapCat = data.CDoDinhDapCat,
                        ChieuDayDapCat = data.ChieuDayDapCat,
                        CDoDayDapDat = data.CDoDayDapDat,
                        CDoDinhDapDat = data.CDoDinhDapDat,
                        ChieuDayDapDat = data.ChieuDayDapDat,
                        CRongDayNhoHLuu = data.CRongDayNhoHLuu,
                        CRongDayNhoTLuu = data.CRongDayNhoTLuu,
                        CRongDayNhoTBinh = data.CRongDayNhoTBinh,
                        ChieuSauDaoTrungBinh = data.ChieuSauDaoTrungBinh,
                        CRongDayLonTrungBinh = data.CRongDayLonTrungBinh,
                        TyLeMoMai = data.TyLeMoMai,
                        SoMaiTrai = data.SoMaiTrai,
                        SoMaiPhai = data.SoMaiPhai,
                        HangMucKlDaoDat = data.HangMucKlDaoDat,
                        LoaiKlDaoDat = data.LoaiKlDaoDat,
                        DienTich = data.DienTich,
                        KlDao = data.KlDao,

                        Id_TrangThaiThiCongDaoDat = data.Id_TrangThaiThiCongDaoDat,

                        CRongDayNhoDemCat = data.CRongDayNhoDemCat,
                        CRongDayLonDemCat = data.CRongDayLonDemCat,
                        DienTichDapCat1 = data.DienTichDapCat1,
                        KlDemCat = data.KlDemCat,
                        CRongDayNhoDapCat = data.CRongDayNhoDapCat,
                        CRongDayLonDapCat = data.CRongDayLonDapCat,
                        DienTichDapCat2 = data.DienTichDapCat2,
                        KlDapCat = data.KlDapCat,
                        HangMucKlDapCat = data.HangMucKlDapCat,
                        LoaiKlDapCat = data.LoaiKlDapCat,
                        KLDapCat_KlOngCCho = data.KLDapCat_KlOngCCho,
                        KlDapCatSauCCho = data.KlDapCatSauCCho,

                        Id_TrangThaiThiCongDapCat = data.Id_TrangThaiThiCongDapCat,

                        CRongDayNhoDapDat = data.CRongDayNhoDapDat,
                        CRongDayLonDapDat = data.CRongDayLonDapDat,
                        DienTichDapDat = data.DienTichDapDat,
                        KlDapDat = data.KlDapDat,
                        HangMucKlDapDat = data.HangMucKlDapDat,
                        LoaiKlDapDat = data.LoaiKlDapDat,
                        KLDapDat_KlOngCCho = data.KLDapDat_KlOngCCho,
                        KlDapDatSauCCho = data.KlDapDatSauCCho,

                        Id_TrangThaiThiCongDapDat = data.Id_TrangThaiThiCongDapDat,

                        HangMucKlDatThua = data.HangMucKlDatThua,
                        LoaiKlDatThua = data.LoaiKlDatThua,
                        KlDatThua = data.KlDatThua,

                        Id_TrangThaiThiCongDatThua = data.Id_TrangThaiThiCongDatThua,

                        X1 = data.X1,
                        Y1 = data.Y1,
                        X2 = data.X2,
                        Y2 = data.Y2,

                        GroupId = data.GroupId,
                        Ordinarily = data.Ordinarily,
                        CreateAt = (DateTime)data.CreateAt,
                        CreateBy = data.CreateBy,
                        IsActive = data.IsActive,
                        ApprovalUserId = data.ApprovalUserId,
                        DateApproval = (DateTime)data.DateApproval,
                        ApprovalDept = data.ApprovalDept,
                        DepartmentId = data.DepartmentId,
                        DepartmentOrder = data.DepartmentOrder,
                        ApprovalOrder = data.ApprovalOrder,
                        ApprovalId = data.ApprovalId,
                        LastApprovalId = data.LastApprovalId,
                        IsStatus = data.IsStatus,
                        IsValid = true,
                        IdChung = data.Id
                    };
                    addLog.Ordinarily = logdata.Ordinarily;
                    context.PKKL_OngNhua_1TtChungNSachDoc_Logs.Add(addLog);
                }
                else if (entity.IsActive == 2)
                {
                    throw new Exception($"Thông tin bạn xóa đang chờ duyệt xóa.");
                }
                else if (entity.IsActive == 3)
                {
                    var logupdate = (from p in context.PKKL_OngNhua_1TtChungNSachDoc_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.PKKL_OngNhua_1TtChungNSachDoc_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new PKKL_OngNhua_1TtChungNSachDoc_Log()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Id_ChiNhanh = data.Id_ChiNhanh,
                        Id_TuyenDuong = data.Id_TuyenDuong,
                        Id_TuLyTrinh = data.Id_TuLyTrinh,
                        Id_DenLyTrinh = data.Id_DenLyTrinh,

                        Id_HangMucCongViec = data.Id_HangMucCongViec,
                        Id_LoaiCauKien = data.Id_LoaiCauKien,
                        Id_HangMucKhoiLuong = data.Id_HangMucKhoiLuong,
                        Id_LoaiKhoiLuong = data.Id_LoaiKhoiLuong,

                        DuongKinhNgoaiOngNhua = data.DuongKinhNgoaiOngNhua,
                        CDayOngOngNhua = data.CDayOngOngNhua,
                        ChieuDaiOngNhua = data.ChieuDaiOngNhua,
                        Id_TrangThaiThiCongOngNhua = data.Id_TrangThaiThiCongOngNhua,
                        Id_LoaiCauKienOngThep = data.Id_LoaiCauKienOngThep,
                        Id_HangMucKhoiLuongOngThep = data.Id_HangMucKhoiLuongOngThep,
                        Id_LoaiKhoiLuongOngThep = data.Id_LoaiKhoiLuongOngThep,

                        DuongKinhNgoaiMOngThep = data.DuongKinhNgoaiMOngThep,
                        CDayOngMOngThep = data.CDayOngMOngThep,
                        ChieuDaiMOngThep = data.ChieuDaiMOngThep,
                        Id_TrangThaiThiCongOngThep = data.Id_TrangThaiThiCongOngThep,

                        Id_HinhThucDapTra = data.Id_HinhThucDapTra,

                        HTrangTruocKhiDaoThuongLuu = data.HTrangTruocKhiDaoThuongLuu,
                        DayDaoThuongLuu = data.DayDaoThuongLuu,
                        ChieuSauDaoThuongLuu = data.ChieuSauDaoThuongLuu,
                        DongChayThuongLuu = data.DongChayThuongLuu,
                        DinhDuongOngThuongLuu = data.DinhDuongOngThuongLuu,
                        CDoDayDemCatThuongLuu = data.CDoDayDemCatThuongLuu,
                        CDayDemCatThuongLuu = data.CDayDemCatThuongLuu,
                        DinhDemCatThuongLuu = data.DinhDemCatThuongLuu,
                        DayDapCatThuongLuu = data.DayDapCatThuongLuu,
                        CDayDapCatThuongLuu = data.CDayDapCatThuongLuu,
                        DinhDapCatThuongLuu = data.DinhDapCatThuongLuu,
                        TongChieuDayDemDapCatThuongLuu = data.TongChieuDayDemDapCatThuongLuu,
                        CDoDayDapDatThuongLuu = data.CDoDayDapDatThuongLuu,
                        ChieuDayDapDatThuongLuu = data.ChieuDayDapDatThuongLuu,
                        DinhDapDatThuongLuu = data.DinhDapDatThuongLuu,
                        DapDatCatThuongLuu = data.DapDatCatThuongLuu,
                        ChenhDapSoVoiDaoThuongLuu = data.ChenhDapSoVoiDaoThuongLuu,
                        HTrangTruocKhiDaoHaLuu = data.HTrangTruocKhiDaoHaLuu,
                        DayDaoHaLuu = data.DayDaoHaLuu,
                        ChieuSauDaoHaLuu = data.ChieuSauDaoHaLuu,
                        DongChayHaLuu = data.DongChayHaLuu,
                        DinhDuongOngHaLuu = data.DinhDuongOngHaLuu,
                        CDoDayDemCatHaLuu = data.CDoDayDemCatHaLuu,
                        CDayDemCatHaLuu = data.CDayDemCatHaLuu,
                        DinhDemCatHaLuu = data.DinhDemCatHaLuu,
                        DayDapCatHaLuu = data.DayDapCatHaLuu,
                        CDayDapCatHaLuu = data.CDayDapCatHaLuu,
                        DinhDapCatHaLuu = data.DinhDapCatHaLuu,
                        TongChieuDayDemDapCatHaLuu = data.TongChieuDayDemDapCatHaLuu,
                        CDoDayDapDatHaLuu = data.CDoDayDapDatHaLuu,
                        ChieuDayDapDatHaLuu = data.ChieuDayDapDatHaLuu,
                        DinhDapDatHaLuu = data.DinhDapDatHaLuu,
                        DapDatCatHaLuu = data.DapDatCatHaLuu,
                        ChenhDapSoVoiDaoHaLuu = data.ChenhDapSoVoiDaoHaLuu,
                        CDoDayDemCat = data.CDoDayDemCat,
                        CDoDinhDemCat = data.CDoDinhDemCat,
                        ChieuDayDemCat = data.ChieuDayDemCat,
                        CDoDayDapCat = data.CDoDayDapCat,
                        CDoDinhDapCat = data.CDoDinhDapCat,
                        ChieuDayDapCat = data.ChieuDayDapCat,
                        CDoDayDapDat = data.CDoDayDapDat,
                        CDoDinhDapDat = data.CDoDinhDapDat,
                        ChieuDayDapDat = data.ChieuDayDapDat,
                        CRongDayNhoHLuu = data.CRongDayNhoHLuu,
                        CRongDayNhoTLuu = data.CRongDayNhoTLuu,
                        CRongDayNhoTBinh = data.CRongDayNhoTBinh,
                        ChieuSauDaoTrungBinh = data.ChieuSauDaoTrungBinh,
                        CRongDayLonTrungBinh = data.CRongDayLonTrungBinh,
                        TyLeMoMai = data.TyLeMoMai,
                        SoMaiTrai = data.SoMaiTrai,
                        SoMaiPhai = data.SoMaiPhai,
                        HangMucKlDaoDat = data.HangMucKlDaoDat,
                        LoaiKlDaoDat = data.LoaiKlDaoDat,
                        DienTich = data.DienTich,
                        KlDao = data.KlDao,

                        Id_TrangThaiThiCongDaoDat = data.Id_TrangThaiThiCongDaoDat,

                        CRongDayNhoDemCat = data.CRongDayNhoDemCat,
                        CRongDayLonDemCat = data.CRongDayLonDemCat,
                        DienTichDapCat1 = data.DienTichDapCat1,
                        KlDemCat = data.KlDemCat,
                        CRongDayNhoDapCat = data.CRongDayNhoDapCat,
                        CRongDayLonDapCat = data.CRongDayLonDapCat,
                        DienTichDapCat2 = data.DienTichDapCat2,
                        KlDapCat = data.KlDapCat,
                        HangMucKlDapCat = data.HangMucKlDapCat,
                        LoaiKlDapCat = data.LoaiKlDapCat,
                        KLDapCat_KlOngCCho = data.KLDapCat_KlOngCCho,
                        KlDapCatSauCCho = data.KlDapCatSauCCho,

                        Id_TrangThaiThiCongDapCat = data.Id_TrangThaiThiCongDapCat,

                        CRongDayNhoDapDat = data.CRongDayNhoDapDat,
                        CRongDayLonDapDat = data.CRongDayLonDapDat,
                        DienTichDapDat = data.DienTichDapDat,
                        KlDapDat = data.KlDapDat,
                        HangMucKlDapDat = data.HangMucKlDapDat,
                        LoaiKlDapDat = data.LoaiKlDapDat,
                        KLDapDat_KlOngCCho = data.KLDapDat_KlOngCCho,
                        KlDapDatSauCCho = data.KlDapDatSauCCho,

                        Id_TrangThaiThiCongDapDat = data.Id_TrangThaiThiCongDapDat,

                        HangMucKlDatThua = data.HangMucKlDatThua,
                        LoaiKlDatThua = data.LoaiKlDatThua,
                        KlDatThua = data.KlDatThua,

                        Id_TrangThaiThiCongDatThua = data.Id_TrangThaiThiCongDatThua,

                        X1 = data.X1,
                        Y1 = data.Y1,
                        X2 = data.X2,
                        Y2 = data.Y2,

                        GroupId = data.GroupId,
                        Ordinarily = data.Ordinarily,
                        CreateAt = (DateTime)data.CreateAt,
                        CreateBy = data.CreateBy,
                        IsActive = data.IsActive,
                        ApprovalUserId = data.ApprovalUserId,
                        DateApproval = (DateTime)data.DateApproval,
                        ApprovalDept = data.ApprovalDept,
                        DepartmentId = data.DepartmentId,
                        DepartmentOrder = data.DepartmentOrder,
                        ApprovalOrder = data.ApprovalOrder,
                        ApprovalId = data.ApprovalId,
                        LastApprovalId = data.LastApprovalId,
                        IsStatus = data.IsStatus,
                        IdChung = data.Id,
                        IsValid = true
                    };
                    context.PKKL_OngNhua_1TtChungNSachDoc_Logs.Add(addLog);
                }
                else if (entity.IsActive == 90)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa không duyệt";
                }
            }
            context.PKKL_OngNhua_1TtChungNSachDocs.Update(data);
            await context.SaveChangesAsync();
        }
       
        public async Task Approval(PKKL_OngNhua_1TtChungNSachDoc data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
                throw new Exception($"Không tìm thấy thông tin đã chọn");

            // Cập nhật entity từ data
            MapFromDataToEntity(data, entity);
            context.PKKL_OngNhua_1TtChungNSachDocs.Update(entity);

            if (data.IsActive == 3 || data.IsActive == 100)
            {
                var updateLogs = await context.PKKL_OngNhua_1TtChungNSachDoc_Logs
                    .Where(p => p.IdChung == entity.Id && p.IsValid == true)
                    .ToListAsync();

                updateLogs.ForEach(p => p.IsValid = false);
                context.PKKL_OngNhua_1TtChungNSachDoc_Logs.UpdateRange(updateLogs);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await context.PKKL_OngNhua_1TtChungNSachDoc_Logs
                    .Where(p => p.IdChung == entity.Id)
                    .OrderByDescending(p => p.CreateAt)
                    .FirstOrDefaultAsync();

                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.PKKL_OngNhua_1TtChungNSachDoc_Logs.Update(updateLog);
                }
            }

            var addLog = CreateLogFromData(data, userId);
            addLog.IsValid = data.IsActive == 100 ? false : true;

            context.PKKL_OngNhua_1TtChungNSachDoc_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task NoApproval(PKKL_OngNhua_1TtChungNSachDoc data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null || entity.IsActive != data.IsActive)
                throw new Exception("Không tìm thấy thông tin đã chọn");

            if (entity.IsActive == 0)
            {
                entity.IsActive = 90;
                entity.IsStatus = "Không duyệt";
                entity.ApprovalUserId = data.ApprovalUserId;
            }
            else if (entity.IsActive == 1 || entity.IsActive == 2)
            {
                var logdata = context.PKKL_OngNhua_1TtChungNSachDoc_Logs
                    .Where(p => p.IdChung == entity.Id && p.IsValid == true)
                    .OrderBy(p => p.CreateAt)
                    .FirstOrDefault();

                if (logdata != null)
                {
                    MapFromLogToData(logdata, data);

                    if (entity.IsActive == 1)
                    {
                        MapFromLogToEntity(logdata, entity);

                        var logupdate = context.PKKL_OngNhua_1TtChungNSachDoc_Logs
                            .Where(p => p.IdChung == entity.Id && p.IsValid == true && p.CreateAt >= logdata.CreateAt)
                            .ToList();

                        logupdate.ForEach(p => p.IsValid = false);
                        context.PKKL_OngNhua_1TtChungNSachDoc_Logs.UpdateRange(logupdate);
                    }
                    else
                    {
                        MapFromDataToEntity(data, entity);
                    }

                    entity.IsActive = 3;
                    entity.IsStatus = "Đã duyệt";
                }
            }
            else if (entity.IsActive == 3)
            {
                throw new Exception("Thông tin hủy duyệt không tồn tại.");
            }

            var addLog = CreateLogFromData(data, userId);
            context.PKKL_OngNhua_1TtChungNSachDoc_Logs.Add(addLog);
            context.PKKL_OngNhua_1TtChungNSachDocs.Update(entity);
            await context.SaveChangesAsync();
        }
        private void MapFromLogToData(PKKL_OngNhua_1TtChungNSachDoc_Log log, PKKL_OngNhua_1TtChungNSachDoc data)
        {
            foreach (var prop in typeof(PKKL_OngNhua_1TtChungNSachDoc_Log).GetProperties())
            {
                var targetProp = typeof(PKKL_OngNhua_1TtChungNSachDoc).GetProperty(prop.Name);
                if (targetProp != null && targetProp.CanWrite)
                {
                    targetProp.SetValue(data, prop.GetValue(log));
                }
            }
        }
        private void MapFromDataToEntity(PKKL_OngNhua_1TtChungNSachDoc data, PKKL_OngNhua_1TtChungNSachDoc entity)
        {
            foreach (var prop in typeof(PKKL_OngNhua_1TtChungNSachDoc).GetProperties())
            {
                if (prop.Name == "Id") continue; // Không ghi đè ID chính
                var entityProp = typeof(PKKL_OngNhua_1TtChungNSachDoc).GetProperty(prop.Name);
                if (entityProp != null && entityProp.CanWrite)
                {
                    entityProp.SetValue(entity, prop.GetValue(data));
                }
            }
        }
        private void MapFromLogToEntity(PKKL_OngNhua_1TtChungNSachDoc_Log log, PKKL_OngNhua_1TtChungNSachDoc entity)
        {
            foreach (var prop in typeof(PKKL_OngNhua_1TtChungNSachDoc_Log).GetProperties())
            {
                if (prop.Name == "Id" || prop.Name == "IsValid") continue;
                var targetProp = typeof(PKKL_OngNhua_1TtChungNSachDoc).GetProperty(prop.Name);
                if (targetProp != null && targetProp.CanWrite)
                {
                    targetProp.SetValue(entity, prop.GetValue(log));
                }
            }
        }
        private PKKL_OngNhua_1TtChungNSachDoc_Log CreateLogFromData(PKKL_OngNhua_1TtChungNSachDoc data, string userId)
        {
            var log = new PKKL_OngNhua_1TtChungNSachDoc_Log();
            foreach (var prop in typeof(PKKL_OngNhua_1TtChungNSachDoc_Log).GetProperties())
            {
                if (prop.Name == "Id")
                {
                    prop.SetValue(log, Guid.NewGuid().ToString());
                }
                else if (prop.Name == "CreateAt")
                {
                    prop.SetValue(log, DateTime.Now);
                }
                else if (prop.Name == "ApprovalUserId")
                {
                    prop.SetValue(log, userId);
                }
                else if (prop.Name == "DateApproval")
                {
                    prop.SetValue(log, DateTime.Now);
                }
                else if (prop.Name == "IsValid")
                {
                    prop.SetValue(log, false);
                }
                else
                {
                    var dataProp = typeof(PKKL_OngNhua_1TtChungNSachDoc).GetProperty(prop.Name);
                    if (dataProp != null)
                    {
                        prop.SetValue(log, dataProp.GetValue(data));
                    }
                }
            }
            log.IdChung = data.Id;
            log.DepartmentId = null;
            log.DepartmentOrder = 0;
            log.ApprovalOrder = 0;
            log.ApprovalId = null;
            log.LastApprovalId = null;
            return log;
        }
        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy thông tin đã chọn");
            }
            context.Set<PKKL_OngNhua_1TtChungNSachDoc>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.PKKL_OngNhua_1TtChungNSachDocs.Where(p => p.Id == ids && p.IsActive != 100).FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception($"Không tìm thấy thông tin đã chọn");
            }
            if (model != null && model.IsActive == 0)
            {
                throw new Exception($"Chi nhánh đang chờ duyệt thêm mới.");
            }
            if (model != null && model.IsActive == 1)
            {
                throw new Exception($"Chi nhánh đang chờ duyệt sửa.");
            }
            if (model != null && model.IsActive == 2)
            {
                throw new Exception($"Chi nhánh đang chờ duyệt xóa.");
            }
            //if (model != null && model.TenChiNhanh.ToUpper() != name.ToUpper() && name != "")
            //{
            //throw new Exception($"Tên thông tin đã bị thay đổi.");
            //}
            return true;
        }
        public async Task<bool> CheckSave(PKKL_OngNhua_1TtChungNSachDoc input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.PKKL_OngNhua_1TtChungNSachDoc_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100
                                    && p.Id_ChiNhanh != input.Id_ChiNhanh
                                    && p.Id_TuyenDuong != input.Id_TuyenDuong
                                    && p.Id_TuLyTrinh != input.Id_TuLyTrinh
                                    && p.Id_DenLyTrinh != input.Id_DenLyTrinh
                                    && p.Id_HangMucCongViec != input.Id_HangMucCongViec
                                    && p.Id_LoaiCauKien != input.Id_LoaiCauKien
                                    && p.Id_HangMucKhoiLuong != input.Id_HangMucKhoiLuong
                                    && p.Id_LoaiKhoiLuong != input.Id_LoaiKhoiLuong
                                    && p.DuongKinhNgoaiOngNhua != input.DuongKinhNgoaiOngNhua
                                    && p.CDayOngOngNhua != input.CDayOngOngNhua
                                    && p.ChieuDaiOngNhua != input.ChieuDaiOngNhua
                                    && p.Id_TrangThaiThiCongOngNhua != input.Id_TrangThaiThiCongOngNhua
                                    && p.Id_LoaiCauKienOngThep != input.Id_LoaiCauKienOngThep
                                    && p.Id_HangMucKhoiLuongOngThep != input.Id_HangMucKhoiLuongOngThep
                                    && p.Id_LoaiKhoiLuongOngThep != input.Id_LoaiKhoiLuongOngThep
                                    && p.DuongKinhNgoaiMOngThep != input.DuongKinhNgoaiMOngThep
                                    && p.CDayOngMOngThep != input.CDayOngMOngThep
                                    && p.ChieuDaiMOngThep != input.ChieuDaiMOngThep
                                    && p.Id_TrangThaiThiCongOngThep != input.Id_TrangThaiThiCongOngThep
                                    && p.Id_HinhThucDapTra != input.Id_HinhThucDapTra
                                    && p.HTrangTruocKhiDaoThuongLuu != input.HTrangTruocKhiDaoThuongLuu
                                    && p.DayDaoThuongLuu != input.DayDaoThuongLuu
                                    && p.ChieuSauDaoThuongLuu != input.ChieuSauDaoThuongLuu
                                    && p.DongChayThuongLuu != input.DongChayThuongLuu
                                    && p.DinhDuongOngThuongLuu != input.DinhDuongOngThuongLuu
                                    && p.CDoDayDemCatThuongLuu != input.CDoDayDemCatThuongLuu
                                    && p.CDayDemCatThuongLuu != input.CDayDemCatThuongLuu
                                    && p.DinhDemCatThuongLuu != input.DinhDemCatThuongLuu
                                    && p.DayDapCatThuongLuu != input.DayDapCatThuongLuu
                                    && p.CDayDapCatThuongLuu != input.CDayDapCatThuongLuu
                                    && p.DinhDapCatThuongLuu != input.DinhDapCatThuongLuu
                                    && p.TongChieuDayDemDapCatThuongLuu != input.TongChieuDayDemDapCatThuongLuu
                                    && p.CDoDayDapDatThuongLuu != input.CDoDayDapDatThuongLuu
                                    && p.ChieuDayDapDatThuongLuu != input.ChieuDayDapDatThuongLuu
                                    && p.DinhDapDatThuongLuu != input.DinhDapDatThuongLuu
                                    && p.DapDatCatThuongLuu != input.DapDatCatThuongLuu
                                    && p.ChenhDapSoVoiDaoThuongLuu != input.ChenhDapSoVoiDaoThuongLuu
                                    && p.HTrangTruocKhiDaoHaLuu != input.HTrangTruocKhiDaoHaLuu
                                    && p.DayDaoHaLuu != input.DayDaoHaLuu
                                    && p.ChieuSauDaoHaLuu != input.ChieuSauDaoHaLuu
                                    && p.DongChayHaLuu != input.DongChayHaLuu
                                    && p.DinhDuongOngHaLuu != input.DinhDuongOngHaLuu
                                    && p.CDoDayDemCatHaLuu != input.CDoDayDemCatHaLuu
                                    && p.CDayDemCatHaLuu != input.CDayDemCatHaLuu
                                    && p.DinhDemCatHaLuu != input.DinhDemCatHaLuu
                                    && p.DayDapCatHaLuu != input.DayDapCatHaLuu
                                    && p.CDayDapCatHaLuu != input.CDayDapCatHaLuu
                                    && p.DinhDapCatHaLuu != input.DinhDapCatHaLuu
                                    && p.TongChieuDayDemDapCatHaLuu != input.TongChieuDayDemDapCatHaLuu
                                    && p.CDoDayDapDatHaLuu != input.CDoDayDapDatHaLuu
                                    && p.ChieuDayDapDatHaLuu != input.ChieuDayDapDatHaLuu
                                    && p.DinhDapDatHaLuu != input.DinhDapDatHaLuu
                                    && p.DapDatCatHaLuu != input.DapDatCatHaLuu
                                    && p.ChenhDapSoVoiDaoHaLuu != input.ChenhDapSoVoiDaoHaLuu
                                    && p.CDoDayDemCat != input.CDoDayDemCat
                                    && p.CDoDinhDemCat != input.CDoDinhDemCat
                                    && p.ChieuDayDemCat != input.ChieuDayDemCat
                                    && p.CDoDayDapCat != input.CDoDayDapCat
                                    && p.CDoDinhDapCat != input.CDoDinhDapCat
                                    && p.ChieuDayDapCat != input.ChieuDayDapCat
                                    && p.CDoDayDapDat != input.CDoDayDapDat
                                    && p.CDoDinhDapDat != input.CDoDinhDapDat
                                    && p.ChieuDayDapDat != input.ChieuDayDapDat
                                    && p.CRongDayNhoHLuu != input.CRongDayNhoHLuu
                                    && p.CRongDayNhoTLuu != input.CRongDayNhoTLuu
                                    && p.CRongDayNhoTBinh != input.CRongDayNhoTBinh
                                    && p.ChieuSauDaoTrungBinh != input.ChieuSauDaoTrungBinh
                                    && p.CRongDayLonTrungBinh != input.CRongDayLonTrungBinh
                                    && p.TyLeMoMai != input.TyLeMoMai
                                    && p.SoMaiTrai != input.SoMaiTrai
                                    && p.SoMaiPhai != input.SoMaiPhai
                                    && p.HangMucKlDaoDat != input.HangMucKlDaoDat
                                    && p.LoaiKlDaoDat != input.LoaiKlDaoDat
                                    && p.DienTich != input.DienTich
                                    && p.KlDao != input.KlDao
                                    && p.Id_TrangThaiThiCongDaoDat != input.Id_TrangThaiThiCongDaoDat
                                    && p.CRongDayNhoDemCat != input.CRongDayNhoDemCat
                                    && p.CRongDayLonDemCat != input.CRongDayLonDemCat
                                    && p.DienTichDapCat1 != input.DienTichDapCat1
                                    && p.KlDemCat != input.KlDemCat
                                    && p.CRongDayNhoDapCat != input.CRongDayNhoDapCat
                                    && p.CRongDayLonDapCat != input.CRongDayLonDapCat
                                    && p.DienTichDapCat2 != input.DienTichDapCat2
                                    && p.KlDapCat != input.KlDapCat
                                    && p.HangMucKlDapCat != input.HangMucKlDapCat
                                    && p.LoaiKlDapCat != input.LoaiKlDapCat
                                    && p.KLDapCat_KlOngCCho != input.KLDapCat_KlOngCCho
                                    && p.KlDapCatSauCCho != input.KlDapCatSauCCho
                                    && p.Id_TrangThaiThiCongDapCat != input.Id_TrangThaiThiCongDapCat
                                    && p.CRongDayNhoDapDat != input.CRongDayNhoDapDat
                                    && p.CRongDayLonDapDat != input.CRongDayLonDapDat
                                    && p.DienTichDapDat != input.DienTichDapDat
                                    && p.KlDapDat != input.KlDapDat
                                    && p.HangMucKlDapDat != input.HangMucKlDapDat
                                    && p.LoaiKlDapDat != input.LoaiKlDapDat
                                    && p.KLDapDat_KlOngCCho != input.KLDapDat_KlOngCCho
                                    && p.KlDapDatSauCCho != input.KlDapDatSauCCho
                                    && p.Id_TrangThaiThiCongDapDat != input.Id_TrangThaiThiCongDapDat
                                    && p.HangMucKlDatThua != input.HangMucKlDatThua
                                    && p.LoaiKlDatThua != input.LoaiKlDatThua
                                    && p.KlDatThua != input.KlDatThua
                                    && p.Id_TrangThaiThiCongDatThua != input.Id_TrangThaiThiCongDatThua
                                    && p.X1 != input.X1
                                    && p.Y1 != input.Y1
                                    && p.X2 != input.X2
                                    && p.Y2 != input.Y2
                                   select p).CountAsync();
                if (model > 0)
                {
                    throw new Exception($"Thông tin bạn nhập đã tồn tại.");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: " + ex.Message);
            }
        }
        public async Task<bool> CheckEdit(PKKL_OngNhua_1TtChungNSachDoc input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.PKKL_OngNhua_1TtChungNSachDoc_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100 && p.Id != input.Id
                                    && p.Id_ChiNhanh != input.Id_ChiNhanh
                                    && p.Id_TuyenDuong != input.Id_TuyenDuong
                                    && p.Id_TuLyTrinh != input.Id_TuLyTrinh
                                    && p.Id_DenLyTrinh != input.Id_DenLyTrinh
                                    && p.Id_HangMucCongViec != input.Id_HangMucCongViec
                                    && p.Id_LoaiCauKien != input.Id_LoaiCauKien
                                    && p.Id_HangMucKhoiLuong != input.Id_HangMucKhoiLuong
                                    && p.Id_LoaiKhoiLuong != input.Id_LoaiKhoiLuong
                                    && p.DuongKinhNgoaiOngNhua != input.DuongKinhNgoaiOngNhua
                                    && p.CDayOngOngNhua != input.CDayOngOngNhua
                                    && p.ChieuDaiOngNhua != input.ChieuDaiOngNhua
                                    && p.Id_TrangThaiThiCongOngNhua != input.Id_TrangThaiThiCongOngNhua
                                    && p.Id_LoaiCauKienOngThep != input.Id_LoaiCauKienOngThep
                                    && p.Id_HangMucKhoiLuongOngThep != input.Id_HangMucKhoiLuongOngThep
                                    && p.Id_LoaiKhoiLuongOngThep != input.Id_LoaiKhoiLuongOngThep
                                    && p.DuongKinhNgoaiMOngThep != input.DuongKinhNgoaiMOngThep
                                    && p.CDayOngMOngThep != input.CDayOngMOngThep
                                    && p.ChieuDaiMOngThep != input.ChieuDaiMOngThep
                                    && p.Id_TrangThaiThiCongOngThep != input.Id_TrangThaiThiCongOngThep
                                    && p.Id_HinhThucDapTra != input.Id_HinhThucDapTra
                                    && p.HTrangTruocKhiDaoThuongLuu != input.HTrangTruocKhiDaoThuongLuu
                                    && p.DayDaoThuongLuu != input.DayDaoThuongLuu
                                    && p.ChieuSauDaoThuongLuu != input.ChieuSauDaoThuongLuu
                                    && p.DongChayThuongLuu != input.DongChayThuongLuu
                                    && p.DinhDuongOngThuongLuu != input.DinhDuongOngThuongLuu
                                    && p.CDoDayDemCatThuongLuu != input.CDoDayDemCatThuongLuu
                                    && p.CDayDemCatThuongLuu != input.CDayDemCatThuongLuu
                                    && p.DinhDemCatThuongLuu != input.DinhDemCatThuongLuu
                                    && p.DayDapCatThuongLuu != input.DayDapCatThuongLuu
                                    && p.CDayDapCatThuongLuu != input.CDayDapCatThuongLuu
                                    && p.DinhDapCatThuongLuu != input.DinhDapCatThuongLuu
                                    && p.TongChieuDayDemDapCatThuongLuu != input.TongChieuDayDemDapCatThuongLuu
                                    && p.CDoDayDapDatThuongLuu != input.CDoDayDapDatThuongLuu
                                    && p.ChieuDayDapDatThuongLuu != input.ChieuDayDapDatThuongLuu
                                    && p.DinhDapDatThuongLuu != input.DinhDapDatThuongLuu
                                    && p.DapDatCatThuongLuu != input.DapDatCatThuongLuu
                                    && p.ChenhDapSoVoiDaoThuongLuu != input.ChenhDapSoVoiDaoThuongLuu
                                    && p.HTrangTruocKhiDaoHaLuu != input.HTrangTruocKhiDaoHaLuu
                                    && p.DayDaoHaLuu != input.DayDaoHaLuu
                                    && p.ChieuSauDaoHaLuu != input.ChieuSauDaoHaLuu
                                    && p.DongChayHaLuu != input.DongChayHaLuu
                                    && p.DinhDuongOngHaLuu != input.DinhDuongOngHaLuu
                                    && p.CDoDayDemCatHaLuu != input.CDoDayDemCatHaLuu
                                    && p.CDayDemCatHaLuu != input.CDayDemCatHaLuu
                                    && p.DinhDemCatHaLuu != input.DinhDemCatHaLuu
                                    && p.DayDapCatHaLuu != input.DayDapCatHaLuu
                                    && p.CDayDapCatHaLuu != input.CDayDapCatHaLuu
                                    && p.DinhDapCatHaLuu != input.DinhDapCatHaLuu
                                    && p.TongChieuDayDemDapCatHaLuu != input.TongChieuDayDemDapCatHaLuu
                                    && p.CDoDayDapDatHaLuu != input.CDoDayDapDatHaLuu
                                    && p.ChieuDayDapDatHaLuu != input.ChieuDayDapDatHaLuu
                                    && p.DinhDapDatHaLuu != input.DinhDapDatHaLuu
                                    && p.DapDatCatHaLuu != input.DapDatCatHaLuu
                                    && p.ChenhDapSoVoiDaoHaLuu != input.ChenhDapSoVoiDaoHaLuu
                                    && p.CDoDayDemCat != input.CDoDayDemCat
                                    && p.CDoDinhDemCat != input.CDoDinhDemCat
                                    && p.ChieuDayDemCat != input.ChieuDayDemCat
                                    && p.CDoDayDapCat != input.CDoDayDapCat
                                    && p.CDoDinhDapCat != input.CDoDinhDapCat
                                    && p.ChieuDayDapCat != input.ChieuDayDapCat
                                    && p.CDoDayDapDat != input.CDoDayDapDat
                                    && p.CDoDinhDapDat != input.CDoDinhDapDat
                                    && p.ChieuDayDapDat != input.ChieuDayDapDat
                                    && p.CRongDayNhoHLuu != input.CRongDayNhoHLuu
                                    && p.CRongDayNhoTLuu != input.CRongDayNhoTLuu
                                    && p.CRongDayNhoTBinh != input.CRongDayNhoTBinh
                                    && p.ChieuSauDaoTrungBinh != input.ChieuSauDaoTrungBinh
                                    && p.CRongDayLonTrungBinh != input.CRongDayLonTrungBinh
                                    && p.TyLeMoMai != input.TyLeMoMai
                                    && p.SoMaiTrai != input.SoMaiTrai
                                    && p.SoMaiPhai != input.SoMaiPhai
                                    && p.HangMucKlDaoDat != input.HangMucKlDaoDat
                                    && p.LoaiKlDaoDat != input.LoaiKlDaoDat
                                    && p.DienTich != input.DienTich
                                    && p.KlDao != input.KlDao
                                    && p.Id_TrangThaiThiCongDaoDat != input.Id_TrangThaiThiCongDaoDat
                                    && p.CRongDayNhoDemCat != input.CRongDayNhoDemCat
                                    && p.CRongDayLonDemCat != input.CRongDayLonDemCat
                                    && p.DienTichDapCat1 != input.DienTichDapCat1
                                    && p.KlDemCat != input.KlDemCat
                                    && p.CRongDayNhoDapCat != input.CRongDayNhoDapCat
                                    && p.CRongDayLonDapCat != input.CRongDayLonDapCat
                                    && p.DienTichDapCat2 != input.DienTichDapCat2
                                    && p.KlDapCat != input.KlDapCat
                                    && p.HangMucKlDapCat != input.HangMucKlDapCat
                                    && p.LoaiKlDapCat != input.LoaiKlDapCat
                                    && p.KLDapCat_KlOngCCho != input.KLDapCat_KlOngCCho
                                    && p.KlDapCatSauCCho != input.KlDapCatSauCCho
                                    && p.Id_TrangThaiThiCongDapCat != input.Id_TrangThaiThiCongDapCat
                                    && p.CRongDayNhoDapDat != input.CRongDayNhoDapDat
                                    && p.CRongDayLonDapDat != input.CRongDayLonDapDat
                                    && p.DienTichDapDat != input.DienTichDapDat
                                    && p.KlDapDat != input.KlDapDat
                                    && p.HangMucKlDapDat != input.HangMucKlDapDat
                                    && p.LoaiKlDapDat != input.LoaiKlDapDat
                                    && p.KLDapDat_KlOngCCho != input.KLDapDat_KlOngCCho
                                    && p.KlDapDatSauCCho != input.KlDapDatSauCCho
                                    && p.Id_TrangThaiThiCongDapDat != input.Id_TrangThaiThiCongDapDat
                                    && p.HangMucKlDatThua != input.HangMucKlDatThua
                                    && p.LoaiKlDatThua != input.LoaiKlDatThua
                                    && p.KlDatThua != input.KlDatThua
                                    && p.Id_TrangThaiThiCongDatThua != input.Id_TrangThaiThiCongDatThua
                                    && p.X1 != input.X1
                                    && p.Y1 != input.Y1
                                    && p.X2 != input.X2
                                    && p.Y2 != input.Y2
                                   select p).CountAsync();
                if (model > 0)
                {
                    throw new Exception($"Thông tin bạn nhập đã tồn tại.");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: " + ex.Message);
            }
        }
        public async Task<bool> CheckDelete(PKKL_OngNhua_1TtChungNSachDoc input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (1 == 0)
                {
                    throw new Exception($"");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: " + ex.Message);
            }
        }
        public async Task<bool> CheckExclusive(string[] ids, DateTime baseTime)
        {
            foreach (var id in ids)
            {
                var model = await GetById(id);
                if (model == null)
                {
                    throw new Exception($"Không tìm thấy thông tin đã chọn");
                }
                if (model.CreateAt > baseTime)
                {
                    throw new Exception($"Thông tin đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang!");
                }
            }
            return true;
        }

    }
}
