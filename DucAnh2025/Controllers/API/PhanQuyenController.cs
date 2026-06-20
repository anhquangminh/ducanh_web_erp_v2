using DucAnh2025.Repository.HeThong;
using DucAnh2025.ViewModels.NhanSu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API
{
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class PhanQuyenController : ControllerBase
    {
        private readonly IPhanQuyenRepository _phanQuyenService;
        public PhanQuyenController(IPhanQuyenRepository phanQuyenService)
        {
            _phanQuyenService = phanQuyenService;
        }

        [HttpGet("GetAllPermissionByGroupId")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PermissionModel>>>> GetAllPermissionByGroupId(string groupId, string userId, string parentMajorId = "", string majorId = "")
        {
            List<PermissionModel> list = new List<PermissionModel>();

            list = await _phanQuyenService.GetAllPermissionByGroupId(groupId, userId, parentMajorId, majorId);

            return Ok(new ApiResponse<IEnumerable<PermissionModel>>(true, "Thành công", list));
        }

    }
}
