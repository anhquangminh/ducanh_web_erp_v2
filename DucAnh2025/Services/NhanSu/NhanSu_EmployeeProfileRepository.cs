using DucAnh2025.Data;
using DucAnh2025.Models;
using DucAnh2025.Repository.NhanSu;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.NhanSu;
using Microsoft.EntityFrameworkCore;
namespace DucAnh2025.Services.NhanSu
{
    public class NhanSu_EmployeeProfileRepository : INhanSu_EmployeeProfileRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;
        public NhanSu_EmployeeProfileRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }
        public async Task<List<NhanSu_EmployeeProfileModel>> GetAllByVM(NhanSu_EmployeeProfileModel dataModel, string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();

                var baseQuery = context.NhanSu_EmployeeProfiles
                    .AsNoTracking()
                    .Where(p => p.GroupId == groupId && p.IsActive != 100);

                if (!string.IsNullOrEmpty(dataModel?.DepartmentFrId))
                {
                    baseQuery = baseQuery.Where(m => m.DepartmentFrId == dataModel.DepartmentFrId);
                }
                if (!string.IsNullOrEmpty(dataModel?.ChucVuId))
                {
                    baseQuery = baseQuery.Where(m => m.ChucVuId == dataModel.ChucVuId);
                }
                if (!string.IsNullOrEmpty(dataModel?.ManagerId))
                {
                    baseQuery = baseQuery.Where(m => m.ManagerId == dataModel.ManagerId);
                }
                if (!string.IsNullOrEmpty(dataModel?.CurrentContractType))
                {
                    baseQuery = baseQuery.Where(m => m.CurrentContractType == dataModel.CurrentContractType);
                }
                if (!string.IsNullOrEmpty(dataModel?.WorkStatusId))
                {
                    baseQuery = baseQuery.Where(m => m.WorkStatusId == dataModel.WorkStatusId);
                }

                var baseData = await baseQuery
                    .OrderByDescending(p => p.CreateAt)
                    .ToListAsync();

                var DepartmentsDict = await context.Departments.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var DM_ChucVusDict = await context.DM_ChucVus.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var NhanSu_EmployeeProfilesDict = await context.NhanSu_EmployeeProfiles.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var NhanSu_DM_ContractTypesDict = await context.NhanSu_DM_ContractTypes.AsNoTracking().ToDictionaryAsync(x => x.Id);
                var NhanSu_DM_WorkStatusDict = await context.NhanSu_DM_WorkStatus.AsNoTracking().ToDictionaryAsync(x => x.Id);

                var data = baseData.Select(p1 => new NhanSu_EmployeeProfileModel
                {
                    Id = p1.Id,
                    EmployeeId = p1.EmployeeId,
                    FullName = p1.FullName,
                    Gender = p1.Gender,
                    DateOfBirth = p1.DateOfBirth,
                    IdentityCardNumber = p1.IdentityCardNumber,
                    IdentityCardIssueDate = p1.IdentityCardIssueDate,
                    IdentityCardIssuePlace = p1.IdentityCardIssuePlace,
                    TaxCode = p1.TaxCode,
                    SocialInsuranceNumber = p1.SocialInsuranceNumber,
                    MobilePhone = p1.MobilePhone,
                    PersonalEmail = p1.PersonalEmail,
                    PermanentAddress = p1.PermanentAddress,
                    CurrentAddress = p1.CurrentAddress,
                    EmergencyContactName = p1.EmergencyContactName,
                    EmergencyContactPhone = p1.EmergencyContactPhone,
                    DepartmentFrId = DepartmentsDict.TryGetValue(p1.DepartmentFrId, out var tmpDepartments) ? tmpDepartments.DeptName : "",
                    ChucVuId = DM_ChucVusDict.TryGetValue(p1.ChucVuId, out var tmpDM_ChucVus) ? tmpDM_ChucVus.ChucVu : "",
                    ManagerId = NhanSu_EmployeeProfilesDict.TryGetValue(p1.ManagerId, out var tmpNhanSu_EmployeeProfiles) ? tmpNhanSu_EmployeeProfiles.FullName : "",
                    WorkEmail = p1.WorkEmail,
                    HireDate = p1.HireDate,
                    OfficialDate = p1.OfficialDate,
                    CurrentContractType = NhanSu_DM_ContractTypesDict.TryGetValue(p1.CurrentContractType, out var tmpNhanSu_DM_ContractTypes) ? tmpNhanSu_DM_ContractTypes.ContractTypeName : "",
                    ContractExpirationDate = p1.ContractExpirationDate,
                    WorkStatusId = NhanSu_DM_WorkStatusDict.TryGetValue(p1.WorkStatusId, out var tmpNhanSu_DM_WorkStatus) ? tmpNhanSu_DM_WorkStatus.WorkStatusName : "",
                    CurrentBasicSalary = p1.CurrentBasicSalary,
                    BankAccountNumber = p1.BankAccountNumber,
                    BankName = p1.BankName,
                    SalaryPaymentMethod = p1.SalaryPaymentMethod,
                    TaxDependentsCount = p1.TaxDependentsCount,
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
                throw new Exception("L?i khi l?y d? li?u: " + ex.Message, ex);
            }
        }
        public async Task<List<NhanSu_EmployeeProfileModel>> GetHistoryIsValidEdit(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var query = (from p1 in context.NhanSu_EmployeeProfile_Logs
                             join Departments1 in context.Departments on p1.DepartmentFrId equals Departments1.Id
                             join DM_ChucVus1 in context.DM_ChucVus on p1.ChucVuId equals DM_ChucVus1.Id
                             join NhanSu_EmployeeProfiles1 in context.NhanSu_EmployeeProfiles on p1.ManagerId equals NhanSu_EmployeeProfiles1.Id
                             join NhanSu_DM_ContractTypes1 in context.NhanSu_DM_ContractTypes on p1.CurrentContractType equals NhanSu_DM_ContractTypes1.Id
                             join NhanSu_DM_WorkStatus1 in context.NhanSu_DM_WorkStatus on p1.WorkStatusId equals NhanSu_DM_WorkStatus1.Id
                             join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                             join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                             where p1.IdChung == id && p1.IsValid == true
                             orderby p1.CreateAt
                             select new NhanSu_EmployeeProfileModel
                             {
                                 Id = p1.Id,
                                 EmployeeId = p1.EmployeeId,
                                 FullName = p1.FullName,
                                 Gender = p1.Gender,
                                 DateOfBirth = p1.DateOfBirth,
                                 IdentityCardNumber = p1.IdentityCardNumber,
                                 IdentityCardIssueDate = p1.IdentityCardIssueDate,
                                 IdentityCardIssuePlace = p1.IdentityCardIssuePlace,
                                 TaxCode = p1.TaxCode,
                                 SocialInsuranceNumber = p1.SocialInsuranceNumber,
                                 MobilePhone = p1.MobilePhone,
                                 PersonalEmail = p1.PersonalEmail,
                                 PermanentAddress = p1.PermanentAddress,
                                 CurrentAddress = p1.CurrentAddress,
                                 EmergencyContactName = p1.EmergencyContactName,
                                 EmergencyContactPhone = p1.EmergencyContactPhone,
                                 DepartmentFrId = Departments1.DeptName,
                                 ChucVuId = DM_ChucVus1.ChucVu,
                                 ManagerId = NhanSu_EmployeeProfiles1.FullName,
                                 WorkEmail = p1.WorkEmail,
                                 HireDate = p1.HireDate,
                                 OfficialDate = p1.OfficialDate,
                                 CurrentContractType = NhanSu_DM_ContractTypes1.ContractTypeName,
                                 ContractExpirationDate = p1.ContractExpirationDate,
                                 WorkStatusId = NhanSu_DM_WorkStatus1.WorkStatusName,
                                 CurrentBasicSalary = p1.CurrentBasicSalary,
                                 BankAccountNumber = p1.BankAccountNumber,
                                 BankName = p1.BankName,
                                 SalaryPaymentMethod = p1.SalaryPaymentMethod,
                                 TaxDependentsCount = p1.TaxDependentsCount,
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
        public async Task<NhanSu_EmployeeProfileModel> GetDetails(string id)
        {
            using var context = _context.CreateDbContext();
            var data = await (from p1 in context.NhanSu_EmployeeProfiles
                              join Departments1 in context.Departments on p1.DepartmentFrId equals Departments1.Id into Departments11
                              from Departments1 in Departments11.DefaultIfEmpty()
                              join DM_ChucVus1 in context.DM_ChucVus on p1.ChucVuId equals DM_ChucVus1.Id into DM_ChucVus11
                              from DM_ChucVus1 in DM_ChucVus11.DefaultIfEmpty()
                              join NhanSu_EmployeeProfiles1 in context.NhanSu_EmployeeProfiles on p1.ManagerId equals NhanSu_EmployeeProfiles1.Id into NhanSu_EmployeeProfiles11
                              from NhanSu_EmployeeProfiles1 in NhanSu_EmployeeProfiles11.DefaultIfEmpty()
                              join NhanSu_DM_ContractTypes1 in context.NhanSu_DM_ContractTypes on p1.CurrentContractType equals NhanSu_DM_ContractTypes1.Id into NhanSu_DM_ContractTypes11
                              from NhanSu_DM_ContractTypes1 in NhanSu_DM_ContractTypes11.DefaultIfEmpty()
                              join NhanSu_DM_WorkStatus1 in context.NhanSu_DM_WorkStatus on p1.WorkStatusId equals NhanSu_DM_WorkStatus1.Id into NhanSu_DM_WorkStatus11
                              from NhanSu_DM_WorkStatus1 in NhanSu_DM_WorkStatus11.DefaultIfEmpty()
                              join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                              join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                              from approvalUserId in a1.DefaultIfEmpty()
                              join b in context.Departments on p1.DepartmentId equals b.Id into b1
                              from departmentId in b1.DefaultIfEmpty()
                              join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                              from approvalDept in c1.DefaultIfEmpty()
                              where p1.Id == id
                              select new NhanSu_EmployeeProfileModel
                              {
                                  Id = p1.Id,
                                  EmployeeId = p1.EmployeeId,
                                  FullName = p1.FullName,
                                  Gender = p1.Gender,
                                  DateOfBirth = p1.DateOfBirth,
                                  IdentityCardNumber = p1.IdentityCardNumber,
                                  IdentityCardIssueDate = p1.IdentityCardIssueDate,
                                  IdentityCardIssuePlace = p1.IdentityCardIssuePlace,
                                  TaxCode = p1.TaxCode,
                                  SocialInsuranceNumber = p1.SocialInsuranceNumber,
                                  MobilePhone = p1.MobilePhone,
                                  PersonalEmail = p1.PersonalEmail,
                                  PermanentAddress = p1.PermanentAddress,
                                  CurrentAddress = p1.CurrentAddress,
                                  EmergencyContactName = p1.EmergencyContactName,
                                  EmergencyContactPhone = p1.EmergencyContactPhone,
                                  DepartmentFrId = Departments1.DeptName,
                                  ChucVuId = DM_ChucVus1.ChucVu,
                                  ManagerId = NhanSu_EmployeeProfiles1.FullName,
                                  WorkEmail = p1.WorkEmail,
                                  HireDate = p1.HireDate,
                                  OfficialDate = p1.OfficialDate,
                                  CurrentContractType = NhanSu_DM_ContractTypes1.ContractTypeName,
                                  ContractExpirationDate = p1.ContractExpirationDate,
                                  WorkStatusId = NhanSu_DM_WorkStatus1.WorkStatusName,
                                  CurrentBasicSalary = p1.CurrentBasicSalary,
                                  BankAccountNumber = p1.BankAccountNumber,
                                  BankName = p1.BankName,
                                  SalaryPaymentMethod = p1.SalaryPaymentMethod,
                                  TaxDependentsCount = p1.TaxDependentsCount,
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
        public async Task<List<NhanSu_EmployeeProfileModel>> GetHistory(string id)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var data = await (from p1 in context.NhanSu_EmployeeProfile_Logs
                                  join Departments1 in context.Departments on p1.DepartmentFrId equals Departments1.Id into Departments11
                                  from Departments1 in Departments11.DefaultIfEmpty()
                                  join DM_ChucVus1 in context.DM_ChucVus on p1.ChucVuId equals DM_ChucVus1.Id into DM_ChucVus11
                                  from DM_ChucVus1 in DM_ChucVus11.DefaultIfEmpty()
                                  join NhanSu_EmployeeProfiles1 in context.NhanSu_EmployeeProfiles on p1.ManagerId equals NhanSu_EmployeeProfiles1.Id into NhanSu_EmployeeProfiles11
                                  from NhanSu_EmployeeProfiles1 in NhanSu_EmployeeProfiles11.DefaultIfEmpty()
                                  join NhanSu_DM_ContractTypes1 in context.NhanSu_DM_ContractTypes on p1.CurrentContractType equals NhanSu_DM_ContractTypes1.Id into NhanSu_DM_ContractTypes11
                                  from NhanSu_DM_ContractTypes1 in NhanSu_DM_ContractTypes11.DefaultIfEmpty()
                                  join NhanSu_DM_WorkStatus1 in context.NhanSu_DM_WorkStatus on p1.WorkStatusId equals NhanSu_DM_WorkStatus1.Id into NhanSu_DM_WorkStatus11
                                  from NhanSu_DM_WorkStatus1 in NhanSu_DM_WorkStatus11.DefaultIfEmpty()
                                  join createBy in context.ApplicationUsers on p1.CreateBy equals createBy.Id
                                  join a in context.ApplicationUsers on p1.ApprovalUserId equals a.Id into a1
                                  from approvalUserId in a1.DefaultIfEmpty()
                                  join b in context.Departments on p1.DepartmentId equals b.Id into b1
                                  from departmentId in b1.DefaultIfEmpty()
                                  join c in context.Departments on p1.ApprovalDept equals c.Id into c1
                                  from approvalDept in c1.DefaultIfEmpty()
                                  where p1.IdChung == id
                                  orderby p1.CreateAt
                                  select new NhanSu_EmployeeProfileModel
                                  {
                                      Id = p1.Id,
                                      EmployeeId = p1.EmployeeId,
                                      FullName = p1.FullName,
                                      Gender = p1.Gender,
                                      DateOfBirth = p1.DateOfBirth,
                                      IdentityCardNumber = p1.IdentityCardNumber,
                                      IdentityCardIssueDate = p1.IdentityCardIssueDate,
                                      IdentityCardIssuePlace = p1.IdentityCardIssuePlace,
                                      TaxCode = p1.TaxCode,
                                      SocialInsuranceNumber = p1.SocialInsuranceNumber,
                                      MobilePhone = p1.MobilePhone,
                                      PersonalEmail = p1.PersonalEmail,
                                      PermanentAddress = p1.PermanentAddress,
                                      CurrentAddress = p1.CurrentAddress,
                                      EmergencyContactName = p1.EmergencyContactName,
                                      EmergencyContactPhone = p1.EmergencyContactPhone,
                                      DepartmentFrId = Departments1.DeptName,
                                      ChucVuId = DM_ChucVus1.ChucVu,
                                      ManagerId = NhanSu_EmployeeProfiles1.FullName,
                                      WorkEmail = p1.WorkEmail,
                                      HireDate = p1.HireDate,
                                      OfficialDate = p1.OfficialDate,
                                      CurrentContractType = NhanSu_DM_ContractTypes1.ContractTypeName,
                                      ContractExpirationDate = p1.ContractExpirationDate,
                                      WorkStatusId = NhanSu_DM_WorkStatus1.WorkStatusName,
                                      CurrentBasicSalary = p1.CurrentBasicSalary,
                                      BankAccountNumber = p1.BankAccountNumber,
                                      BankName = p1.BankName,
                                      SalaryPaymentMethod = p1.SalaryPaymentMethod,
                                      TaxDependentsCount = p1.TaxDependentsCount,
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
        public async Task<List<NhanSu_EmployeeProfile>> GetAll(string groupId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                var entity = await context.NhanSu_EmployeeProfiles.Where(p => p.IsActive != 100).OrderByDescending(p => p.CreateAt).ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Lỗi khi lấy dữ liệu:  {ex.Message}");
                throw;
            }
        }
        public async Task<NhanSu_EmployeeProfile> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.NhanSu_EmployeeProfiles.Where(x => x.Id.Equals(id) && x.IsActive != 100).FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn : ");
            }
            return entity;
        }
        public async Task Insert(NhanSu_EmployeeProfile entity, string userId)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có dữ liệu dưọc thêm!");
                }
                context.NhanSu_EmployeeProfiles.Add(entity);
                var addLog = new NhanSu_EmployeeProfile_Log()
                {
                    EmployeeId = entity.EmployeeId,
                    FullName = entity.FullName,
                    Gender = entity.Gender,
                    DateOfBirth = entity.DateOfBirth,
                    IdentityCardNumber = entity.IdentityCardNumber,
                    IdentityCardIssueDate = entity.IdentityCardIssueDate,
                    IdentityCardIssuePlace = entity.IdentityCardIssuePlace,
                    TaxCode = entity.TaxCode,
                    SocialInsuranceNumber = entity.SocialInsuranceNumber,
                    MobilePhone = entity.MobilePhone,
                    PersonalEmail = entity.PersonalEmail,
                    PermanentAddress = entity.PermanentAddress,
                    CurrentAddress = entity.CurrentAddress,
                    EmergencyContactName = entity.EmergencyContactName,
                    EmergencyContactPhone = entity.EmergencyContactPhone,
                    DepartmentFrId = entity.DepartmentFrId,
                    ChucVuId = entity.ChucVuId,
                    ManagerId = entity.ManagerId,
                    WorkEmail = entity.WorkEmail,
                    HireDate = entity.HireDate,
                    OfficialDate = entity.OfficialDate,
                    CurrentContractType = entity.CurrentContractType,
                    ContractExpirationDate = entity.ContractExpirationDate,
                    WorkStatusId = entity.WorkStatusId,
                    CurrentBasicSalary = entity.CurrentBasicSalary,
                    BankAccountNumber = entity.BankAccountNumber,
                    BankName = entity.BankName,
                    SalaryPaymentMethod = entity.SalaryPaymentMethod,
                    TaxDependentsCount = entity.TaxDependentsCount,
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
                context.NhanSu_EmployeeProfile_Logs.Add(addLog);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task Update(NhanSu_EmployeeProfile data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn !");
            }
            context.NhanSu_EmployeeProfiles.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.NhanSu_EmployeeProfile_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.NhanSu_EmployeeProfile_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.NhanSu_EmployeeProfile_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.NhanSu_EmployeeProfile_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.NhanSu_EmployeeProfile_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt).FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.NhanSu_EmployeeProfile_Logs.Update(updateLog);
                }
            }
            var addLog = new NhanSu_EmployeeProfile_Log
            {
                FullName = data.FullName,
                Gender = data.Gender,
                DateOfBirth = data.DateOfBirth,
                IdentityCardNumber = data.IdentityCardNumber,
                IdentityCardIssueDate = data.IdentityCardIssueDate,
                IdentityCardIssuePlace = data.IdentityCardIssuePlace,
                TaxCode = data.TaxCode,
                SocialInsuranceNumber = data.SocialInsuranceNumber,
                MobilePhone = data.MobilePhone,
                PersonalEmail = data.PersonalEmail,
                PermanentAddress = data.PermanentAddress,
                CurrentAddress = data.CurrentAddress,
                EmergencyContactName = data.EmergencyContactName,
                EmergencyContactPhone = data.EmergencyContactPhone,
                DepartmentFrId = data.DepartmentFrId,
                ChucVuId = data.ChucVuId,
                ManagerId = data.ManagerId,
                WorkEmail = data.WorkEmail,
                HireDate = data.HireDate,
                OfficialDate = data.OfficialDate,
                CurrentContractType = data.CurrentContractType,
                ContractExpirationDate = data.ContractExpirationDate,
                WorkStatusId = data.WorkStatusId,
                CurrentBasicSalary = data.CurrentBasicSalary,
                BankAccountNumber = data.BankAccountNumber,
                BankName = data.BankName,
                SalaryPaymentMethod = data.SalaryPaymentMethod,
                TaxDependentsCount = data.TaxDependentsCount,
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
            context.NhanSu_EmployeeProfile_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task UpdateMulti(NhanSu_EmployeeProfile[] NhanSu_EmployeeProfiles)
        {
            using var context = _context.CreateDbContext();
            string[] ids = NhanSu_EmployeeProfiles.Select(x => x.Id).ToArray();
            var listEntities = await context.NhanSu_EmployeeProfiles.Where(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var entity in listEntities)
            {
                context.NhanSu_EmployeeProfiles.Update(entity);
            }
            await context.SaveChangesAsync();
        }
        public async Task Delete(NhanSu_EmployeeProfile data, string userId)
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
                    var logdata = (from p in context.NhanSu_EmployeeProfile_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        data.FullName = logdata.FullName;
                        data.Gender = logdata.Gender;
                        data.DateOfBirth = logdata.DateOfBirth;
                        data.IdentityCardNumber = logdata.IdentityCardNumber;
                        data.IdentityCardIssueDate = logdata.IdentityCardIssueDate;
                        data.IdentityCardIssuePlace = logdata.IdentityCardIssuePlace;
                        data.TaxCode = logdata.TaxCode;
                        data.SocialInsuranceNumber = logdata.SocialInsuranceNumber;
                        data.MobilePhone = logdata.MobilePhone;
                        data.PersonalEmail = logdata.PersonalEmail;
                        data.PermanentAddress = logdata.PermanentAddress;
                        data.CurrentAddress = logdata.CurrentAddress;
                        data.EmergencyContactName = logdata.EmergencyContactName;
                        data.EmergencyContactPhone = logdata.EmergencyContactPhone;
                        data.DepartmentFrId = logdata.DepartmentFrId;
                        data.ChucVuId = logdata.ChucVuId;
                        data.ManagerId = logdata.ManagerId;
                        data.WorkEmail = logdata.WorkEmail;
                        data.HireDate = logdata.HireDate;
                        data.OfficialDate = logdata.OfficialDate;
                        data.CurrentContractType = logdata.CurrentContractType;
                        data.ContractExpirationDate = logdata.ContractExpirationDate;
                        data.WorkStatusId = logdata.WorkStatusId;
                        data.CurrentBasicSalary = logdata.CurrentBasicSalary;
                        data.BankAccountNumber = logdata.BankAccountNumber;
                        data.BankName = logdata.BankName;
                        data.SalaryPaymentMethod = logdata.SalaryPaymentMethod;
                        data.TaxDependentsCount = logdata.TaxDependentsCount;
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
                        context.NhanSu_EmployeeProfile_Logs.Update(logdata);
                    }
                    var logupdate = (from p in context.NhanSu_EmployeeProfile_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.NhanSu_EmployeeProfile_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new NhanSu_EmployeeProfile_Log()
                    {
                        EmployeeId = entity.EmployeeId,
                        FullName = data.FullName,
                        Gender = data.Gender,
                        DateOfBirth = data.DateOfBirth,
                        IdentityCardNumber = data.IdentityCardNumber,
                        IdentityCardIssueDate = data.IdentityCardIssueDate,
                        IdentityCardIssuePlace = data.IdentityCardIssuePlace,
                        TaxCode = data.TaxCode,
                        SocialInsuranceNumber = data.SocialInsuranceNumber,
                        MobilePhone = data.MobilePhone,
                        PersonalEmail = data.PersonalEmail,
                        PermanentAddress = data.PermanentAddress,
                        CurrentAddress = data.CurrentAddress,
                        EmergencyContactName = data.EmergencyContactName,
                        EmergencyContactPhone = data.EmergencyContactPhone,
                        DepartmentFrId = data.DepartmentFrId,
                        ChucVuId = data.ChucVuId,
                        ManagerId = data.ManagerId,
                        WorkEmail = data.WorkEmail,
                        HireDate = data.HireDate,
                        OfficialDate = data.OfficialDate,
                        CurrentContractType = data.CurrentContractType,
                        ContractExpirationDate = data.ContractExpirationDate,
                        WorkStatusId = data.WorkStatusId,
                        CurrentBasicSalary = data.CurrentBasicSalary,
                        BankAccountNumber = data.BankAccountNumber,
                        BankName = data.BankName,
                        SalaryPaymentMethod = data.SalaryPaymentMethod,
                        TaxDependentsCount = data.TaxDependentsCount,
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
                    context.NhanSu_EmployeeProfile_Logs.Add(addLog);
                }
                else if (entity.IsActive == 2)
                {
                    throw new Exception($"Thông tin đang chờ duyệt xóa!");
                }
                else if (entity.IsActive == 3)
                {
                    var logupdate = (from p in context.NhanSu_EmployeeProfile_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.NhanSu_EmployeeProfile_Logs.UpdateRange(logupdate);
                    }
                    var addLog = new NhanSu_EmployeeProfile_Log()
                    {
                        EmployeeId = entity.EmployeeId,
                        FullName = data.FullName,
                        Gender = data.Gender,
                        DateOfBirth = data.DateOfBirth,
                        IdentityCardNumber = data.IdentityCardNumber,
                        IdentityCardIssueDate = data.IdentityCardIssueDate,
                        IdentityCardIssuePlace = data.IdentityCardIssuePlace,
                        TaxCode = data.TaxCode,
                        SocialInsuranceNumber = data.SocialInsuranceNumber,
                        MobilePhone = data.MobilePhone,
                        PersonalEmail = data.PersonalEmail,
                        PermanentAddress = data.PermanentAddress,
                        CurrentAddress = data.CurrentAddress,
                        EmergencyContactName = data.EmergencyContactName,
                        EmergencyContactPhone = data.EmergencyContactPhone,
                        DepartmentFrId = data.DepartmentFrId,
                        ChucVuId = data.ChucVuId,
                        ManagerId = data.ManagerId,
                        WorkEmail = data.WorkEmail,
                        HireDate = data.HireDate,
                        OfficialDate = data.OfficialDate,
                        CurrentContractType = data.CurrentContractType,
                        ContractExpirationDate = data.ContractExpirationDate,
                        WorkStatusId = data.WorkStatusId,
                        CurrentBasicSalary = data.CurrentBasicSalary,
                        BankAccountNumber = data.BankAccountNumber,
                        BankName = data.BankName,
                        SalaryPaymentMethod = data.SalaryPaymentMethod,
                        TaxDependentsCount = data.TaxDependentsCount,
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
                    context.NhanSu_EmployeeProfile_Logs.Add(addLog);
                }
                else if (entity.IsActive == 90)
                {
                    data.IsActive = 100;
                    data.IsStatus = "Đã xóa không duyệt !";
                }
            }
            context.NhanSu_EmployeeProfiles.Update(data);
            await context.SaveChangesAsync();
        }
        public async Task Approval(NhanSu_EmployeeProfile data, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(data.Id);
            if (entity == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chonj!");
            }
            context.NhanSu_EmployeeProfiles.Update(data);
            if (data.IsActive == 3)
            {
                var updateLog = await (from p in context.NhanSu_EmployeeProfile_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.NhanSu_EmployeeProfile_Logs.UpdateRange(updateLog);
            }
            else if (data.IsActive == 100)
            {
                var updateLog = await (from p in context.NhanSu_EmployeeProfile_Logs
                                       where p.IdChung == entity.Id && p.IsValid == true
                                       select p).ToListAsync();
                updateLog.ForEach(p => p.IsValid = false);
                context.NhanSu_EmployeeProfile_Logs.UpdateRange(updateLog);
            }
            else if (entity.IsActive != 3)
            {
                var updateLog = await (from p in context.NhanSu_EmployeeProfile_Logs
                                       where p.IdChung == entity.Id
                                       select p).OrderByDescending(p => p.CreateAt).FirstOrDefaultAsync();
                if (updateLog != null)
                {
                    updateLog.IsValid = false;
                    context.NhanSu_EmployeeProfile_Logs.Update(updateLog);
                }
            }
            var addLog = new NhanSu_EmployeeProfile_Log()
            {
                EmployeeId = entity.EmployeeId,
                FullName = data.FullName,
                Gender = data.Gender,
                DateOfBirth = data.DateOfBirth,
                IdentityCardNumber = data.IdentityCardNumber,
                IdentityCardIssueDate = data.IdentityCardIssueDate,
                IdentityCardIssuePlace = data.IdentityCardIssuePlace,
                TaxCode = data.TaxCode,
                SocialInsuranceNumber = data.SocialInsuranceNumber,
                MobilePhone = data.MobilePhone,
                PersonalEmail = data.PersonalEmail,
                PermanentAddress = data.PermanentAddress,
                CurrentAddress = data.CurrentAddress,
                EmergencyContactName = data.EmergencyContactName,
                EmergencyContactPhone = data.EmergencyContactPhone,
                DepartmentFrId = data.DepartmentFrId,
                ChucVuId = data.ChucVuId,
                ManagerId = data.ManagerId,
                WorkEmail = data.WorkEmail,
                HireDate = data.HireDate,
                OfficialDate = data.OfficialDate,
                CurrentContractType = data.CurrentContractType,
                ContractExpirationDate = data.ContractExpirationDate,
                WorkStatusId = data.WorkStatusId,
                CurrentBasicSalary = data.CurrentBasicSalary,
                BankAccountNumber = data.BankAccountNumber,
                BankName = data.BankName,
                SalaryPaymentMethod = data.SalaryPaymentMethod,
                TaxDependentsCount = data.TaxDependentsCount,
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
            context.NhanSu_EmployeeProfile_Logs.Add(addLog);
            await context.SaveChangesAsync();
        }
        public async Task NoApproval(NhanSu_EmployeeProfile data, string userId)
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
                    var logdata = (from p in context.NhanSu_EmployeeProfile_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   orderby p.CreateAt ascending
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.FullName = logdata.FullName;
                        entity.Gender = logdata.Gender;
                        entity.DateOfBirth = logdata.DateOfBirth;
                        entity.IdentityCardNumber = logdata.IdentityCardNumber;
                        entity.IdentityCardIssueDate = logdata.IdentityCardIssueDate;
                        entity.IdentityCardIssuePlace = logdata.IdentityCardIssuePlace;
                        entity.TaxCode = logdata.TaxCode;
                        entity.SocialInsuranceNumber = logdata.SocialInsuranceNumber;
                        entity.MobilePhone = logdata.MobilePhone;
                        entity.PersonalEmail = logdata.PersonalEmail;
                        entity.PermanentAddress = logdata.PermanentAddress;
                        entity.CurrentAddress = logdata.CurrentAddress;
                        entity.EmergencyContactName = logdata.EmergencyContactName;
                        entity.EmergencyContactPhone = logdata.EmergencyContactPhone;
                        entity.DepartmentFrId = logdata.DepartmentFrId;
                        entity.ChucVuId = logdata.ChucVuId;
                        entity.ManagerId = logdata.ManagerId;
                        entity.WorkEmail = logdata.WorkEmail;
                        entity.HireDate = logdata.HireDate;
                        entity.OfficialDate = logdata.OfficialDate;
                        entity.CurrentContractType = logdata.CurrentContractType;
                        entity.ContractExpirationDate = logdata.ContractExpirationDate;
                        entity.WorkStatusId = logdata.WorkStatusId;
                        entity.CurrentBasicSalary = logdata.CurrentBasicSalary;
                        entity.BankAccountNumber = logdata.BankAccountNumber;
                        entity.BankName = logdata.BankName;
                        entity.SalaryPaymentMethod = logdata.SalaryPaymentMethod;
                        entity.TaxDependentsCount = logdata.TaxDependentsCount;
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
                    var logupdate = (from p in context.NhanSu_EmployeeProfile_Logs
                                     where p.IdChung == entity.Id && p.IsValid == true && p.CreateAt >= logdata.CreateAt
                                     select p).ToList();
                    if (logupdate != null)
                    {
                        logupdate.ForEach(p => p.IsValid = false);
                        context.NhanSu_EmployeeProfile_Logs.UpdateRange(logupdate);
                    }
                }
                else if (entity.IsActive == 2)
                {
                    var logdata = (from p in context.NhanSu_EmployeeProfile_Logs
                                   where p.IdChung == entity.Id && p.IsValid == true
                                   select p).Take(1).FirstOrDefault();
                    if (logdata != null)
                    {
                        entity.FullName = logdata.FullName;
                        entity.Gender = logdata.Gender;
                        entity.DateOfBirth = logdata.DateOfBirth;
                        entity.IdentityCardNumber = logdata.IdentityCardNumber;
                        entity.IdentityCardIssueDate = logdata.IdentityCardIssueDate;
                        entity.IdentityCardIssuePlace = logdata.IdentityCardIssuePlace;
                        entity.TaxCode = logdata.TaxCode;
                        entity.SocialInsuranceNumber = logdata.SocialInsuranceNumber;
                        entity.MobilePhone = logdata.MobilePhone;
                        entity.PersonalEmail = logdata.PersonalEmail;
                        entity.PermanentAddress = logdata.PermanentAddress;
                        entity.CurrentAddress = logdata.CurrentAddress;
                        entity.EmergencyContactName = logdata.EmergencyContactName;
                        entity.EmergencyContactPhone = logdata.EmergencyContactPhone;
                        entity.DepartmentFrId = logdata.DepartmentFrId;
                        entity.ChucVuId = logdata.ChucVuId;
                        entity.ManagerId = logdata.ManagerId;
                        entity.WorkEmail = logdata.WorkEmail;
                        entity.HireDate = logdata.HireDate;
                        entity.OfficialDate = logdata.OfficialDate;
                        entity.CurrentContractType = logdata.CurrentContractType;
                        entity.ContractExpirationDate = logdata.ContractExpirationDate;
                        entity.WorkStatusId = logdata.WorkStatusId;
                        entity.CurrentBasicSalary = logdata.CurrentBasicSalary;
                        entity.BankAccountNumber = logdata.BankAccountNumber;
                        entity.BankName = logdata.BankName;
                        entity.SalaryPaymentMethod = logdata.SalaryPaymentMethod;
                        entity.TaxDependentsCount = logdata.TaxDependentsCount;
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
            var addLog = new NhanSu_EmployeeProfile_Log()
            {
                EmployeeId = entity.EmployeeId,
                FullName = data.FullName,
                Gender = data.Gender,
                DateOfBirth = data.DateOfBirth,
                IdentityCardNumber = data.IdentityCardNumber,
                IdentityCardIssueDate = data.IdentityCardIssueDate,
                IdentityCardIssuePlace = data.IdentityCardIssuePlace,
                TaxCode = data.TaxCode,
                SocialInsuranceNumber = data.SocialInsuranceNumber,
                MobilePhone = data.MobilePhone,
                PersonalEmail = data.PersonalEmail,
                PermanentAddress = data.PermanentAddress,
                CurrentAddress = data.CurrentAddress,
                EmergencyContactName = data.EmergencyContactName,
                EmergencyContactPhone = data.EmergencyContactPhone,
                DepartmentFrId = data.DepartmentFrId,
                ChucVuId = data.ChucVuId,
                ManagerId = data.ManagerId,
                WorkEmail = data.WorkEmail,
                HireDate = data.HireDate,
                OfficialDate = data.OfficialDate,
                CurrentContractType = data.CurrentContractType,
                ContractExpirationDate = data.ContractExpirationDate,
                WorkStatusId = data.WorkStatusId,
                CurrentBasicSalary = data.CurrentBasicSalary,
                BankAccountNumber = data.BankAccountNumber,
                BankName = data.BankName,
                SalaryPaymentMethod = data.SalaryPaymentMethod,
                TaxDependentsCount = data.TaxDependentsCount,
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
            context.NhanSu_EmployeeProfile_Logs.Add(addLog);
            context.NhanSu_EmployeeProfiles.Update(entity);
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
            context.Set<NhanSu_EmployeeProfile>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> CheckStatus(string ids, string name)
        {
            using var context = _context.CreateDbContext();
            var model = await context.NhanSu_EmployeeProfiles.Where(p => p.Id == ids && p.IsActive != 100).FirstOrDefaultAsync();
            if (model == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu đã chọn!");
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
        public async Task<bool> CheckSave(NhanSu_EmployeeProfile input)
        {
            using var context = _context.CreateDbContext();

            // 1. Kiểm tra trùng EmployeeId (trừ bản ghi hiện tại, trạng thái chưa xóa)
            var isDuplicateEmployeeId = await context.NhanSu_EmployeeProfiles
                .AnyAsync(p =>
                    p.GroupId == input.GroupId &&
                    p.Id != input.Id &&
                    p.IsActive != 100 && p.IsActive != 90 &&
                    p.EmployeeId == input.EmployeeId
                );
            if (isDuplicateEmployeeId)
            {
                throw new Exception("EmployeeID đã tồn tại, vui lòng kiểm tra lại!");
            }

            // 2. Kiểm tra trùng toàn bộ thông tin (trừ bản ghi hiện tại, trạng thái chưa xóa)
            var isDuplicateProfile = await context.NhanSu_EmployeeProfiles
                .AnyAsync(p =>
                    p.GroupId == input.GroupId &&
                    p.Id != input.Id &&
                    p.IsActive != 100 && p.IsActive != 90 &&
                    p.EmployeeId == input.EmployeeId &&
                    p.FullName == input.FullName &&
                    p.Gender == input.Gender &&
                    p.DateOfBirth == input.DateOfBirth &&
                    p.IdentityCardNumber == input.IdentityCardNumber &&
                    p.IdentityCardIssueDate == input.IdentityCardIssueDate &&
                    p.IdentityCardIssuePlace == input.IdentityCardIssuePlace &&
                    p.TaxCode == input.TaxCode &&
                    p.SocialInsuranceNumber == input.SocialInsuranceNumber &&
                    p.MobilePhone == input.MobilePhone &&
                    p.PersonalEmail == input.PersonalEmail &&
                    p.PermanentAddress == input.PermanentAddress &&
                    p.CurrentAddress == input.CurrentAddress &&
                    p.EmergencyContactName == input.EmergencyContactName &&
                    p.EmergencyContactPhone == input.EmergencyContactPhone &&
                    p.DepartmentFrId == input.DepartmentFrId &&
                    p.ChucVuId == input.ChucVuId &&
                    p.ManagerId == input.ManagerId &&
                    p.WorkEmail == input.WorkEmail &&
                    p.HireDate == input.HireDate &&
                    p.OfficialDate == input.OfficialDate &&
                    p.CurrentContractType == input.CurrentContractType &&
                    p.ContractExpirationDate == input.ContractExpirationDate &&
                    p.WorkStatusId == input.WorkStatusId &&
                    p.CurrentBasicSalary == input.CurrentBasicSalary &&
                    p.BankAccountNumber == input.BankAccountNumber &&
                    p.BankName == input.BankName &&
                    p.SalaryPaymentMethod == input.SalaryPaymentMethod &&
                    p.TaxDependentsCount == input.TaxDependentsCount
                );
            if (isDuplicateProfile)
            {
                throw new Exception("Thông tin bạn nhập đã tồn tại!");
            }

            return true;
        }
        public async Task<bool> CheckEdit(NhanSu_EmployeeProfile input)
        {
            using var context = _context.CreateDbContext();
            var isDuplicate = await context.NhanSu_EmployeeProfile_Logs
                .AnyAsync(p =>
                    p.GroupId == input.GroupId &&
                    p.IdChung != input.Id &&
                    p.IsValid == true &&
                    p.IsActive != 100 &&
                    p.Id != input.Id &&
                    p.FullName == input.FullName &&
                    p.Gender == input.Gender &&
                    p.DateOfBirth == input.DateOfBirth &&
                    p.IdentityCardNumber == input.IdentityCardNumber &&
                    p.IdentityCardIssueDate == input.IdentityCardIssueDate &&
                    p.IdentityCardIssuePlace == input.IdentityCardIssuePlace &&
                    p.TaxCode == input.TaxCode &&
                    p.SocialInsuranceNumber == input.SocialInsuranceNumber &&
                    p.MobilePhone == input.MobilePhone &&
                    p.PersonalEmail == input.PersonalEmail &&
                    p.PermanentAddress == input.PermanentAddress &&
                    p.CurrentAddress == input.CurrentAddress &&
                    p.EmergencyContactName == input.EmergencyContactName &&
                    p.EmergencyContactPhone == input.EmergencyContactPhone &&
                    p.DepartmentFrId == input.DepartmentFrId &&
                    p.ChucVuId == input.ChucVuId &&
                    p.ManagerId == input.ManagerId &&
                    p.WorkEmail == input.WorkEmail &&
                    p.HireDate == input.HireDate &&
                    p.OfficialDate == input.OfficialDate &&
                    p.CurrentContractType == input.CurrentContractType &&
                    p.ContractExpirationDate == input.ContractExpirationDate &&
                    p.WorkStatusId == input.WorkStatusId &&
                    p.CurrentBasicSalary == input.CurrentBasicSalary &&
                    p.BankAccountNumber == input.BankAccountNumber &&
                    p.BankName == input.BankName &&
                    p.SalaryPaymentMethod == input.SalaryPaymentMethod &&
                    p.TaxDependentsCount == input.TaxDependentsCount
                );
            if (isDuplicate)
            {
                throw new Exception("Thông tin bạn nhập đã tồn tại !");
            }
            return true;
        }
        public async Task<bool> CheckDelete(NhanSu_EmployeeProfile input)
        {
            using var context = _context.CreateDbContext();

            // Kiểm tra liên kết với lịch sử lương
            bool hasSalaryHistory = await context.NhanSu_SalaryHistorys
                .AnyAsync(x => x.EmployeeId == input.EmployeeId && x.IsActive != 100);

            if (hasSalaryHistory)
                throw new Exception("Không thể xóa: Nhân sự đã có lịch sử lương.");

            // Kiểm tra liên kết với kỷ luật
            bool hasDiscipline = await context.NhanSu_Disciplines
                .AnyAsync(x => x.EmployeeId == input.EmployeeId && x.IsActive != 100);

            if (hasDiscipline)
                throw new Exception("Không thể xóa: Nhân sự đã có quyết định kỷ luật.");

            // Kiểm tra liên kết với nghỉ việc
            bool hasTermination = await context.NhanSu_Terminations
                .AnyAsync(x => x.EmployeeId == input.EmployeeId && x.IsActive != 100);

            if (hasTermination)
                throw new Exception("Không thể xóa: Nhân sự đã có quyết định nghỉ việc.");

            // Có thể bổ sung kiểm tra liên kết khác tại đây...

            return true;
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
