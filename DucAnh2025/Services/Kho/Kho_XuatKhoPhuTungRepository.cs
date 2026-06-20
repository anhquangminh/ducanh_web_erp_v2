using DucAnh2025.Data;
using DucAnh2025.Models;
using DucAnh2025.Repository;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace DucAnh2025.Services
{
    public class Kho_XuatKhoPhuTungRepository : IKho_XuatKhoPhuTungRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public Kho_XuatKhoPhuTungRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public async Task<List<Kho_XuatKhoPhuTungModel>> GetAllByVM(Kho_XuatKhoPhuTungModel dataModel, string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();

                var baseQuery = context.Kho_XuatKhoPhuTungs
                    .AsNoTracking()
                    .Where(p => p.GroupId == groupId && p.IsActive != 100);

                if (!string.IsNullOrEmpty(dataModel?.Id_Tram))
                {
                    baseQuery = baseQuery.Where(m => m.Id_Tram == dataModel.Id_Tram);
                }
                if (!string.IsNullOrEmpty(dataModel?.Id_NhomPhuTung))
                {
                    baseQuery = baseQuery.Where(m => m.Id_NhomPhuTung == dataModel.Id_NhomPhuTung);
                }
                if (!string.IsNullOrEmpty(dataModel?.Id_LoaiPhuTung))
                {
                    baseQuery = baseQuery.Where(m => m.Id_LoaiPhuTung == dataModel.Id_LoaiPhuTung);
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
                var Kho_DM_NhomPhuTungsDict = await context.Kho_DM_NhomPhuTungs.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var kho_DM_LoaiPhuTungsDict = await context.kho_DM_LoaiPhuTungs.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_NhanHieusDict = await context.Kho_DM_NhanHieus.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var Kho_DM_DonVisDict = await context.Kho_DM_DonVis.AsNoTracking().ToDictionaryAsync(x => x.Id);

                var data = baseData.Select(p1 => new Kho_XuatKhoPhuTungModel
                {
                    Id = p1.Id,
                    Id_Tram = ChiNhanhsDict.TryGetValue(p1.Id_Tram, out var tmpChiNhanhs) ? tmpChiNhanhs.TenChiNhanh : "",
                    NgayXuatKho = p1.NgayXuatKho,
                    SoPhieu = p1.SoPhieu,
                    DoiTuongNhan = p1.DoiTuongNhan,
                    Id_NhomPhuTung = Kho_DM_NhomPhuTungsDict.TryGetValue(p1.Id_NhomPhuTung, out var tmpKho_DM_NhomPhuTungs) ? tmpKho_DM_NhomPhuTungs.TenNhomPhuTung : "",
                    Id_LoaiPhuTung = kho_DM_LoaiPhuTungsDict.TryGetValue(p1.Id_LoaiPhuTung, out var tmpkho_DM_LoaiPhuTungs) ? tmpkho_DM_LoaiPhuTungs.TenLoaiPhuTung : "",
                    Id_NhanHieu = Kho_DM_NhanHieusDict.TryGetValue(p1.Id_NhanHieu, out var tmpKho_DM_NhanHieus) ? tmpKho_DM_NhanHieus.TenNhanHieu : "",
                    Seri = p1.Seri,
                    Id_DonVi = Kho_DM_DonVisDict.TryGetValue(p1.Id_DonVi, out var tmpKho_DM_DonVis) ? tmpKho_DM_DonVis.TenDonVi : "",
                    KLCoThue = p1.KLCoThue,
                    KLKhongThue = p1.KLKhongThue,
                    DonGiaCoThue = p1.DonGiaCoThue,
                    DonGiaKhongThue = p1.DonGiaKhongThue,
                    ThanhTienCoThue = p1.ThanhTienCoThue,
                    ThanhTienKhongThue = p1.ThanhTienKhongThue,
                    TongTien = p1.TongTien,
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
        public async Task<List<Kho_XuatKhoPhuTungModel>> GetHistoryIsValidEdit(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var query = (from p1 in context.Kho_XuatKhoPhuTung_Logs
                             join ChiNhanhs1 in context.ChiNhanhs on p1.Id_Tram equals ChiNhanhs1.Id
                             join Kho_DM_NhomPhuTungs1 in context.Kho_DM_NhomPhuTungs on p1.Id_NhomPhuTung equals Kho_DM_NhomPhuTungs1.Id
                             join kho_DM_LoaiPhuTungs1 in context.kho_DM_LoaiPhuTungs on p1.Id_LoaiPhuTung equals kho_DM_LoaiPhuTungs1.Id
                             join Kho_DM_NhanHieus1 in context.Kho_DM_NhanHieus on p1.Id_NhanHieu equals Kho_DM_NhanHieus1.Id
                             join Kho_DM_DonVis1 in context.Kho_DM_DonVis on p1.Id_DonVi equals Kho_DM_DonVis1.Id
                             join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                             join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                             where p1.IdChung == id && p1.IsValid == true
                             orderby p1.CreateAt
                             select new Kho_XuatKhoPhuTungModel
                             {
                                 Id = p1.Id,
                                 Id_Tram = ChiNhanhs1.TenChiNhanh,
                                 NgayXuatKho = p1.NgayXuatKho,
                                 SoPhieu = p1.SoPhieu,
                                 DoiTuongNhan = p1.DoiTuongNhan,
                                 Id_NhomPhuTung = Kho_DM_NhomPhuTungs1.TenNhomPhuTung,
                                 Id_LoaiPhuTung = kho_DM_LoaiPhuTungs1.TenLoaiPhuTung,
                                 Id_NhanHieu = Kho_DM_NhanHieus1.TenNhanHieu,
                                 Seri = p1.Seri,
                                 Id_DonVi = Kho_DM_DonVis1.TenDonVi,
                                 KLCoThue = p1.KLCoThue,
                                 KLKhongThue = p1.KLKhongThue,
                                 DonGiaCoThue = p1.DonGiaCoThue,
                                 DonGiaKhongThue = p1.DonGiaKhongThue,
                                 ThanhTienCoThue = p1.ThanhTienCoThue,
                                 ThanhTienKhongThue = p1.ThanhTienKhongThue,
                                 TongTien = p1.TongTien,
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
        public async Task<Kho_XuatKhoPhuTungModel> GetDetails(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.Kho_XuatKhoPhuTungs
                              join ChiNhanhs1 in context.ChiNhanhs on p1.Id_Tram equals ChiNhanhs1.Id into ChiNhanhs11
                              from ChiNhanhs1 in ChiNhanhs11.DefaultIfEmpty()
                              join Kho_DM_NhomPhuTungs1 in context.Kho_DM_NhomPhuTungs on p1.Id_NhomPhuTung equals Kho_DM_NhomPhuTungs1.Id into Kho_DM_NhomPhuTungs11
                              from Kho_DM_NhomPhuTungs1 in Kho_DM_NhomPhuTungs11.DefaultIfEmpty()
                              join kho_DM_LoaiPhuTungs1 in context.kho_DM_LoaiPhuTungs on p1.Id_LoaiPhuTung equals kho_DM_LoaiPhuTungs1.Id into kho_DM_LoaiPhuTungs11
                              from kho_DM_LoaiPhuTungs1 in kho_DM_LoaiPhuTungs11.DefaultIfEmpty()
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
                              select new Kho_XuatKhoPhuTungModel
                              {
                                  Id = p1.Id,
                                  Id_Tram = ChiNhanhs1.TenChiNhanh,
                                  NgayXuatKho = p1.NgayXuatKho,
                                  SoPhieu = p1.SoPhieu,
                                  DoiTuongNhan = p1.DoiTuongNhan,
                                  Id_NhomPhuTung = Kho_DM_NhomPhuTungs1.TenNhomPhuTung,
                                  Id_LoaiPhuTung = kho_DM_LoaiPhuTungs1.TenLoaiPhuTung,
                                  Id_NhanHieu = Kho_DM_NhanHieus1.TenNhanHieu,
                                  Seri = p1.Seri,
                                  Id_DonVi = Kho_DM_DonVis1.TenDonVi,
                                  KLCoThue = p1.KLCoThue,
                                  KLKhongThue = p1.KLKhongThue,
                                  DonGiaCoThue = p1.DonGiaCoThue,
                                  DonGiaKhongThue = p1.DonGiaKhongThue,
                                  ThanhTienCoThue = p1.ThanhTienCoThue,
                                  ThanhTienKhongThue = p1.ThanhTienKhongThue,
                                  TongTien = p1.TongTien,
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
        public async Task<List<Kho_XuatKhoPhuTungModel>> GetHistory(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var data = await (from p1 in context.Kho_XuatKhoPhuTung_Logs
                                  join ChiNhanhs1 in context.ChiNhanhs on p1.Id_Tram equals ChiNhanhs1.Id into ChiNhanhs11
                                  from ChiNhanhs1 in ChiNhanhs11.DefaultIfEmpty()
                                  join Kho_DM_NhomPhuTungs1 in context.Kho_DM_NhomPhuTungs on p1.Id_NhomPhuTung equals Kho_DM_NhomPhuTungs1.Id into Kho_DM_NhomPhuTungs11
                                  from Kho_DM_NhomPhuTungs1 in Kho_DM_NhomPhuTungs11.DefaultIfEmpty()
                                  join kho_DM_LoaiPhuTungs1 in context.kho_DM_LoaiPhuTungs on p1.Id_LoaiPhuTung equals kho_DM_LoaiPhuTungs1.Id into kho_DM_LoaiPhuTungs11
                                  from kho_DM_LoaiPhuTungs1 in kho_DM_LoaiPhuTungs11.DefaultIfEmpty()
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
                                  select new Kho_XuatKhoPhuTungModel
                                  {
                                      Id = p1.Id,
                                      Id_Tram = ChiNhanhs1.TenChiNhanh,
                                      NgayXuatKho = p1.NgayXuatKho,
                                      SoPhieu = p1.SoPhieu,
                                      DoiTuongNhan = p1.DoiTuongNhan,
                                      Id_NhomPhuTung = Kho_DM_NhomPhuTungs1.TenNhomPhuTung,
                                      Id_LoaiPhuTung = kho_DM_LoaiPhuTungs1.TenLoaiPhuTung,
                                      Id_NhanHieu = Kho_DM_NhanHieus1.TenNhanHieu,
                                      Seri = p1.Seri,
                                      Id_DonVi = Kho_DM_DonVis1.TenDonVi,
                                      KLCoThue = p1.KLCoThue,
                                      KLKhongThue = p1.KLKhongThue,
                                      DonGiaCoThue = p1.DonGiaCoThue,
                                      DonGiaKhongThue = p1.DonGiaKhongThue,
                                      ThanhTienCoThue = p1.ThanhTienCoThue,
                                      ThanhTienKhongThue = p1.ThanhTienKhongThue,
                                      TongTien = p1.TongTien,
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
        public async Task<List<Kho_XuatKhoPhuTung>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.Kho_XuatKhoPhuTungs.Where(p => p.IsActive != 100).OrderByDescending(p => p.CreateAt).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi khi lấy dữ liệu:  {ex.Message}");
                throw;
            }
        }
        public async Task<Kho_XuatKhoPhuTung> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.Kho_XuatKhoPhuTungs.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn : ");
            }
            return entity;
        }
        public async Task Insert(Kho_XuatKhoPhuTung entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có dữ liệu dưọc thêm!");
                }
                context.Kho_XuatKhoPhuTungs.Add(entity);
                var addLog = new Kho_XuatKhoPhuTung_Log()
                {
                    Id_Tram = entity.Id_Tram,
                    NgayXuatKho = entity.NgayXuatKho,
                    SoPhieu = entity.SoPhieu,
                    DoiTuongNhan = entity.DoiTuongNhan,
                    Id_NhomPhuTung = entity.Id_NhomPhuTung,
                    Id_LoaiPhuTung = entity.Id_LoaiPhuTung,
                    Id_NhanHieu = entity.Id_NhanHieu,
                    Seri = entity.Seri,
                    Id_DonVi = entity.Id_DonVi,
                    KLCoThue = entity.KLCoThue,
                    KLKhongThue = entity.KLKhongThue,
                    DonGiaCoThue = entity.DonGiaCoThue,
                    DonGiaKhongThue = entity.DonGiaKhongThue,
                    ThanhTienCoThue = entity.ThanhTienCoThue,
                    ThanhTienKhongThue = entity.ThanhTienKhongThue,
                    TongTien = entity.TongTien,
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
                context.Kho_XuatKhoPhuTung_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(Kho_XuatKhoPhuTung data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn !");
            }
            context.Kho_XuatKhoPhuTungs.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.Kho_XuatKhoPhuTung_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.Kho_XuatKhoPhuTung_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.Kho_XuatKhoPhuTung_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.Kho_XuatKhoPhuTung_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.Kho_XuatKhoPhuTung_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt).FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.Kho_XuatKhoPhuTung_Logs.Update(updateLog);
                }
            }
            var addLog = new Kho_XuatKhoPhuTung_Log
            {
                Id_Tram = data.Id_Tram,
                NgayXuatKho = data.NgayXuatKho,
                SoPhieu = data.SoPhieu,
                DoiTuongNhan = data.DoiTuongNhan,
                Id_NhomPhuTung = data.Id_NhomPhuTung,
                Id_LoaiPhuTung = data.Id_LoaiPhuTung,
                Id_NhanHieu = data.Id_NhanHieu,
                Seri = data.Seri,
                Id_DonVi = data.Id_DonVi,
                KLCoThue = data.KLCoThue,
                KLKhongThue = data.KLKhongThue,
                DonGiaCoThue = data.DonGiaCoThue,
                DonGiaKhongThue = data.DonGiaKhongThue,
                ThanhTienCoThue = data.ThanhTienCoThue,
                ThanhTienKhongThue = data.ThanhTienKhongThue,
                TongTien = data.TongTien,
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
            context.Kho_XuatKhoPhuTung_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(Kho_XuatKhoPhuTung[] Kho_XuatKhoPhuTungs)
        {
            using var context = _context.CreateDbContext();
            string[] ids = Kho_XuatKhoPhuTungs.Select(x => x.Id).ToArray();
            var listEntities = await context.Kho_XuatKhoPhuTungs.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.Kho_XuatKhoPhuTungs.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task Delete(Kho_XuatKhoPhuTung data, string userId)
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
                    var logdata = (from p in context.Kho_XuatKhoPhuTung_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        data.Id_Tram = logdata.Id_Tram;
                        data.NgayXuatKho = logdata.NgayXuatKho;
                        data.SoPhieu = logdata.SoPhieu;
                        data.DoiTuongNhan = logdata.DoiTuongNhan;
                        data.Id_NhomPhuTung = logdata.Id_NhomPhuTung;
                        data.Id_LoaiPhuTung = logdata.Id_LoaiPhuTung;
                        data.Id_NhanHieu = logdata.Id_NhanHieu;
                        data.Seri = logdata.Seri;
                        data.Id_DonVi = logdata.Id_DonVi;
                        data.KLCoThue = logdata.KLCoThue;
                        data.KLKhongThue = logdata.KLKhongThue;
                        data.DonGiaCoThue = logdata.DonGiaCoThue;
                        data.DonGiaKhongThue = logdata.DonGiaKhongThue;
                        data.ThanhTienCoThue = logdata.ThanhTienCoThue;
                        data.ThanhTienKhongThue = logdata.ThanhTienKhongThue;
                        data.TongTien = logdata.TongTien;
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
                        context.Kho_XuatKhoPhuTung_Logs.Update(logdata);
                    }
                    var logupdate = (from p in context.Kho_XuatKhoPhuTung_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.Kho_XuatKhoPhuTung_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new Kho_XuatKhoPhuTung_Log()
                    {
                        Id_Tram = data.Id_Tram,
                        NgayXuatKho = data.NgayXuatKho,
                        SoPhieu = data.SoPhieu,
                        DoiTuongNhan = data.DoiTuongNhan,
                        Id_NhomPhuTung = data.Id_NhomPhuTung,
                        Id_LoaiPhuTung = data.Id_LoaiPhuTung,
                        Id_NhanHieu = data.Id_NhanHieu,
                        Seri = data.Seri,
                        Id_DonVi = data.Id_DonVi,
                        KLCoThue = data.KLCoThue,
                        KLKhongThue = data.KLKhongThue,
                        DonGiaCoThue = data.DonGiaCoThue,
                        DonGiaKhongThue = data.DonGiaKhongThue,
                        ThanhTienCoThue = data.ThanhTienCoThue,
                        ThanhTienKhongThue = data.ThanhTienKhongThue,
                        TongTien = data.TongTien,
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
                    context.Kho_XuatKhoPhuTung_Logs.Add(addLog);
                }
                else if (entity.IsActive == 2)
                {
                    throw new Exception($"Thông tin đang chờ duyệt xóa!");
                }
                else if (entity.IsActive == 3)
                {
                    var logupdate = (from p in context.Kho_XuatKhoPhuTung_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.Kho_XuatKhoPhuTung_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new Kho_XuatKhoPhuTung_Log()
                    {
                        Id_Tram = data.Id_Tram,
                        NgayXuatKho = data.NgayXuatKho,
                        SoPhieu = data.SoPhieu,
                        DoiTuongNhan = data.DoiTuongNhan,
                        Id_NhomPhuTung = data.Id_NhomPhuTung,
                        Id_LoaiPhuTung = data.Id_LoaiPhuTung,
                        Id_NhanHieu = data.Id_NhanHieu,
                        Seri = data.Seri,
                        Id_DonVi = data.Id_DonVi,
                        KLCoThue = data.KLCoThue,
                        KLKhongThue = data.KLKhongThue,
                        DonGiaCoThue = data.DonGiaCoThue,
                        DonGiaKhongThue = data.DonGiaKhongThue,
                        ThanhTienCoThue = data.ThanhTienCoThue,
                        ThanhTienKhongThue = data.ThanhTienKhongThue,
                        TongTien = data.TongTien,
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
                    context.Kho_XuatKhoPhuTung_Logs.Add(addLog);
                }
                else if (entity.IsActive == 90)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa không duyệt !";
                }
            }
            context.Kho_XuatKhoPhuTungs.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task Approval(Kho_XuatKhoPhuTung data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chonj!");
            }
            context.Kho_XuatKhoPhuTungs.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.Kho_XuatKhoPhuTung_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.Kho_XuatKhoPhuTung_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.Kho_XuatKhoPhuTung_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.Kho_XuatKhoPhuTung_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.Kho_XuatKhoPhuTung_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt).FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.Kho_XuatKhoPhuTung_Logs.Update(updateLog);
                }
            }
            var addLog = new Kho_XuatKhoPhuTung_Log()
            {
                Id_Tram = data.Id_Tram,
                NgayXuatKho = data.NgayXuatKho,
                SoPhieu = data.SoPhieu,
                DoiTuongNhan = data.DoiTuongNhan,
                Id_NhomPhuTung = data.Id_NhomPhuTung,
                Id_LoaiPhuTung = data.Id_LoaiPhuTung,
                Id_NhanHieu = data.Id_NhanHieu,
                Seri = data.Seri,
                Id_DonVi = data.Id_DonVi,
                KLCoThue = data.KLCoThue,
                KLKhongThue = data.KLKhongThue,
                DonGiaCoThue = data.DonGiaCoThue,
                DonGiaKhongThue = data.DonGiaKhongThue,
                ThanhTienCoThue = data.ThanhTienCoThue,
                ThanhTienKhongThue = data.ThanhTienKhongThue,
                TongTien = data.TongTien,
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
            context.Kho_XuatKhoPhuTung_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task NoApproval(Kho_XuatKhoPhuTung data, string userId)
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
                    var logdata = (from p in context.Kho_XuatKhoPhuTung_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.Id_Tram = logdata.Id_Tram;
                        entity.NgayXuatKho = logdata.NgayXuatKho;
                        entity.SoPhieu = logdata.SoPhieu;
                        entity.DoiTuongNhan = logdata.DoiTuongNhan;
                        entity.Id_NhomPhuTung = logdata.Id_NhomPhuTung;
                        entity.Id_LoaiPhuTung = logdata.Id_LoaiPhuTung;
                        entity.Id_NhanHieu = logdata.Id_NhanHieu;
                        entity.Seri = logdata.Seri;
                        entity.Id_DonVi = logdata.Id_DonVi;
                        entity.KLCoThue = logdata.KLCoThue;
                        entity.KLKhongThue = logdata.KLKhongThue;
                        entity.DonGiaCoThue = logdata.DonGiaCoThue;
                        entity.DonGiaKhongThue = logdata.DonGiaKhongThue;
                        entity.ThanhTienCoThue = logdata.ThanhTienCoThue;
                        entity.ThanhTienKhongThue = logdata.ThanhTienKhongThue;
                        entity.TongTien = logdata.TongTien;
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
                    var logupdate = (from p in context.Kho_XuatKhoPhuTung_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true && p.CreateAt >= logdata.CreateAt
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.Kho_XuatKhoPhuTung_Logs.UpdateRange(logupdate);
                    }
                }
                else if (entity.IsActive == 2)
                {
                    var logdata = (from p in context.Kho_XuatKhoPhuTung_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.Id_Tram = logdata.Id_Tram;
                        entity.NgayXuatKho = logdata.NgayXuatKho;
                        entity.SoPhieu = logdata.SoPhieu;
                        entity.DoiTuongNhan = logdata.DoiTuongNhan;
                        entity.Id_NhomPhuTung = logdata.Id_NhomPhuTung;
                        entity.Id_LoaiPhuTung = logdata.Id_LoaiPhuTung;
                        entity.Id_NhanHieu = logdata.Id_NhanHieu;
                        entity.Seri = logdata.Seri;
                        entity.Id_DonVi = logdata.Id_DonVi;
                        entity.KLCoThue = logdata.KLCoThue;
                        entity.KLKhongThue = logdata.KLKhongThue;
                        entity.DonGiaCoThue = logdata.DonGiaCoThue;
                        entity.DonGiaKhongThue = logdata.DonGiaKhongThue;
                        entity.ThanhTienCoThue = logdata.ThanhTienCoThue;
                        entity.ThanhTienKhongThue = logdata.ThanhTienKhongThue;
                        entity.TongTien = logdata.TongTien;
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
            var addLog = new Kho_XuatKhoPhuTung_Log()
            {
                Id_Tram = data.Id_Tram,
                NgayXuatKho = data.NgayXuatKho,
                SoPhieu = data.SoPhieu,
                DoiTuongNhan = data.DoiTuongNhan,
                Id_NhomPhuTung = data.Id_NhomPhuTung,
                Id_LoaiPhuTung = data.Id_LoaiPhuTung,
                Id_NhanHieu = data.Id_NhanHieu,
                Seri = data.Seri,
                Id_DonVi = data.Id_DonVi,
                KLCoThue = data.KLCoThue,
                KLKhongThue = data.KLKhongThue,
                DonGiaCoThue = data.DonGiaCoThue,
                DonGiaKhongThue = data.DonGiaKhongThue,
                ThanhTienCoThue = data.ThanhTienCoThue,
                ThanhTienKhongThue = data.ThanhTienKhongThue,
                TongTien = data.TongTien,
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
            context.Kho_XuatKhoPhuTung_Logs.Add(addLog);
            context.Kho_XuatKhoPhuTungs.Update(entity);
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
            context.Set<Kho_XuatKhoPhuTung>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.Kho_XuatKhoPhuTungs.Where(p => p.Id == ids && p.IsActive != 100).FirstOrDefaultAsync();
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
        public async Task<bool> CheckSave(Kho_XuatKhoPhuTung input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.Kho_XuatKhoPhuTung_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100
                            && p.Id_Tram == input.Id_Tram
                            && p.NgayXuatKho == input.NgayXuatKho
                            && p.SoPhieu == input.SoPhieu
                            && p.DoiTuongNhan == input.DoiTuongNhan
                            && p.Id_NhomPhuTung == input.Id_NhomPhuTung
                            && p.Id_LoaiPhuTung == input.Id_LoaiPhuTung
                            && p.Id_NhanHieu == input.Id_NhanHieu
                            && p.Seri == input.Seri
                            && p.Id_DonVi == input.Id_DonVi
                            && p.KLCoThue == input.KLCoThue
                            && p.KLKhongThue == input.KLKhongThue
                            && p.DonGiaCoThue == input.DonGiaCoThue
                            && p.DonGiaKhongThue == input.DonGiaKhongThue
                            && p.ThanhTienCoThue == input.ThanhTienCoThue
                            && p.ThanhTienKhongThue == input.ThanhTienKhongThue
                            && p.TongTien == input.TongTien

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
        public async Task<bool> CheckEdit(Kho_XuatKhoPhuTung input)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var model = await (from p in context.Kho_XuatKhoPhuTung_Logs
                                   where p.GroupId == input.GroupId && p.IdChung != input.Id && p.IsValid == true && p.IsActive != 100 && p.Id != input.Id
                            && p.Id_Tram == input.Id_Tram
                            && p.NgayXuatKho == input.NgayXuatKho
                            && p.SoPhieu == input.SoPhieu
                            && p.DoiTuongNhan == input.DoiTuongNhan
                            && p.Id_NhomPhuTung == input.Id_NhomPhuTung
                            && p.Id_LoaiPhuTung == input.Id_LoaiPhuTung
                            && p.Id_NhanHieu == input.Id_NhanHieu
                            && p.Seri == input.Seri
                            && p.Id_DonVi == input.Id_DonVi
                            && p.KLCoThue == input.KLCoThue
                            && p.KLKhongThue == input.KLKhongThue
                            && p.DonGiaCoThue == input.DonGiaCoThue
                            && p.DonGiaKhongThue == input.DonGiaKhongThue
                            && p.ThanhTienCoThue == input.ThanhTienCoThue
                            && p.ThanhTienKhongThue == input.ThanhTienKhongThue
                            && p.TongTien == input.TongTien
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
        public async Task<bool> CheckDelete(Kho_XuatKhoPhuTung input)
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

