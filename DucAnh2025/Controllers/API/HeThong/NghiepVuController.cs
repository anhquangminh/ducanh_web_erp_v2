using DucAnh2025.Models.HeThong;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API.HeThong
{
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class NghiepVuController : ControllerBase
    {

        private string _userNameFromToken = string.Empty;
        private string _parentMajorId = "249ff511-8f10-45e8-bf8f-29b0ada5ab84";
        private string _majorId = "2105f7e7-1d45-4369-85a9-fdd185c3490b";

        private readonly IMajorRepository _majorService;
        private readonly IApplicationUserRepository _applicationUserService;
        //GetAllParentMajor
        //GetMajorByParentId

        public NghiepVuController(IMajorRepository majorRepository,
            IApplicationUserRepository applicationUserRepository)
        {
            _majorService = majorRepository;
            _applicationUserService = applicationUserRepository;
        }

        [HttpGet("GetAllParentMajor")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Major>>>> GetAllParentMajor()
        {
            try
            {
                var list = await _majorService.GetAllParentMajor();
                return Ok(new ApiResponse<IEnumerable<Major>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }
        [HttpGet("GetMajorByParentId")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Major>>>> GetMajorByParentId(string parentId)
        {
            try
            {
                var list = await _majorService.GetMajorByParentId(parentId);
                return Ok(new ApiResponse<IEnumerable<Major>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<object>>(false, ex.Message, null));
            }
        }

        //-- new
        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MajorModel>>>> GetByVM( [FromBody] MajorModel input)
        {
            try
            {
                var list = await _majorService.GetAllByVM(input);
                return Ok(new ApiResponse<IEnumerable<MajorModel>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<MajorModel>>(false, ex.Message, null));
            }
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Major>>>> GetAll(string groupId)
        {
            try
            {
                var list = await _majorService.GetAll(groupId);
                return Ok(new ApiResponse<IEnumerable<Major>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<IEnumerable<Major>>(false, ex.Message, null));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] Major input, [FromQuery] bool isEdit)
        {
            try
            {
                var user = _applicationUserService.GetCurrentUser();

                input.CreateBy = user.Id;
                input.CreateAt = DateTime.Now;
                input.Id ??= Guid.NewGuid().ToString();

                var majorObj = await _majorService.GetMajorByName(input.MajorName);

                bool isSameTable = majorObj != null &&
                                   majorObj.Table.Trim().ToUpper() == input.Table.Trim().ToUpper() &&
                                   !string.IsNullOrWhiteSpace(majorObj.Table);

                if (majorObj != null && isSameTable && (!isEdit || majorObj.Id != input.Id))
                {
                    return Ok(new ApiResponse<object>(false,
                        "Đã tồn tại tên nghiệp vụ và tên bảng giống nội dung bạn đã nhập. Vui lòng thay đổi nội dung nhập và thử lại!", null));
                }

                if (isEdit)
                {
                    var isValid = await _majorService.CheckExclusive(new[] { input.Id }, DateTime.Now);
                    if (!isValid)
                    {
                        return Ok(new ApiResponse<object>(false, "Dữ liệu đã bị thay đổi bời người khác", null));
                    }

                    await _majorService.Update(input, "");
                    return Ok(new ApiResponse<object>(true, "Cập nhật thành công.", null));
                }
                else
                {
                    await _majorService.AddMajor(input);
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
                var result = await _majorService.GetById(id);
                return Ok(new ApiResponse<object>(true, "Lưu thành công.", result));

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
                var isValid = await _majorService.CheckExclusive([id], DateTime.Now);
                if (isValid)
                {
                    await _majorService.DeleteById(id, "");
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
