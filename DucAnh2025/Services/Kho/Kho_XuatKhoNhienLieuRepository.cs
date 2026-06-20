using DucAnh2025.Data;
using DucAnh2025.Models;
using DucAnh2025.Repository;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace DucAnh2025.Services
{
    public class Kho_XuatKhoNhienLieuRepository : IKho_XuatKhoNhienLieuRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public Kho_XuatKhoNhienLieuRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public async Task<List<Kho_XuatKhoNhienLieuModel>> GetAllByVM(Kho_XuatKhoNhienLieuModel dataModel, string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();

                var baseQuery = context.Kho_XuatKhoNhienLieus
                    .AsNoTracking()
                    .Where(p => p.GroupId == groupId && p.IsActive != 100);

                if (!string.IsNullOrEmpty(dataModel?.Id_TramTron))
                {
                    baseQuery = baseQuery.Where(m => m.Id_TramTron == dataModel.Id_TramTron);
                }
                if (!string.IsNullOrEmpty(dataModel?.Id_NhomNhienLieu))
                {
                    baseQuery = baseQuery.Where(m => m.Id_NhomNhienLieu == dataModel.Id_NhomNhienLieu);
                }
                if (!string.IsNullOrEmpty(dataModel?.Id_LoaiNhienLieu))
                {
                    baseQuery = baseQuery.Where(m => m.Id_LoaiNhienLieu == dataModel.Id_LoaiNhienLieu);
                }
                if (!string.IsNullOrEmpty(dataModel?.Id_NhanHieu))
                {
                    baseQuery = baseQuery.Where(m => m.Id_NhanHieu == dataModel.Id_NhanHieu);
                }
                if (!string.IsNullOrEmpty(dataModel?.Id_DonVi))
                {
                    baseQuery = baseQuery.Where(m => m.Id_DonVi == dataModel.Id_DonVi);
                }

                var baseData = await baseQuery
                    .OrderByDescending(p => p.CreateAt)
                    .ToListAsync();

                var ChiNhanhsDict = await context.ChiNhanhs.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_NhomNhienLieusDict = await context.Kho_DM_NhomNhienLieus.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_LoaiNhienLieusDict = await context.Kho_DM_LoaiNhienLieus.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_NhanHieusDict = await context.Kho_DM_NhanHieus.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_DonVisDict = await context.Kho_DM_DonVis.AsNoTracking().ToDictionaryAsync(x => x.Id);

                var data = baseData.Select(p1 => new Kho_XuatKhoNhienLieuModel
                {
                    Id = p1.Id,
                    Id_TramTron = ChiNhanhsDict.TryGetValue(p1.Id_TramTron, out var tmpChiNhanhs) ? tmpChiNhanhs.TenChiNhanh : "",
                    NgayNhapKho = p1.NgayNhapKho,
                    ChotSoCoCay = p1.ChotSoCoCay,
                    SoPhieu = p1.SoPhieu,
                    ChotSoKMTruocKhiCap = p1.ChotSoKMTruocKhiCap,
                    ChieuCaoBinhNhienLieu = p1.ChieuCaoBinhNhienLieu,
                    DoiTuongNhan = p1.DoiTuongNhan,
                    Id_NhomNhienLieu = Kho_DM_NhomNhienLieusDict.TryGetValue(p1.Id_NhomNhienLieu, out var tmpKho_DM_NhomNhienLieus) ? tmpKho_DM_NhomNhienLieus.TenNhienLieu : "",
                    Id_LoaiNhienLieu = Kho_DM_LoaiNhienLieusDict.TryGetValue(p1.Id_LoaiNhienLieu, out var tmpKho_DM_LoaiNhienLieus) ? tmpKho_DM_LoaiNhienLieus.LoaiNhienLieu : "",
                    Id_NhanHieu = Kho_DM_NhanHieusDict.TryGetValue(p1.Id_NhanHieu, out var tmpKho_DM_NhanHieus) ? tmpKho_DM_NhanHieus.TenNhanHieu : "",
                    Id_DonVi = Kho_DM_DonVisDict.TryGetValue(p1.Id_DonVi, out var tmpKho_DM_DonVis) ? tmpKho_DM_DonVis.TenDonVi : "",
                    KLXuatKhoCoThue = p1.KLXuatKhoCoThue,
                    KLXuatKhoKhongThue = p1.KLXuatKhoKhongThue,
                    DonViSauQuyDoi = p1.DonViSauQuyDoi,
                    SLThucTeCoThue = p1.SLThucTeCoThue,
                    SLThucTeKhongThue = p1.SLThucTeKhongThue,
                    TongSoLuong = p1.TongSoLuong,
                    DonGiaCoThue = p1.DonGiaCoThue,
                    DonGiaKhongThue = p1.DonGiaKhongThue,
                    ThanhTienCoThue = p1.ThanhTienCoThue,
                    ThanhTienKhongThue = p1.ThanhTienKhongThue,
                    ThanhTienTongTien = p1.ThanhTienTongTien,
                    GroupId = p1.GroupId,
                    Ordinarily = p1.Ordinarily,
                    CreateAt = p1.CreateAt.HasValue ? (DateTime)p1.CreateAt : DateTime.MinValue,
                    CreateBy = p1.CreateBy,
                    IsActive = p1.IsActive,
                    ApprovalUserId = p1.ApprovalUserId,
                    DateApproval = p1.DateApproval.HasValue ? (DateTime)p1.DateApproval : DateTime.MinValue,
                    ApprovalDept = p1.ApprovalDept,
                    DepartmentId = p1.DepartmentId,
                    DepartmentOrder = p1.DepartmentOrder,
                    ApprovalOrder = p1.ApprovalOrder,
                    ApprovalId = p1.ApprovalId,
                    LastApprovalId = p1.LastApprovalId,
                    IsStatus = p1.IsStatus,
                }).ToList();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy dữ liệu:  {ex.Message}");
            }
        }
        public async Task<List<Kho_XuatKhoNhienLieuModel>> GetHistoryIsValidEdit(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var query = (from p1 in context.Kho_XuatKhoNhienLieu_Logs
                             join ChiNhanhs1 in context.ChiNhanhs on p1.Id_TramTron equals ChiNhanhs1.Id
                             join Kho_DM_NhomNhienLieus1 in context.Kho_DM_NhomNhienLieus on p1.Id_NhomNhienLieu equals Kho_DM_NhomNhienLieus1.Id
                             join Kho_DM_LoaiNhienLieus1 in context.Kho_DM_LoaiNhienLieus on p1.Id_LoaiNhienLieu equals Kho_DM_LoaiNhienLieus1.Id
                             join Kho_DM_NhanHieus1 in context.Kho_DM_NhanHieus on p1.Id_NhanHieu equals Kho_DM_NhanHieus1.Id
                             join Kho_DM_DonVis1 in context.Kho_DM_DonVis on p1.Id_DonVi equals Kho_DM_DonVis1.Id
                             join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                             join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                             where p1.IdChung == id && p1.IsValid == true
                             orderby p1.CreateAt
                             select new Kho_XuatKhoNhienLieuModel
                             {
                                 Id = p1.Id,
                                 Id_TramTron = ChiNhanhs1.TenChiNhanh,
                                 NgayNhapKho = p1.NgayNhapKho,
                                 ChotSoCoCay = p1.ChotSoCoCay,
                                 SoPhieu = p1.SoPhieu,
                                 ChotSoKMTruocKhiCap = p1.ChotSoKMTruocKhiCap,
                                 ChieuCaoBinhNhienLieu = p1.ChieuCaoBinhNhienLieu,
                                 DoiTuongNhan = p1.DoiTuongNhan,
                                 Id_NhomNhienLieu = Kho_DM_NhomNhienLieus1.TenNhienLieu,
                                 Id_LoaiNhienLieu = Kho_DM_LoaiNhienLieus1.LoaiNhienLieu,
                                 Id_NhanHieu = Kho_DM_NhanHieus1.TenNhanHieu,
                                 Id_DonVi = Kho_DM_DonVis1.TenDonVi,
                                 KLXuatKhoCoThue = p1.KLXuatKhoCoThue,
                                 KLXuatKhoKhongThue = p1.KLXuatKhoKhongThue,
                                 DonViSauQuyDoi = p1.DonViSauQuyDoi,
                                 SLThucTeCoThue = p1.SLThucTeCoThue,
                                 SLThucTeKhongThue = p1.SLThucTeKhongThue,
                                 TongSoLuong = p1.TongSoLuong,
                                 DonGiaCoThue = p1.DonGiaCoThue,
                                 DonGiaKhongThue = p1.DonGiaKhongThue,
                                 ThanhTienCoThue = p1.ThanhTienCoThue,
                                 ThanhTienKhongThue = p1.ThanhTienKhongThue,
                                 ThanhTienTongTien = p1.ThanhTienTongTien,
                                 GroupId = p1.GroupId,
                                 Ordinarily = p1.Ordinarily,
                                 CreateAt = p1.CreateAt ?? DateTime.MinValue,
                                 CreateBy = p1.CreateBy,
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
                return await query;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<Kho_XuatKhoNhienLieuModel> GetDetails(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.Kho_XuatKhoNhienLieus
                              join ChiNhanhs1 in context.ChiNhanhs on p1.Id_TramTron equals ChiNhanhs1.Id into ChiNhanhs11
                              from ChiNhanhs1 in ChiNhanhs11.DefaultIfEmpty()
                              join Kho_DM_NhomNhienLieus1 in context.Kho_DM_NhomNhienLieus on p1.Id_NhomNhienLieu equals Kho_DM_NhomNhienLieus1.Id into Kho_DM_NhomNhienLieus11
                              from Kho_DM_NhomNhienLieus1 in Kho_DM_NhomNhienLieus11.DefaultIfEmpty()
                              join Kho_DM_LoaiNhienLieus1 in context.Kho_DM_LoaiNhienLieus on p1.Id_LoaiNhienLieu equals Kho_DM_LoaiNhienLieus1.Id into Kho_DM_LoaiNhienLieus11
                              from Kho_DM_LoaiNhienLieus1 in Kho_DM_LoaiNhienLieus11.DefaultIfEmpty()
                              join Kho_DM_NhanHieus1 in context.Kho_DM_NhanHieus on p1.Id_NhanHieu equals Kho_DM_NhanHieus1.Id into Kho_DM_NhanHieus11
                              from Kho_DM_NhanHieus1 in Kho_DM_NhanHieus11.DefaultIfEmpty()
                              join Kho_DM_DonVis1 in context.Kho_DM_DonVis on p1.Id_DonVi equals Kho_DM_DonVis1.Id into Kho_DM_DonVis11
                              from Kho_DM_DonVis1 in Kho_DM_DonVis11.DefaultIfEmpty()
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                              from approvalDept in c1.DefaultIfEmpty()
                              where p1.Id == id
                              select new Kho_XuatKhoNhienLieuModel
                              {
                                  Id = p1.Id,
                                  Id_TramTron = ChiNhanhs1.TenChiNhanh,
                                  NgayNhapKho = p1.NgayNhapKho,
                                  ChotSoCoCay = p1.ChotSoCoCay,
                                  SoPhieu = p1.SoPhieu,
                                  ChotSoKMTruocKhiCap = p1.ChotSoKMTruocKhiCap,
                                  ChieuCaoBinhNhienLieu = p1.ChieuCaoBinhNhienLieu,
                                  DoiTuongNhan = p1.DoiTuongNhan,
                                  Id_NhomNhienLieu = Kho_DM_NhomNhienLieus1.TenNhienLieu,
                                  Id_LoaiNhienLieu = Kho_DM_LoaiNhienLieus1.LoaiNhienLieu,
                                  Id_NhanHieu = Kho_DM_NhanHieus1.TenNhanHieu,
                                  Id_DonVi = Kho_DM_DonVis1.TenDonVi,
                                  KLXuatKhoCoThue = p1.KLXuatKhoCoThue,
                                  KLXuatKhoKhongThue = p1.KLXuatKhoKhongThue,
                                  DonViSauQuyDoi = p1.DonViSauQuyDoi,
                                  SLThucTeCoThue = p1.SLThucTeCoThue,
                                  SLThucTeKhongThue = p1.SLThucTeKhongThue,
                                  TongSoLuong = p1.TongSoLuong,
                                  DonGiaCoThue = p1.DonGiaCoThue,
                                  DonGiaKhongThue = p1.DonGiaKhongThue,
                                  ThanhTienCoThue = p1.ThanhTienCoThue,
                                  ThanhTienKhongThue = p1.ThanhTienKhongThue,
                                  ThanhTienTongTien = p1.ThanhTienTongTien,
                                  GroupId = p1.GroupId,
                                  Ordinarily = p1.Ordinarily,
                                  CreateAt = (DateTime)p1.CreateAt,
                                  CreateBy = p1.CreateBy,
                                  IsActive = p1.IsActive,
                                  ApprovalUserId = p1.ApprovalUserId,
                                  DateApproval = (DateTime)p1.DateApproval,
                                  ApprovalDept = p1.ApprovalDept,
                                  DepartmentId = p1.DepartmentId,
                                  DepartmentOrder = p1.DepartmentOrder,
                                  ApprovalOrder = p1.ApprovalOrder,
                                  ApprovalId = p1.ApprovalId,
                                  LastApprovalId = p1.LastApprovalId,
                                  IsStatus = p1.IsStatus,
                              }).FirstOrDefaultAsync();
            return data;
        }
        public async Task<List<Kho_XuatKhoNhienLieuModel>> GetHistory(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var data = await (from p1 in context.Kho_XuatKhoNhienLieu_Logs
                                  join ChiNhanhs1 in context.ChiNhanhs on p1.Id_TramTron equals ChiNhanhs1.Id into ChiNhanhs11
                                  from ChiNhanhs1 in ChiNhanhs11.DefaultIfEmpty()
                                  join Kho_DM_NhomNhienLieus1 in context.Kho_DM_NhomNhienLieus on p1.Id_NhomNhienLieu equals Kho_DM_NhomNhienLieus1.Id into Kho_DM_NhomNhienLieus11
                                  from Kho_DM_NhomNhienLieus1 in Kho_DM_NhomNhienLieus11.DefaultIfEmpty()
                                  join Kho_DM_LoaiNhienLieus1 in context.Kho_DM_LoaiNhienLieus on p1.Id_LoaiNhienLieu equals Kho_DM_LoaiNhienLieus1.Id into Kho_DM_LoaiNhienLieus11
                                  from Kho_DM_LoaiNhienLieus1 in Kho_DM_LoaiNhienLieus11.DefaultIfEmpty()
                                  join Kho_DM_NhanHieus1 in context.Kho_DM_NhanHieus on p1.Id_NhanHieu equals Kho_DM_NhanHieus1.Id into Kho_DM_NhanHieus11
                                  from Kho_DM_NhanHieus1 in Kho_DM_NhanHieus11.DefaultIfEmpty()
                                  join Kho_DM_DonVis1 in context.Kho_DM_DonVis on p1.Id_DonVi equals Kho_DM_DonVis1.Id into Kho_DM_DonVis11
                                  from Kho_DM_DonVis1 in Kho_DM_DonVis11.DefaultIfEmpty()
                                  join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                                  join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                                  from approvalUserId in a1.DefaultIfEmpty()
                                  join b in context.Departments on p1.DepartmentId equals b.Id into b1
                                  from departmentId in b1.DefaultIfEmpty()
                                  join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                                  from approvalDept in c1.DefaultIfEmpty()
                                  where p1.IdChung == id
                                  orderby p1.CreateAt
                                  select new Kho_XuatKhoNhienLieuModel
                                  {
                                      Id = p1.Id,
                                      Id_TramTron = ChiNhanhs1.TenChiNhanh,
                                      NgayNhapKho = p1.NgayNhapKho,
                                      ChotSoCoCay = p1.ChotSoCoCay,
                                      SoPhieu = p1.SoPhieu,
                                      ChotSoKMTruocKhiCap = p1.ChotSoKMTruocKhiCap,
                                      ChieuCaoBinhNhienLieu = p1.ChieuCaoBinhNhienLieu,
                                      DoiTuongNhan = p1.DoiTuongNhan,
                                      Id_NhomNhienLieu = Kho_DM_NhomNhienLieus1.TenNhienLieu,
                                      Id_LoaiNhienLieu = Kho_DM_LoaiNhienLieus1.LoaiNhienLieu,
                                      Id_NhanHieu = Kho_DM_NhanHieus1.TenNhanHieu,
                                      Id_DonVi = Kho_DM_DonVis1.TenDonVi,
                                      KLXuatKhoCoThue = p1.KLXuatKhoCoThue,
                                      KLXuatKhoKhongThue = p1.KLXuatKhoKhongThue,
                                      DonViSauQuyDoi = p1.DonViSauQuyDoi,
                                      SLThucTeCoThue = p1.SLThucTeCoThue,
                                      SLThucTeKhongThue = p1.SLThucTeKhongThue,
                                      TongSoLuong = p1.TongSoLuong,
                                      DonGiaCoThue = p1.DonGiaCoThue,
                                      DonGiaKhongThue = p1.DonGiaKhongThue,
                                      ThanhTienCoThue = p1.ThanhTienCoThue,
                                      ThanhTienKhongThue = p1.ThanhTienKhongThue,
                                      ThanhTienTongTien = p1.ThanhTienTongTien,
                                      GroupId = p1.GroupId,
                                      Ordinarily = p1.Ordinarily,
                                      CreateAt = (DateTime)p1.CreateAt,
                                      CreateBy = p1.CreateBy,
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
        public async Task<List<Kho_XuatKhoNhienLieu>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.Kho_XuatKhoNhienLieus.Where(p => p.IsActive != 100).OrderByDescending(p => p.CreateAt).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi khi lấy dữ liệu:  {ex.Message}");
                throw;
            }
        }
        public async Task<Kho_XuatKhoNhienLieu> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.Kho_XuatKhoNhienLieus.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn : ");
            }
            return entity;
        }
        public async Task Insert(Kho_XuatKhoNhienLieu entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có dữ liệu dưọc thêm!");
                }
                context.Kho_XuatKhoNhienLieus.Add(entity);
                var addLog = new Kho_XuatKhoNhienLieu_Log()
                {
                    Id_TramTron = entity.Id_TramTron,
                    NgayNhapKho = entity.NgayNhapKho,
                    ChotSoCoCay = entity.ChotSoCoCay,
                    SoPhieu = entity.SoPhieu,
                    ChotSoKMTruocKhiCap = entity.ChotSoKMTruocKhiCap,
                    ChieuCaoBinhNhienLieu = entity.ChieuCaoBinhNhienLieu,
                    DoiTuongNhan = entity.DoiTuongNhan,
                    Id_NhomNhienLieu = entity.Id_NhomNhienLieu,
                    Id_LoaiNhienLieu = entity.Id_LoaiNhienLieu,
                    Id_NhanHieu = entity.Id_NhanHieu,
                    Id_DonVi = entity.Id_DonVi,
                    KLXuatKhoCoThue = entity.KLXuatKhoCoThue,
                    KLXuatKhoKhongThue = entity.KLXuatKhoKhongThue,
                    DonViSauQuyDoi = entity.DonViSauQuyDoi,
                    SLThucTeCoThue = entity.SLThucTeCoThue,
                    SLThucTeKhongThue = entity.SLThucTeKhongThue,
                    TongSoLuong = entity.TongSoLuong,
                    DonGiaCoThue = entity.DonGiaCoThue,
                    DonGiaKhongThue = entity.DonGiaKhongThue,
                    ThanhTienCoThue = entity.ThanhTienCoThue,
                    ThanhTienKhongThue = entity.ThanhTienKhongThue,
                    ThanhTienTongTien = entity.ThanhTienTongTien,
                    GroupId = entity.GroupId,
                    Ordinarily = entity.Ordinarily,
                    CreateAt = entity.CreateAt,
                    CreateBy = entity.CreateBy,
                    IsActive = entity.IsActive,
                    ApprovalUserId = entity.ApprovalUserId,
                    DateApproval = entity.DateApproval,
                    ApprovalDept = entity.ApprovalDept,
                    DepartmentId = entity.DepartmentId,
                    DepartmentOrder = entity.DepartmentOrder,
                    ApprovalOrder = entity.ApprovalOrder,
                    ApprovalId = entity.ApprovalId,
                    LastApprovalId = entity.LastApprovalId,
                    IsStatus = entity.IsStatus,
                    IdChung = entity.Id,
                };
                context.Kho_XuatKhoNhienLieu_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(Kho_XuatKhoNhienLieu data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn !");
            }
            context.Kho_XuatKhoNhienLieus.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.Kho_XuatKhoNhienLieu_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.Kho_XuatKhoNhienLieu_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.Kho_XuatKhoNhienLieu_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.Kho_XuatKhoNhienLieu_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.Kho_XuatKhoNhienLieu_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt).FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.Kho_XuatKhoNhienLieu_Logs.Update(updateLog);
                }
            }
            var addLog = new Kho_XuatKhoNhienLieu_Log
            {
                Id_TramTron = data.Id_TramTron,
                NgayNhapKho = data.NgayNhapKho,
                ChotSoCoCay = data.ChotSoCoCay,
                SoPhieu = data.SoPhieu,
                ChotSoKMTruocKhiCap = data.ChotSoKMTruocKhiCap,
                ChieuCaoBinhNhienLieu = data.ChieuCaoBinhNhienLieu,
                DoiTuongNhan = data.DoiTuongNhan,
                Id_NhomNhienLieu = data.Id_NhomNhienLieu,
                Id_LoaiNhienLieu = data.Id_LoaiNhienLieu,
                Id_NhanHieu = data.Id_NhanHieu,
                Id_DonVi = data.Id_DonVi,
                KLXuatKhoCoThue = data.KLXuatKhoCoThue,
                KLXuatKhoKhongThue = data.KLXuatKhoKhongThue,
                DonViSauQuyDoi = data.DonViSauQuyDoi,
                SLThucTeCoThue = data.SLThucTeCoThue,
                SLThucTeKhongThue = data.SLThucTeKhongThue,
                TongSoLuong = data.TongSoLuong,
                DonGiaCoThue = data.DonGiaCoThue,
                DonGiaKhongThue = data.DonGiaKhongThue,
                ThanhTienCoThue = data.ThanhTienCoThue,
                ThanhTienKhongThue = data.ThanhTienKhongThue,
                ThanhTienTongTien = data.ThanhTienTongTien,
                GroupId = data.GroupId,
                Ordinarily = data.Ordinarily,
                CreateAt = data.CreateAt,
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
            context.Kho_XuatKhoNhienLieu_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(Kho_XuatKhoNhienLieu[] Kho_XuatKhoNhienLieus)
        {
            using var context = _context.CreateDbContext();
            string[] ids = Kho_XuatKhoNhienLieus.Select(x => x.Id).ToArray();
            var listEntities = await context.Kho_XuatKhoNhienLieus.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.Kho_XuatKhoNhienLieus.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task Delete(Kho_XuatKhoNhienLieu data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chonj!");
            }
            else
            {
                if (entity.IsActive == 0)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Ðã xóa";
                    data.ApprovalUserId = userId;
                }
                else if (entity.IsActive == 1)
                {
                    var logdata = (from p in context.Kho_XuatKhoNhienLieu_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        data.Id_TramTron = logdata.Id_TramTron;
                        data.NgayNhapKho = logdata.NgayNhapKho;
                        data.ChotSoCoCay = logdata.ChotSoCoCay;
                        data.SoPhieu = logdata.SoPhieu;
                        data.ChotSoKMTruocKhiCap = logdata.ChotSoKMTruocKhiCap;
                        data.ChieuCaoBinhNhienLieu = logdata.ChieuCaoBinhNhienLieu;
                        data.DoiTuongNhan = logdata.DoiTuongNhan;
                        data.Id_NhomNhienLieu = logdata.Id_NhomNhienLieu;
                        data.Id_LoaiNhienLieu = logdata.Id_LoaiNhienLieu;
                        data.Id_NhanHieu = logdata.Id_NhanHieu;
                        data.Id_DonVi = logdata.Id_DonVi;
                        data.KLXuatKhoCoThue = logdata.KLXuatKhoCoThue;
                        data.KLXuatKhoKhongThue = logdata.KLXuatKhoKhongThue;
                        data.DonViSauQuyDoi = logdata.DonViSauQuyDoi;
                        data.SLThucTeCoThue = logdata.SLThucTeCoThue;
                        data.SLThucTeKhongThue = logdata.SLThucTeKhongThue;
                        data.TongSoLuong = logdata.TongSoLuong;
                        data.DonGiaCoThue = logdata.DonGiaCoThue;
                        data.DonGiaKhongThue = logdata.DonGiaKhongThue;
                        data.ThanhTienCoThue = logdata.ThanhTienCoThue;
                        data.ThanhTienKhongThue = logdata.ThanhTienKhongThue;
                        data.ThanhTienTongTien = logdata.ThanhTienTongTien;
                        data.GroupId = logdata.GroupId;
                        data.Ordinarily = logdata.Ordinarily;
                        data.CreateAt = logdata.CreateAt;
                        data.CreateBy = logdata.CreateBy;
                        data.IsActive = logdata.IsActive;
                        data.ApprovalUserId = logdata.ApprovalUserId;
                        data.DateApproval = logdata.DateApproval;
                        data.ApprovalDept = logdata.ApprovalDept;
                        data.DepartmentId = logdata.DepartmentId;
                        data.DepartmentOrder = logdata.DepartmentOrder;
                        data.ApprovalOrder = logdata.ApprovalOrder;
                        data.ApprovalId = logdata.ApprovalId;
                        data.LastApprovalId = logdata.LastApprovalId;
                        data.IsStatus = logdata.IsStatus;
                        data.ApprovalUserId = "";
                        data.DateApproval = null;
                        logdata.IsValid = true;
                        context.Kho_XuatKhoNhienLieu_Logs.Update(logdata);
                    }
                    var logupdate = (from p in context.Kho_XuatKhoNhienLieu_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.Kho_XuatKhoNhienLieu_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new Kho_XuatKhoNhienLieu_Log()
                    {
                        Id_TramTron = data.Id_TramTron,
                        NgayNhapKho = data.NgayNhapKho,
                        ChotSoCoCay = data.ChotSoCoCay,
                        SoPhieu = data.SoPhieu,
                        ChotSoKMTruocKhiCap = data.ChotSoKMTruocKhiCap,
                        ChieuCaoBinhNhienLieu = data.ChieuCaoBinhNhienLieu,
                        DoiTuongNhan = data.DoiTuongNhan,
                        Id_NhomNhienLieu = data.Id_NhomNhienLieu,
                        Id_LoaiNhienLieu = data.Id_LoaiNhienLieu,
                        Id_NhanHieu = data.Id_NhanHieu,
                        Id_DonVi = data.Id_DonVi,
                        KLXuatKhoCoThue = data.KLXuatKhoCoThue,
                        KLXuatKhoKhongThue = data.KLXuatKhoKhongThue,
                        DonViSauQuyDoi = data.DonViSauQuyDoi,
                        SLThucTeCoThue = data.SLThucTeCoThue,
                        SLThucTeKhongThue = data.SLThucTeKhongThue,
                        TongSoLuong = data.TongSoLuong,
                        DonGiaCoThue = data.DonGiaCoThue,
                        DonGiaKhongThue = data.DonGiaKhongThue,
                        ThanhTienCoThue = data.ThanhTienCoThue,
                        ThanhTienKhongThue = data.ThanhTienKhongThue,
                        ThanhTienTongTien = data.ThanhTienTongTien,
                        GroupId = data.GroupId,
                        Ordinarily = data.Ordinarily,
                        CreateAt = data.CreateAt,
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
                        IsValid = true,
                        IdChung = data.Id
                    };
                    addLog.Ordinarily = logdata != null ? logdata.Ordinarily : addLog.Ordinarily;
                    context.Kho_XuatKhoNhienLieu_Logs.Add(addLog);
                }
                else if (entity.IsActive == 2)
                {
                    throw new Exception($"Thông tin đang chờ duyệt xóa!");
                }
                else if (entity.IsActive == 3)
                {
                    var logupdate = (from p in context.Kho_XuatKhoNhienLieu_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.Kho_XuatKhoNhienLieu_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new Kho_XuatKhoNhienLieu_Log()
                    {
                        Id_TramTron = data.Id_TramTron,
                        NgayNhapKho = data.NgayNhapKho,
                        ChotSoCoCay = data.ChotSoCoCay,
                        SoPhieu = data.SoPhieu,
                        ChotSoKMTruocKhiCap = data.ChotSoKMTruocKhiCap,
                        ChieuCaoBinhNhienLieu = data.ChieuCaoBinhNhienLieu,
                        DoiTuongNhan = data.DoiTuongNhan,
                        Id_NhomNhienLieu = data.Id_NhomNhienLieu,
                        Id_LoaiNhienLieu = data.Id_LoaiNhienLieu,
                        Id_NhanHieu = data.Id_NhanHieu,
                        Id_DonVi = data.Id_DonVi,
                        KLXuatKhoCoThue = data.KLXuatKhoCoThue,
                        KLXuatKhoKhongThue = data.KLXuatKhoKhongThue,
                        DonViSauQuyDoi = data.DonViSauQuyDoi,
                        SLThucTeCoThue = data.SLThucTeCoThue,
                        SLThucTeKhongThue = data.SLThucTeKhongThue,
                        TongSoLuong = data.TongSoLuong,
                        DonGiaCoThue = data.DonGiaCoThue,
                        DonGiaKhongThue = data.DonGiaKhongThue,
                        ThanhTienCoThue = data.ThanhTienCoThue,
                        ThanhTienKhongThue = data.ThanhTienKhongThue,
                        ThanhTienTongTien = data.ThanhTienTongTien,
                        GroupId = data.GroupId,
                        Ordinarily = data.Ordinarily,
                        CreateAt = data.CreateAt,
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
                        IsValid = true
                    };
                    context.Kho_XuatKhoNhienLieu_Logs.Add(addLog);
                }
                else if (entity.IsActive == 90)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa không duyệt !";
                }
            }
            context.Kho_XuatKhoNhienLieus.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task Approval(Kho_XuatKhoNhienLieu data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chonj!");
            }
            context.Kho_XuatKhoNhienLieus.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.Kho_XuatKhoNhienLieu_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.Kho_XuatKhoNhienLieu_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.Kho_XuatKhoNhienLieu_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.Kho_XuatKhoNhienLieu_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.Kho_XuatKhoNhienLieu_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt).FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.Kho_XuatKhoNhienLieu_Logs.Update(updateLog);
                }
            }
            var addLog = new Kho_XuatKhoNhienLieu_Log()
            {
                Id_TramTron = data.Id_TramTron,
                NgayNhapKho = data.NgayNhapKho,
                ChotSoCoCay = data.ChotSoCoCay,
                SoPhieu = data.SoPhieu,
                ChotSoKMTruocKhiCap = data.ChotSoKMTruocKhiCap,
                ChieuCaoBinhNhienLieu = data.ChieuCaoBinhNhienLieu,
                DoiTuongNhan = data.DoiTuongNhan,
                Id_NhomNhienLieu = data.Id_NhomNhienLieu,
                Id_LoaiNhienLieu = data.Id_LoaiNhienLieu,
                Id_NhanHieu = data.Id_NhanHieu,
                Id_DonVi = data.Id_DonVi,
                KLXuatKhoCoThue = data.KLXuatKhoCoThue,
                KLXuatKhoKhongThue = data.KLXuatKhoKhongThue,
                DonViSauQuyDoi = data.DonViSauQuyDoi,
                SLThucTeCoThue = data.SLThucTeCoThue,
                SLThucTeKhongThue = data.SLThucTeKhongThue,
                TongSoLuong = data.TongSoLuong,
                DonGiaCoThue = data.DonGiaCoThue,
                DonGiaKhongThue = data.DonGiaKhongThue,
                ThanhTienCoThue = data.ThanhTienCoThue,
                ThanhTienKhongThue = data.ThanhTienKhongThue,
                ThanhTienTongTien = data.ThanhTienTongTien,
                GroupId = data.GroupId,
                Ordinarily = data.Ordinarily,
                CreateAt = data.CreateAt,
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
            context.Kho_XuatKhoNhienLieu_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task NoApproval(Kho_XuatKhoNhienLieu data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null || entity.IsActive != data.IsActive)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chonj!");
            }
            else
            {
                if (entity.IsActive == 0)
                {
                    entity.IsActive = 90;
                    entity.IsStatus = "Không duyệt!";
                    entity.ApprovalUserId = data.ApprovalUserId;
                }
                else if (entity.IsActive == 1)
                {
                    var logdata = (from p in context.Kho_XuatKhoNhienLieu_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.Id_TramTron = logdata.Id_TramTron;
                        entity.NgayNhapKho = logdata.NgayNhapKho;
                        entity.ChotSoCoCay = logdata.ChotSoCoCay;
                        entity.SoPhieu = logdata.SoPhieu;
                        entity.ChotSoKMTruocKhiCap = logdata.ChotSoKMTruocKhiCap;
                        entity.ChieuCaoBinhNhienLieu = logdata.ChieuCaoBinhNhienLieu;
                        entity.DoiTuongNhan = logdata.DoiTuongNhan;
                        entity.Id_NhomNhienLieu = logdata.Id_NhomNhienLieu;
                        entity.Id_LoaiNhienLieu = logdata.Id_LoaiNhienLieu;
                        entity.Id_NhanHieu = logdata.Id_NhanHieu;
                        entity.Id_DonVi = logdata.Id_DonVi;
                        entity.KLXuatKhoCoThue = logdata.KLXuatKhoCoThue;
                        entity.KLXuatKhoKhongThue = logdata.KLXuatKhoKhongThue;
                        entity.DonViSauQuyDoi = logdata.DonViSauQuyDoi;
                        entity.SLThucTeCoThue = logdata.SLThucTeCoThue;
                        entity.SLThucTeKhongThue = logdata.SLThucTeKhongThue;
                        entity.TongSoLuong = logdata.TongSoLuong;
                        entity.DonGiaCoThue = logdata.DonGiaCoThue;
                        entity.DonGiaKhongThue = logdata.DonGiaKhongThue;
                        entity.ThanhTienCoThue = logdata.ThanhTienCoThue;
                        entity.ThanhTienKhongThue = logdata.ThanhTienKhongThue;
                        entity.ThanhTienTongTien = logdata.ThanhTienTongTien;
                        entity.GroupId = logdata.GroupId;
                        entity.Ordinarily = logdata.Ordinarily;
                        entity.CreateAt = logdata.CreateAt;
                        entity.CreateBy = logdata.CreateBy;
                        entity.IsActive = logdata.IsActive;
                        entity.ApprovalUserId = logdata.ApprovalUserId;
                        entity.DateApproval = logdata.DateApproval;
                        entity.ApprovalDept = logdata.ApprovalDept;
                        entity.DepartmentId = logdata.DepartmentId;
                        entity.DepartmentOrder = logdata.DepartmentOrder;
                        entity.ApprovalOrder = logdata.ApprovalOrder;
                        entity.ApprovalId = logdata.ApprovalId;
                        entity.LastApprovalId = logdata.LastApprovalId;
                        entity.IsStatus = logdata.IsStatus;
                    }
                    entity.IsActive = 3;
                    entity.IsStatus = "Đã duyệt";
                    var logupdate = (from p in context.Kho_XuatKhoNhienLieu_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true && p.CreateAt >= logdata.CreateAt
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.Kho_XuatKhoNhienLieu_Logs.UpdateRange(logupdate);
                    }
                }
                else if (entity.IsActive == 2)
                {
                    var logdata = (from p in context.Kho_XuatKhoNhienLieu_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.Id_TramTron = logdata.Id_TramTron;
                        entity.NgayNhapKho = logdata.NgayNhapKho;
                        entity.ChotSoCoCay = logdata.ChotSoCoCay;
                        entity.SoPhieu = logdata.SoPhieu;
                        entity.ChotSoKMTruocKhiCap = logdata.ChotSoKMTruocKhiCap;
                        entity.ChieuCaoBinhNhienLieu = logdata.ChieuCaoBinhNhienLieu;
                        entity.DoiTuongNhan = logdata.DoiTuongNhan;
                        entity.Id_NhomNhienLieu = logdata.Id_NhomNhienLieu;
                        entity.Id_LoaiNhienLieu = logdata.Id_LoaiNhienLieu;
                        entity.Id_NhanHieu = logdata.Id_NhanHieu;
                        entity.Id_DonVi = logdata.Id_DonVi;
                        entity.KLXuatKhoCoThue = logdata.KLXuatKhoCoThue;
                        entity.KLXuatKhoKhongThue = logdata.KLXuatKhoKhongThue;
                        entity.DonViSauQuyDoi = logdata.DonViSauQuyDoi;
                        entity.SLThucTeCoThue = logdata.SLThucTeCoThue;
                        entity.SLThucTeKhongThue = logdata.SLThucTeKhongThue;
                        entity.TongSoLuong = logdata.TongSoLuong;
                        entity.DonGiaCoThue = logdata.DonGiaCoThue;
                        entity.DonGiaKhongThue = logdata.DonGiaKhongThue;
                        entity.ThanhTienCoThue = logdata.ThanhTienCoThue;
                        entity.ThanhTienKhongThue = logdata.ThanhTienKhongThue;
                        entity.ThanhTienTongTien = logdata.ThanhTienTongTien;
                        entity.GroupId = data.GroupId;
                        entity.Ordinarily = data.Ordinarily;
                        entity.CreateAt = data.CreateAt;
                        entity.CreateBy = data.CreateBy;
                        entity.IsActive = data.IsActive;
                        entity.ApprovalUserId = data.ApprovalUserId;
                        entity.DateApproval = data.DateApproval;
                        entity.ApprovalDept = data.ApprovalDept;
                        entity.DepartmentId = data.DepartmentId;
                        entity.DepartmentOrder = data.DepartmentOrder;
                        entity.ApprovalOrder = data.ApprovalOrder;
                        entity.ApprovalId = data.ApprovalId;
                        entity.LastApprovalId = data.LastApprovalId;
                        entity.IsStatus = data.IsStatus;
                    }
                    entity.IsActive = 3;
                    entity.IsStatus = "Đã duyệt!";
                }
                else if (entity.IsActive == 3)
                {
                    throw new Exception($"Thông tin hủy duyệt không tồn tại !");
                }
            }
            var addLog = new Kho_XuatKhoNhienLieu_Log()
            {
                Id_TramTron = data.Id_TramTron,
                NgayNhapKho = data.NgayNhapKho,
                ChotSoCoCay = data.ChotSoCoCay,
                SoPhieu = data.SoPhieu,
                ChotSoKMTruocKhiCap = data.ChotSoKMTruocKhiCap,
                ChieuCaoBinhNhienLieu = data.ChieuCaoBinhNhienLieu,
                DoiTuongNhan = data.DoiTuongNhan,
                Id_NhomNhienLieu = data.Id_NhomNhienLieu,
                Id_LoaiNhienLieu = data.Id_LoaiNhienLieu,
                Id_NhanHieu = data.Id_NhanHieu,
                Id_DonVi = data.Id_DonVi,
                KLXuatKhoCoThue = data.KLXuatKhoCoThue,
                KLXuatKhoKhongThue = data.KLXuatKhoKhongThue,
                DonViSauQuyDoi = data.DonViSauQuyDoi,
                SLThucTeCoThue = data.SLThucTeCoThue,
                SLThucTeKhongThue = data.SLThucTeKhongThue,
                TongSoLuong = data.TongSoLuong,
                DonGiaCoThue = data.DonGiaCoThue,
                DonGiaKhongThue = data.DonGiaKhongThue,
                ThanhTienCoThue = data.ThanhTienCoThue,
                ThanhTienKhongThue = data.ThanhTienKhongThue,
                ThanhTienTongTien = data.ThanhTienTongTien,
                GroupId = data.GroupId,
                Ordinarily = data.Ordinarily,
                CreateAt = data.CreateAt,
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
                IsValid = false
            };
            context.Kho_XuatKhoNhienLieu_Logs.Add(addLog);
            context.Kho_XuatKhoNhienLieus.Update(entity);
            await context.SaveChangesAsync();
        }
        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chonj!");
            }
            context.Set<Kho_XuatKhoNhienLieu>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.Kho_XuatKhoNhienLieus.Where(p => p.Id == ids && p.IsActive != 100).FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chonj!");
            }
            if (model != null && model.IsActive == 0)
            {
                throw new Exception($"Đang chờ duyệt thêm mới !");
            }
            if (model != null && model.IsActive == 1)
            {
                throw new Exception($"Đang chờ duyệt sửa !");
            }
            if (model != null && model.IsActive == 2)
            {
                throw new Exception($"Đang chờ duyệt xóa !");
            }
            return true;
        }
        public async Task<bool> CheckSave(Kho_XuatKhoNhienLieu input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.Kho_XuatKhoNhienLieu_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100
                            && p.Id_TramTron == input.Id_TramTron
                            && p.NgayNhapKho == input.NgayNhapKho
                            && p.ChotSoCoCay == input.ChotSoCoCay
                            && p.SoPhieu == input.SoPhieu
                            && p.ChotSoKMTruocKhiCap == input.ChotSoKMTruocKhiCap
                            && p.ChieuCaoBinhNhienLieu == input.ChieuCaoBinhNhienLieu
                            && p.DoiTuongNhan == input.DoiTuongNhan
                            && p.Id_NhomNhienLieu == input.Id_NhomNhienLieu
                            && p.Id_LoaiNhienLieu == input.Id_LoaiNhienLieu
                            && p.Id_NhanHieu == input.Id_NhanHieu
                            && p.Id_DonVi == input.Id_DonVi
                            && p.KLXuatKhoCoThue == input.KLXuatKhoCoThue
                            && p.KLXuatKhoKhongThue == input.KLXuatKhoKhongThue
                            && p.DonViSauQuyDoi == input.DonViSauQuyDoi
                            && p.SLThucTeCoThue == input.SLThucTeCoThue
                            && p.SLThucTeKhongThue == input.SLThucTeKhongThue
                            && p.TongSoLuong == input.TongSoLuong
                            && p.DonGiaCoThue == input.DonGiaCoThue
                            && p.DonGiaKhongThue == input.DonGiaKhongThue
                            && p.ThanhTienCoThue == input.ThanhTienCoThue
                            && p.ThanhTienKhongThue == input.ThanhTienKhongThue
                            && p.ThanhTienTongTien == input.ThanhTienTongTien

                                   select p).CountAsync();
                if (model > 0)
                {
                    throw new Exception("Thông tin bạn nhập đã tồn tại !");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<bool> CheckEdit(Kho_XuatKhoNhienLieu input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.Kho_XuatKhoNhienLieu_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100 && p.Id != input.Id
                            && p.Id_TramTron == input.Id_TramTron
                            && p.NgayNhapKho == input.NgayNhapKho
                            && p.ChotSoCoCay == input.ChotSoCoCay
                            && p.SoPhieu == input.SoPhieu
                            && p.ChotSoKMTruocKhiCap == input.ChotSoKMTruocKhiCap
                            && p.ChieuCaoBinhNhienLieu == input.ChieuCaoBinhNhienLieu
                            && p.DoiTuongNhan == input.DoiTuongNhan
                            && p.Id_NhomNhienLieu == input.Id_NhomNhienLieu
                            && p.Id_LoaiNhienLieu == input.Id_LoaiNhienLieu
                            && p.Id_NhanHieu == input.Id_NhanHieu
                            && p.Id_DonVi == input.Id_DonVi
                            && p.KLXuatKhoCoThue == input.KLXuatKhoCoThue
                            && p.KLXuatKhoKhongThue == input.KLXuatKhoKhongThue
                            && p.DonViSauQuyDoi == input.DonViSauQuyDoi
                            && p.SLThucTeCoThue == input.SLThucTeCoThue
                            && p.SLThucTeKhongThue == input.SLThucTeKhongThue
                            && p.TongSoLuong == input.TongSoLuong
                            && p.DonGiaCoThue == input.DonGiaCoThue
                            && p.DonGiaKhongThue == input.DonGiaKhongThue
                            && p.ThanhTienCoThue == input.ThanhTienCoThue
                            && p.ThanhTienKhongThue == input.ThanhTienKhongThue
                            && p.ThanhTienTongTien == input.ThanhTienTongTien
                                   select p).CountAsync();
                if (model > 0)
                {
                    throw new Exception("Thông tin bạn nhập đã tồn tại !");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<bool> CheckDelete(Kho_XuatKhoNhienLieu input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (1 == 0)
                {
                    throw new Exception("");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public async Task<bool> CheckExclusive(string[] ids, DateTime baseTime)
        {
            foreach (var id in ids)
            {
                var model = await GetById(id);
                if (model == null)
                {
                    throw new Exception($"Không tìm thấy dữ liệu đã chonj!");
                }
                if (model.CreateAt > baseTime)
                {
                    throw new Exception($"Thông tin đã đuợc thay đổi bởi người khác . Vui lòng tải lại trang !");
                }
            }
            return true;
        }
    }
}
