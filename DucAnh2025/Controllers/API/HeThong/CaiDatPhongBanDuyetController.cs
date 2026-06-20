using DucAnh2025.Models;
using DucAnh2025.Models.HeThong;
using DucAnh2025.Models.NhanSu;
using DucAnh2025.Repository.HeThong;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API.HeThong
{

    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CaiDatPhongBanDuyetController : ControllerBase
    {
        private string _userNameFromToken = string.Empty;
        private string _parentMajorId = "249ff511-8f10-45e8-bf8f-29b0ada5ab84";
        private string _majorId = "2105f7e7-1d45-4369-85a9-fdd185c3490b";

        private readonly IApprovalDeptSettingRepository _approvalDeptSettingService;


        public CaiDatPhongBanDuyetController(IApprovalDeptSettingRepository approvalDeptSettingRepository)
        {
            _approvalDeptSettingService = approvalDeptSettingRepository;

        }

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApprovalDeptSettingModel>>>> GetByVM(string groupId, [FromBody] ApprovalDeptSettingModel input)
        {
            try
            {
                var list = await _approvalDeptSettingService.GetAllByVM(input, groupId);
                return Ok(new ApiResponse<IEnumerable<ApprovalDeptSettingModel>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }

        [HttpGet("ListDept")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Department>>>> ListDept(string groupId)
        {
            try
            {
                var list =  _approvalDeptSettingService.ListDept(groupId);
                return Ok(new ApiResponse<IEnumerable<Department>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<Department>>(false, ex.Message, null));
            }
        }

        [HttpGet("CheckChoDuyet")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ChiNhanh>>>> CheckChoDuyet(string mainId)
        {
            try
            {
                var list = await _approvalDeptSettingService.CheckChoDuyet(mainId);
                return Ok(new ApiResponse<IEnumerable<ChiNhanh>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }

        [HttpGet("CheckSave")]
        public async Task<ActionResult<ApiResponse<IEnumerable<bool>>>> CheckSave(string companyId, string majorId, string mainId, bool loai)
        {
            try
            {
                var list = await _approvalDeptSettingService.CheckSave(companyId, majorId, mainId, loai);
                if (list)
                {
                    return Ok(new ApiResponse<IEnumerable<bool>>(true, "Thành công", null));
                }
                else
                {
                    return Ok(new ApiResponse<IEnumerable<bool>>(false, "Không thành công", null));
                }

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<bool>>(false, ex.Message, null));
            }
        }

        [HttpGet("GetByMainId")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApprovalDeptSetting>>>> GetByMainId(string idMain)
        {
            try
            {
                var list = await _approvalDeptSettingService.GetByMainId(idMain);
                return Ok(new ApiResponse<IEnumerable<ApprovalDeptSetting>>(true, "Thành công", list));

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }

        [HttpPost("CreateCaiDatPhongBanDuyet")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApprovalDeptWrapper>>>> CreateCaiDatPhongBanDuyet(List<ApprovalDeptWrapper> input)
        {
            try
            {
                bool result = await _approvalDeptSettingService.CreateApprovalDeptSetting(input, DateTime.Now);

                if (result)
                {
                    return Ok(new ApiResponse<ApprovalDeptWrapper>(true, "Lưu thành công", null));
                }
                else
                {
                    return BadRequest(new ApiResponse<ApprovalDeptWrapper>(false, "Lưu thất bại", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApprovalDeptWrapper>>>> DeleteCaiDatPhongBanDuyet(string id)
        {
            try
            {
                bool result = await _approvalDeptSettingService.DeleteApprovalDeptSetting(id);
                if (result)
                {
                    return Ok(new ApiResponse<ApprovalDeptWrapper>(true, "Xóa thành công", null));
                }
                else
                {
                    return BadRequest(new ApiResponse<ApprovalDeptWrapper>(false, "Xóa thất bại", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }
        
    }
}
