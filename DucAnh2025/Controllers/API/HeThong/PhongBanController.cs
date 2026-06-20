using DucAnh2025.Models.NhanSu;
using DucAnh2025.Repository.NhanSu;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API.HeThong
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhongBanController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentService;
        public PhongBanController(IDepartmentRepository departmentRepository)
        {
            _departmentService = departmentRepository;
        }

        [HttpPost("GetByChiNhanhs")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Department>>>> GetByChiNhanhs(string companyId)
        {
            try
            {
                var list= await _departmentService.GetByChiNhanhs(companyId);
                return Ok(new ApiResponse<IEnumerable<Department>>(true, "Thành công", list));
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, new ApiResponse<IEnumerable<Department>>(false, "Internal Server Error", null));
            }
        }
    }
}
