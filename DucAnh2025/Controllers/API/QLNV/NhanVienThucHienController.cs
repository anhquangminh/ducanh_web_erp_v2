using DucAnh2025.Models.QLNV;
using DucAnh2025.Repository.QLNV;
using DucAnh2025.ViewModel.QLNV;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API.QLNV
{
    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class NhanVienThucHienController : ControllerBase
    {
        private readonly IQLNV_NhanVienThucHienRepository _nhanVienThucHienRepository;

        public NhanVienThucHienController(IQLNV_NhanVienThucHienRepository nhanVienThucHienRepository)
        {
            _nhanVienThucHienRepository = nhanVienThucHienRepository;
        }

        [HttpGet("NhanViensByCongViec")]
        public async Task<ActionResult<ApiResponse<List<QLNV_NhanVien>>>> GetNhanViensByCongViec([FromQuery] string id_CongViec, [FromQuery] string groupId)
        {
            var result = await _nhanVienThucHienRepository.GetNhanViensByCongViec(id_CongViec, groupId);
            return Ok(new ApiResponse<List<QLNV_NhanVien>>(true, "Thành công", result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<QLNV_NhanVienThucHien>>> GetById(string id)
        {
            var entity = await _nhanVienThucHienRepository.GetById(id);
            if (entity == null)
                return Ok(new ApiResponse<QLNV_NhanVienThucHien>(false, "Không tìm th?y", null));
            return Ok(new ApiResponse<QLNV_NhanVienThucHien>(true, "Thành công", entity));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_NhanVienThucHien>>>> GetAll([FromQuery] string groupId)
        {
            var list = await _nhanVienThucHienRepository.GetAll(groupId);
            return Ok(new ApiResponse<IEnumerable<QLNV_NhanVienThucHien>>(true, "Thành công", list));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<QLNV_NhanVienThucHien>>> Create([FromBody] QLNV_NhanVienThucHien input, [FromQuery] string userName)
        {
            input.Id = Guid.NewGuid().ToString();
            input.CreateAt = DateTime.Now;
            await _nhanVienThucHienRepository.Insert(input, userName);
            return Ok(new ApiResponse<QLNV_NhanVienThucHien>(true, "Thêm thành công", input));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<QLNV_NhanVienThucHien>>> Update(string id, [FromBody] QLNV_NhanVienThucHien input, [FromQuery] string userName)
        {
            input.Id = id;
            await _nhanVienThucHienRepository.Update(input, userName);
            return Ok(new ApiResponse<QLNV_NhanVienThucHien>(true, "C?p nh?t thành công", input));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(string id, [FromQuery] string userName)
        {
            await _nhanVienThucHienRepository.Delete(id, userName);
            return Ok(new ApiResponse<object>(true, "Xóa thành công", null));
        }
    }
}