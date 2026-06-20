using DucAnh2025.Models.QLNV;
using DucAnh2025.Repository.QLNV;
using DucAnh2025.Services;
using DucAnh2025.ViewModel.QLNV;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DucAnh2025.Controllers.API.QLNV
{

    [Authorize(AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class DanhGiaController : ControllerBase
    {
        private readonly IQLNV_DanhGiaRepository _danhGiaRepository;
        private readonly IQLNV_CongViecRepository _congViecRepository;
        private readonly IQLNV_NhanVienRepository _nhanVienRepository;
        private readonly FirebaseNotificationService _firebaseNotificationService;

        public DanhGiaController(IQLNV_DanhGiaRepository danhGiaRepository
            , IQLNV_CongViecRepository congViecRepository,
            IQLNV_NhanVienRepository nhanVienRepository,
            FirebaseNotificationService firebaseNotificationService)
        {
            _nhanVienRepository = nhanVienRepository;
            _danhGiaRepository = danhGiaRepository;
            _congViecRepository = congViecRepository;
            _firebaseNotificationService = firebaseNotificationService;
        }

        [HttpGet("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_DanhGiaModel>>>> GetByVM(string groupId, string Id_NguoiGiaoViec, QLNV_DanhGiaModel input)
        {
            var danhgias = await _danhGiaRepository.GetByVM(groupId, input, Id_NguoiGiaoViec);
            return Ok(new ApiResponse<IEnumerable<QLNV_DanhGiaModel>>(true, "Thành công", danhgias));
        }

        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_DanhGiaModel>>>> GetByVM1(string groupId, string Id_NguoiGiaoViec, QLNV_DanhGiaModel input)
        {
            var danhgias = await _danhGiaRepository.GetByVM(groupId, input, Id_NguoiGiaoViec);
            return Ok(new ApiResponse<IEnumerable<QLNV_DanhGiaModel>>(true, "Thành công", danhgias));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<QLNV_DanhGia>>> CreateDanhGia(QLNV_DanhGia danhGia, [FromQuery] string userName)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse<QLNV_DanhGia>(false, "Dữ liệu không hợp lệ", null));
            }
            danhGia.Id = Guid.NewGuid().ToString();
            await _danhGiaRepository.Insert(danhGia, userName);

            var listNVTH_Task = _congViecRepository.GetByIdCongViecNVTH(danhGia.Id_CongViec);
            var cv_Task = _congViecRepository.GetById(danhGia.Id_CongViec);

            await Task.WhenAll(listNVTH_Task, cv_Task);

            var listNVTH = listNVTH_Task.Result ?? new List<QLNV_NhanVienThucHien>();
            var cv = cv_Task.Result ?? new QLNV_CongViec();

            var idNhanVienArr = listNVTH.Select(x => x.Id_NhanVien).ToArray();
            var sendNotifyTask = Task.CompletedTask;
            if (idNhanVienArr.Length > 0)
            {
                var id_nv = await _nhanVienRepository.GetByIdApplicationUser(idNhanVienArr);
                sendNotifyTask = _firebaseNotificationService.SendNotificationToMultipleAsync(
                    id_nv,
                    "Đã đánh giá",
                    cv.NoiDungCongViec ?? string.Empty
                );
            }
            var response = CreatedAtAction(nameof(GetDanhGiaById), new { id = danhGia.Id }, new ApiResponse<QLNV_DanhGia>(true, "Tạo thành công", danhGia));

            _ = sendNotifyTask;

            return response;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDanhGia([FromRoute] string id, [FromQuery] string userName, [FromBody] QLNV_DanhGia danhGia)
        {
            if (id != danhGia.Id)
            {
                return Ok(new ApiResponse<QLNV_DanhGia>(false, "Id không khớp", null));
            }

            if (!ModelState.IsValid)
            {
                return Ok(new ApiResponse<QLNV_DanhGia>(false, "Dữ liệu không hợp lệ", null));
            }

            var updateTask = _danhGiaRepository.Update(danhGia, userName);
            var listNVTH_Task = _congViecRepository.GetByIdCongViecNVTH(danhGia.Id_CongViec);
            var cv_Task = _congViecRepository.GetById(danhGia.Id_CongViec);

            await Task.WhenAll(updateTask, listNVTH_Task, cv_Task);

            var listNVTH = listNVTH_Task.Result ?? new List<QLNV_NhanVienThucHien>();
            var cv = cv_Task.Result ?? new QLNV_CongViec();

            var idNhanVienArr = listNVTH.Select(x => x.Id_NhanVien).ToArray();
            var id_nv = idNhanVienArr.Length > 0
                ? await _nhanVienRepository.GetByIdApplicationUser(idNhanVienArr)
                : new List<string>();

            _ = _firebaseNotificationService.SendNotificationToMultipleAsync(
                id_nv,
                "Đã cập nhật đánh giá",
                cv.NoiDungCongViec ?? string.Empty
            );

            return Ok(new ApiResponse<QLNV_DanhGia>(true, "Thành công", danhGia));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<QLNV_DanhGia>>> GetDanhGiaById([FromRoute] string id)
        {
            var danhGia = await _danhGiaRepository.GetById(id);
            if (danhGia == null)
            {
                return Ok(new ApiResponse<QLNV_DanhGia>(true, "Thành công", null));
            }
            return Ok(new ApiResponse<QLNV_DanhGia>(true, "Thành công", danhGia));
        }

        [HttpGet("GetByIdCongViec")]
        public async Task<ActionResult<ApiResponse<QLNV_DanhGia>>> GetByIdCongViec([FromQuery] string idCongViec)
        {
            try
            {
                var danhGia = await _danhGiaRepository.GetByIdCongViec(idCongViec);
                if (danhGia == null)
                {
                    return Ok(new ApiResponse<QLNV_DanhGia>(false, "Không tìm thấy ", null));
                }
                return Ok(new ApiResponse<QLNV_DanhGia>(true, "Thành công", danhGia));
            }
            catch (Exception)
            {
                return Ok(new ApiResponse<QLNV_DanhGia>(false, "Không tìm thấy", null));
            }
        }
    }
}
