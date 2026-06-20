using DucAnh2025.Data;
using DucAnh2025.Models.Kho;
using DucAnh2025.Repository.Kho;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.Kho;
using Microsoft.EntityFrameworkCore;
namespace DucAnh2025.Services.Kho
{
    public class Kho_HDMuaNhienLieuRepository : IKho_HDMuaNhienLieuRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public Kho_HDMuaNhienLieuRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public async Task<List<Kho_HDMuaNhienLieuModel>> GetAllByVM(Kho_HDMuaNhienLieuModel dataModel, string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();

                var baseQuery = context.Kho_HDMuaNhienLieus
                    .AsNoTracking()
                    .Where(p => p.GroupId == groupId && p.IsActive != 100);

                if (!string.IsNullOrEmpty(dataModel?.Id_TenTram))
                {
                    baseQuery = baseQuery.Where(m => m.Id_TenTram == dataModel.Id_TenTram);
                }
                if (!string.IsNullOrEmpty(dataModel?.Id_LoaiNhaCungCap))
                {
                    baseQuery = baseQuery.Where(m => m.Id_LoaiNhaCungCap == dataModel.Id_LoaiNhaCungCap);
                }
                if (!string.IsNullOrEmpty(dataModel?.Id_NhaCungcap))
                {
                    baseQuery = baseQuery.Where(m => m.Id_NhaCungcap == dataModel.Id_NhaCungcap);
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
                var Kho_DM_LoaiNhaCungCapsDict = await context.Kho_DM_LoaiNhaCungCaps.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_NhaCungCapsDict = await context.Kho_DM_NhaCungCaps.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_NhomNhienLieusDict = await context.Kho_DM_NhomNhienLieus.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_LoaiNhienLieusDict = await context.Kho_DM_LoaiNhienLieus.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_NhanHieusDict = await context.Kho_DM_NhanHieus.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_DonVisDict = await context.Kho_DM_DonVis.AsNoTracking().ToDictionaryAsync(x => x.Id);

                var data = baseData.Select(p1 => new Kho_HDMuaNhienLieuModel
                {
                    Id = p1.Id,
                    Id_TenTram = ChiNhanhsDict.TryGetValue(p1.Id_TenTram, out var tmpChiNhanhs) ? tmpChiNhanhs.TenChiNhanh : p1.Id_TenTram,
                    NgayKyHopDong = p1.NgayKyHopDong,
                    SoHopDong = p1.SoHopDong,
                    Id_LoaiNhaCungCap = Kho_DM_LoaiNhaCungCapsDict.TryGetValue(p1.Id_LoaiNhaCungCap, out var tmpKho_DM_LoaiNhaCungCaps) ? tmpKho_DM_LoaiNhaCungCaps.TenLoaiNhaCungCap : "",
                    Id_NhaCungcap = Kho_DM_NhaCungCapsDict.TryGetValue(p1.Id_NhaCungcap, out var tmpKho_DM_NhaCungCaps) ? tmpKho_DM_NhaCungCaps.TenNhaCungcap : "",
                    DiaChi = Kho_DM_NhaCungCapsDict.TryGetValue(p1.Id_NhaCungcap, out var tmpKho_DM_DiaChi) ? tmpKho_DM_DiaChi.DiaChi : "",
                    MaSoThue = Kho_DM_NhaCungCapsDict.TryGetValue(p1.Id_NhaCungcap, out var tmpKho_DM_MST) ? tmpKho_DM_MST.MaSoThue : "",
                    Id_NhomNhienLieu = Kho_DM_NhomNhienLieusDict.TryGetValue(p1.Id_NhomNhienLieu, out var tmpKho_DM_NhomNhienLieus) ? tmpKho_DM_NhomNhienLieus.TenNhienLieu : "",
                    Id_LoaiNhienLieu = Kho_DM_LoaiNhienLieusDict.TryGetValue(p1.Id_LoaiNhienLieu, out var tmpKho_DM_LoaiNhienLieus) ? tmpKho_DM_LoaiNhienLieus.LoaiNhienLieu : "",
                    Id_NhanHieu = Kho_DM_NhanHieusDict.TryGetValue(p1.Id_NhanHieu, out var tmpKho_DM_NhanHieus) ? tmpKho_DM_NhanHieus.TenNhanHieu : "",
                    Id_DonVi = Kho_DM_DonVisDict.TryGetValue(p1.Id_DonVi, out var tmpKho_DM_DonVis) ? tmpKho_DM_DonVis.TenDonVi : "",
                    DonGiaBGCoThue = p1.DonGiaBGCoThue,
                    DonGiaBGKhongThue = p1.DonGiaBGKhongThue,
                    DonGiaKBCoThue = p1.DonGiaKBCoThue,
                    DonGiaKBKhongThue = p1.DonGiaKBKhongThue,
                    TuNgay = p1.TuNgay,
                    DenNgay = p1.DenNgay,
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
        public async Task<List<Kho_HDMuaNhienLieuModel>> GetHistoryIsValidEdit(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var query = (from p1 in context.Kho_HDMuaNhienLieu_Logs
                             join ChiNhanhs1 in context.ChiNhanhs on p1.Id_TenTram equals ChiNhanhs1.Id
                             join Kho_DM_LoaiNhaCungCaps1 in context.Kho_DM_LoaiNhaCungCaps on p1.Id_LoaiNhaCungCap equals Kho_DM_LoaiNhaCungCaps1.Id
                             join Kho_DM_NhaCungCaps1 in context.Kho_DM_NhaCungCaps on p1.Id_NhaCungcap equals Kho_DM_NhaCungCaps1.Id
                             join Kho_DM_NhomNhienLieus1 in context.Kho_DM_NhomNhienLieus on p1.Id_NhomNhienLieu equals Kho_DM_NhomNhienLieus1.Id
                             join Kho_DM_LoaiNhienLieus1 in context.Kho_DM_LoaiNhienLieus on p1.Id_LoaiNhienLieu equals Kho_DM_LoaiNhienLieus1.Id
                             join Kho_DM_NhanHieus1 in context.Kho_DM_NhanHieus on p1.Id_NhanHieu equals Kho_DM_NhanHieus1.Id
                             join Kho_DM_DonVis1 in context.Kho_DM_DonVis on p1.Id_DonVi equals Kho_DM_DonVis1.Id
                             join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                             join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                             where p1.IdChung == id && p1.IsValid == true
                             orderby p1.CreateAt
                             select new Kho_HDMuaNhienLieuModel
                             {
                                 Id = p1.Id,
                                 Id_TenTram = ChiNhanhs1.TenChiNhanh,
                                 NgayKyHopDong = p1.NgayKyHopDong,
                                 SoHopDong = p1.SoHopDong,
                                 Id_LoaiNhaCungCap = Kho_DM_LoaiNhaCungCaps1.TenLoaiNhaCungCap,
                                 Id_NhaCungcap = Kho_DM_NhaCungCaps1.TenNhaCungcap,
                                 DiaChi = Kho_DM_NhaCungCaps1.DiaChi,
                                 MaSoThue = Kho_DM_NhaCungCaps1.MaSoThue,
                                 Id_NhomNhienLieu = Kho_DM_NhomNhienLieus1.TenNhienLieu,
                                 Id_LoaiNhienLieu = Kho_DM_LoaiNhienLieus1.LoaiNhienLieu,
                                 Id_NhanHieu = Kho_DM_NhanHieus1.TenNhanHieu,
                                 Id_DonVi = Kho_DM_DonVis1.TenDonVi,
                                 DonGiaBGCoThue = p1.DonGiaBGCoThue,
                                 DonGiaBGKhongThue = p1.DonGiaBGKhongThue,
                                 DonGiaKBCoThue = p1.DonGiaKBCoThue,
                                 DonGiaKBKhongThue = p1.DonGiaKBKhongThue,
                                 TuNgay = p1.TuNgay,
                                 DenNgay = p1.DenNgay,
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
        public async Task<Kho_HDMuaNhienLieuModel> GetDetails(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.Kho_HDMuaNhienLieus
                              join ChiNhanhs1 in context.ChiNhanhs on p1.Id_TenTram equals ChiNhanhs1.Id into ChiNhanhs11
                              from ChiNhanhs1 in ChiNhanhs11.DefaultIfEmpty()
                              join Kho_DM_LoaiNhaCungCaps1 in context.Kho_DM_LoaiNhaCungCaps on p1.Id_LoaiNhaCungCap equals Kho_DM_LoaiNhaCungCaps1.Id into Kho_DM_LoaiNhaCungCaps11
                              from Kho_DM_LoaiNhaCungCaps1 in Kho_DM_LoaiNhaCungCaps11.DefaultIfEmpty()
                              join Kho_DM_NhaCungCaps1 in context.Kho_DM_NhaCungCaps on p1.Id_NhaCungcap equals Kho_DM_NhaCungCaps1.Id into Kho_DM_NhaCungCaps11
                              from Kho_DM_NhaCungCaps1 in Kho_DM_NhaCungCaps11.DefaultIfEmpty()
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
                              select new Kho_HDMuaNhienLieuModel
                              {
                                  Id = p1.Id,
                                  Id_TenTram = ChiNhanhs1.TenChiNhanh,
                                  NgayKyHopDong = p1.NgayKyHopDong,
                                  SoHopDong = p1.SoHopDong,
                                  Id_LoaiNhaCungCap = Kho_DM_LoaiNhaCungCaps1.TenLoaiNhaCungCap,
                                  Id_NhaCungcap = Kho_DM_NhaCungCaps1.TenNhaCungcap,
                                  DiaChi = Kho_DM_NhaCungCaps1.DiaChi,
                                  MaSoThue = Kho_DM_NhaCungCaps1.MaSoThue,
                                  Id_NhomNhienLieu = Kho_DM_NhomNhienLieus1.TenNhienLieu,
                                  Id_LoaiNhienLieu = Kho_DM_LoaiNhienLieus1.LoaiNhienLieu,
                                  Id_NhanHieu = Kho_DM_NhanHieus1.TenNhanHieu,
                                  Id_DonVi = Kho_DM_DonVis1.TenDonVi,
                                  DonGiaBGCoThue = p1.DonGiaBGCoThue,
                                  DonGiaBGKhongThue = p1.DonGiaBGKhongThue,
                                  DonGiaKBCoThue = p1.DonGiaKBCoThue,
                                  DonGiaKBKhongThue = p1.DonGiaKBKhongThue,
                                  TuNgay = p1.TuNgay,
                                  DenNgay = p1.DenNgay,
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
        public async Task<List<Kho_HDMuaNhienLieuModel>> GetHistory(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var data = await (from p1 in context.Kho_HDMuaNhienLieu_Logs
                                  join ChiNhanhs1 in context.ChiNhanhs on p1.Id_TenTram equals ChiNhanhs1.Id into ChiNhanhs11
                                  from ChiNhanhs1 in ChiNhanhs11.DefaultIfEmpty()
                                  join Kho_DM_LoaiNhaCungCaps1 in context.Kho_DM_LoaiNhaCungCaps on p1.Id_LoaiNhaCungCap equals Kho_DM_LoaiNhaCungCaps1.Id into Kho_DM_LoaiNhaCungCaps11
                                  from Kho_DM_LoaiNhaCungCaps1 in Kho_DM_LoaiNhaCungCaps11.DefaultIfEmpty()
                                  join Kho_DM_NhaCungCaps1 in context.Kho_DM_NhaCungCaps on p1.Id_NhaCungcap equals Kho_DM_NhaCungCaps1.Id into Kho_DM_NhaCungCaps11
                                  from Kho_DM_NhaCungCaps1 in Kho_DM_NhaCungCaps11.DefaultIfEmpty()
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
                                  select new Kho_HDMuaNhienLieuModel
                                  {
                                      Id = p1.Id,
                                      Id_TenTram = ChiNhanhs1.TenChiNhanh,
                                      NgayKyHopDong = p1.NgayKyHopDong,
                                      SoHopDong = p1.SoHopDong,
                                      Id_LoaiNhaCungCap = Kho_DM_LoaiNhaCungCaps1.TenLoaiNhaCungCap,
                                      Id_NhaCungcap = Kho_DM_NhaCungCaps1.TenNhaCungcap,
                                      DiaChi = Kho_DM_NhaCungCaps1.DiaChi,
                                      MaSoThue = Kho_DM_NhaCungCaps1.MaSoThue,
                                      Id_NhomNhienLieu = Kho_DM_NhomNhienLieus1.TenNhienLieu,
                                      Id_LoaiNhienLieu = Kho_DM_LoaiNhienLieus1.LoaiNhienLieu,
                                      Id_NhanHieu = Kho_DM_NhanHieus1.TenNhanHieu,
                                      Id_DonVi = Kho_DM_DonVis1.TenDonVi,
                                      DonGiaBGCoThue = p1.DonGiaBGCoThue,
                                      DonGiaBGKhongThue = p1.DonGiaBGKhongThue,
                                      DonGiaKBCoThue = p1.DonGiaKBCoThue,
                                      DonGiaKBKhongThue = p1.DonGiaKBKhongThue,
                                      TuNgay = p1.TuNgay,
                                      DenNgay = p1.DenNgay,
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
        public async Task<List<Kho_HDMuaNhienLieuModel>> GetAllModel(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();

                var baseQuery = context.Kho_HDMuaNhienLieus
                    .AsNoTracking()
                    .Where(p => p.GroupId == groupId && p.IsActive != 100);

                var baseData = await baseQuery
                    .OrderByDescending(p => p.CreateAt)
                    .ToListAsync();

                var ChiNhanhsDict = await context.ChiNhanhs.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_LoaiNhaCungCapsDict = await context.Kho_DM_LoaiNhaCungCaps.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_NhaCungCapsDict = await context.Kho_DM_NhaCungCaps.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_NhomNhienLieusDict = await context.Kho_DM_NhomNhienLieus.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_LoaiNhienLieusDict = await context.Kho_DM_LoaiNhienLieus.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_NhanHieusDict = await context.Kho_DM_NhanHieus.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_DonVisDict = await context.Kho_DM_DonVis.AsNoTracking().ToDictionaryAsync(x => x.Id);

                var data = baseData.Select(p1 => new Kho_HDMuaNhienLieuModel
                {
                    Id = p1.Id,
                    Id_TenTram = ChiNhanhsDict.TryGetValue(p1.Id_TenTram, out var tmpChiNhanhs) ? tmpChiNhanhs.TenChiNhanh : p1.Id_TenTram,
                    NgayKyHopDong = p1.NgayKyHopDong,
                    SoHopDong = p1.SoHopDong,
                    Id_LoaiNhaCungCap = Kho_DM_LoaiNhaCungCapsDict.TryGetValue(p1.Id_LoaiNhaCungCap, out var tmpKho_DM_LoaiNhaCungCaps) ? tmpKho_DM_LoaiNhaCungCaps.TenLoaiNhaCungCap : "",
                    Id_NhaCungcap = Kho_DM_NhaCungCapsDict.TryGetValue(p1.Id_NhaCungcap, out var tmpKho_DM_NhaCungCaps) ? tmpKho_DM_NhaCungCaps.TenNhaCungcap : "",
                    DiaChi = Kho_DM_NhaCungCapsDict.TryGetValue(p1.Id_NhaCungcap, out var tmpKho_DM_DiaChi) ? tmpKho_DM_DiaChi.DiaChi : "",
                    MaSoThue = Kho_DM_NhaCungCapsDict.TryGetValue(p1.Id_NhaCungcap, out var tmpKho_DM_MST) ? tmpKho_DM_MST.MaSoThue : "",
                    Id_NhomNhienLieu = Kho_DM_NhomNhienLieusDict.TryGetValue(p1.Id_NhomNhienLieu, out var tmpKho_DM_NhomNhienLieus) ? tmpKho_DM_NhomNhienLieus.TenNhienLieu : "",
                    Id_LoaiNhienLieu = Kho_DM_LoaiNhienLieusDict.TryGetValue(p1.Id_LoaiNhienLieu, out var tmpKho_DM_LoaiNhienLieus) ? tmpKho_DM_LoaiNhienLieus.LoaiNhienLieu : "",
                    Id_NhanHieu = Kho_DM_NhanHieusDict.TryGetValue(p1.Id_NhanHieu, out var tmpKho_DM_NhanHieus) ? tmpKho_DM_NhanHieus.TenNhanHieu : "",
                    Id_DonVi = Kho_DM_DonVisDict.TryGetValue(p1.Id_DonVi, out var tmpKho_DM_DonVis) ? tmpKho_DM_DonVis.TenDonVi : "",
                    DonGiaBGCoThue = p1.DonGiaBGCoThue,
                    DonGiaBGKhongThue = p1.DonGiaBGKhongThue,
                    DonGiaKBCoThue = p1.DonGiaKBCoThue,
                    DonGiaKBKhongThue = p1.DonGiaKBKhongThue,
                    TuNgay = p1.TuNgay,
                    DenNgay = p1.DenNgay,
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
        public async Task<List<Kho_HDMuaNhienLieu>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.Kho_HDMuaNhienLieus.Where(p => p.IsActive != 100).OrderByDescending(p => p.CreateAt).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi khi lấy dữ liệu:  {ex.Message}");
                throw;
            }
        }
        public async Task<Kho_HDMuaNhienLieu> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.Kho_HDMuaNhienLieus.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn : ");
            }
            return entity;
        }
        public async Task Insert(Kho_HDMuaNhienLieu entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có dữ liệu dưọc thêm!");
                }
                context.Kho_HDMuaNhienLieus.Add(entity);
                var addLog = new Kho_HDMuaNhienLieu_Log()
                {
                    Id_TenTram = entity.Id_TenTram,
                    NgayKyHopDong = entity.NgayKyHopDong,
                    SoHopDong = entity.SoHopDong,
                    Id_LoaiNhaCungCap = entity.Id_LoaiNhaCungCap,
                    Id_NhaCungcap = entity.Id_NhaCungcap,
                    Id_NhomNhienLieu = entity.Id_NhomNhienLieu,
                    Id_LoaiNhienLieu = entity.Id_LoaiNhienLieu,
                    Id_NhanHieu = entity.Id_NhanHieu,
                    Id_DonVi = entity.Id_DonVi,
                    DonGiaBGCoThue = entity.DonGiaBGCoThue,
                    DonGiaBGKhongThue = entity.DonGiaBGKhongThue,
                    DonGiaKBCoThue = entity.DonGiaKBCoThue,
                    DonGiaKBKhongThue = entity.DonGiaKBKhongThue,
                    TuNgay = entity.TuNgay,
                    DenNgay = entity.DenNgay,
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
                context.Kho_HDMuaNhienLieu_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(Kho_HDMuaNhienLieu data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn !");
            }
            context.Kho_HDMuaNhienLieus.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.Kho_HDMuaNhienLieu_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.Kho_HDMuaNhienLieu_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.Kho_HDMuaNhienLieu_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.Kho_HDMuaNhienLieu_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.Kho_HDMuaNhienLieu_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt).FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.Kho_HDMuaNhienLieu_Logs.Update(updateLog);
                }
            }
            var addLog = new Kho_HDMuaNhienLieu_Log
            {
                Id_TenTram = data.Id_TenTram,
                NgayKyHopDong = data.NgayKyHopDong,
                SoHopDong = data.SoHopDong,
                Id_LoaiNhaCungCap = data.Id_LoaiNhaCungCap,
                Id_NhaCungcap = data.Id_NhaCungcap,
                Id_NhomNhienLieu = data.Id_NhomNhienLieu,
                Id_LoaiNhienLieu = data.Id_LoaiNhienLieu,
                Id_NhanHieu = data.Id_NhanHieu,
                Id_DonVi = data.Id_DonVi,
                DonGiaBGCoThue = data.DonGiaBGCoThue,
                DonGiaBGKhongThue = data.DonGiaBGKhongThue,
                DonGiaKBCoThue = data.DonGiaKBCoThue,
                DonGiaKBKhongThue = data.DonGiaKBKhongThue,
                TuNgay = data.TuNgay,
                DenNgay = data.DenNgay,
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
            context.Kho_HDMuaNhienLieu_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(Kho_HDMuaNhienLieu[] Kho_HDMuaNhienLieus)
        {
            using var context = _context.CreateDbContext();
            string[] ids = Kho_HDMuaNhienLieus.Select(x => x.Id).ToArray();
            var listEntities = await context.Kho_HDMuaNhienLieus.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.Kho_HDMuaNhienLieus.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task Delete(Kho_HDMuaNhienLieu data, string userId)
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
                    var logdata = (from p in context.Kho_HDMuaNhienLieu_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        data.Id_TenTram = logdata.Id_TenTram;
                        data.NgayKyHopDong = logdata.NgayKyHopDong;
                        data.SoHopDong = logdata.SoHopDong;
                        data.Id_LoaiNhaCungCap = logdata.Id_LoaiNhaCungCap;
                        data.Id_NhaCungcap = logdata.Id_NhaCungcap;
                        data.Id_NhomNhienLieu = logdata.Id_NhomNhienLieu;
                        data.Id_LoaiNhienLieu = logdata.Id_LoaiNhienLieu;
                        data.Id_NhanHieu = logdata.Id_NhanHieu;
                        data.Id_DonVi = logdata.Id_DonVi;
                        data.DonGiaBGCoThue = logdata.DonGiaBGCoThue;
                        data.DonGiaBGKhongThue = logdata.DonGiaBGKhongThue;
                        data.DonGiaKBCoThue = logdata.DonGiaKBCoThue;
                        data.DonGiaKBKhongThue = logdata.DonGiaKBKhongThue;
                        data.TuNgay = logdata.TuNgay;
                        data.DenNgay = logdata.DenNgay;
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
                        context.Kho_HDMuaNhienLieu_Logs.Update(logdata);
                    }
                    var logupdate = (from p in context.Kho_HDMuaNhienLieu_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.Kho_HDMuaNhienLieu_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new Kho_HDMuaNhienLieu_Log()
                    {
                        Id_TenTram = data.Id_TenTram,
                        NgayKyHopDong = data.NgayKyHopDong,
                        SoHopDong = data.SoHopDong,
                        Id_LoaiNhaCungCap = data.Id_LoaiNhaCungCap,
                        Id_NhaCungcap = data.Id_NhaCungcap,
                        Id_NhomNhienLieu = data.Id_NhomNhienLieu,
                        Id_LoaiNhienLieu = data.Id_LoaiNhienLieu,
                        Id_NhanHieu = data.Id_NhanHieu,
                        Id_DonVi = data.Id_DonVi,
                        DonGiaBGCoThue = data.DonGiaBGCoThue,
                        DonGiaBGKhongThue = data.DonGiaBGKhongThue,
                        DonGiaKBCoThue = data.DonGiaKBCoThue,
                        DonGiaKBKhongThue = data.DonGiaKBKhongThue,
                        TuNgay = data.TuNgay,
                        DenNgay = data.DenNgay,
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
                    context.Kho_HDMuaNhienLieu_Logs.Add(addLog);
                }
                else if (entity.IsActive == 2)
                {
                    throw new Exception($"Thông tin đang chờ duyệt xóa!");
                }
                else if (entity.IsActive == 3)
                {
                    var logupdate = (from p in context.Kho_HDMuaNhienLieu_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.Kho_HDMuaNhienLieu_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new Kho_HDMuaNhienLieu_Log()
                    {
                        Id_TenTram = data.Id_TenTram,
                        NgayKyHopDong = data.NgayKyHopDong,
                        SoHopDong = data.SoHopDong,
                        Id_LoaiNhaCungCap = data.Id_LoaiNhaCungCap,
                        Id_NhaCungcap = data.Id_NhaCungcap,
                        Id_NhomNhienLieu = data.Id_NhomNhienLieu,
                        Id_LoaiNhienLieu = data.Id_LoaiNhienLieu,
                        Id_NhanHieu = data.Id_NhanHieu,
                        Id_DonVi = data.Id_DonVi,
                        DonGiaBGCoThue = data.DonGiaBGCoThue,
                        DonGiaBGKhongThue = data.DonGiaBGKhongThue,
                        DonGiaKBCoThue = data.DonGiaKBCoThue,
                        DonGiaKBKhongThue = data.DonGiaKBKhongThue,
                        TuNgay = data.TuNgay,
                        DenNgay = data.DenNgay,
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
                    context.Kho_HDMuaNhienLieu_Logs.Add(addLog);
                }
                else if (entity.IsActive == 90)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa không duyệt !";
                }
            }
            context.Kho_HDMuaNhienLieus.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task Approval(Kho_HDMuaNhienLieu data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chonj!");
            }
            context.Kho_HDMuaNhienLieus.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.Kho_HDMuaNhienLieu_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.Kho_HDMuaNhienLieu_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.Kho_HDMuaNhienLieu_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.Kho_HDMuaNhienLieu_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.Kho_HDMuaNhienLieu_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt).FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.Kho_HDMuaNhienLieu_Logs.Update(updateLog);
                }
            }
            var addLog = new Kho_HDMuaNhienLieu_Log()
            {
                Id_TenTram = data.Id_TenTram,
                NgayKyHopDong = data.NgayKyHopDong,
                SoHopDong = data.SoHopDong,
                Id_LoaiNhaCungCap = data.Id_LoaiNhaCungCap,
                Id_NhaCungcap = data.Id_NhaCungcap,
                Id_NhomNhienLieu = data.Id_NhomNhienLieu,
                Id_LoaiNhienLieu = data.Id_LoaiNhienLieu,
                Id_NhanHieu = data.Id_NhanHieu,
                Id_DonVi = data.Id_DonVi,
                DonGiaBGCoThue = data.DonGiaBGCoThue,
                DonGiaBGKhongThue = data.DonGiaBGKhongThue,
                DonGiaKBCoThue = data.DonGiaKBCoThue,
                DonGiaKBKhongThue = data.DonGiaKBKhongThue,
                TuNgay = data.TuNgay,
                DenNgay = data.DenNgay,
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
            context.Kho_HDMuaNhienLieu_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task NoApproval(Kho_HDMuaNhienLieu data, string userId)
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
                    var logdata = (from p in context.Kho_HDMuaNhienLieu_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.Id_TenTram = logdata.Id_TenTram;
                        entity.NgayKyHopDong = logdata.NgayKyHopDong;
                        entity.SoHopDong = logdata.SoHopDong;
                        entity.Id_LoaiNhaCungCap = logdata.Id_LoaiNhaCungCap;
                        entity.Id_NhaCungcap = logdata.Id_NhaCungcap;
                        entity.Id_NhomNhienLieu = logdata.Id_NhomNhienLieu;
                        entity.Id_LoaiNhienLieu = logdata.Id_LoaiNhienLieu;
                        entity.Id_NhanHieu = logdata.Id_NhanHieu;
                        entity.Id_DonVi = logdata.Id_DonVi;
                        entity.DonGiaBGCoThue = logdata.DonGiaBGCoThue;
                        entity.DonGiaBGKhongThue = logdata.DonGiaBGKhongThue;
                        entity.DonGiaKBCoThue = logdata.DonGiaKBCoThue;
                        entity.DonGiaKBKhongThue = logdata.DonGiaKBKhongThue;
                        entity.TuNgay = logdata.TuNgay;
                        entity.DenNgay = logdata.DenNgay;
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
                    var logupdate = (from p in context.Kho_HDMuaNhienLieu_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true && p.CreateAt >= logdata.CreateAt
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.Kho_HDMuaNhienLieu_Logs.UpdateRange(logupdate);
                    }
                }
                else if (entity.IsActive == 2)
                {
                    var logdata = (from p in context.Kho_HDMuaNhienLieu_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.Id_TenTram = logdata.Id_TenTram;
                        entity.NgayKyHopDong = logdata.NgayKyHopDong;
                        entity.SoHopDong = logdata.SoHopDong;
                        entity.Id_LoaiNhaCungCap = logdata.Id_LoaiNhaCungCap;
                        entity.Id_NhaCungcap = logdata.Id_NhaCungcap;
                        entity.Id_NhomNhienLieu = logdata.Id_NhomNhienLieu;
                        entity.Id_LoaiNhienLieu = logdata.Id_LoaiNhienLieu;
                        entity.Id_NhanHieu = logdata.Id_NhanHieu;
                        entity.Id_DonVi = logdata.Id_DonVi;
                        entity.DonGiaBGCoThue = logdata.DonGiaBGCoThue;
                        entity.DonGiaBGKhongThue = logdata.DonGiaBGKhongThue;
                        entity.DonGiaKBCoThue = logdata.DonGiaKBCoThue;
                        entity.DonGiaKBKhongThue = logdata.DonGiaKBKhongThue;
                        entity.TuNgay = logdata.TuNgay;
                        entity.DenNgay = logdata.DenNgay;
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
            var addLog = new Kho_HDMuaNhienLieu_Log()
            {
                Id_TenTram = data.Id_TenTram,
                NgayKyHopDong = data.NgayKyHopDong,
                SoHopDong = data.SoHopDong,
                Id_LoaiNhaCungCap = data.Id_LoaiNhaCungCap,
                Id_NhaCungcap = data.Id_NhaCungcap,
                Id_NhomNhienLieu = data.Id_NhomNhienLieu,
                Id_LoaiNhienLieu = data.Id_LoaiNhienLieu,
                Id_NhanHieu = data.Id_NhanHieu,
                Id_DonVi = data.Id_DonVi,
                DonGiaBGCoThue = data.DonGiaBGCoThue,
                DonGiaBGKhongThue = data.DonGiaBGKhongThue,
                DonGiaKBCoThue = data.DonGiaKBCoThue,
                DonGiaKBKhongThue = data.DonGiaKBKhongThue,
                TuNgay = data.TuNgay,
                DenNgay = data.DenNgay,
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
            context.Kho_HDMuaNhienLieu_Logs.Add(addLog);
            context.Kho_HDMuaNhienLieus.Update(entity);
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
            context.Set<Kho_HDMuaNhienLieu>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.Kho_HDMuaNhienLieus.Where(p => p.Id == ids && p.IsActive != 100).FirstOrDefaultAsync();
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
        public async Task<bool> CheckSave(Kho_HDMuaNhienLieu input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.Kho_HDMuaNhienLieu_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100
                            && p.Id_TenTram == input.Id_TenTram
                            && p.NgayKyHopDong == input.NgayKyHopDong
                            && p.SoHopDong == input.SoHopDong
                            && p.Id_LoaiNhaCungCap == input.Id_LoaiNhaCungCap
                            && p.Id_NhaCungcap == input.Id_NhaCungcap
                            && p.Id_NhomNhienLieu == input.Id_NhomNhienLieu
                            && p.Id_LoaiNhienLieu == input.Id_LoaiNhienLieu
                            && p.Id_NhanHieu == input.Id_NhanHieu
                            && p.Id_DonVi == input.Id_DonVi
                            && p.DonGiaBGCoThue == input.DonGiaBGCoThue
                            && p.DonGiaBGKhongThue == input.DonGiaBGKhongThue
                            && p.DonGiaKBCoThue == input.DonGiaKBCoThue
                            && p.DonGiaKBKhongThue == input.DonGiaKBKhongThue
                            && p.TuNgay == input.TuNgay
                            && p.DenNgay == input.DenNgay

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
        public async Task<bool> CheckEdit(Kho_HDMuaNhienLieu input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.Kho_HDMuaNhienLieu_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100 && p.Id != input.Id
                            && p.Id_TenTram == input.Id_TenTram
                            && p.NgayKyHopDong == input.NgayKyHopDong
                            && p.SoHopDong == input.SoHopDong
                            && p.Id_LoaiNhaCungCap == input.Id_LoaiNhaCungCap
                            && p.Id_NhaCungcap == input.Id_NhaCungcap
                            && p.Id_NhomNhienLieu == input.Id_NhomNhienLieu
                            && p.Id_LoaiNhienLieu == input.Id_LoaiNhienLieu
                            && p.Id_NhanHieu == input.Id_NhanHieu
                            && p.Id_DonVi == input.Id_DonVi
                            && p.DonGiaBGCoThue == input.DonGiaBGCoThue
                            && p.DonGiaBGKhongThue == input.DonGiaBGKhongThue
                            && p.DonGiaKBCoThue == input.DonGiaKBCoThue
                            && p.DonGiaKBKhongThue == input.DonGiaKBKhongThue
                            && p.TuNgay == input.TuNgay
                            && p.DenNgay == input.DenNgay
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
        public async Task<bool> CheckDelete(Kho_HDMuaNhienLieu input)
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
