using DucAnh2025.Repository.NhanSu;
using DucAnh2025.ViewModels;
using DucAnh2025.ViewModels.NhanSu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API
{

    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class NhanSuController : ControllerBase
    {
        private readonly IDM_ChucVuRepository _dM_ChucVuRepository;
        private readonly IDM_ChuyenMonRepository _dM_ChuyenMonRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public NhanSuController(
            IDM_ChucVuRepository dM_ChucVuRepository,
            IDM_ChuyenMonRepository dM_ChuyenMonRepository,
            IDepartmentRepository departmentRepository
         )
        {
            _dM_ChucVuRepository = dM_ChucVuRepository;
            _dM_ChuyenMonRepository = dM_ChuyenMonRepository;
            _departmentRepository = departmentRepository;
        }

        [HttpPost("ChucVuGetAllByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<DM_ChucVuModel>>>> ChucVuGetAllByVM([FromQuery] string groupId, [FromBody] DM_ChucVuModel input)
        {
            try
            {
                var chuVus = await _dM_ChucVuRepository.GetAllByVM(input, groupId);
                var Data = new ApiResponse<IEnumerable<DM_ChucVuModel>>(true, "Thành công", chuVus);
                return Ok(Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new ApiResponse<IEnumerable<DM_ChucVuModel>>(false, "Internal Server Error", null));
            }
        }


        [HttpPost("ChuyenMonGetAllByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<DM_ChuyenMonModel>>>> ChuyenMonGetAllByVM(string groupId, [FromBody] DM_ChuyenMonModel input)
        {
            try
            {

                var chuyenMons = await _dM_ChuyenMonRepository.GetAllByVM(input, groupId);
                return Ok(new ApiResponse<IEnumerable<DM_ChuyenMonModel>>(true, "Thành công", chuyenMons));
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, new ApiResponse<IEnumerable<DM_ChuyenMonModel>>(false, "Internal Server Error", null));
            }
        }

        [HttpPost("DepartmentGetAllByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentModel>>>> DepartmentGetAllByVM(string groupId, [FromBody] DepartmentModel input)
        {
            try
            {
                var departments = await _departmentRepository.GetAllByVM(input, groupId);
                return Ok(new ApiResponse<IEnumerable<DepartmentModel>>(true, "Thành công", departments));
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, new ApiResponse<IEnumerable<DepartmentModel>>(false, "Internal Server Error", null));
            }
        }

    }
}
