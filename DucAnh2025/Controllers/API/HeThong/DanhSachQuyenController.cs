using DucAnh2025.Models.HeThong;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.ViewModels.NhanSu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API.HeThong
{

    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class DanhSachQuyenController : ControllerBase
    {

        private string _userNameFromToken = string.Empty;
        private string _parentPermissionId = "";
        private string _PermissionId = "";

        private readonly IPermissionRepository _permissionService;
        private readonly IApplicationUserRepository _applicationUserService;
        //GetAllParentPermission
        //GetPermissionByParentId

        public DanhSachQuyenController(IPermissionRepository permissionRepository,
            IApplicationUserRepository applicationUserRepository)
        {
            _permissionService = permissionRepository;
            _applicationUserService = applicationUserRepository;
        }

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PermissionModel>>>> GetByVM([FromBody] PermissionModel input)
        {
            try
            {
                var list = await _permissionService.GetAllByVM(input);
                return Ok(new ApiResponse<IEnumerable<PermissionModel>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<PermissionModel>>(false, ex.Message, null));
            }
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Permission>>>> GetAll(string groupId)
        {
            try
            {
                var list = await _permissionService.GetAll(groupId);
                return Ok(new ApiResponse<IEnumerable<Permission>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<Permission>>(false, ex.Message, null));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] Permission input, [FromQuery] bool isEdit)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();

                input.CreateBy = user.Id;
                input.CreateAt = DateTime.Now;
                input.Id = isEdit ? input.Id : Guid.NewGuid().ToString();
                var condition = await _permissionService.GetExist(input);
                if (isEdit)
                {
                    if (condition != null && condition.Count() > 0 && input.PermissionName.ToUpper().Trim() == condition[0]?.PermissionName?.ToUpper().Trim())
                    {
                        return Ok(new ApiResponse<object>(false, "Đã tồn tại", null));
                    }
                    var isValid = await _permissionService.CheckExclusive(new[] { input.Id }, DateTime.Now);
                    if (!isValid)
                    {
                        return Ok(new ApiResponse<object>(false, "Dữ liệu đã bị thay đổi bời người khác", null));
                    }

                    await _permissionService.Update(input, "");
                    return Ok(new ApiResponse<object>(true, "Cập nhật thành công.", null));
                }
                else
                {
                    if (condition != null && condition.Count() > 0)
                    {
                        return Ok(new ApiResponse<object>(false, "Đã tồn tại", null));
                    }
                    await _permissionService.Insert(input,user.Id);
                    return Ok(new ApiResponse<object>(true, "Thêm thành công.", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<ApiResponse<object>>> GetById(string id)
        {
            try
            {
                var result = await _permissionService.GetById(id);
                return Ok(new ApiResponse<object>(true, "lấy dữ liệu thành công", result));

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var isValid = await _permissionService.CheckExclusive([id], DateTime.Now);
                if (isValid)
                {
                    await _permissionService.DeleteById(id, "");
                    return Ok(new ApiResponse<object>(true, "Xóa thành công.", null));
                }
                else
                {
                    return Ok(new ApiResponse<object>(false, "Dữ liệu đã bị thay đổi bời người khác", null));
                }

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>(false, ex.Message, null));
            }
        }
    }
}
