using DucAnh2025.Models;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Models.NhanSu;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.ViewModels.HeThong;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API.HeThong
{
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CaiDatSoLuotDuyetController : ControllerBase
    {
        private string _userNameFromToken = string.Empty;
        private string _parentMajorId = "249ff511-8f10-45e8-bf8f-29b0ada5ab84";
        private string _majorId = "2105f7e7-1d45-4369-85a9-fdd185c3490b";

        private readonly IApprovalStepSettingRepository _approvalStepSettingService;
        private readonly IPhanQuyenRepository _phanQuyenService;
        private readonly IApplicationUserRepository _applicationUserService;


        public CaiDatSoLuotDuyetController(IApprovalStepSettingRepository approvalStepSettingRepository
            ,IPhanQuyenRepository phanQuyenRepository, IApplicationUserRepository applicationUserRepository)
        {
            _approvalStepSettingService = approvalStepSettingRepository;
            _phanQuyenService = phanQuyenRepository;
            _applicationUserService = applicationUserRepository;
        }

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApprovalStepSettingModel>>>> GetByVM(string groupId, [FromBody] ApprovalStepSettingModel input)
        {
            try
            {
                var list = await _approvalStepSettingService.GetAllByVM(input, groupId);
                return Ok(new ApiResponse<IEnumerable<ApprovalStepSettingModel>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }

        [HttpGet("LoadParentMajors")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Major>>>> LoadParentMajors(string companyId)
        {
            try
            {
                var list = await _approvalStepSettingService.LoadParentMajors(companyId);
                return Ok(new ApiResponse<IEnumerable<Major>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }
        [HttpGet("LoadMajors")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Major>>>> LoadMajors(string companyId ,string parentmajorId)
        {
            try
            {
                var list = await _approvalStepSettingService.LoadMajors(companyId, parentmajorId);
                return Ok(new ApiResponse<IEnumerable<Major>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }
        [HttpGet("LoadDepartments")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Department>>>> LoadDepartments(string companyId, string parentmajorId ,string majorId)
        {
            try
            {
                var list = await _approvalStepSettingService.LoadDepartments(companyId, parentmajorId, majorId);
                return Ok(new ApiResponse<IEnumerable<Department>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }

        [HttpGet("CheckChoDuyet")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ChiNhanh>>>> CheckChoDuyet(string mainId)
        {
            try
            {
                var list = await _approvalStepSettingService.CheckChoDuyet(mainId);
                return Ok(new ApiResponse<IEnumerable<ChiNhanh>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }

        [HttpGet("CheckSave")]
        public async Task<ActionResult<ApiResponse<IEnumerable<bool>>>> CheckSaveCheckSave(string companyId, string majorId, string deptId, string permissionId, string mainId, bool loai)
        {
            try
            {
                bool list = await _approvalStepSettingService.CheckSave(companyId, majorId, deptId, permissionId, mainId, loai);
                if (list)
                {
                    return Ok(new ApiResponse<IEnumerable<bool>>(true, "Thành công", null));
                }
                else
                {
                    return Ok(new ApiResponse<IEnumerable<bool>>(false, "Thành công", null));
                }

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }

        [HttpGet("GetByMainId")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApprovalStepSetting>>>> GetByMainId(string idMain)
        {
            try
            {
                var list = await _approvalStepSettingService.GetByMainId(idMain);
                return Ok(new ApiResponse<IEnumerable<ApprovalStepSetting>>(true, "Thành công", list));

            }
            catch (Exception ex)
            {
               return Ok(new ApiResponse<IEnumerable<ApprovalStepSetting>>(false, ex.Message, null));
            }
        }
        
        [HttpGet("GetIdApprodeptSetting")]
        public ActionResult<ApiResponse<ApprovalDeptSetting>> GetIdApprodeptSetting(string companyId, string majorId, string deptId)
        {
            try
            {
                var approvalDept = _approvalStepSettingService.GetIdApprodeptSetting(companyId, majorId, deptId);
                return Ok(new ApiResponse<ApprovalDeptSetting>(true, "Thành công", approvalDept));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<ApprovalDeptSetting>(false, ex.Message, null));
            }
        }
        [HttpGet("GetChiNhanhsForCompanyId")]
        public async Task<ActionResult<ApiResponse<object>>> GetChiNhanhsForCompanyId(string groupId)
        {
            try
            {
                var list = await _approvalStepSettingService.GetChiNhanhsForCompanyId(groupId);
                return Ok(new ApiResponse<object>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpGet("ListStepSetting")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApprovalStepSettingModel>>>> ListStepSetting(string groupId, string majorId, string departmentId, string permissionId)
        {
            try
            {
                var list = _approvalStepSettingService.ListStepSetting(groupId, majorId, departmentId, permissionId);
                return Ok(new ApiResponse<IEnumerable<ApprovalStepSettingModel>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }


        [HttpGet("Approval")]
        public async Task<ActionResult<ApiResponse<ApprovalDeptSetting>>> Approval(string id)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                if (user == null)
                    return Ok(new ApiResponse<ApprovalDeptSetting>(false, "Không xác định được user.", null));

                var entity = await _approvalStepSettingService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<ApprovalDeptSetting>(false, "Không tìm thấy bản ghi.", null));

                // Kiểm tra quyền (nếu cần)
                var checkquyen = await _phanQuyenService.CheckApproval(entity.GroupId, entity.DepartmentId, user, entity.ApprovalId);

                if (entity.IsActive == 3)
                    return Ok(new ApiResponse<ApprovalDeptSetting>(false, "Bản ghi đã duyệt.", null));

                if (entity.ApprovalId == entity.LastApprovalId)
                {
                    if (entity.IsActive == 0 || entity.IsActive == 1)
                    {
                        entity.ApprovalUserId = user.Id;
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
                        entity.ApprovalUserId = user.Id;
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
                        entity.CompanyId,
                        "cb02028d-04c5-45e7-a1dd-30627f34bef6",
                        entity.IsActive == 0 ? "adc4d282-e65a-bc1f-4a29-2dbabd4dab3b"
                            : entity.IsActive == 1 ? "64411bc2-7c8f-021a-d43e-1bb75c2659f0"
                            : entity.IsActive == 2 ? "d6b2018d-da98-a264-d21d-6e54c1973b13"
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
                    entity.ApprovalUserId = user.Id;
                    entity.DateApproval = DateTime.Now;
                    entity.ApprovalDept = entity.DepartmentId;
                }

                await _approvalStepSettingService.Approval(entity, user.Id);
                return Ok(new ApiResponse<ApprovalDeptSetting>(true, "Thành công", null));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<ApprovalDeptSetting>(false, ex.Message, null));
            }
        }

        [HttpGet("ApprovalCancel")]
        public async Task<ActionResult<ApiResponse<ApprovalDeptSetting>>> ApprovalCancel(string id)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();
                if (user == null)
                    return Ok(new ApiResponse<ApprovalDeptSetting>(false, "Không xác định được user.", null));

                var entity = await _approvalStepSettingService.GetById(id);
                if (entity == null)
                    return Ok(new ApiResponse<ApprovalDeptSetting>(false, "Không tìm thấy bản ghi.", null));

                // Kiểm tra quyền nếu cần
                var checkquyen = await _phanQuyenService.CheckApproval(entity.GroupId, entity.DepartmentId, user, entity.ApprovalId);

                if (entity.IsActive == 3)
                    return Ok(new ApiResponse<ApprovalDeptSetting>(false, "Thông tin hủy duyệt không tồn tại.", null));

                await _approvalStepSettingService.NoApproval(entity, user.Id);

                return Ok(new ApiResponse<ApprovalDeptSetting>(true, $"Đã hủy duyệt: {entity.IsStatus?.ToLower()}", null));
            }
            catch (Exception ex)
            {
                // Log exception nếu cần
                return Ok(new ApiResponse<ApprovalDeptSetting>(false, ex.Message, null));
            }
        }

        [HttpPost("CreateCaiDatSoLuotDuyet")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApprovalStepWrapper>>>> CreateCaiDatSoLuotDuyet(List<ApprovalStepWrapper> input)
        {
            try
            {
                bool result = await _approvalStepSettingService.CreateApprovalStepSetting(input, DateTime.Now);
                return Ok(new ApiResponse<ApprovalStepWrapper>(result, "Lưu thành công", null));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApprovalStepSetting>>>> DeleteCaiDatPhongBanDuyet(string id)
        {
            try
            {
                bool result = await _approvalStepSettingService.DeleteApprovalStepSetting(id);
                if (result)
                {
                    return Ok(new ApiResponse<ApprovalStepSetting>(true, "Xóa thành công", null));
                }
                else
                {
                    return BadRequest(new ApiResponse<ApprovalStepSetting>(false, "Xóa thất bại", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<ApprovalStepSetting>>(false, ex.Message, null));
            }
        }
        
        [HttpPost("LoadStepByApprovalControl")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApprovalStepSetting>>>> LoadStepByApprovalControl(ApprovalControl input)
        {
            try
            {
                var list = await _approvalStepSettingService.LoadStepByApprovalControl(input);
                return Ok(new ApiResponse<IEnumerable<ApprovalStepSetting>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<ApprovalStepSetting>>(false, ex.Message, null));
            }
        }

        [HttpPost("LoadStepByMajorUserApproval")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApprovalStepSetting>>>> LoadStepByApprovalControl(MajorUserApproval input)
        {
            try
            {
                var list = await _approvalStepSettingService.LoadStepByMajorUserApproval(input);
                return Ok(new ApiResponse<IEnumerable<ApprovalStepSetting>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<ApprovalStepSetting>>(false, ex.Message, null));
            }
        }

    }
}
