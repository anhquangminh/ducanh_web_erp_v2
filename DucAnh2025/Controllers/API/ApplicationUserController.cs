using DucAnh2025.Models.Accounts;
using DucAnh2025.Repository;
using DucAnh2025.Repository.NhanSu;
using DucAnh2025.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API
{

    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private readonly IApplicationUserRepository _applicationUserService;

        public ApplicationUserController(IApplicationUserRepository applicationUserService,
            ICompanyRepository companyService
            )
        {
            _applicationUserService = applicationUserService;
        }

        [HttpGet("getInforUser")]
        public async Task<IActionResult> GetInforUser([FromQuery] string userName)
        {
            try
            {
                var user = _applicationUserService.GetByUserName(userName);
                if (user == null)
                    return Ok(new ApiResponse<ApplicationUser>(false, "Tài khoản không tồn tại!", null));

                return Ok(new ApiResponse<ApplicationUser>(true, "", user));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<ApplicationUser>(false, $"Lỗi: {ex.Message}", null));
            }
        }

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApplicationUserModel>>>> GetByVM(string groupId, [FromBody] ApplicationUserModel input)
        {
            try
            {
                var list = await _applicationUserService.GetAllByVM(input, groupId);
                return Ok(new ApiResponse<IEnumerable<ApplicationUserModel>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<ApplicationUserModel>>(false, ex.Message, null));
            }
            
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ApplicationUser>>>> GetAll(string groupId)
        {
            try
            {
                var list = await _applicationUserService.GetAll(groupId);
                return Ok(new ApiResponse<IEnumerable<ApplicationUser>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<ApplicationUser>>(false, ex.Message, null));
            }
            
        }
    }
}
