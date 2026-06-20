using DucAnh2025.Models.HeThong;
using DucAnh2025.Repository.HeThong;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API.HeThong
{
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionRepository _permissionService;
        private readonly IApprovalStepSettingRepository _approvalStepSettingService;
        public PermissionController(IPermissionRepository permissionRepository, IApprovalStepSettingRepository approvalStepSettingService)
        {
            _permissionService = permissionRepository;
            _approvalStepSettingService = approvalStepSettingService;
        }


        [HttpGet("GetPermisonByMajorId")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Permission>>>> GetPermisonByMajorId(string majorId)
        {
            try
            {
                var list = await _permissionService.LoadToApproval(majorId);
                return Ok(new ApiResponse<IEnumerable<Permission>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                 return Ok(new ApiResponse<IEnumerable<Permission>>(false, ex.Message, null));
            }
        }

        [HttpPost("LoadPermissionsByApprovalControl")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Permission>>>> LoadPermissionsByApprovalControl(ApprovalControl input)
        {
            try
            {
                var list = await _approvalStepSettingService.LoadPermissionsByApprovalControl(input);
                return Ok(new ApiResponse<IEnumerable<Permission>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                 return Ok(new ApiResponse<IEnumerable<Permission>>(false, ex.Message, null));
            }
        }

        [HttpPost("LoadPermissionsByMajorUserApproval")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Permission>>>> LoadPermissionsByMajorUserApproval(MajorUserApproval input)
        {
            try
            {
                var list = await _approvalStepSettingService.LoadPermissionsByMajorUserApproval(input);
                return Ok(new ApiResponse<IEnumerable<Permission>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return Ok(new ApiResponse<IEnumerable<Permission>>(false, ex.Message, null));
            }
        }

        [HttpPost("LoadByMajor1")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Permission>>>> LoadByMajor1(string majorId)
        {
            try
            {
                var list = _permissionService.LoadByMajor1(majorId);
                return Ok(new ApiResponse<IEnumerable<Permission>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return Ok(new ApiResponse<IEnumerable<Permission>>(false, ex.Message, null));
            }
        }
        
    }
}
