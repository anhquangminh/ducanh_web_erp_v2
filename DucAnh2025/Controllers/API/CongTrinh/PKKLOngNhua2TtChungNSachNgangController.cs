using DucAnh2025.Models;
using DucAnh2025.Models.CongTrinh;
using DucAnh2025.Repository;
using DucAnh2025.Repository.CongTrinh;
using DucAnh2025.Repository.CongTrinh.DanhMuc;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.ViewModel;
using DucAnh2025.ViewModels.CongTrinh;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DucAnh2025.Controllers.API
{
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class PKKLOngNhua2TtChungNSachNgangController : ControllerBase
    {
        private readonly IApprovalTaskRepository _approvalTaskService;
        private readonly IPKKL_OngNhua_2TtChungNSachNgangRepository _2TtChungNSachNgangService;
        private readonly IDM_TuyenDuongRepository _tuyenDuongService;
        private readonly IDM_LyTrinhRepository _lyTrinhService;
        private readonly IDM_HangMucCongViecRepository _hangMucCongViecService;
        private readonly IDM_LoaiCauKienRepository _loaiCauKienService;
        private readonly IDM_HangMucKhoiLuongRepository _hangMucKhoiLuongService;
        private readonly IDM_LoaiKhoiLuongRepository _loaiKhoiLuongService;
        private readonly IDM_TrangThaiThiCongRepository _trangThaiThiCongService;
        private readonly IDM_HinhThucDapTraRepository _hinhThucDapTraService;

        private readonly IEmailHistoryRepository _emailHistoryService;
        private readonly IPhanQuyenRepository _phanQuyenService;
        private readonly IApplicationUserRepository _applicationUserService;
        private readonly IPermissionRepository _permissionService;
        public PKKLOngNhua2TtChungNSachNgangController(
            IApprovalTaskRepository approvalTaskRepository,
            IPKKL_OngNhua_2TtChungNSachNgangRepository pKKL_OngNhua_2TtChungNSachNgangRepository,
            IDM_TuyenDuongRepository dM_TuyenDuongRepository,
            IDM_LyTrinhRepository dM_LyTrinhRepository,
            IDM_HangMucCongViecRepository dM_HangMucCongViecRepository,
            IDM_LoaiCauKienRepository dM_LoaiCauKienRepository,
            IDM_HangMucKhoiLuongRepository dM_HangMucKhoiLuongRepository,
            IDM_LoaiKhoiLuongRepository dM_LoaiKhoiLuongRepository,
            IDM_TrangThaiThiCongRepository dM_TrangThaiThiCongRepository,
            IDM_HinhThucDapTraRepository dM_HinhThucDapTraRepository,

            IEmailHistoryRepository emailHistoryService,
            IPhanQuyenRepository phanQuyenService,
            IApplicationUserRepository applicationUserService,
            IPermissionRepository permissionService)
        {
            _approvalTaskService = approvalTaskRepository;
            _2TtChungNSachNgangService = pKKL_OngNhua_2TtChungNSachNgangRepository;
            _tuyenDuongService = dM_TuyenDuongRepository;
            _lyTrinhService = dM_LyTrinhRepository;
            _hangMucCongViecService = dM_HangMucCongViecRepository;
            _hangMucKhoiLuongService = dM_HangMucKhoiLuongRepository;
            _loaiCauKienService = dM_LoaiCauKienRepository;
            _loaiKhoiLuongService = dM_LoaiKhoiLuongRepository;
            _trangThaiThiCongService = dM_TrangThaiThiCongRepository;
            _hinhThucDapTraService = dM_HinhThucDapTraRepository;

            _emailHistoryService = emailHistoryService;
            _phanQuyenService = phanQuyenService;
            _applicationUserService = applicationUserService;
            _permissionService = permissionService;
        }
        public string parentMajorId = "85fe739a-0bfa-4f8e-9ab9-55a85e5670d9";
        public string tableName = "PKKL_OngNhua_2TtChungNSachNgangs";

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetByVM(string groupId, [FromBody] PKKL_OngNhua_2TtChungNSachNgangModel? input = null)
        {
            try
            {
                var model = input ?? new PKKL_OngNhua_2TtChungNSachNgangModel();
                var list = await _2TtChungNSachNgangService.GetAllByVM(model, groupId);
                return Ok(new ApiResponse<IEnumerable<object>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetAll(string groupId)
        {
            try
            {
                var list = await _2TtChungNSachNgangService.GetAll(groupId);
                return Ok(new ApiResponse<IEnumerable<object>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }
        [HttpGet("GetById")]
        public async Task<ActionResult<ApiResponse<object>>> GetById(string id)
        {
            try
            {
                var result = await _2TtChungNSachNgangService.GetById(id);
                return Ok(new ApiResponse<object>(true, "Lưu thành công.", result));

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpGet("GetHistory")]
        public async Task<ActionResult<ApiResponse<object>>> GetHistory(string id)
        {
            try
            {
                var result = await _2TtChungNSachNgangService.GetHistory(id);
                return Ok(new ApiResponse<object>(true, "Lưu thành công.", result));

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        public async Task<bool> validateDM(PKKL_OngNhua_2TtChungNSachNgang input)
        {
            var baseTime = DateTime.Now;

            // Helper để lọc ra các Id hợp lệ
            string[] GetValidIds(params string[] ids)
                => ids.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();

            var id_DM_TuyenDuongs = GetValidIds(input.Id_TuyenDuong);
            if (id_DM_TuyenDuongs.Any())
                await _tuyenDuongService.CheckExclusive(id_DM_TuyenDuongs, baseTime);

            var id_DM_LyTrinhs = GetValidIds(input.Id_TuLyTrinh, input.Id_DenLyTrinh);
            if (id_DM_LyTrinhs.Any())
                await _lyTrinhService.CheckExclusive(id_DM_LyTrinhs, baseTime);

            var id_DM_HangMucCongViecs = GetValidIds(input.Id_HangMucCongViec);
            if (id_DM_HangMucCongViecs.Any())
                await _hangMucCongViecService.CheckExclusive(id_DM_HangMucCongViecs, baseTime);

            var id_DM_LoaiCauKiens = GetValidIds(input.Id_LoaiCauKien, input.Id_LoaiCauKienOngThep);
            if (id_DM_LoaiCauKiens.Any())
                await _loaiCauKienService.CheckExclusive(id_DM_LoaiCauKiens, baseTime);

            var id_DM_HangMucKhoiLuongs = GetValidIds(input.Id_HangMucKhoiLuong, input.Id_HangMucKhoiLuongOngThep);
            if (id_DM_HangMucKhoiLuongs.Any())
                await _hangMucKhoiLuongService.CheckExclusive(id_DM_HangMucKhoiLuongs, baseTime);

            var id_DM_LoaiKhoiLuongs = GetValidIds(input.Id_LoaiKhoiLuong, input.Id_LoaiKhoiLuongOngThep);
            if (id_DM_LoaiKhoiLuongs.Any())
                await _loaiKhoiLuongService.CheckExclusive(id_DM_LoaiKhoiLuongs, baseTime);

            var id_DM_TrangThaiThiCongs = GetValidIds(
                input.Id_TrangThaiThiCongOngNhua,
                input.Id_TrangThaiThiCongOngThep,
                input.Id_TrangThaiThiCongDaoDat,
                input.Id_TrangThaiThiCongDapCat,
                input.Id_TrangThaiThiCongDapDat,
                input.Id_TrangThaiThiCongDatThua
            );
            if (id_DM_TrangThaiThiCongs.Any())
                await _trangThaiThiCongService.CheckExclusive(id_DM_TrangThaiThiCongs, baseTime);

            var id_DM_HinhThucDapTras = GetValidIds(input.Id_HinhThucDapTra);
            if (id_DM_HinhThucDapTras.Any())
                await _hinhThucDapTraService.CheckExclusive(id_DM_HinhThucDapTras, baseTime);

            return true;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] PKKL_OngNhua_2TtChungNSachNgang input, [FromQuery] bool isEdit)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var groupId = user.GroupId;
                var now = DateTime.Now;

                input.GroupId = groupId;
                input.CreateBy = userId;
                input.CreateAt = now;

                bool isEditSpecial = !string.IsNullOrEmpty(input.Id) && input.Ordinarily == 0 && input.IsActive == 3;
                bool editCheck = isEdit || isEditSpecial;

                await validateDM(input);
                // Lấy quyền
                var listPer = await _permissionService.GetPermissionsByTable(tableName);
                if (listPer == null || !listPer.Any())
                    return Ok(new ApiResponse<object>(false, "Chưa cài đặt quyền cho chức năng", null));

                var firstPer = listPer.First();
                var majorId = firstPer.MajorId;
                var idTao = listPer.FirstOrDefault(p => p.PermissionType == 3)?.Id;
                var idSua = listPer.FirstOrDefault(p => p.PermissionType == 4)?.Id;

                if (editCheck)
                {
                    // Quyền sửa
                    if (!await _phanQuyenService.CheckPermission(groupId, input.GroupId, user, idSua))
                        return Ok(new ApiResponse<object>(false, "Bạn không có quyền sửa", null));

                    // Check dữ liệu
                    if (!await _2TtChungNSachNgangService.CheckExclusive(new[] { input.Id }, now))
                        return Ok(new ApiResponse<object>(false, "Không thể sửa dữ liệu", null));

                    var firstApproval = await _phanQuyenService.GetFirstApprovalStep(input.GroupId, majorId, idSua);
                    var lastApproval = await _phanQuyenService.GetLastApprovalStep(input.GroupId, majorId, idSua);

                    input.DepartmentId = firstApproval.DepartmentId;
                    input.ApprovalId = firstApproval.Id;
                    input.LastApprovalId = lastApproval.Id;
                    input.Ordinarily = input.IsActive == 3 ? input.Ordinarily + 1 : input.Ordinarily;
                    input.IsActive = 1;
                    input.ApprovalOrder = 1;
                    input.DepartmentOrder = 1;
                    input.IsStatus = firstApproval.Content;
                    input.ApprovalUserId = null;
                    input.DateApproval = null;
                    input.ApprovalDept = null;

                    await _2TtChungNSachNgangService.Update(input, "");

                    // Nội dung email
                    string content = await BuildEmailContentEdit(input.Id);

                    // Tạo ApprovalTask
                    var dataApproval = await _approvalTaskService.GetByOriginalId(input.Id);
                    if (dataApproval == null)
                    {
                        await _approvalTaskService.Insert(CreateApprovalTask("2. TT chung N.Sạch ngang - Sửa", content, input, tableName, parentMajorId, majorId, userId), userId);
                    }
                    else
                    {
                        await _approvalTaskService.Update(UpdateApprovalTask(dataApproval.Id, "2. TT chung N.Sạch ngang - Sửa", content, input, tableName, parentMajorId, majorId, userId), userId);
                    }
                    // Gửi email
                    await InsertEmailHistories(content, "2. TT chung N.Sạch ngang - Sửa.", input.GroupId, groupId, firstApproval.Id, input.Id, userId);

                    return Ok(new ApiResponse<object>(true, "Cập nhật thành công.", null));
                }
                else
                {
                    // Quyền thêm
                    if (!await _phanQuyenService.CheckPermission(groupId, input.GroupId, user, idTao))
                        return Ok(new ApiResponse<object>(false, "Bạn không có quyền thêm mới", null));

                    var firstApproval = await _phanQuyenService.GetFirstApprovalStep(input.GroupId, majorId, idTao);
                    var lastApproval = await _phanQuyenService.GetLastApprovalStep(input.GroupId, majorId, idTao);

                    input.Id = Guid.NewGuid().ToString();
                    input.DepartmentId = firstApproval.DepartmentId;
                    input.ApprovalId = firstApproval.Id;
                    input.LastApprovalId = lastApproval.Id;
                    input.IsActive = 0;
                    input.ApprovalOrder = 1;
                    input.DepartmentOrder = 1;
                    input.Ordinarily = 0;
                    input.IsStatus = firstApproval.Content;
                    input.ApprovalUserId = null;
                    input.DateApproval = null;
                    input.ApprovalDept = null;
                    await _2TtChungNSachNgangService.Insert(input, "");
                    string content = await BuildEmailContentAdd(input.Id);
                    await _approvalTaskService.Insert(CreateApprovalTask("2. TT chung N.Sạch ngang - Thêm mới", content, input, tableName, parentMajorId, majorId, userId), userId);

                    await InsertEmailHistories(content, "2. TT chung N.Sạch ngang - Thêm", input.GroupId, groupId, firstApproval.Id, input.Id, userId);

                    return Ok(new ApiResponse<object>(true, "Lưu thành công.", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Ok(new ApiResponse<object>(false, "Không xác định được dữ liệu.", null));

            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var groupId = user.GroupId;
                var baseTime = DateTime.Now;

                var query = await _2TtChungNSachNgangService.GetById(id);
                if (query == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy dữ liệu.", null));

                if (query.IsActive == 2)
                    return Ok(new ApiResponse<object>(false, "Thông tin bạn chọn đang chờ", null));

                if (!await _2TtChungNSachNgangService.CheckDelete(query))
                    return Ok(new ApiResponse<object>(false, "Không thể xoá - đã liên kết dữ liệu", null));

                var listPer = await _permissionService.GetPermissionsByTable(tableName);
                if (listPer == null || !listPer.Any())
                    return Ok(new ApiResponse<object>(false, "Chưa cài đặt quyền cho chức năng", null));

                var majorId = listPer.FirstOrDefault()?.MajorId;
                var deletePermissionId = listPer.FirstOrDefault(p => p.PermissionType == 5)?.Id;

                if (!await _phanQuyenService.CheckPermission(user.GroupId, query.GroupId, user, deletePermissionId))
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền xóa", null));

                if (!await _2TtChungNSachNgangService.CheckExclusive(new[] { id }, baseTime))
                    return Ok(new ApiResponse<object>(false, "Xóa thất bại - dữ liệu đã bị thay đổi", null));

                var firstApproval = await _phanQuyenService.GetFirstApprovalStep(query.GroupId, majorId, deletePermissionId);
                var lastApproval = await _phanQuyenService.GetLastApprovalStep(query.GroupId, majorId, deletePermissionId);

                query.DepartmentId = firstApproval.DepartmentId;
                query.ApprovalId = firstApproval.Id;
                query.LastApprovalId = lastApproval.Id;
                query.IsActive = 2;
                query.ApprovalOrder = 1;
                query.DepartmentOrder = 1;
                query.CreateAt = DateTime.Now;
                query.CreateBy = userId;
                query.IsStatus = firstApproval.Content;
                query.ApprovalUserId = null;
                query.DateApproval = null;
                query.ApprovalDept = null;

                await _2TtChungNSachNgangService.Update(query, "");

                var content = await BuildEmailContentDelete(id);
                var dataApproval = await _approvalTaskService.GetByOriginalId(id);
                if (dataApproval == null)
                {
                    await _approvalTaskService.Insert(CreateApprovalTask("2. TT chung N.Sạch ngang - xóa", content, query, tableName, parentMajorId, majorId, userId), userId);
                }
                else
                {
                    await _approvalTaskService.Update(UpdateApprovalTask(dataApproval.Id, "2. TT chung N.Sạch ngang - xóa", content, query, tableName, parentMajorId, majorId, userId), userId);
                }

                // Hàm gửi email đã có → dùng lại
                await InsertEmailHistories(content, "2. TT chung N.Sạch ngang - xóa", query.GroupId, groupId, firstApproval.Id, query.Id, userId);

                return Ok(new ApiResponse<object>(true, "Xóa thành công.", null));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpPost("Duyet")]
        public async Task<ActionResult<ApiResponse<object>>> Duyet(string id)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var entity = await _2TtChungNSachNgangService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy thông tin đã chọn", null));

                var checkquyen = await _phanQuyenService.CheckApproval(entity.GroupId, entity.DepartmentId, user, entity.ApprovalId);
                if (!checkquyen)
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền duyệt", null));

                string thongbao = entity.IsStatus;

                var listPer = await _permissionService.GetPermissionsByTable(tableName);
                if (listPer == null || !listPer.Any())
                    return Ok(new ApiResponse<object>(false, "Chưa cài đặt quyền cho chức năng", null));
                var MajorId = listPer.FirstOrDefault()?.MajorId;
                var id_tao = listPer.FirstOrDefault(p => p.PermissionType == 3)?.Id;
                var id_sua = listPer.FirstOrDefault(p => p.PermissionType == 4)?.Id;
                var id_xoa = listPer.FirstOrDefault(p => p.PermissionType == 5)?.Id;

                if (entity.IsActive != 3)
                {
                    if (entity.ApprovalId == entity.LastApprovalId)
                    {
                        if (entity.IsActive == 0 || entity.IsActive == 1)
                        {
                            entity.ApprovalUserId = userId;
                            entity.DateApproval = DateTime.Now;
                            entity.ApprovalDept = entity.DepartmentId;
                            entity.DepartmentId = null;
                            entity.ApprovalId = null;
                            entity.ApprovalOrder = 0;
                            entity.DepartmentOrder = 0;
                            entity.LastApprovalId = null;
                            entity.IsActive = 3;
                            entity.IsStatus = "Đã duyệt";
                        }
                        else if (entity.IsActive == 2)
                        {
                            entity.ApprovalUserId = userId;
                            entity.DateApproval = DateTime.Now;
                            entity.ApprovalDept = entity.DepartmentId;
                            entity.DepartmentId = null;
                            entity.ApprovalId = null;
                            entity.ApprovalOrder = 0;
                            entity.DepartmentOrder = 0;
                            entity.LastApprovalId = null;
                            entity.IsActive = 100;
                            entity.IsStatus = "Đã duyệt xóa";
                        }
                    }
                    else
                    {
                        var nextApproval = await _phanQuyenService.GetNextApprovalStep(
                            entity.GroupId,
                            MajorId,
                            entity.IsActive == 0 ? id_tao
                                : entity.IsActive == 1 ? id_sua
                                : entity.IsActive == 2 ? id_xoa
                                : "",
                            entity.DepartmentId,
                            entity.DepartmentOrder,
                            entity.ApprovalOrder
                        );
                        entity.DepartmentId = nextApproval.DeptId;
                        entity.IsStatus = nextApproval.Content;
                        entity.ApprovalId = nextApproval.Id;
                        entity.ApprovalOrder = nextApproval.ApprovalStep;
                        entity.DepartmentOrder = nextApproval.DeptOrder;
                        entity.ApprovalUserId = userId;
                        entity.DateApproval = DateTime.Now;
                        entity.ApprovalDept = entity.DepartmentId;
                    }

                    var rowApproval = await _approvalTaskService.GetByOriginalId(entity.Id);
                    rowApproval.ApprovalUserId = entity.ApprovalUserId;
                    rowApproval.DateApproval = entity.DateApproval;
                    rowApproval.ApprovalDept = entity.ApprovalDept;
                    rowApproval.DepartmentId = entity.DepartmentId ?? "";
                    rowApproval.ApprovalId = entity.ApprovalId ?? "";
                    rowApproval.ApprovalOrder = entity.ApprovalOrder;
                    rowApproval.DepartmentOrder = entity.DepartmentOrder;
                    rowApproval.LastApprovalId = entity.LastApprovalId ?? "";
                    rowApproval.IsActive = entity.IsActive;
                    rowApproval.IsStatus = entity.IsStatus;
                    await _approvalTaskService.Update(rowApproval, userId);

                    await _2TtChungNSachNgangService.Approval(entity, userId);
                    return Ok(new ApiResponse<object>(true, "Đã duyệt " + (thongbao ?? "").ToLower(), null));
                }
                else
                {
                    return Ok(new ApiResponse<object>(false, "Bản ghi đã duyệt.", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpPost("HuyDuyet")]
        public async Task<ActionResult<ApiResponse<object>>> HuyDuyet(string id)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                var userId = user.Id;
                var entity = await _2TtChungNSachNgangService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<object>(false, "Không tìm thấy thông tin đã chọn", null));

                var checkquyen = await _phanQuyenService.CheckApproval(entity.GroupId, entity.DepartmentId, user, entity.ApprovalId);
                if (!checkquyen)
                    return Ok(new ApiResponse<object>(false, "Bạn không có quyền hủy duyệt", null));

                string thongbao = entity.IsStatus;

                if (entity.IsActive != 3)
                {
                    await _2TtChungNSachNgangService.NoApproval(entity, userId);
                    var dataNoApproval = await _2TtChungNSachNgangService.GetById(entity.Id);

                    var rowApproval = await _approvalTaskService.GetByOriginalId(entity.Id);
                    rowApproval.ApprovalUserId = dataNoApproval.ApprovalUserId;
                    rowApproval.DateApproval = dataNoApproval.DateApproval;
                    rowApproval.ApprovalDept = dataNoApproval.ApprovalDept;
                    rowApproval.DepartmentId = dataNoApproval.DepartmentId ?? "";
                    rowApproval.ApprovalId = dataNoApproval.ApprovalId ?? "";
                    rowApproval.ApprovalOrder = dataNoApproval.ApprovalOrder;
                    rowApproval.DepartmentOrder = dataNoApproval.DepartmentOrder;
                    rowApproval.LastApprovalId = dataNoApproval.LastApprovalId ?? "";
                    rowApproval.IsActive = dataNoApproval.IsActive;
                    rowApproval.IsStatus = dataNoApproval.IsStatus;
                    await _approvalTaskService.Update(rowApproval, userId);

                    return Ok(new ApiResponse<object>(true, "Đã hủy duyệt " + (thongbao ?? "").ToLower(), null));
                }
                else
                {
                    return Ok(new ApiResponse<object>(false, "Bản ghi đã duyệt, không thể hủy duyệt.", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }
        private async Task<string> BuildEmailContentEdit(string id)
        {
            var isValidModel = await _2TtChungNSachNgangService.GetHistoryIsValidEdit(id);
            var sb = new StringBuilder();

            // Header
            sb.AppendLine("<h3>Thông tin sửa</h3>");
            sb.AppendLine("<div class=\"table-responsive\">");
            sb.AppendLine("<table class=\"table table-bordered table-hover mb-0\">");
            sb.AppendLine("  <thead>");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td colspan=\"1\" rowspan=\"3\" class=\"text-center\"></td>");
            sb.AppendLine("      <td colspan=\"3\" rowspan=\"3\" class=\"text-center\">Thông tin tuyến đường</td>");
            sb.AppendLine("      <td colspan=\"15\" rowspan=\"2\" class=\"text-center\">Thông tin đường ống</td>");
            sb.AppendLine("      <td colspan=\"18\" rowspan=\"2\" class=\"text-center\">Thông tin cao độ, kết cấu thượng lưu (m)</td>");
            sb.AppendLine("      <td colspan=\"17\" rowspan=\"2\" class=\"text-center\">Thông tin cao độ, kết cấu hạ lưu (m)</td>");
            sb.AppendLine("      <td colspan=\"9\" rowspan=\"2\" class=\"text-center\">Thông tin cao độ đắp trung bình (m)</td>");
            sb.AppendLine("      <td colspan=\"5\" rowspan=\"2\" class=\"text-center\">Thông tin chiều rộng đáy đào (m)</td>");
            sb.AppendLine("      <td colspan=\"3\" rowspan=\"3\" class=\"text-center\">Thông tin mái đào</td>");
            sb.AppendLine("      <td colspan=\"5\" rowspan=\"3\" class=\"text-center\">Thông tin KL đào đất biện pháp</td>");
            sb.AppendLine("      <td colspan=\"26\" class=\"text-center\">KL đắp trả</td>");
            sb.AppendLine("      <td colspan=\"4\" rowspan=\"3\" class=\"text-center\">Tọa độ</td>");
            sb.AppendLine("      <td rowspan=\"3\" class=\"text-center\">Trạng thái</td>");
            sb.AppendLine("      <td rowspan=\"3\" class=\"text-center\">Người thao tác</td>");
            sb.AppendLine("      <td rowspan=\"3\" class=\"text-center\">Thời gian</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td colspan=\"13\" class=\"text-center\">KL đắp cát</td>");
            sb.AppendLine("      <td colspan=\"9\" class=\"text-center\">KL đắp đất</td>");
            sb.AppendLine("      <td colspan=\"4\" rowspan=\"2\" class=\"text-center\">Thông tin đất thừa</td>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <td colspan=\"8\" class=\"text-center\">Thông tin ống nhựa</td>");
            sb.AppendLine("      <td colspan=\"7\" class=\"text-center\">Thông tin khối lượng ống thép</td>");
            sb.AppendLine("      <td colspan=\"6\" class=\"text-center\">Thông tin chung</td>");
            sb.AppendLine("      <td colspan=\"7\" class=\"text-center\">Thông tin đắp cát</td>");
            sb.AppendLine("      <td colspan=\"3\" class=\"text-center\">Thông tin đắp đất</td>");
            sb.AppendLine("      <td colspan=\"2\" class=\"text-center\">Tổng chiều dầy (m)</td>");
            sb.AppendLine("      <td colspan=\"5\" class=\"text-center\">Thông tin chung</td>");
            sb.AppendLine("      <td colspan=\"7\" class=\"text-center\">Thông tin đắp cát</td>");
            sb.AppendLine("      <td colspan=\"3\" class=\"text-center\">Thông tin đắp đất</td>");
            sb.AppendLine("      <td colspan=\"2\" class=\"text-center\">Tổng chiều dầy (m)</td>");
            sb.AppendLine("      <td colspan=\"3\" class=\"text-center\">Đệm cát</td>");
            sb.AppendLine("      <td colspan=\"3\" class=\"text-center\">Đắp cát</td>");
            sb.AppendLine("      <td colspan=\"3\" class=\"text-center\">Đắp đất</td>");
            sb.AppendLine("      <td colspan=\"3\" class=\"text-center\">Thông tin đáy đào nhỏ</td>");
            sb.AppendLine("      <td colspan=\"2\" class=\"text-center\">Thông tin đáy đào lớn</td>");
            sb.AppendLine("      <td colspan=\"8\" class=\"text-center\">KL đắp cát trước chiếm chỗ</td>");
            sb.AppendLine("      <td colspan=\"5\" class=\"text-center\">KL ống chiếm chỗ</td>");
            sb.AppendLine("      <td colspan=\"4\" class=\"text-center\">KL đắp đất trước chiếm chỗ</td>");
            sb.AppendLine("      <td colspan=\"5\" class=\"text-center\">KL chiếm chỗ</td>");
            sb.AppendLine("    </tr>");
            // Thêm dòng tiêu đề cuối cùng (tên cột)
            sb.AppendLine("    <tr>");
            sb.AppendLine("      <th>#</th>");
            sb.AppendLine("<th>Tuyến đường</th>");
            sb.AppendLine("<th>Từ lý trình</th>");
            sb.AppendLine("<th>Đến lý trình</th>");
            sb.AppendLine("<th>Hạng mục công việc</th>");
            sb.AppendLine("<th>Loại cấu kiện</th>");
            sb.AppendLine("<th>Hạng mục KL</th>");
            sb.AppendLine("<th>Loại khối lượng</th>");
            sb.AppendLine("<th>Đường kính ngoài (m)</th>");
            sb.AppendLine("<th>C.Dầy ống (m)</th>");
            sb.AppendLine("<th>Chiều dài (m)</th>");
            sb.AppendLine("<th>Trạng thái thi công</th>");
            sb.AppendLine("<th>Loại cấu kiện</th>");
            sb.AppendLine("<th>Hạng mục KL</th>");
            sb.AppendLine("<th>Loại khối lượng</th>");
            sb.AppendLine("<th>Đường kính ngoài (m)</th>");
            sb.AppendLine("<th>C.Dầy ống (m)</th>");
            sb.AppendLine("<th>Chiều dài (m)</th>");
            sb.AppendLine("<th>Trạng thái thi công</th>");
            sb.AppendLine("<th>Hình thức đắp trả</th>");
            sb.AppendLine("<th>H.Trạng trước khi đào</th>");
            sb.AppendLine("<th>Đáy đào </th>");
            sb.AppendLine("<th>Chiều sâu đào</th>");
            sb.AppendLine("<th>Dòng chảy</th>");
            sb.AppendLine("<th>Đỉnh đường ống</th>");
            sb.AppendLine("<th>C.Độ đáy đệm cát</th>");
            sb.AppendLine("<th>C.Dầy đệm cát</th>");
            sb.AppendLine("<th>Đỉnh đệm cát</th>");
            sb.AppendLine("<th>Đáy đắp cát</th>");
            sb.AppendLine("<th>C.Dầy đắp cát</th>");
            sb.AppendLine("<th>Đỉnh đắp cát</th>");
            sb.AppendLine("<th>Tổng chiều dầy đệm + đắp cát</th>");
            sb.AppendLine("<th>C.Độ đáy đắp đất</th>");
            sb.AppendLine("<th>Chiều dầy đắp đất</th>");
            sb.AppendLine("<th>Đỉnh đắp đất</th>");
            sb.AppendLine("<th>Đắp đất + cát</th>");
            sb.AppendLine("<th>Chênh đắp so với đào</th>");
            sb.AppendLine("<th>H.Trạng trước khi đào</th>");
            sb.AppendLine("<th>Đáy đào </th>");
            sb.AppendLine("<th>Chiều sâu đào</th>");
            sb.AppendLine("<th>Dòng chảy</th>");
            sb.AppendLine("<th>Đỉnh đường ống</th>");
            sb.AppendLine("<th>C.Độ đáy đệm cát</th>");
            sb.AppendLine("<th>C.Dầy đệm cát</th>");
            sb.AppendLine("<th>Đỉnh đệm cát</th>");
            sb.AppendLine("<th>Đáy đắp cát</th>");
            sb.AppendLine("<th>C.Dầy đắp cát</th>");
            sb.AppendLine("<th>Đỉnh đắp cát</th>");
            sb.AppendLine("<th>Tổng chiều dầy đệm + đắp cát</th>");
            sb.AppendLine("<th>C.Độ đáy đắp đất</th>");
            sb.AppendLine("<th>Chiều dầy đắp đất</th>");
            sb.AppendLine("<th>Đỉnh đắp đất</th>");
            sb.AppendLine("<th>Đắp đất + cát</th>");
            sb.AppendLine("<th>Chênh đắp so với đào</th>");
            sb.AppendLine("<th>C.Độ đáy đệm cát</th>");
            sb.AppendLine("<th>C.Độ đỉnh đệm cát</th>");
            sb.AppendLine("<th>Chiều dầy đệm cát</th>");
            sb.AppendLine("<th>C.Độ đáy đắp cát</th>");
            sb.AppendLine("<th>C.độ đỉnh đắp cát</th>");
            sb.AppendLine("<th>Chiều dầy đắp cát</th>");
            sb.AppendLine("<th>C.Độ đáy đắp đất</th>");
            sb.AppendLine("<th>C.Độ đỉnh đắp đất</th>");
            sb.AppendLine("<th>Chiều dầy đắp đất</th>");
            sb.AppendLine("<th>C.Rộng đáy nhỏ H.Lưu</th>");
            sb.AppendLine("<th>C.Rộng đáy nhỏ T.Lưu</th>");
            sb.AppendLine("<th>C.Rộng đáy nhỏ T.Bình</th>");
            sb.AppendLine("<th>Chiều sâu đào trung bình</th>");
            sb.AppendLine("<th>C.Rộng đáy lớn trung bình</th>");
            sb.AppendLine("<th>Tỷ lệ mở mái</th>");
            sb.AppendLine("<th>Số mái trái</th>");
            sb.AppendLine("<th>Số mái phải</th>");
            sb.AppendLine("<th>Hạng mục KL</th>");
            sb.AppendLine("<th>Loại KL</th>");
            sb.AppendLine("<th>Diện tích (m2)</th>");
            sb.AppendLine("<th>KL đào (m3)</th>");
            sb.AppendLine("<th>Trạng thái thi công</th>");
            sb.AppendLine("<th>C.Rộng đáy nhỏ đệm cát (m)</th>");
            sb.AppendLine("<th>C.Rộng đáy lớn đệm cát (m)</th>");
            sb.AppendLine("<th>Diện tích đắp cát (m2)</th>");
            sb.AppendLine("<th>KL đệm cát (m3)</th>");
            sb.AppendLine("<th>C.Rộng đáy nhỏ đắp cát (m)</th>");
            sb.AppendLine("<th>C.Rộng đáy lớn đắp cát (m)</th>");
            sb.AppendLine("<th>Diện tích đắp cát (m2)</th>");
            sb.AppendLine("<th>KL đắp cát (m3)</th>");
            sb.AppendLine("<th>Hạng mục KL</th>");
            sb.AppendLine("<th>Loại KL</th>");
            sb.AppendLine("<th>KL ống C.Chỗ (m3)</th>");
            sb.AppendLine("<th>KL đắp cát sau C.Chỗ (m3)</th>");
            sb.AppendLine("<th>Trạng thái thi công</th>");
            sb.AppendLine("<th>C.Rộng đáy nhỏ đắp đất (m)</th>");
            sb.AppendLine("<th>C.Rộng đáy lớn đắp đất (m)</th>");
            sb.AppendLine("<th>Diện tích đắp đất (m2)</th>");
            sb.AppendLine("<th>KL đắp đất (m3)</th>");
            sb.AppendLine("<th>Hạng mục KL</th>");
            sb.AppendLine("<th>Loại KL</th>");
            sb.AppendLine("<th>KL ống C.Chỗ (m3)</th>");
            sb.AppendLine("<th>KL đắp đất sau C.Chỗ (m3)</th>");
            sb.AppendLine("<th>Trạng thái thi công</th>");
            sb.AppendLine("<th>Hạng mục KL</th>");
            sb.AppendLine("<th>Loại KL</th>");
            sb.AppendLine("<th>KL đất thừa (m3)</th>");
            sb.AppendLine("<th>Trạng thái thi công</th>");
            sb.AppendLine("<th>X</th>");
            sb.AppendLine("<th>Y</th>");
            sb.AppendLine("<th>X</th>");
            sb.AppendLine("<th>Y</th>");
            sb.AppendLine("    </tr>");
            sb.AppendLine("  </thead>");
            sb.AppendLine("  <tbody>");

            if (isValidModel.Any())
            {
                var oldData = isValidModel.FirstOrDefault();
                var newData = isValidModel.Count > 1 ? isValidModel[1] : null;

                // Dòng dữ liệu cũ
                sb.AppendLine("<tr>")
                .AppendLine("<td>Dữ liệu cũ</td>")
                .AppendLine($"<td>{oldData?.TuyenDuong}</td>")
                .AppendLine($"<td>{oldData?.TuLyTrinh}</td>")
                .AppendLine($"<td>{oldData?.DenLyTrinh}</td>")
                .AppendLine($"<td>{oldData?.HangMucCongViec}</td>")
                .AppendLine($"<td>{oldData?.LoaiCauKien}</td>")
                .AppendLine($"<td>{oldData?.HangMucKhoiLuong}</td>")
                .AppendLine($"<td>{oldData?.LoaiKhoiLuong}</td>")
                .AppendLine($"<td>{oldData?.DuongKinhNgoaiOngNhua}</td>")
                .AppendLine($"<td>{oldData?.CDayOngOngNhua}</td>")
                .AppendLine($"<td>{oldData?.ChieuDaiOngNhua}</td>")
                .AppendLine($"<td>{oldData?.TrangThaiThiCongOngNhua}</td>")
                .AppendLine($"<td>{oldData?.LoaiCauKienOngThep}</td>")
                .AppendLine($"<td>{oldData?.HangMucKhoiLuongOngThep}</td>")
                .AppendLine($"<td>{oldData?.LoaiKhoiLuongOngThep}</td>")
                .AppendLine($"<td>{oldData?.DuongKinhNgoaiMOngThep}</td>")
                .AppendLine($"<td>{oldData?.CDayOngMOngThep}</td>")
                .AppendLine($"<td>{oldData?.ChieuDaiMOngThep}</td>")
                .AppendLine($"<td>{oldData?.TrangThaiThiCongOngThep}</td>")
                .AppendLine($"<td>{oldData?.HinhThucDapTra}</td>")
                .AppendLine($"<td>{oldData?.HTrangTruocKhiDaoThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.DayDaoThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.ChieuSauDaoThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.DongChayThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.DinhDuongOngThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.CDoDayDemCatThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.CDayDemCatThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.DinhDemCatThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.DayDapCatThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.CDayDapCatThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.DinhDapCatThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.TongChieuDayDemDapCatThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.CDoDayDapDatThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.ChieuDayDapDatThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.DinhDapDatThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.DapDatCatThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.ChenhDapSoVoiDaoThuongLuu}</td>")
                .AppendLine($"<td>{oldData?.HTrangTruocKhiDaoHaLuu}</td>")
                .AppendLine($"<td>{oldData?.DayDaoHaLuu}</td>")
                .AppendLine($"<td>{oldData?.ChieuSauDaoHaLuu}</td>")
                .AppendLine($"<td>{oldData?.DongChayHaLuu}</td>")
                .AppendLine($"<td>{oldData?.DinhDuongOngHaLuu}</td>")
                .AppendLine($"<td>{oldData?.CDoDayDemCatHaLuu}</td>")
                .AppendLine($"<td>{oldData?.CDayDemCatHaLuu}</td>")
                .AppendLine($"<td>{oldData?.DinhDemCatHaLuu}</td>")
                .AppendLine($"<td>{oldData?.DayDapCatHaLuu}</td>")
                .AppendLine($"<td>{oldData?.CDayDapCatHaLuu}</td>")
                .AppendLine($"<td>{oldData?.DinhDapCatHaLuu}</td>")
                .AppendLine($"<td>{oldData?.TongChieuDayDemDapCatHaLuu}</td>")
                .AppendLine($"<td>{oldData?.CDoDayDapDatHaLuu}</td>")
                .AppendLine($"<td>{oldData?.ChieuDayDapDatHaLuu}</td>")
                .AppendLine($"<td>{oldData?.DinhDapDatHaLuu}</td>")
                .AppendLine($"<td>{oldData?.DapDatCatHaLuu}</td>")
                .AppendLine($"<td>{oldData?.ChenhDapSoVoiDaoHaLuu}</td>")
                .AppendLine($"<td>{oldData?.CDoDayDemCat}</td>")
                .AppendLine($"<td>{oldData?.CDoDinhDemCat}</td>")
                .AppendLine($"<td>{oldData?.ChieuDayDemCat}</td>")
                .AppendLine($"<td>{oldData?.CDoDayDapCat}</td>")
                .AppendLine($"<td>{oldData?.CDoDinhDapCat}</td>")
                .AppendLine($"<td>{oldData?.ChieuDayDapCat}</td>")
                .AppendLine($"<td>{oldData?.CDoDayDapDat}</td>")
                .AppendLine($"<td>{oldData?.CDoDinhDapDat}</td>")
                .AppendLine($"<td>{oldData?.ChieuDayDapDat}</td>")
                .AppendLine($"<td>{oldData?.CRongDayNhoHLuu}</td>")
                .AppendLine($"<td>{oldData?.CRongDayNhoTLuu}</td>")
                .AppendLine($"<td>{oldData?.CRongDayNhoTBinh}</td>")
                .AppendLine($"<td>{oldData?.ChieuSauDaoTrungBinh}</td>")
                .AppendLine($"<td>{oldData?.CRongDayLonTrungBinh}</td>")
                .AppendLine($"<td>{oldData?.TyLeMoMai}</td>")
                .AppendLine($"<td>{oldData?.SoMaiTrai}</td>")
                .AppendLine($"<td>{oldData?.SoMaiPhai}</td>")
                .AppendLine($"<td>{oldData?.HangMucKlDaoDat}</td>")
                .AppendLine($"<td>{oldData?.LoaiKlDaoDat}</td>")
                .AppendLine($"<td>{oldData?.DienTich}</td>")
                .AppendLine($"<td>{oldData?.KlDao}</td>")
                .AppendLine($"<td>{oldData?.TrangThaiThiCongDaoDat}</td>")
                .AppendLine($"<td>{oldData?.CRongDayNhoDemCat}</td>")
                .AppendLine($"<td>{oldData?.CRongDayLonDemCat}</td>")
                .AppendLine($"<td>{oldData?.DienTichDapCat1}</td>")
                .AppendLine($"<td>{oldData?.KlDemCat}</td>")
                .AppendLine($"<td>{oldData?.CRongDayNhoDapCat}</td>")
                .AppendLine($"<td>{oldData?.CRongDayLonDapCat}</td>")
                .AppendLine($"<td>{oldData?.DienTichDapCat2}</td>")
                .AppendLine($"<td>{oldData?.KlDapCat}</td>")
                .AppendLine($"<td>{oldData?.HangMucKlDapCat}</td>")
                .AppendLine($"<td>{oldData?.LoaiKlDapCat}</td>")
                .AppendLine($"<td>{oldData?.KLDapCat_KlOngCCho}</td>")
                .AppendLine($"<td>{oldData?.KlDapCatSauCCho}</td>")
                .AppendLine($"<td>{oldData?.TrangThaiThiCongDapCat}</td>")
                .AppendLine($"<td>{oldData?.CRongDayNhoDapDat}</td>")
                .AppendLine($"<td>{oldData?.CRongDayLonDapDat}</td>")
                .AppendLine($"<td>{oldData?.DienTichDapDat}</td>")
                .AppendLine($"<td>{oldData?.KlDapDat}</td>")
                .AppendLine($"<td>{oldData?.HangMucKlDapDat}</td>")
                .AppendLine($"<td>{oldData?.LoaiKlDapDat}</td>")
                .AppendLine($"<td>{oldData?.KLDapDat_KlOngCCho}</td>")
                .AppendLine($"<td>{oldData?.KlDapDatSauCCho}</td>")
                .AppendLine($"<td>{oldData?.TrangThaiThiCongDapDat}</td>")
                .AppendLine($"<td>{oldData?.HangMucKlDatThua}</td>")
                .AppendLine($"<td>{oldData?.LoaiKlDatThua}</td>")
                .AppendLine($"<td>{oldData?.KlDatThua}</td>")
                .AppendLine($"<td>{oldData?.TrangThaiThiCongDatThua}</td>")
                .AppendLine($"<td>{oldData?.X1}</td>")
                .AppendLine($"<td>{oldData?.Y1}</td>")
                .AppendLine($"<td>{oldData?.X2}</td>")
                .AppendLine($"<td>{oldData?.Y2}</td>")
                .AppendLine($"<td>{oldData?.IsStatus}</td>")
                .AppendLine($"<td>{oldData?.CreateBy}</td>")
                .AppendLine($"<td>{oldData?.CreateAt}</td>")
                .AppendLine("</tr>");

                // Dòng dữ liệu mới
                sb.AppendLine("<tr>")
                .AppendLine("<td>Dữ liệu cũ</td>")
                .AppendLine($"<td>{newData?.TuyenDuong}</td>")
                .AppendLine($"<td>{newData?.TuLyTrinh}</td>")
                .AppendLine($"<td>{newData?.DenLyTrinh}</td>")
                .AppendLine($"<td>{newData?.HangMucCongViec}</td>")
                .AppendLine($"<td>{newData?.LoaiCauKien}</td>")
                .AppendLine($"<td>{newData?.HangMucKhoiLuong}</td>")
                .AppendLine($"<td>{newData?.LoaiKhoiLuong}</td>")
                .AppendLine($"<td>{newData?.DuongKinhNgoaiOngNhua}</td>")
                .AppendLine($"<td>{newData?.CDayOngOngNhua}</td>")
                .AppendLine($"<td>{newData?.ChieuDaiOngNhua}</td>")
                .AppendLine($"<td>{newData?.TrangThaiThiCongOngNhua}</td>")
                .AppendLine($"<td>{newData?.LoaiCauKienOngThep}</td>")
                .AppendLine($"<td>{newData?.HangMucKhoiLuongOngThep}</td>")
                .AppendLine($"<td>{newData?.LoaiKhoiLuongOngThep}</td>")
                .AppendLine($"<td>{newData?.DuongKinhNgoaiMOngThep}</td>")
                .AppendLine($"<td>{newData?.CDayOngMOngThep}</td>")
                .AppendLine($"<td>{newData?.ChieuDaiMOngThep}</td>")
                .AppendLine($"<td>{newData?.TrangThaiThiCongOngThep}</td>")
                .AppendLine($"<td>{newData?.HinhThucDapTra}</td>")
                .AppendLine($"<td>{newData?.HTrangTruocKhiDaoThuongLuu}</td>")
                .AppendLine($"<td>{newData?.DayDaoThuongLuu}</td>")
                .AppendLine($"<td>{newData?.ChieuSauDaoThuongLuu}</td>")
                .AppendLine($"<td>{newData?.DongChayThuongLuu}</td>")
                .AppendLine($"<td>{newData?.DinhDuongOngThuongLuu}</td>")
                .AppendLine($"<td>{newData?.CDoDayDemCatThuongLuu}</td>")
                .AppendLine($"<td>{newData?.CDayDemCatThuongLuu}</td>")
                .AppendLine($"<td>{newData?.DinhDemCatThuongLuu}</td>")
                .AppendLine($"<td>{newData?.DayDapCatThuongLuu}</td>")
                .AppendLine($"<td>{newData?.CDayDapCatThuongLuu}</td>")
                .AppendLine($"<td>{newData?.DinhDapCatThuongLuu}</td>")
                .AppendLine($"<td>{newData?.TongChieuDayDemDapCatThuongLuu}</td>")
                .AppendLine($"<td>{newData?.CDoDayDapDatThuongLuu}</td>")
                .AppendLine($"<td>{newData?.ChieuDayDapDatThuongLuu}</td>")
                .AppendLine($"<td>{newData?.DinhDapDatThuongLuu}</td>")
                .AppendLine($"<td>{newData?.DapDatCatThuongLuu}</td>")
                .AppendLine($"<td>{newData?.ChenhDapSoVoiDaoThuongLuu}</td>")
                .AppendLine($"<td>{newData?.HTrangTruocKhiDaoHaLuu}</td>")
                .AppendLine($"<td>{newData?.DayDaoHaLuu}</td>")
                .AppendLine($"<td>{newData?.ChieuSauDaoHaLuu}</td>")
                .AppendLine($"<td>{newData?.DongChayHaLuu}</td>")
                .AppendLine($"<td>{newData?.DinhDuongOngHaLuu}</td>")
                .AppendLine($"<td>{newData?.CDoDayDemCatHaLuu}</td>")
                .AppendLine($"<td>{newData?.CDayDemCatHaLuu}</td>")
                .AppendLine($"<td>{newData?.DinhDemCatHaLuu}</td>")
                .AppendLine($"<td>{newData?.DayDapCatHaLuu}</td>")
                .AppendLine($"<td>{newData?.CDayDapCatHaLuu}</td>")
                .AppendLine($"<td>{newData?.DinhDapCatHaLuu}</td>")
                .AppendLine($"<td>{newData?.TongChieuDayDemDapCatHaLuu}</td>")
                .AppendLine($"<td>{newData?.CDoDayDapDatHaLuu}</td>")
                .AppendLine($"<td>{newData?.ChieuDayDapDatHaLuu}</td>")
                .AppendLine($"<td>{newData?.DinhDapDatHaLuu}</td>")
                .AppendLine($"<td>{newData?.DapDatCatHaLuu}</td>")
                .AppendLine($"<td>{newData?.ChenhDapSoVoiDaoHaLuu}</td>")
                .AppendLine($"<td>{newData?.CDoDayDemCat}</td>")
                .AppendLine($"<td>{newData?.CDoDinhDemCat}</td>")
                .AppendLine($"<td>{newData?.ChieuDayDemCat}</td>")
                .AppendLine($"<td>{newData?.CDoDayDapCat}</td>")
                .AppendLine($"<td>{newData?.CDoDinhDapCat}</td>")
                .AppendLine($"<td>{newData?.ChieuDayDapCat}</td>")
                .AppendLine($"<td>{newData?.CDoDayDapDat}</td>")
                .AppendLine($"<td>{newData?.CDoDinhDapDat}</td>")
                .AppendLine($"<td>{newData?.ChieuDayDapDat}</td>")
                .AppendLine($"<td>{newData?.CRongDayNhoHLuu}</td>")
                .AppendLine($"<td>{newData?.CRongDayNhoTLuu}</td>")
                .AppendLine($"<td>{newData?.CRongDayNhoTBinh}</td>")
                .AppendLine($"<td>{newData?.ChieuSauDaoTrungBinh}</td>")
                .AppendLine($"<td>{newData?.CRongDayLonTrungBinh}</td>")
                .AppendLine($"<td>{newData?.TyLeMoMai}</td>")
                .AppendLine($"<td>{newData?.SoMaiTrai}</td>")
                .AppendLine($"<td>{newData?.SoMaiPhai}</td>")
                .AppendLine($"<td>{newData?.HangMucKlDaoDat}</td>")
                .AppendLine($"<td>{newData?.LoaiKlDaoDat}</td>")
                .AppendLine($"<td>{newData?.DienTich}</td>")
                .AppendLine($"<td>{newData?.KlDao}</td>")
                .AppendLine($"<td>{newData?.TrangThaiThiCongDaoDat}</td>")
                .AppendLine($"<td>{newData?.CRongDayNhoDemCat}</td>")
                .AppendLine($"<td>{newData?.CRongDayLonDemCat}</td>")
                .AppendLine($"<td>{newData?.DienTichDapCat1}</td>")
                .AppendLine($"<td>{newData?.KlDemCat}</td>")
                .AppendLine($"<td>{newData?.CRongDayNhoDapCat}</td>")
                .AppendLine($"<td>{newData?.CRongDayLonDapCat}</td>")
                .AppendLine($"<td>{newData?.DienTichDapCat2}</td>")
                .AppendLine($"<td>{newData?.KlDapCat}</td>")
                .AppendLine($"<td>{newData?.HangMucKlDapCat}</td>")
                .AppendLine($"<td>{newData?.LoaiKlDapCat}</td>")
                .AppendLine($"<td>{newData?.KLDapCat_KlOngCCho}</td>")
                .AppendLine($"<td>{newData?.KlDapCatSauCCho}</td>")
                .AppendLine($"<td>{newData?.TrangThaiThiCongDapCat}</td>")
                .AppendLine($"<td>{newData?.CRongDayNhoDapDat}</td>")
                .AppendLine($"<td>{newData?.CRongDayLonDapDat}</td>")
                .AppendLine($"<td>{newData?.DienTichDapDat}</td>")
                .AppendLine($"<td>{newData?.KlDapDat}</td>")
                .AppendLine($"<td>{newData?.HangMucKlDapDat}</td>")
                .AppendLine($"<td>{newData?.LoaiKlDapDat}</td>")
                .AppendLine($"<td>{newData?.KLDapDat_KlOngCCho}</td>")
                .AppendLine($"<td>{newData?.KlDapDatSauCCho}</td>")
                .AppendLine($"<td>{newData?.TrangThaiThiCongDapDat}</td>")
                .AppendLine($"<td>{newData?.HangMucKlDatThua}</td>")
                .AppendLine($"<td>{newData?.LoaiKlDatThua}</td>")
                .AppendLine($"<td>{newData?.KlDatThua}</td>")
                .AppendLine($"<td>{newData?.TrangThaiThiCongDatThua}</td>")
                .AppendLine($"<td>{newData?.X1}</td>")
                .AppendLine($"<td>{newData?.Y1}</td>")
                .AppendLine($"<td>{newData?.X2}</td>")
                .AppendLine($"<td>{newData?.Y2}</td>")
                .AppendLine($"<td>{newData?.IsStatus}</td>")
                .AppendLine($"<td>{newData?.CreateBy}</td>")
                .AppendLine($"<td>{newData?.CreateAt}</td>")
                .AppendLine("</tr>");
            }
            else
            {
                sb.AppendLine("    <tr><td class=\"text-danger\" colspan=\"109\">Không có dữ liệu</td></tr>");
            }

            sb.AppendLine("  </tbody>");
            sb.AppendLine("</table>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }
        private async Task<string> BuildEmailContentAdd(string id)
        {
            var detail = await _2TtChungNSachNgangService.GetDetails(id);
            var sb = new StringBuilder();
            sb.AppendLine("<h3>Thêm - 2. TT chung N.Sạch ngang</h3>")
            .AppendLine("<div class=\"table-responsive\">")
            .AppendLine("<table class=\"table table-bordered table-hover mb-0\">")
            .AppendLine("  <thead>")
            .AppendLine("    <tr>")
            .AppendLine("      <td colspan=\"1\" rowspan=\"3\" class=\"text-center\"></td>")
            .AppendLine("      <td colspan=\"3\" rowspan=\"3\" class=\"text-center\">Thông tin tuyến đường</td>")
            .AppendLine("      <td colspan=\"15\" rowspan=\"2\" class=\"text-center\">Thông tin đường ống</td>")
            .AppendLine("      <td colspan=\"18\" rowspan=\"2\" class=\"text-center\">Thông tin cao độ, kết cấu thượng lưu (m)</td>")
            .AppendLine("      <td colspan=\"17\" rowspan=\"2\" class=\"text-center\">Thông tin cao độ, kết cấu hạ lưu (m)</td>")
            .AppendLine("      <td colspan=\"9\" rowspan=\"2\" class=\"text-center\">Thông tin cao độ đắp trung bình (m)</td>")
            .AppendLine("      <td colspan=\"5\" rowspan=\"2\" class=\"text-center\">Thông tin chiều rộng đáy đào (m)</td>")
            .AppendLine("      <td colspan=\"3\" rowspan=\"3\" class=\"text-center\">Thông tin mái đào</td>")
            .AppendLine("      <td colspan=\"5\" rowspan=\"3\" class=\"text-center\">Thông tin KL đào đất biện pháp</td>")
            .AppendLine("      <td colspan=\"26\" class=\"text-center\">KL đắp trả</td>")
            .AppendLine("      <td colspan=\"4\" rowspan=\"3\" class=\"text-center\">Tọa độ</td>")
            .AppendLine("      <td rowspan=\"3\" class=\"text-center\">Trạng thái</td>")
            .AppendLine("      <td rowspan=\"3\" class=\"text-center\">Người thao tác</td>")
            .AppendLine("      <td rowspan=\"3\" class=\"text-center\">Thời gian</td>")
            .AppendLine("    </tr>")
            .AppendLine("    <tr>")
            .AppendLine("      <td colspan=\"13\" class=\"text-center\">KL đắp cát</td>")
            .AppendLine("      <td colspan=\"9\" class=\"text-center\">KL đắp đất</td>")
            .AppendLine("      <td colspan=\"4\" rowspan=\"2\" class=\"text-center\">Thông tin đất thừa</td>")
            .AppendLine("    </tr>")
            .AppendLine("    <tr>")
            .AppendLine("      <td colspan=\"8\" class=\"text-center\">Thông tin ống nhựa</td>")
            .AppendLine("      <td colspan=\"7\" class=\"text-center\">Thông tin khối lượng ống thép</td>")
            .AppendLine("      <td colspan=\"6\" class=\"text-center\">Thông tin chung</td>")
            .AppendLine("      <td colspan=\"7\" class=\"text-center\">Thông tin đắp cát</td>")
            .AppendLine("      <td colspan=\"3\" class=\"text-center\">Thông tin đắp đất</td>")
            .AppendLine("      <td colspan=\"2\" class=\"text-center\">Tổng chiều dầy (m)</td>")
            .AppendLine("      <td colspan=\"5\" class=\"text-center\">Thông tin chung</td>")
            .AppendLine("      <td colspan=\"7\" class=\"text-center\">Thông tin đắp cát</td>")
            .AppendLine("      <td colspan=\"3\" class=\"text-center\">Thông tin đắp đất</td>")
            .AppendLine("      <td colspan=\"2\" class=\"text-center\">Tổng chiều dầy (m)</td>")
            .AppendLine("      <td colspan=\"3\" class=\"text-center\">Đệm cát</td>")
            .AppendLine("      <td colspan=\"3\" class=\"text-center\">Đắp cát</td>")
            .AppendLine("      <td colspan=\"3\" class=\"text-center\">Đắp đất</td>")
            .AppendLine("      <td colspan=\"3\" class=\"text-center\">Thông tin đáy đào nhỏ</td>")
            .AppendLine("      <td colspan=\"2\" class=\"text-center\">Thông tin đáy đào lớn</td>")
            .AppendLine("      <td colspan=\"8\" class=\"text-center\">KL đắp cát trước chiếm chỗ</td>")
            .AppendLine("      <td colspan=\"5\" class=\"text-center\">KL ống chiếm chỗ</td>")
            .AppendLine("      <td colspan=\"4\" class=\"text-center\">KL đắp đất trước chiếm chỗ</td>")
            .AppendLine("      <td colspan=\"5\" class=\"text-center\">KL chiếm chỗ</td>")
            .AppendLine("    </tr>")

            .AppendLine("    <tr>")
            .AppendLine("      <th>#</th>")
            .AppendLine("<th>Tuyến đường</th>")
            .AppendLine("<th>Từ lý trình</th>")
            .AppendLine("<th>Đến lý trình</th>")
            .AppendLine("<th>Hạng mục công việc</th>")
            .AppendLine("<th>Loại cấu kiện</th>")
            .AppendLine("<th>Hạng mục KL</th>")
            .AppendLine("<th>Loại khối lượng</th>")
            .AppendLine("<th>Đường kính ngoài (m)</th>")
            .AppendLine("<th>C.Dầy ống (m)</th>")
            .AppendLine("<th>Chiều dài (m)</th>")
            .AppendLine("<th>Trạng thái thi công</th>")
            .AppendLine("<th>Loại cấu kiện</th>")
            .AppendLine("<th>Hạng mục KL</th>")
            .AppendLine("<th>Loại khối lượng</th>")
            .AppendLine("<th>Đường kính ngoài (m)</th>")
            .AppendLine("<th>C.Dầy ống (m)</th>")
            .AppendLine("<th>Chiều dài (m)</th>")
            .AppendLine("<th>Trạng thái thi công</th>")
            .AppendLine("<th>Hình thức đắp trả</th>")
            .AppendLine("<th>H.Trạng trước khi đào</th>")
            .AppendLine("<th>Đáy đào </th>")
            .AppendLine("<th>Chiều sâu đào</th>")
            .AppendLine("<th>Dòng chảy</th>")
            .AppendLine("<th>Đỉnh đường ống</th>")
            .AppendLine("<th>C.Độ đáy đệm cát</th>")
            .AppendLine("<th>C.Dầy đệm cát</th>")
            .AppendLine("<th>Đỉnh đệm cát</th>")
            .AppendLine("<th>Đáy đắp cát</th>")
            .AppendLine("<th>C.Dầy đắp cát</th>")
            .AppendLine("<th>Đỉnh đắp cát</th>")
            .AppendLine("<th>Tổng chiều dầy đệm + đắp cát</th>")
            .AppendLine("<th>C.Độ đáy đắp đất</th>")
            .AppendLine("<th>Chiều dầy đắp đất</th>")
            .AppendLine("<th>Đỉnh đắp đất</th>")
            .AppendLine("<th>Đắp đất + cát</th>")
            .AppendLine("<th>Chênh đắp so với đào</th>")
            .AppendLine("<th>H.Trạng trước khi đào</th>")
            .AppendLine("<th>Đáy đào </th>")
            .AppendLine("<th>Chiều sâu đào</th>")
            .AppendLine("<th>Dòng chảy</th>")
            .AppendLine("<th>Đỉnh đường ống</th>")
            .AppendLine("<th>C.Độ đáy đệm cát</th>")
            .AppendLine("<th>C.Dầy đệm cát</th>")
            .AppendLine("<th>Đỉnh đệm cát</th>")
            .AppendLine("<th>Đáy đắp cát</th>")
            .AppendLine("<th>C.Dầy đắp cát</th>")
            .AppendLine("<th>Đỉnh đắp cát</th>")
            .AppendLine("<th>Tổng chiều dầy đệm + đắp cát</th>")
            .AppendLine("<th>C.Độ đáy đắp đất</th>")
            .AppendLine("<th>Chiều dầy đắp đất</th>")
            .AppendLine("<th>Đỉnh đắp đất</th>")
            .AppendLine("<th>Đắp đất + cát</th>")
            .AppendLine("<th>Chênh đắp so với đào</th>")
            .AppendLine("<th>C.Độ đáy đệm cát</th>")
            .AppendLine("<th>C.Độ đỉnh đệm cát</th>")
            .AppendLine("<th>Chiều dầy đệm cát</th>")
            .AppendLine("<th>C.Độ đáy đắp cát</th>")
            .AppendLine("<th>C.độ đỉnh đắp cát</th>")
            .AppendLine("<th>Chiều dầy đắp cát</th>")
            .AppendLine("<th>C.Độ đáy đắp đất</th>")
            .AppendLine("<th>C.Độ đỉnh đắp đất</th>")
            .AppendLine("<th>Chiều dầy đắp đất</th>")
            .AppendLine("<th>C.Rộng đáy nhỏ H.Lưu</th>")
            .AppendLine("<th>C.Rộng đáy nhỏ T.Lưu</th>")
            .AppendLine("<th>C.Rộng đáy nhỏ T.Bình</th>")
            .AppendLine("<th>Chiều sâu đào trung bình</th>")
            .AppendLine("<th>C.Rộng đáy lớn trung bình</th>")
            .AppendLine("<th>Tỷ lệ mở mái</th>")
            .AppendLine("<th>Số mái trái</th>")
            .AppendLine("<th>Số mái phải</th>")
            .AppendLine("<th>Hạng mục KL</th>")
            .AppendLine("<th>Loại KL</th>")
            .AppendLine("<th>Diện tích (m2)</th>")
            .AppendLine("<th>KL đào (m3)</th>")
            .AppendLine("<th>Trạng thái thi công</th>")
            .AppendLine("<th>C.Rộng đáy nhỏ đệm cát (m)</th>")
            .AppendLine("<th>C.Rộng đáy lớn đệm cát (m)</th>")
            .AppendLine("<th>Diện tích đắp cát (m2)</th>")
            .AppendLine("<th>KL đệm cát (m3)</th>")
            .AppendLine("<th>C.Rộng đáy nhỏ đắp cát (m)</th>")
            .AppendLine("<th>C.Rộng đáy lớn đắp cát (m)</th>")
            .AppendLine("<th>Diện tích đắp cát (m2)</th>")
            .AppendLine("<th>KL đắp cát (m3)</th>")
            .AppendLine("<th>Hạng mục KL</th>")
            .AppendLine("<th>Loại KL</th>")
            .AppendLine("<th>KL ống C.Chỗ (m3)</th>")
            .AppendLine("<th>KL đắp cát sau C.Chỗ (m3)</th>")
            .AppendLine("<th>Trạng thái thi công</th>")
            .AppendLine("<th>C.Rộng đáy nhỏ đắp đất (m)</th>")
            .AppendLine("<th>C.Rộng đáy lớn đắp đất (m)</th>")
            .AppendLine("<th>Diện tích đắp đất (m2)</th>")
            .AppendLine("<th>KL đắp đất (m3)</th>")
            .AppendLine("<th>Hạng mục KL</th>")
            .AppendLine("<th>Loại KL</th>")
            .AppendLine("<th>KL ống C.Chỗ (m3)</th>")
            .AppendLine("<th>KL đắp đất sau C.Chỗ (m3)</th>")
            .AppendLine("<th>Trạng thái thi công</th>")
            .AppendLine("<th>Hạng mục KL</th>")
            .AppendLine("<th>Loại KL</th>")
            .AppendLine("<th>KL đất thừa (m3)</th>")
            .AppendLine("<th>Trạng thái thi công</th>")
            .AppendLine("<th>X</th>")
            .AppendLine("<th>Y</th>")
            .AppendLine("<th>X</th>")
            .AppendLine("<th>Y</th>")
            .AppendLine("    </tr>")
            .AppendLine("  </thead>")
            .AppendLine("  <tbody>")
            .AppendLine("<tr>")
            .AppendLine("<td>1</td>")

            .AppendLine($"<td>{detail.Id_TuyenDuong}</td>")
            .AppendLine($"<td>{detail.Id_TuLyTrinh}</td>")
            .AppendLine($"<td>{detail.Id_DenLyTrinh}</td>")
            .AppendLine($"<td>{detail.Id_HangMucCongViec}</td>")
            .AppendLine($"<td>{detail.Id_LoaiCauKien}</td>")
            .AppendLine($"<td>{detail.Id_HangMucKhoiLuong}</td>")
            .AppendLine($"<td>{detail.Id_LoaiKhoiLuong}</td>")
            .AppendLine($"<td>{detail.DuongKinhNgoaiOngNhua}</td>")
            .AppendLine($"<td>{detail.CDayOngOngNhua}</td>")
            .AppendLine($"<td>{detail.ChieuDaiOngNhua}</td>")
            .AppendLine($"<td>{detail.Id_TrangThaiThiCongOngNhua}</td>")
            .AppendLine($"<td>{detail.Id_LoaiCauKienOngThep}</td>")
            .AppendLine($"<td>{detail.Id_HangMucKhoiLuongOngThep}</td>")
            .AppendLine($"<td>{detail.Id_LoaiKhoiLuongOngThep}</td>")
            .AppendLine($"<td>{detail.DuongKinhNgoaiMOngThep}</td>")
            .AppendLine($"<td>{detail.CDayOngMOngThep}</td>")
            .AppendLine($"<td>{detail.ChieuDaiMOngThep}</td>")
            .AppendLine($"<td>{detail.Id_TrangThaiThiCongOngThep}</td>")
            .AppendLine($"<td>{detail.Id_HinhThucDapTra}</td>")
            .AppendLine($"<td>{detail.HTrangTruocKhiDaoThuongLuu}</td>")
            .AppendLine($"<td>{detail.DayDaoThuongLuu}</td>")
            .AppendLine($"<td>{detail.ChieuSauDaoThuongLuu}</td>")
            .AppendLine($"<td>{detail.DongChayThuongLuu}</td>")
            .AppendLine($"<td>{detail.DinhDuongOngThuongLuu}</td>")
            .AppendLine($"<td>{detail.CDoDayDemCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.CDayDemCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.DinhDemCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.DayDapCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.CDayDapCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.DinhDapCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.TongChieuDayDemDapCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.CDoDayDapDatThuongLuu}</td>")
            .AppendLine($"<td>{detail.ChieuDayDapDatThuongLuu}</td>")
            .AppendLine($"<td>{detail.DinhDapDatThuongLuu}</td>")
            .AppendLine($"<td>{detail.DapDatCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.ChenhDapSoVoiDaoThuongLuu}</td>")
            .AppendLine($"<td>{detail.HTrangTruocKhiDaoHaLuu}</td>")
            .AppendLine($"<td>{detail.DayDaoHaLuu}</td>")
            .AppendLine($"<td>{detail.ChieuSauDaoHaLuu}</td>")
            .AppendLine($"<td>{detail.DongChayHaLuu}</td>")
            .AppendLine($"<td>{detail.DinhDuongOngHaLuu}</td>")
            .AppendLine($"<td>{detail.CDoDayDemCatHaLuu}</td>")
            .AppendLine($"<td>{detail.CDayDemCatHaLuu}</td>")
            .AppendLine($"<td>{detail.DinhDemCatHaLuu}</td>")
            .AppendLine($"<td>{detail.DayDapCatHaLuu}</td>")
            .AppendLine($"<td>{detail.CDayDapCatHaLuu}</td>")
            .AppendLine($"<td>{detail.DinhDapCatHaLuu}</td>")
            .AppendLine($"<td>{detail.TongChieuDayDemDapCatHaLuu}</td>")
            .AppendLine($"<td>{detail.CDoDayDapDatHaLuu}</td>")
            .AppendLine($"<td>{detail.ChieuDayDapDatHaLuu}</td>")
            .AppendLine($"<td>{detail.DinhDapDatHaLuu}</td>")
            .AppendLine($"<td>{detail.DapDatCatHaLuu}</td>")
            .AppendLine($"<td>{detail.ChenhDapSoVoiDaoHaLuu}</td>")
            .AppendLine($"<td>{detail.CDoDayDemCat}</td>")
            .AppendLine($"<td>{detail.CDoDinhDemCat}</td>")
            .AppendLine($"<td>{detail.ChieuDayDemCat}</td>")
            .AppendLine($"<td>{detail.CDoDayDapCat}</td>")
            .AppendLine($"<td>{detail.CDoDinhDapCat}</td>")
            .AppendLine($"<td>{detail.ChieuDayDapCat}</td>")
            .AppendLine($"<td>{detail.CDoDayDapDat}</td>")
            .AppendLine($"<td>{detail.CDoDinhDapDat}</td>")
            .AppendLine($"<td>{detail.ChieuDayDapDat}</td>")
            .AppendLine($"<td>{detail.CRongDayNhoHLuu}</td>")
            .AppendLine($"<td>{detail.CRongDayNhoTLuu}</td>")
            .AppendLine($"<td>{detail.CRongDayNhoTBinh}</td>")
            .AppendLine($"<td>{detail.ChieuSauDaoTrungBinh}</td>")
            .AppendLine($"<td>{detail.CRongDayLonTrungBinh}</td>")
            .AppendLine($"<td>{detail.TyLeMoMai}</td>")
            .AppendLine($"<td>{detail.SoMaiTrai}</td>")
            .AppendLine($"<td>{detail.SoMaiPhai}</td>")
            .AppendLine($"<td>{detail.HangMucKlDaoDat}</td>")
            .AppendLine($"<td>{detail.LoaiKlDaoDat}</td>")
            .AppendLine($"<td>{detail.DienTich}</td>")
            .AppendLine($"<td>{detail.KlDao}</td>")
            .AppendLine($"<td>{detail.Id_TrangThaiThiCongDaoDat}</td>")
            .AppendLine($"<td>{detail.CRongDayNhoDemCat}</td>")
            .AppendLine($"<td>{detail.CRongDayLonDemCat}</td>")
            .AppendLine($"<td>{detail.DienTichDapCat1}</td>")
            .AppendLine($"<td>{detail.KlDemCat}</td>")
            .AppendLine($"<td>{detail.CRongDayNhoDapCat}</td>")
            .AppendLine($"<td>{detail.CRongDayLonDapCat}</td>")
            .AppendLine($"<td>{detail.DienTichDapCat2}</td>")
            .AppendLine($"<td>{detail.KlDapCat}</td>")
            .AppendLine($"<td>{detail.HangMucKlDapCat}</td>")
            .AppendLine($"<td>{detail.LoaiKlDapCat}</td>")
            .AppendLine($"<td>{detail.KLDapCat_KlOngCCho}</td>")
            .AppendLine($"<td>{detail.KlDapCatSauCCho}</td>")
            .AppendLine($"<td>{detail.Id_TrangThaiThiCongDapCat}</td>")
            .AppendLine($"<td>{detail.CRongDayNhoDapDat}</td>")
            .AppendLine($"<td>{detail.CRongDayLonDapDat}</td>")
            .AppendLine($"<td>{detail.DienTichDapDat}</td>")
            .AppendLine($"<td>{detail.KlDapDat}</td>")
            .AppendLine($"<td>{detail.HangMucKlDapDat}</td>")
            .AppendLine($"<td>{detail.LoaiKlDapDat}</td>")
            .AppendLine($"<td>{detail.KLDapDat_KlOngCCho}</td>")
            .AppendLine($"<td>{detail.KlDapDatSauCCho}</td>")
            .AppendLine($"<td>{detail.Id_TrangThaiThiCongDapDat}</td>")
            .AppendLine($"<td>{detail.HangMucKlDatThua}</td>")
            .AppendLine($"<td>{detail.LoaiKlDatThua}</td>")
            .AppendLine($"<td>{detail.KlDatThua}</td>")
            .AppendLine($"<td>{detail.Id_TrangThaiThiCongDatThua}</td>")
            .AppendLine($"<td>{detail.X1}</td>")
            .AppendLine($"<td>{detail.Y1}</td>")
            .AppendLine($"<td>{detail.X2}</td>")
            .AppendLine($"<td>{detail.Y2}</td>")
            .AppendLine($"<td>{detail.IsStatus}</td>")
            .AppendLine($"<td>{detail.CreateBy}</td>")
            .AppendLine($"<td>{detail.CreateAt}</td>")
            .AppendLine("</tr>")

            .AppendLine("  </tbody>")
            .AppendLine("</table>")
            .AppendLine("</div>");
            return sb.ToString();
        }
        private async Task<string> BuildEmailContentDelete(string id)
        {
            var detail = await _2TtChungNSachNgangService.GetDetails(id);
            var sb = new StringBuilder();
            sb.AppendLine("<h3>Xóa - 2. TT chung N.Sạch ngang</h3>")
            .AppendLine("<div class=\"table-responsive\">")
            .AppendLine("<table class=\"table table-bordered table-hover mb-0\">")
            .AppendLine("  <thead>")
            .AppendLine("    <tr>")
            .AppendLine("      <td colspan=\"1\" rowspan=\"3\" class=\"text-center\"></td>")
            .AppendLine("      <td colspan=\"3\" rowspan=\"3\" class=\"text-center\">Thông tin tuyến đường</td>")
            .AppendLine("      <td colspan=\"15\" rowspan=\"2\" class=\"text-center\">Thông tin đường ống</td>")
            .AppendLine("      <td colspan=\"18\" rowspan=\"2\" class=\"text-center\">Thông tin cao độ, kết cấu thượng lưu (m)</td>")
            .AppendLine("      <td colspan=\"17\" rowspan=\"2\" class=\"text-center\">Thông tin cao độ, kết cấu hạ lưu (m)</td>")
            .AppendLine("      <td colspan=\"9\" rowspan=\"2\" class=\"text-center\">Thông tin cao độ đắp trung bình (m)</td>")
            .AppendLine("      <td colspan=\"5\" rowspan=\"2\" class=\"text-center\">Thông tin chiều rộng đáy đào (m)</td>")
            .AppendLine("      <td colspan=\"3\" rowspan=\"3\" class=\"text-center\">Thông tin mái đào</td>")
            .AppendLine("      <td colspan=\"5\" rowspan=\"3\" class=\"text-center\">Thông tin KL đào đất biện pháp</td>")
            .AppendLine("      <td colspan=\"26\" class=\"text-center\">KL đắp trả</td>")
            .AppendLine("      <td colspan=\"4\" rowspan=\"3\" class=\"text-center\">Tọa độ</td>")
            .AppendLine("      <td rowspan=\"3\" class=\"text-center\">Trạng thái</td>")
            .AppendLine("      <td rowspan=\"3\" class=\"text-center\">Người thao tác</td>")
            .AppendLine("      <td rowspan=\"3\" class=\"text-center\">Thời gian</td>")
            .AppendLine("    </tr>")
            .AppendLine("    <tr>")
            .AppendLine("      <td colspan=\"13\" class=\"text-center\">KL đắp cát</td>")
            .AppendLine("      <td colspan=\"9\" class=\"text-center\">KL đắp đất</td>")
            .AppendLine("      <td colspan=\"4\" rowspan=\"2\" class=\"text-center\">Thông tin đất thừa</td>")
            .AppendLine("    </tr>")
            .AppendLine("    <tr>")
            .AppendLine("      <td colspan=\"8\" class=\"text-center\">Thông tin ống nhựa</td>")
            .AppendLine("      <td colspan=\"7\" class=\"text-center\">Thông tin khối lượng ống thép</td>")
            .AppendLine("      <td colspan=\"6\" class=\"text-center\">Thông tin chung</td>")
            .AppendLine("      <td colspan=\"7\" class=\"text-center\">Thông tin đắp cát</td>")
            .AppendLine("      <td colspan=\"3\" class=\"text-center\">Thông tin đắp đất</td>")
            .AppendLine("      <td colspan=\"2\" class=\"text-center\">Tổng chiều dầy (m)</td>")
            .AppendLine("      <td colspan=\"5\" class=\"text-center\">Thông tin chung</td>")
            .AppendLine("      <td colspan=\"7\" class=\"text-center\">Thông tin đắp cát</td>")
            .AppendLine("      <td colspan=\"3\" class=\"text-center\">Thông tin đắp đất</td>")
            .AppendLine("      <td colspan=\"2\" class=\"text-center\">Tổng chiều dầy (m)</td>")
            .AppendLine("      <td colspan=\"3\" class=\"text-center\">Đệm cát</td>")
            .AppendLine("      <td colspan=\"3\" class=\"text-center\">Đắp cát</td>")
            .AppendLine("      <td colspan=\"3\" class=\"text-center\">Đắp đất</td>")
            .AppendLine("      <td colspan=\"3\" class=\"text-center\">Thông tin đáy đào nhỏ</td>")
            .AppendLine("      <td colspan=\"2\" class=\"text-center\">Thông tin đáy đào lớn</td>")
            .AppendLine("      <td colspan=\"8\" class=\"text-center\">KL đắp cát trước chiếm chỗ</td>")
            .AppendLine("      <td colspan=\"5\" class=\"text-center\">KL ống chiếm chỗ</td>")
            .AppendLine("      <td colspan=\"4\" class=\"text-center\">KL đắp đất trước chiếm chỗ</td>")
            .AppendLine("      <td colspan=\"5\" class=\"text-center\">KL chiếm chỗ</td>")
            .AppendLine("    </tr>")

            .AppendLine("    <tr>")
            .AppendLine("      <th>#</th>")
            .AppendLine("<th>Tuyến đường</th>")
            .AppendLine("<th>Từ lý trình</th>")
            .AppendLine("<th>Đến lý trình</th>")
            .AppendLine("<th>Hạng mục công việc</th>")
            .AppendLine("<th>Loại cấu kiện</th>")
            .AppendLine("<th>Hạng mục KL</th>")
            .AppendLine("<th>Loại khối lượng</th>")
            .AppendLine("<th>Đường kính ngoài (m)</th>")
            .AppendLine("<th>C.Dầy ống (m)</th>")
            .AppendLine("<th>Chiều dài (m)</th>")
            .AppendLine("<th>Trạng thái thi công</th>")
            .AppendLine("<th>Loại cấu kiện</th>")
            .AppendLine("<th>Hạng mục KL</th>")
            .AppendLine("<th>Loại khối lượng</th>")
            .AppendLine("<th>Đường kính ngoài (m)</th>")
            .AppendLine("<th>C.Dầy ống (m)</th>")
            .AppendLine("<th>Chiều dài (m)</th>")
            .AppendLine("<th>Trạng thái thi công</th>")
            .AppendLine("<th>Hình thức đắp trả</th>")
            .AppendLine("<th>H.Trạng trước khi đào</th>")
            .AppendLine("<th>Đáy đào </th>")
            .AppendLine("<th>Chiều sâu đào</th>")
            .AppendLine("<th>Dòng chảy</th>")
            .AppendLine("<th>Đỉnh đường ống</th>")
            .AppendLine("<th>C.Độ đáy đệm cát</th>")
            .AppendLine("<th>C.Dầy đệm cát</th>")
            .AppendLine("<th>Đỉnh đệm cát</th>")
            .AppendLine("<th>Đáy đắp cát</th>")
            .AppendLine("<th>C.Dầy đắp cát</th>")
            .AppendLine("<th>Đỉnh đắp cát</th>")
            .AppendLine("<th>Tổng chiều dầy đệm + đắp cát</th>")
            .AppendLine("<th>C.Độ đáy đắp đất</th>")
            .AppendLine("<th>Chiều dầy đắp đất</th>")
            .AppendLine("<th>Đỉnh đắp đất</th>")
            .AppendLine("<th>Đắp đất + cát</th>")
            .AppendLine("<th>Chênh đắp so với đào</th>")
            .AppendLine("<th>H.Trạng trước khi đào</th>")
            .AppendLine("<th>Đáy đào </th>")
            .AppendLine("<th>Chiều sâu đào</th>")
            .AppendLine("<th>Dòng chảy</th>")
            .AppendLine("<th>Đỉnh đường ống</th>")
            .AppendLine("<th>C.Độ đáy đệm cát</th>")
            .AppendLine("<th>C.Dầy đệm cát</th>")
            .AppendLine("<th>Đỉnh đệm cát</th>")
            .AppendLine("<th>Đáy đắp cát</th>")
            .AppendLine("<th>C.Dầy đắp cát</th>")
            .AppendLine("<th>Đỉnh đắp cát</th>")
            .AppendLine("<th>Tổng chiều dầy đệm + đắp cát</th>")
            .AppendLine("<th>C.Độ đáy đắp đất</th>")
            .AppendLine("<th>Chiều dầy đắp đất</th>")
            .AppendLine("<th>Đỉnh đắp đất</th>")
            .AppendLine("<th>Đắp đất + cát</th>")
            .AppendLine("<th>Chênh đắp so với đào</th>")
            .AppendLine("<th>C.Độ đáy đệm cát</th>")
            .AppendLine("<th>C.Độ đỉnh đệm cát</th>")
            .AppendLine("<th>Chiều dầy đệm cát</th>")
            .AppendLine("<th>C.Độ đáy đắp cát</th>")
            .AppendLine("<th>C.độ đỉnh đắp cát</th>")
            .AppendLine("<th>Chiều dầy đắp cát</th>")
            .AppendLine("<th>C.Độ đáy đắp đất</th>")
            .AppendLine("<th>C.Độ đỉnh đắp đất</th>")
            .AppendLine("<th>Chiều dầy đắp đất</th>")
            .AppendLine("<th>C.Rộng đáy nhỏ H.Lưu</th>")
            .AppendLine("<th>C.Rộng đáy nhỏ T.Lưu</th>")
            .AppendLine("<th>C.Rộng đáy nhỏ T.Bình</th>")
            .AppendLine("<th>Chiều sâu đào trung bình</th>")
            .AppendLine("<th>C.Rộng đáy lớn trung bình</th>")
            .AppendLine("<th>Tỷ lệ mở mái</th>")
            .AppendLine("<th>Số mái trái</th>")
            .AppendLine("<th>Số mái phải</th>")
            .AppendLine("<th>Hạng mục KL</th>")
            .AppendLine("<th>Loại KL</th>")
            .AppendLine("<th>Diện tích (m2)</th>")
            .AppendLine("<th>KL đào (m3)</th>")
            .AppendLine("<th>Trạng thái thi công</th>")
            .AppendLine("<th>C.Rộng đáy nhỏ đệm cát (m)</th>")
            .AppendLine("<th>C.Rộng đáy lớn đệm cát (m)</th>")
            .AppendLine("<th>Diện tích đắp cát (m2)</th>")
            .AppendLine("<th>KL đệm cát (m3)</th>")
            .AppendLine("<th>C.Rộng đáy nhỏ đắp cát (m)</th>")
            .AppendLine("<th>C.Rộng đáy lớn đắp cát (m)</th>")
            .AppendLine("<th>Diện tích đắp cát (m2)</th>")
            .AppendLine("<th>KL đắp cát (m3)</th>")
            .AppendLine("<th>Hạng mục KL</th>")
            .AppendLine("<th>Loại KL</th>")
            .AppendLine("<th>KL ống C.Chỗ (m3)</th>")
            .AppendLine("<th>KL đắp cát sau C.Chỗ (m3)</th>")
            .AppendLine("<th>Trạng thái thi công</th>")
            .AppendLine("<th>C.Rộng đáy nhỏ đắp đất (m)</th>")
            .AppendLine("<th>C.Rộng đáy lớn đắp đất (m)</th>")
            .AppendLine("<th>Diện tích đắp đất (m2)</th>")
            .AppendLine("<th>KL đắp đất (m3)</th>")
            .AppendLine("<th>Hạng mục KL</th>")
            .AppendLine("<th>Loại KL</th>")
            .AppendLine("<th>KL ống C.Chỗ (m3)</th>")
            .AppendLine("<th>KL đắp đất sau C.Chỗ (m3)</th>")
            .AppendLine("<th>Trạng thái thi công</th>")
            .AppendLine("<th>Hạng mục KL</th>")
            .AppendLine("<th>Loại KL</th>")
            .AppendLine("<th>KL đất thừa (m3)</th>")
            .AppendLine("<th>Trạng thái thi công</th>")
            .AppendLine("<th>X</th>")
            .AppendLine("<th>Y</th>")
            .AppendLine("<th>X</th>")
            .AppendLine("<th>Y</th>")
            .AppendLine("      <th>Người thao tác</th>")
            .AppendLine("      <th>Thời gian</th>")
            .AppendLine("    </tr>")
            .AppendLine("  </thead>")
            .AppendLine("  <tbody>")
            .AppendLine("<tr>")
            .AppendLine("<td>1</td>")

            .AppendLine($"<td>{detail.Id_TuyenDuong}</td>")
            .AppendLine($"<td>{detail.Id_TuLyTrinh}</td>")
            .AppendLine($"<td>{detail.Id_DenLyTrinh}</td>")
            .AppendLine($"<td>{detail.Id_HangMucCongViec}</td>")
            .AppendLine($"<td>{detail.Id_LoaiCauKien}</td>")
            .AppendLine($"<td>{detail.Id_HangMucKhoiLuong}</td>")
            .AppendLine($"<td>{detail.Id_LoaiKhoiLuong}</td>")
            .AppendLine($"<td>{detail.DuongKinhNgoaiOngNhua}</td>")
            .AppendLine($"<td>{detail.CDayOngOngNhua}</td>")
            .AppendLine($"<td>{detail.ChieuDaiOngNhua}</td>")
            .AppendLine($"<td>{detail.Id_TrangThaiThiCongOngNhua}</td>")
            .AppendLine($"<td>{detail.Id_LoaiCauKienOngThep}</td>")
            .AppendLine($"<td>{detail.Id_HangMucKhoiLuongOngThep}</td>")
            .AppendLine($"<td>{detail.Id_LoaiKhoiLuongOngThep}</td>")
            .AppendLine($"<td>{detail.DuongKinhNgoaiMOngThep}</td>")
            .AppendLine($"<td>{detail.CDayOngMOngThep}</td>")
            .AppendLine($"<td>{detail.ChieuDaiMOngThep}</td>")
            .AppendLine($"<td>{detail.Id_TrangThaiThiCongOngThep}</td>")
            .AppendLine($"<td>{detail.Id_HinhThucDapTra}</td>")
            .AppendLine($"<td>{detail.HTrangTruocKhiDaoThuongLuu}</td>")
            .AppendLine($"<td>{detail.DayDaoThuongLuu}</td>")
            .AppendLine($"<td>{detail.ChieuSauDaoThuongLuu}</td>")
            .AppendLine($"<td>{detail.DongChayThuongLuu}</td>")
            .AppendLine($"<td>{detail.DinhDuongOngThuongLuu}</td>")
            .AppendLine($"<td>{detail.CDoDayDemCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.CDayDemCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.DinhDemCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.DayDapCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.CDayDapCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.DinhDapCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.TongChieuDayDemDapCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.CDoDayDapDatThuongLuu}</td>")
            .AppendLine($"<td>{detail.ChieuDayDapDatThuongLuu}</td>")
            .AppendLine($"<td>{detail.DinhDapDatThuongLuu}</td>")
            .AppendLine($"<td>{detail.DapDatCatThuongLuu}</td>")
            .AppendLine($"<td>{detail.ChenhDapSoVoiDaoThuongLuu}</td>")
            .AppendLine($"<td>{detail.HTrangTruocKhiDaoHaLuu}</td>")
            .AppendLine($"<td>{detail.DayDaoHaLuu}</td>")
            .AppendLine($"<td>{detail.ChieuSauDaoHaLuu}</td>")
            .AppendLine($"<td>{detail.DongChayHaLuu}</td>")
            .AppendLine($"<td>{detail.DinhDuongOngHaLuu}</td>")
            .AppendLine($"<td>{detail.CDoDayDemCatHaLuu}</td>")
            .AppendLine($"<td>{detail.CDayDemCatHaLuu}</td>")
            .AppendLine($"<td>{detail.DinhDemCatHaLuu}</td>")
            .AppendLine($"<td>{detail.DayDapCatHaLuu}</td>")
            .AppendLine($"<td>{detail.CDayDapCatHaLuu}</td>")
            .AppendLine($"<td>{detail.DinhDapCatHaLuu}</td>")
            .AppendLine($"<td>{detail.TongChieuDayDemDapCatHaLuu}</td>")
            .AppendLine($"<td>{detail.CDoDayDapDatHaLuu}</td>")
            .AppendLine($"<td>{detail.ChieuDayDapDatHaLuu}</td>")
            .AppendLine($"<td>{detail.DinhDapDatHaLuu}</td>")
            .AppendLine($"<td>{detail.DapDatCatHaLuu}</td>")
            .AppendLine($"<td>{detail.ChenhDapSoVoiDaoHaLuu}</td>")
            .AppendLine($"<td>{detail.CDoDayDemCat}</td>")
            .AppendLine($"<td>{detail.CDoDinhDemCat}</td>")
            .AppendLine($"<td>{detail.ChieuDayDemCat}</td>")
            .AppendLine($"<td>{detail.CDoDayDapCat}</td>")
            .AppendLine($"<td>{detail.CDoDinhDapCat}</td>")
            .AppendLine($"<td>{detail.ChieuDayDapCat}</td>")
            .AppendLine($"<td>{detail.CDoDayDapDat}</td>")
            .AppendLine($"<td>{detail.CDoDinhDapDat}</td>")
            .AppendLine($"<td>{detail.ChieuDayDapDat}</td>")
            .AppendLine($"<td>{detail.CRongDayNhoHLuu}</td>")
            .AppendLine($"<td>{detail.CRongDayNhoTLuu}</td>")
            .AppendLine($"<td>{detail.CRongDayNhoTBinh}</td>")
            .AppendLine($"<td>{detail.ChieuSauDaoTrungBinh}</td>")
            .AppendLine($"<td>{detail.CRongDayLonTrungBinh}</td>")
            .AppendLine($"<td>{detail.TyLeMoMai}</td>")
            .AppendLine($"<td>{detail.SoMaiTrai}</td>")
            .AppendLine($"<td>{detail.SoMaiPhai}</td>")
            .AppendLine($"<td>{detail.HangMucKlDaoDat}</td>")
            .AppendLine($"<td>{detail.LoaiKlDaoDat}</td>")
            .AppendLine($"<td>{detail.DienTich}</td>")
            .AppendLine($"<td>{detail.KlDao}</td>")
            .AppendLine($"<td>{detail.Id_TrangThaiThiCongDaoDat}</td>")
            .AppendLine($"<td>{detail.CRongDayNhoDemCat}</td>")
            .AppendLine($"<td>{detail.CRongDayLonDemCat}</td>")
            .AppendLine($"<td>{detail.DienTichDapCat1}</td>")
            .AppendLine($"<td>{detail.KlDemCat}</td>")
            .AppendLine($"<td>{detail.CRongDayNhoDapCat}</td>")
            .AppendLine($"<td>{detail.CRongDayLonDapCat}</td>")
            .AppendLine($"<td>{detail.DienTichDapCat2}</td>")
            .AppendLine($"<td>{detail.KlDapCat}</td>")
            .AppendLine($"<td>{detail.HangMucKlDapCat}</td>")
            .AppendLine($"<td>{detail.LoaiKlDapCat}</td>")
            .AppendLine($"<td>{detail.KLDapCat_KlOngCCho}</td>")
            .AppendLine($"<td>{detail.KlDapCatSauCCho}</td>")
            .AppendLine($"<td>{detail.Id_TrangThaiThiCongDapCat}</td>")
            .AppendLine($"<td>{detail.CRongDayNhoDapDat}</td>")
            .AppendLine($"<td>{detail.CRongDayLonDapDat}</td>")
            .AppendLine($"<td>{detail.DienTichDapDat}</td>")
            .AppendLine($"<td>{detail.KlDapDat}</td>")
            .AppendLine($"<td>{detail.HangMucKlDapDat}</td>")
            .AppendLine($"<td>{detail.LoaiKlDapDat}</td>")
            .AppendLine($"<td>{detail.KLDapDat_KlOngCCho}</td>")
            .AppendLine($"<td>{detail.KlDapDatSauCCho}</td>")
            .AppendLine($"<td>{detail.Id_TrangThaiThiCongDapDat}</td>")
            .AppendLine($"<td>{detail.HangMucKlDatThua}</td>")
            .AppendLine($"<td>{detail.LoaiKlDatThua}</td>")
            .AppendLine($"<td>{detail.KlDatThua}</td>")
            .AppendLine($"<td>{detail.Id_TrangThaiThiCongDatThua}</td>")
            .AppendLine($"<td>{detail.X1}</td>")
            .AppendLine($"<td>{detail.Y1}</td>")
            .AppendLine($"<td>{detail.X2}</td>")
            .AppendLine($"<td>{detail.Y2}</td>")
            .AppendLine($"<td>{detail.IsStatus}</td>")
            .AppendLine($"<td>{detail.CreateBy}</td>")
            .AppendLine($"<td>{detail.CreateAt}</td>")
            .AppendLine("</tr>")

            .AppendLine("  </tbody>")
            .AppendLine("</table>")
            .AppendLine("</div>");

            return sb.ToString();
        }
        private ApprovalTask CreateApprovalTask(string title, string content, PKKL_OngNhua_2TtChungNSachNgang input, string tableName, string parentMajorId, string majorId, string userId)
        {
            return new ApprovalTask
            {
                Id = Guid.NewGuid().ToString(),
                Title = title,
                Content = content,
                IsActive = input.IsActive,
                OriginalId = input.Id,
                RelatedTable = tableName,
                ParentMajorId = parentMajorId,
                MajorId = majorId,
                CompanyId = input.GroupId,
                GroupId = input.GroupId,
                Ordinarily = input.Ordinarily,
                ApprovalUserId = input.ApprovalUserId,
                DateApproval = input.DateApproval,
                ApprovalDept = input.ApprovalDept,
                DepartmentId = input.DepartmentId ?? "",
                DepartmentOrder = input.DepartmentOrder,
                ApprovalOrder = input.ApprovalOrder,
                ApprovalId = input.ApprovalId ?? "",
                LastApprovalId = input.LastApprovalId ?? "",
                IsStatus = input.IsStatus,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };
        }
        private ApprovalTask UpdateApprovalTask(string Id, string title, string content, PKKL_OngNhua_2TtChungNSachNgang input, string tableName, string parentMajorId, string majorId, string userId)
        {
            return new ApprovalTask
            {
                Id = Id,
                Title = title,
                Content = content,
                IsActive = input.IsActive,
                OriginalId = input.Id,
                RelatedTable = tableName,
                ParentMajorId = parentMajorId,
                MajorId = majorId,
                CompanyId = input.GroupId,
                GroupId = input.GroupId,
                Ordinarily = input.Ordinarily,
                ApprovalUserId = input.ApprovalUserId,
                DateApproval = input.DateApproval,
                ApprovalDept = input.ApprovalDept,
                DepartmentId = input.DepartmentId ?? "",
                DepartmentOrder = input.DepartmentOrder,
                ApprovalOrder = input.ApprovalOrder,
                ApprovalId = input.ApprovalId ?? "",
                LastApprovalId = input.LastApprovalId ?? "",
                IsStatus = input.IsStatus,
                CreateAt = DateTime.Now,
                CreateBy = userId
            };
        }
        private async Task InsertEmailHistories(string content, string subject, string companyId, string groupId, string approvalId, string idCheck, string userId)
        {
            var listEmail = await _emailHistoryService.GetUserPermission(companyId, approvalId);
            var listInsert = listEmail.Select(item => new EmailHistory
            {
                Id = Guid.NewGuid().ToString(),
                Receiver = item.Mail,
                Subject = subject,
                Content = content,
                CompanyId = companyId,
                UserId = item.UserId,
                ParentMajorId = item.ParentMajorId,
                MajorId = item.MajorId,
                IdCheck = idCheck,
                IdLog = "",
                IsMail = true,
                IsNotification = true,
                IsSMS = true,
                GroupId = groupId,
                CreateAt = DateTime.Now,
                CreateBy = userId,
                IsRead = 0
            }).ToList();

            await _emailHistoryService.InsertMulti(listInsert);
        }

    }
}
