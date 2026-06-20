using DucAnh2025.Models.QLNV;
using DucAnh2025.Repository;
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
    public class CongViecController : ControllerBase
    {
        private readonly IQLNV_CongViecRepository _congViecRepository;
        private readonly IQLNV_NhanVienRepository _nhanVienRepository;
        private readonly IQLNV_QuanLyNhanVienRepository _quanLyNhanVienRepository;
        private readonly IQLNV_NhomNhanVienRepository _nhomNhanVienRepository;
        private readonly IQLNV_TaskCollaborationRepository _taskCollaborationRepository;
        private readonly IApplicationUserRepository _applicationUserService;
        private readonly FirebaseNotificationService _firebaseNotificationService;
        private readonly SignalRNotificationService _signalRNotificationService;

        public CongViecController(IQLNV_CongViecRepository congViecRepository,
            IQLNV_NhanVienRepository nhanVienRepository,
            IQLNV_QuanLyNhanVienRepository quanLyNhanVienRepository,
            IQLNV_NhomNhanVienRepository nhomNhanVienRepository,
            IQLNV_TaskCollaborationRepository taskCollaborationRepository,
            FirebaseNotificationService firebaseNotificationService,
            IApplicationUserRepository applicationUserService,
            SignalRNotificationService signalRNotificationService
            )
        {
            _congViecRepository = congViecRepository;
            _nhanVienRepository = nhanVienRepository;
            _quanLyNhanVienRepository = quanLyNhanVienRepository;
            _nhomNhanVienRepository = nhomNhanVienRepository;
            _taskCollaborationRepository = taskCollaborationRepository;
            _firebaseNotificationService = firebaseNotificationService;
            _applicationUserService = applicationUserService;
            _signalRNotificationService = signalRNotificationService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<QLNV_CongViec>>> GetCongViecById(string id)
        {
            var congViec = await _congViecRepository.GetById(id);
            if (congViec == null)
            {
                return Ok(new ApiResponse<QLNV_CongViec>(false, "Không tìm thấy công việc", null));
            }
            return Ok(new ApiResponse<QLNV_CongViec>(true, "Thành công", congViec));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_CongViec>>>> GetAllCongViecs(string groupId)
        {
            var congViecs = await _congViecRepository.GetAll(groupId);
            return Ok(new ApiResponse<IEnumerable<QLNV_CongViec>>(true, "Thành công", congViecs));
        }

        private async Task<(bool Result, string Message)> ValidateBeforeInsert(QLNV_CongViec congViec, string[] nhanVienIds)
        {
            if (nhanVienIds == null || nhanVienIds.Length == 0)
                return (false, "Vui lòng chọn người thực hiện.");

            if (string.IsNullOrWhiteSpace(congViec.NhomCongViec))
                return (false, "Vui lòng chọn nhóm công việc.");

            if (congViec.NgayKetThuc != null && congViec.NgayBatDau.Date > congViec.NgayKetThuc.Value.Date)
                return (false, "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");

            try
            {
                var checkNhanVien = _nhanVienRepository.CheckExclusive(nhanVienIds, DateTime.Now);
                var checkNhom = _nhomNhanVienRepository.CheckExclusive(new[] { congViec.NhomCongViec }, DateTime.Now);
                var checkNVbyNhom = _quanLyNhanVienRepository.CheckExclusiveNVbyNhom(
                    congViec.GroupId,
                    congViec.NhomCongViec,
                    nhanVienIds,
                    congViec.NgayBatDau
                );

                await Task.WhenAll(checkNhanVien, checkNhom, checkNVbyNhom);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi kiểm tra dữ liệu: {ex.Message}");
            }

            return (true, null);
        }

        [HttpPost("CreateCongViec")]
        public async Task<ActionResult<ApiResponse<QLNV_CongViec>>> CreateCongViec([FromBody] CongViecRequestModel model)
        {
            try
            {
                model.CongViec.Id = Guid.NewGuid().ToString();
                if (!ModelState.IsValid)
                    return Ok(new ApiResponse<QLNV_CongViec>(false, "Dữ liệu không hợp lệ", null));

                var validation = await ValidateBeforeInsert(model.CongViec, model.NhanVienThucHien);
                if (!validation.Result)
                    return Ok(new ApiResponse<QLNV_CongViec>(false, validation.Message, null));

                await _congViecRepository.Insert(model.CongViec, model.CongViec.CreateBy);

                var nvthTasks = model.NhanVienThucHien.Select(id_nhanvien =>
                {
                    var nhv = new QLNV_NhanVienThucHien
                    {
                        Id = Guid.NewGuid().ToString(),
                        Id_CongViec = model.CongViec.Id,
                        Id_NhanVien = id_nhanvien,
                        GroupId = model.CongViec.GroupId,
                        CreateAt = DateTime.Now,
                        CreateBy = model.CongViec.CreateBy
                    };
                    return _congViecRepository.InsertNVTH(nhv, model.CongViec.CreateBy);
                }).ToList();

                Task? themNgayTask = null;
                Task? updateCvTask = null;
                if (!string.IsNullOrEmpty(model.Themngay.Id_CongViecThemNgay) && model.Themngay.SoNgay > 0)
                {
                    var inputTN = new QLNV_ThemNgay
                    {
                        Id = Guid.NewGuid().ToString(),
                        Id_CongViec = model.CongViec.Id,
                        Id_CongViecThemNgay = model.Themngay.Id_CongViecThemNgay,
                        SoNgay = model.Themngay.SoNgay,
                        GroupId = model.CongViec.GroupId,
                        CreateAt = DateTime.Now,
                        CreateBy = model.CongViec.CreateBy
                    };
                    themNgayTask = _congViecRepository.InsertThemNgay(inputTN, model.CongViec.CreateBy);

                    updateCvTask = Task.Run(async () =>
                    {
                        var cv = await _congViecRepository.GetById(model.Themngay.Id_CongViecThemNgay);
                        if (cv != null)
                        {
                            cv.NgayKetThuc = cv.NgayKetThuc?.AddDays(model.Themngay.SoNgay);
                            await _congViecRepository.Update(cv, model.CongViec.CreateBy);
                        }
                    });
                }

                var allTasks = nvthTasks.ToList();
                if (themNgayTask != null) allTasks.Add(themNgayTask);
                if (updateCvTask != null) allTasks.Add(updateCvTask);
                await Task.WhenAll(allTasks);

                await _taskCollaborationRepository.RecordActivity(new QLNV_RecordActivityRequest
                {
                    Id_CongViec = model.CongViec.Id,
                    EventType = "task.created",
                    Description = "Tạo công việc",
                    CompanyId = model.CongViec.CompanyId,
                    GroupId = model.CongViec.GroupId,
                    MetadataJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        model.CongViec.TenCongViec,
                        model.CongViec.NhomCongViec,
                        model.CongViec.MucDoUuTien,
                        model.NhanVienThucHien
                    })
                }, model.CongViec.CreateBy, null);

                _ = Task.Run(async () =>
                {
                    var title = "Bạn mới được giao một công việc mới: " + model.CongViec.TenCongViec;
                    var body = model.CongViec.NoiDungCongViec;
                    var id_nvs = await _nhanVienRepository.GetByIdApplicationUser(model.NhanVienThucHien.ToArray());
                    await _firebaseNotificationService.SendNotificationToMultipleAsync(
                        id_nvs,
                        title,
                        body,
                        "congviecduocgiao",
                        model.CongViec.Id
                    );
                    await _signalRNotificationService.SendToUsersAsync(id_nvs, title, body);
                });

                return CreatedAtAction(nameof(GetCongViecById), new { id = model.CongViec.Id },
                    new ApiResponse<QLNV_CongViec>(true, "Tạo công việc thành công", model.CongViec));
            }
            catch (Exception ex)
            {
                return CreatedAtAction(nameof(GetCongViecById), new { id = model.CongViec.Id },
                    new ApiResponse<QLNV_CongViec>(false, "Lỗi " + ex.Message, null));
            }
        }


        [HttpPut("UpdateCongViec")]
        public async Task<ActionResult<ApiResponse<QLNV_CongViec>>> UpdateCongViec(string id, [FromBody] CongViecRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(new ApiResponse<QLNV_CongViec>(false, "Dữ liệu không hợp lệ", null));

                var congViec = await _congViecRepository.GetById(model.CongViec.Id);
                if (congViec == null)
                    return Ok(new ApiResponse<QLNV_CongViec>(false, "Không tìm thấy công việc", null));

                var deleteTasks = new List<Task>
                {
                    _congViecRepository.DeleteByIdCVThemNgay(model.CongViec.Id, model.CongViec.CreateBy),
                    _congViecRepository.DeleteById(model.CongViec.Id, model.CongViec.CreateBy)
                };
                await Task.WhenAll(deleteTasks);

                await _congViecRepository.Insert(model.CongViec, model.CongViec.CreateBy);

                bool isDanhGia = false;
                Task? nvthTask = null;

                if (model.NhanVienThucHien.Length > 0)
                {
                    nvthTask = Task.Run(async () =>
                    {
                        await _congViecRepository.DeleteByIdCongViecNVTH(model.CongViec.Id, model.CongViec.CreateBy);
                        var nvthTasks = model.NhanVienThucHien.Select(id_nhanvien =>
                        {
                            var nhv = new QLNV_NhanVienThucHien
                            {
                                Id = Guid.NewGuid().ToString(),
                                Id_CongViec = model.CongViec.Id,
                                Id_NhanVien = id_nhanvien,
                                GroupId = model.CongViec.GroupId,
                                CreateAt = DateTime.Now,
                                CreateBy = model.CongViec.CreateBy
                            };
                            return _congViecRepository.InsertNVTH(nhv, model.CongViec.CreateBy);
                        }).ToList();
                        await Task.WhenAll(nvthTasks);
                    });
                }
                else
                {
                    isDanhGia = true;
                    _ = Task.Run(async () =>
                    {
                        var title = "Công việc đã được tự đánh giá: "+ model.CongViec.TenCongViec;
                        var body = model.CongViec.NoiDungCongViec;
                        var listIdNVTH = await _nhanVienRepository.GetByIdApplicationUser(model.NhanVienThucHien.ToArray());
                        await _firebaseNotificationService.SendNotificationToMultipleAsync(
                            listIdNVTH,
                            title,
                            body
                        );
                        await _signalRNotificationService.SendToUsersAsync(listIdNVTH, title, body);
                    });
                }

                Task? themNgayTask = null;
                Task? updateCvTask = null;
                if (!string.IsNullOrEmpty(model.Themngay.Id_CongViecThemNgay) && model.Themngay.SoNgay > 0)
                {
                    var inputTN = new QLNV_ThemNgay
                    {
                        Id = Guid.NewGuid().ToString(),
                        Id_CongViec = model.CongViec.Id,
                        Id_CongViecThemNgay = model.Themngay.Id_CongViecThemNgay,
                        SoNgay = model.Themngay.SoNgay,
                        GroupId = model.CongViec.GroupId,
                        CreateAt = DateTime.Now,
                        CreateBy = model.CongViec.CreateBy
                    };
                    themNgayTask = _congViecRepository.InsertThemNgay(inputTN, model.CongViec.CreateBy);

                    updateCvTask = Task.Run(async () =>
                    {
                        var cv = await _congViecRepository.GetById(model.Themngay.Id_CongViecThemNgay);
                        if (cv != null)
                        {
                            cv.NgayKetThuc = cv.NgayKetThuc?.AddDays(model.Themngay.SoNgay);
                            await _congViecRepository.Update(cv, model.CongViec.CreateBy);
                        }
                    });
                }

                var allTasks = new List<Task>();
                if (nvthTask != null) allTasks.Add(nvthTask);
                if (themNgayTask != null) allTasks.Add(themNgayTask);
                if (updateCvTask != null) allTasks.Add(updateCvTask);
                if (allTasks.Count > 0)
                    await Task.WhenAll(allTasks);

                await _taskCollaborationRepository.RecordActivity(new QLNV_RecordActivityRequest
                {
                    Id_CongViec = model.CongViec.Id,
                    EventType = "task.updated",
                    Description = "Cập nhật công việc",
                    CompanyId = model.CongViec.CompanyId,
                    GroupId = model.CongViec.GroupId,
                    MetadataJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        model.CongViec.TenCongViec,
                        model.CongViec.NhomCongViec,
                        model.CongViec.MucDoUuTien,
                        model.NhanVienThucHien
                    })
                }, model.CongViec.CreateBy, null);

                if (!isDanhGia && model.NhanVienThucHien.Length > 0)
                {
                    _ = Task.Run(async () =>
                    {
                        var title = "Công việc đã được cập nhật: " + model.CongViec.TenCongViec;
                        var body = model.CongViec.NoiDungCongViec;
                        var id_nvs = await _nhanVienRepository.GetByIdApplicationUser(model.NhanVienThucHien.ToArray());
                        await _firebaseNotificationService.SendNotificationToMultipleAsync(
                            id_nvs,
                            title,
                            body
                        );
                        await _signalRNotificationService.SendToUsersAsync(id_nvs, title, body);

                    });
                }

                return Ok(new ApiResponse<QLNV_CongViec>(true, "Thành công", model.CongViec));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<QLNV_CongViec>(false, ex.Message, null));
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCongViec(string id, [FromQuery] string userName)
        {
            // Kiểm tra xem công việc có đang được sử dụng không
            if (await _congViecRepository.IsIdInUse(id))
            {
                return Ok(new ApiResponse<QLNV_CongViec>(false, "Không thể xóa công việc đang thực hiện", null));
            }

            var listNVTHTask = SafeCall(() => _congViecRepository.GetByIdCongViecNVTH(id));
            var cvTask = SafeCall(() => _congViecRepository.GetById(id));
            await Task.WhenAll(listNVTHTask, cvTask);

            var listNVTH = listNVTHTask.Result ?? new List<QLNV_NhanVienThucHien>();
            var cv = cvTask.Result ?? new QLNV_CongViec();

            var id_nvs = await _nhanVienRepository.GetByIdApplicationUser(listNVTH.Select(x => x.Id_NhanVien).ToArray());

            await _taskCollaborationRepository.RecordActivity(new QLNV_RecordActivityRequest
            {
                Id_CongViec = id,
                EventType = "task.deleted",
                Description = "Xóa công việc",
                CompanyId = cv.CompanyId,
                GroupId = cv.GroupId,
                MetadataJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    cv.TenCongViec,
                    cv.NhomCongViec,
                    cv.MucDoUuTien
                })
            }, userName, null);

            var deleteTasks = new List<Task>
            {
                _congViecRepository.DeleteById(id, userName),
                _congViecRepository.DeleteByIdCongViecCVC(id, userName),
                _congViecRepository.DeleteByIdCongViecNVTH(id, userName)
            };
            await Task.WhenAll(deleteTasks);

           
            _= Task.Run(async () =>
            {
                var title = "Công việc đã bị xóa: "+ cv.TenCongViec;
                var body = cv.NoiDungCongViec ?? string.Empty;
                await _firebaseNotificationService.SendNotificationToMultipleAsync(
                    id_nvs,
                    title,
                    body
                );
                await _signalRNotificationService.SendToUsersAsync(id_nvs, title, body);
            });

            return Ok(new ApiResponse<QLNV_CongViecModel>(true, "Thành công", new QLNV_CongViecModel()));
        }


        [HttpPost("GetByVM")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_CongViecModel>>>> GetByVM(string groupId, [FromBody] QLNV_CongViecModel input)
        {
            try
            {
                var congViecs = await _congViecRepository.GetByVM(groupId, input);
                return Ok(new ApiResponse<IEnumerable<QLNV_CongViecModel>>(true, "Thành công", congViecs));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<string>(false, ex.Message, null));
            }
        }

        [HttpGet("CheckExist")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckExist(string id, QLNV_CongViec input)
        {
            var exists = await _congViecRepository.CheckExist(id, input);
            return Ok(new ApiResponse<bool>(true, "Thành công", exists));
        }

        [HttpGet("IsIdInUse/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> IsIdInUse(string id)
        {
            var isInUse = await _congViecRepository.IsIdInUse(id);
            return Ok(new ApiResponse<bool>(true, "Thành công", isInUse));
        }

        [HttpGet("CheckStatus")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckStatus(string ids, string name)
        {
            var status = await _congViecRepository.CheckStatus(ids, name);
            return Ok(new ApiResponse<bool>(true, "Thành công", status));
        }

        [HttpGet("CheckExclusive")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckExclusive(string[] ids, DateTime baseTime)
        {
            var exclusive = await _congViecRepository.CheckExclusive(ids, baseTime);
            return Ok(new ApiResponse<bool>(true, "Thành công", exclusive));
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCVC(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Ok(new ApiResponse<string>(false, "File không hợp lệ", null));

                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload_qlnv");

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var filePath = Path.Combine(uploadFolder, uniqueFileName);
                var maxAllowedSize = 500 * 1024 * 1024;

                if (file.Length > maxAllowedSize)
                    return Ok(new ApiResponse<string>(false, "Dung lượng file vượt quá 500MB", null));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileUrl = $"/upload_qlnv/{uniqueFileName}";

                return Ok(new ApiResponse<string>(true, "Upload thành công", fileUrl));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<string>(false, ex.Message, null));
            }
        }

        // Các phương thức cho công việc con
        [HttpPost("InsertCVC")]
        public async Task<IActionResult> InsertCVC(QLNV_CongViecCon entity, [FromQuery] string userName)
        {
            if (string.IsNullOrWhiteSpace(entity.NoiDungCongViec))
                return Ok(new ApiResponse<string>(false, "Nội dung công việc không được để trống", null));
                

            if (string.IsNullOrWhiteSpace(entity.Id_CongViec))
                return Ok(new ApiResponse<string>(false, "Thiếu ID công việc cha", null));

            entity.Id = Guid.NewGuid().ToString();
            await _congViecRepository.InsertCVC(entity, userName);

            var listNVTH_Task = SafeCall(() => _congViecRepository.GetByIdCongViecNVTH(entity.Id_CongViec));
            var cv_Task = SafeCall(() => _congViecRepository.GetById(entity.Id_CongViec));
            await Task.WhenAll(listNVTH_Task, cv_Task);

            var listNVTH = listNVTH_Task.Result ?? new List<QLNV_NhanVienThucHien>();
            var cv = cv_Task.Result ?? new QLNV_CongViec();

            var id_nvs = await _nhanVienRepository.GetByIdApplicationUser(listNVTH.Select(x => x.Id_NhanVien).ToArray());


            _ = Task.Run(async () =>
            {
                var title = "Thêm công việc con";
                var body = (cv.NoiDungCongViec ?? string.Empty) + " - " + entity.NoiDungCongViec;
                await _firebaseNotificationService.SendNotificationToMultipleAsync(
                    id_nvs,
                    title,
                    body
                );
                await _signalRNotificationService.SendToUsersAsync(id_nvs, title, body);
            });

            return Ok(new ApiResponse<QLNV_CongViecCon>(true, "Thành công", entity));
        }


        [HttpGet("GetAllCVC")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_CongViecCon>>>> GetAllCVC()
        {
            var congViecCons = await _congViecRepository.GetAllCVC();
            return Ok(new ApiResponse<IEnumerable<QLNV_CongViecCon>>(true, "Thành công", congViecCons));
        }

        [HttpGet("GetByIdCVC/{id}")]
        public async Task<ActionResult<ApiResponse<QLNV_CongViecCon>>> GetByIdCVC(string id)
        {
            var congViecCon = await _congViecRepository.GetByIdCVC(id);
            if (congViecCon == null)
            {
                return NotFound(new ApiResponse<QLNV_CongViecCon>(false, "Không tìm thấy công việc con", null));
            }
            return Ok(new ApiResponse<QLNV_CongViecCon>(true, "Thành công", congViecCon));
        }

        [HttpGet("GetByIdCongViecCVC/{id_task}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_CongViecCon>>>> GetByIdCongViecCVC(string id_task)
        {
            var congViecCons = await _congViecRepository.GetByIdCongViecCVC(id_task);
            return Ok(new ApiResponse<IEnumerable<QLNV_CongViecCon>>(true, "Thành công", congViecCons));
        }

        //[HttpDelete("DeleteByIdCVC/{id}")]
        //public async Task<IActionResult> DeleteByIdCVC(string id, [FromQuery] string userName)
        //{

        //    await _congViecRepository.DeleteByIdCVC(id, userName);
        //    return Ok(new ApiResponse<QLNV_CongViecCon>(true, "Thành công", null));
        //}

        [HttpDelete("DeleteByIdCVC/{id}")]
        public async Task<IActionResult> DeleteByIdCVC(string id, [FromQuery] string userName)
        {
            // Lấy thông tin công việc con trước khi xóa
            var cvc = await _congViecRepository.GetByIdCVC(id);
            if (cvc == null)
                return NotFound(new ApiResponse<QLNV_CongViecCon>(false, "Không tìm thấy công việc con", null));

            // Lấy thông tin công việc cha
            var cv = await _congViecRepository.GetById(cvc.Id_CongViec);
            // Lấy danh sách nhân viên thực hiện công việc cha
            var listNVTH = await _congViecRepository.GetByIdCongViecNVTH(cvc.Id_CongViec);
            var id_nvs = await _nhanVienRepository.GetByIdApplicationUser(listNVTH.Select(x => x.Id_NhanVien).ToArray());

            // Xóa công việc con
            await _congViecRepository.DeleteByIdCVC(id, userName);

            // Gửi thông báo bất đồng bộ
            _ = Task.Run(async () =>
            {
                var title = "Công việc con đã bị xóa";
                var body = ((cv?.NoiDungCongViec ?? "") + " - " + (cvc?.NoiDungCongViec ?? "")).Trim(' ', '-');
                await _firebaseNotificationService.SendNotificationToMultipleAsync(id_nvs, title, body);
                await _signalRNotificationService.SendToUsersAsync(id_nvs, title, body);
            });

            return Ok(new ApiResponse<QLNV_CongViecCon>(true, "Thành công", null));
        }

        [HttpDelete("DeleteByIdCongViecCVC/{Id_Task}")]
        public async Task<IActionResult> DeleteByIdCongViecCVC(string Id_Task, [FromQuery] string userName)
        {
            await _congViecRepository.DeleteByIdCongViecCVC(Id_Task, userName);
            return NoContent();
        }

        //[HttpPut("UpdateCVC")]
        //public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_CongViecCon>>>> UpdateCVC(QLNV_CongViecCon data, [FromQuery] string userName)
        //{
        //    await _congViecRepository.UpdateCVC(data, userName);
        //    return Ok(new ApiResponse<QLNV_CongViecCon>(true, "Thành công", null));
        //}

        [HttpPut("UpdateCVC")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_CongViecCon>>>> UpdateCVC(QLNV_CongViecCon data, [FromQuery] string userName)
        {
            await _congViecRepository.UpdateCVC(data, userName);

            // Lấy thông tin công việc cha
            var cv = await _congViecRepository.GetById(data.Id_CongViec);
            // Lấy danh sách nhân viên thực hiện công việc cha
            var listNVTH = await _congViecRepository.GetByIdCongViecNVTH(data.Id_CongViec);
            var id_nvs = await _nhanVienRepository.GetByIdApplicationUser(listNVTH.Select(x => x.Id_NhanVien).ToArray());

            // Gửi thông báo bất đồng bộ
            _ = Task.Run(async () =>
            {
                var title = "Công việc con đã được cập nhật";
                var body = ((cv?.NoiDungCongViec ?? "") + " - " + (data?.NoiDungCongViec ?? "")).Trim(' ', '-');
                await _firebaseNotificationService.SendNotificationToMultipleAsync(id_nvs, title, body);
                await _signalRNotificationService.SendToUsersAsync(id_nvs, title, body);
            });

            return Ok(new ApiResponse<QLNV_CongViecCon>(true, "Thành công", null));
        }

        [HttpGet("CheckExistCVC")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckExistCVC(string id, QLNV_CongViecCon input)
        {
            var exists = await _congViecRepository.CheckExistCVC(id, input);
            return Ok(new ApiResponse<bool>(true, "Thành công", exists));
        }

        [HttpGet("CheckExclusiveCVC")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckExclusiveCVC(string[] ids, DateTime baseTime)
        {
            var exclusive = await _congViecRepository.CheckExclusiveCVC(ids, baseTime);
            return Ok(new ApiResponse<bool>(true, "Thành công", exclusive));
        }

        // Các phương thức cho nhân viên thực hiện
        [HttpPost("InsertNVTH")]
        public async Task<IActionResult> InsertNVTH(QLNV_NhanVienThucHien entity, [FromQuery] string userName)
        {
            await _congViecRepository.InsertNVTH(entity, userName);
            return NoContent();
        }

        [HttpGet("GetByIdCongViecNVTH/{Id_CongViec}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_NhanVienThucHien>>>> GetByIdCongViecNVTH(string Id_CongViec)
        {
            var nhanVienThucHiens = await _congViecRepository.GetByIdCongViecNVTH(Id_CongViec);
            return Ok(new ApiResponse<IEnumerable<QLNV_NhanVienThucHien>>(true, "Thành công", nhanVienThucHiens));
        }

        [HttpDelete("DeleteByIdCongViecNVTH/{Id_CongViec}")]
        public async Task<IActionResult> DeleteByIdCongViecNVTH(string Id_CongViec, [FromQuery] string userName)
        {
            await _congViecRepository.DeleteByIdCongViecNVTH(Id_CongViec, userName);
            return NoContent();
        }

        [HttpPut("UpdateNVTH")]
        public async Task<IActionResult> UpdateNVTH(QLNV_NhanVienThucHien data, [FromQuery] string userName)
        {
            await _congViecRepository.UpdateNVTH(data, userName);
            return NoContent();
        }

        [HttpGet("GetByIdNVTH/{id}")]
        public async Task<ActionResult<ApiResponse<QLNV_NhanVienThucHien>>> GetByIdNVTH(string id)
        {
            var nhanVienThucHien = await _congViecRepository.GetByIdNVTH(id);
            if (nhanVienThucHien == null)
            {
                return NotFound(new ApiResponse<QLNV_NhanVienThucHien>(false, "Không tìm thấy nhân viên thực hiện", null));
            }
            return Ok(new ApiResponse<QLNV_NhanVienThucHien>(true, "Thành công", nhanVienThucHien));
        }

        [HttpGet("GetIdNVTHByIdCongViec/{Id_CongViec}")]
        public async Task<ActionResult<ApiResponse<QLNV_NhanVienThucHien>>> GetIdNVTHByIdCongViec(string Id_CongViec, string userName)
        {
            var nhanVienThucHien = await _congViecRepository.GetIdNVTHByIdCongViec(Id_CongViec, userName);
            if (nhanVienThucHien == null)
            {
                return NotFound(new ApiResponse<QLNV_NhanVienThucHien>(false, "Không tìm thấy nhân viên thực hiện", null));
            }
            return Ok(new ApiResponse<QLNV_NhanVienThucHien>(true, "Thành công", nhanVienThucHien));
        }

        [HttpGet("GetAllNVTH")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_NhanVienThucHien>>>> GetAllNVTH([FromQuery] string groupId, [FromBody] QLNV_NhanVienThucHien input)
        {
            var nhanVienThucHiens = await _congViecRepository.GetAllNVTH(groupId, input);
            return Ok(new ApiResponse<IEnumerable<QLNV_NhanVienThucHien>>(true, "Thành công", nhanVienThucHiens));
        }

        [HttpPost("GetAllNVTH")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_NhanVienThucHien>>>> GetAllNVTH1([FromQuery] string groupId, [FromBody] QLNV_NhanVienThucHien input)
        {
            var nhanVienThucHiens = await _congViecRepository.GetAllNVTH(groupId, input);
            return Ok(new ApiResponse<IEnumerable<QLNV_NhanVienThucHien>>(true, "Thành công", nhanVienThucHiens));
        }

        // Các phương thức cho thêm ngày
        [HttpGet("GetCVByIdNhanVien")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_CongViec>>>> GetCVByIdNhanVien(string groupId, string[] Id_NhanVien)
        {
            var congViecs = await _congViecRepository.GetCVByIdNhanVien(groupId, Id_NhanVien);
            return Ok(new ApiResponse<IEnumerable<QLNV_CongViec>>(true, "Thành công", congViecs));
        }

        [HttpPost("InsertThemNgay")]
        public async Task<IActionResult> InsertThemNgay(QLNV_ThemNgay entity, [FromQuery] string userName)
        {
            await _congViecRepository.InsertThemNgay(entity, userName);
            return NoContent();
        }

        [HttpGet("GetByIdThemNgay/{Id}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QLNV_ThemNgay>>>> GetByIdThemNgay(string Id)
        {
            var themNgays = await _congViecRepository.GetByIdThemNgay(Id);
            return Ok(new ApiResponse<IEnumerable<QLNV_ThemNgay>>(true, "Thành công", themNgays));
        }

        [HttpGet("GetByIdCV/{Id}")]
        public async Task<ActionResult<ApiResponse<QLNV_ThemNgay>>> GetByIdCV(string Id)
        {
            var themNgay = await _congViecRepository.GetByIdCV(Id);
            if (themNgay == null)
            {
                return Ok(new ApiResponse<QLNV_ThemNgay>(false, "Không tìm thấy thêm ngày", null));
            }
            return Ok(new ApiResponse<QLNV_ThemNgay>(true, "Thành công", themNgay));
        }

        [HttpDelete("DeleteByIdThemNgay/{Id}")]
        public async Task<IActionResult> DeleteByIdThemNgay(string Id, [FromQuery] string userName)
        {
            await _congViecRepository.DeleteByIdThemNgay(Id, userName);
            return NoContent();
        }

        [HttpDelete("DeleteByIdCVThemNgay/{Id}")]
        public async Task<IActionResult> DeleteByIdCVThemNgay(string Id, [FromQuery] string userName)
        {
            await _congViecRepository.DeleteByIdCVThemNgay(Id, userName);
            return NoContent();
        }
        private async Task<T?> SafeCall<T>(Func<Task<T>> func)
        {
            try
            {
                return await func();
            }
            catch
            {
                return default;
            }
        }

        //Báo cáo theo người giao việc
        [HttpGet("GetStatusReport")]
        public async Task<ActionResult<ApiResponse<CongViecStatusReport>>> GetStatusReport([FromQuery] string groupId, [FromQuery] string id_NguoiGiaoViec)
        {
            var report = await _congViecRepository.GetStatusReport(groupId, id_NguoiGiaoViec);
            return Ok(new ApiResponse<CongViecStatusReport>(true, "Thành công", report));
        }
        [HttpGet("BaoCaoTheoNhom")]
        public async Task<ActionResult<ApiResponse<List<CongViecByNhomReport>>>> BaoCaoTheoNhom([FromQuery] string groupId, [FromQuery] string id_NguoiGiaoViec)
        {
            var data = await _congViecRepository.GetBaoCaoTheoNhom(groupId, id_NguoiGiaoViec);
            return Ok(new ApiResponse<List<CongViecByNhomReport>>(true, "Thành công", data));
        }
        [HttpGet("GetTienDoTrungBinh")]
        public async Task<ActionResult<ApiResponse<TienDoTrungBinhReport>>> GetTienDoTrungBinh([FromQuery] string groupId, [FromQuery] string id_NguoiGiaoViec)
        {
            var data = await _congViecRepository.GetTienDoTrungBinh(groupId, id_NguoiGiaoViec);
            return Ok(new ApiResponse<TienDoTrungBinhReport>(true, "Thành công", data));
        }

        [HttpGet("GetSoLuongTheoUuTien")]
        public async Task<ActionResult<ApiResponse<List<SoLuongTheoUuTienReport>>>> GetSoLuongTheoUuTien([FromQuery] string groupId, [FromQuery] string id_NguoiGiaoViec)
        {
            var data = await _congViecRepository.GetSoLuongTheoUuTien(groupId, id_NguoiGiaoViec);
            return Ok(new ApiResponse<List<SoLuongTheoUuTienReport>>(true, "Thành công", data));
        }

        [HttpGet("GetSoLuongTheoThoiGian")]
        public async Task<ActionResult<ApiResponse<List<SoLuongTheoThoiGianReport>>>> GetSoLuongTheoThoiGian([FromQuery] string groupId, [FromQuery] string id_NguoiGiaoViec)
        {
            var data = await _congViecRepository.GetSoLuongTheoThoiGian(groupId, id_NguoiGiaoViec);
            return Ok(new ApiResponse<List<SoLuongTheoThoiGianReport>>(true, "Thành công", data));
        }
        // báo cáo theo người thực hiện
        [HttpGet("DashboardNVTH")]
        public async Task<IActionResult> DashboardNVTH(string groupId,string taiKhoan)
        {
            try
            {
                var nhanVien = await _nhanVienRepository.GetNhanVienByTaiKhoan(taiKhoan);
                if (nhanVien.Id != null && nhanVien.Id != "")
                {
                    var idNhanVien = nhanVien.Id;
                    var data = new DashboardNVTHReport
                    {
                        TrangThai = await _congViecRepository.GetStatusReportNVTH(groupId, idNhanVien),
                        DanhGia = await _congViecRepository.GetBaoCaoDanhGiaNVTH(groupId, idNhanVien),
                        ThoiHan = await _congViecRepository.GetBaoCaoThoiHanNVTH(groupId, idNhanVien),
                        UuTien = await _congViecRepository.GetBaoCaoTheoUuTienNVTH(groupId, idNhanVien),
                        TienDoTrungBinh = (await _congViecRepository.GetTienDoTrungBinhNVTH(groupId, idNhanVien)).TienDoTrungBinh
                    };
                    return Ok(new ApiResponse<DashboardNVTHReport>(true, "Thành công", data));
                }
                else
                {
                    return Ok(new ApiResponse<DashboardNVTHReport>(false, "Không tìm thấy nhân viên thực hiện", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<DashboardNVTHReport>(false, ex.Message, null));
            }

        }


    }
}


public class CongViecRequestModel
{
    public QLNV_CongViec CongViec { get; set; }
    public string[] NhanVienThucHien { get; set; }
    public QLNV_ThemNgay Themngay { get; set; }
}

