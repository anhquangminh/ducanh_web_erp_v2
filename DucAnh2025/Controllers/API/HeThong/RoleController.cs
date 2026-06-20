using DucAnh2025.Models.HeThong;
using DucAnh2025.Repository;
using Microsoft.AspNetCore.Mvc;
namespace DucAnh2025.Controllers.API.HeThong
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;

        public RoleController(
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IRolePermissionRepository rolePermissionRepository,
            IApplicationUserRepository applicationUserRepository)
        {
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _applicationUserRepository = applicationUserRepository;
        }

        // ===============================================================
        // 2) ROLE PERMISSIONS
        // ===============================================================
        [HttpGet("Role/{roleId}/Permissions")]
        public async Task<IActionResult> GetRolePermissions(string roleId)
        {
            var data = await _rolePermissionRepository.GetByRoleAsync(roleId);
            return Ok(new ApiResponse<object>(true, "Lấy quyền của Role thành công", data));
        }

        [HttpPost("Role/{roleId}/Permissions/Update")]
        public async Task<IActionResult> UpdateRolePermissions(string roleId, [FromBody] List<string> permissionIds)
        {
            await _rolePermissionRepository.RemoveByRoleAsync(roleId);

            foreach (var pid in permissionIds)
            {
                await _rolePermissionRepository.AddAsync(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = pid
                });
            }

            return Ok(new ApiResponse<bool>(true, "Cập nhật quyền Role thành công", true));
        }

        // ===============================================================
        // 3) ROLE CRUD
        // ===============================================================
        [HttpGet("Roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleRepository.GetAllAsync();
            return Ok(new ApiResponse<object>(true, "Lấy danh sách Roles thành công", roles));
        }

        [HttpGet("Role/{id}")]
        public async Task<IActionResult> GetRole(string id)
        {
            var role = await _roleRepository.GetByIdAsync(id);

            if (role == null)
                return NotFound(new ApiResponse<object>(false, "Role không tồn tại", null));

            return Ok(new ApiResponse<object>(true, "Lấy Role thành công", role));
        }

        [HttpPost("Role")]
        public async Task<IActionResult> AddRole([FromBody] Role role)
        {
            await _roleRepository.AddAsync(role);
            return Ok(new ApiResponse<bool>(true, "Thêm Role thành công", true));
        }

        [HttpPut("Role")]
        public async Task<IActionResult> UpdateRole([FromBody] Role role)
        {
            await _roleRepository.UpdateAsync(role);
            return Ok(new ApiResponse<bool>(true, "Cập nhật Role thành công", true));
        }

        [HttpDelete("Role/{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            await _roleRepository.DeleteAsync(id);
            return Ok(new ApiResponse<bool>(true, "Xóa Role thành công", true));
        }

        // ===============================================================
        // 4) USER ROLES
        // ===============================================================
        [HttpGet("User/{userId}/Roles")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var data = await _userRoleRepository.GetRolesByUserAsync(userId);
            return Ok(new ApiResponse<object>(true, "Lấy Roles của User thành công", data));
        }

        [HttpGet("UserRole/all")]
        public async Task<IActionResult> GetAllUserRoles(string groupId)
        {
            var userRoles = await _userRoleRepository.GetAllAsync();
            var users = await _applicationUserRepository.GetAll(groupId);
            var roles = await _roleRepository.GetAllAsync();

            var userDict = users.ToDictionary(u => u.Id);
            var roleDict = roles.ToDictionary(r => r.Id);

            var result = userRoles.Select(ur =>
            {
                userDict.TryGetValue(ur.UserId, out var u);
                roleDict.TryGetValue(ur.RoleId, out var r);

                return new
                {
                    userId = ur.UserId,
                    roleId = ur.RoleId,
                    userName = u == null
                        ? ""
                        : $"{u.FirstName} {u.LastName}".Trim() +
                          (string.IsNullOrEmpty(u.Email) ? "" : $" ({u.Email})"),
                    roleName = r?.Name ?? ""
                };
            });

            return Ok(new ApiResponse<object>(true, "Lấy toàn bộ UserRole thành công", result));
        }



        [HttpPost("UserRole")]
        public async Task<IActionResult> AddUserRole([FromBody] UserRole userRole)
        {
            await _userRoleRepository.AddAsync(userRole);
            return Ok(new ApiResponse<bool>(true, "Thêm UserRole thành công", true));
        }

        [HttpDelete("UserRole")]
        public async Task<IActionResult> RemoveUserRole([FromBody] UserRole userRole)
        {
            await _userRoleRepository.RemoveAsync(userRole);
            return Ok(new ApiResponse<bool>(true, "Xóa UserRole thành công", true));
        }

        // ===============================================================
        // 5) ROLE PERMISSIONS CRUD
        // ===============================================================
        [HttpGet("RolePermissions")]
        public async Task<IActionResult> GetAllRolePermissions()
        {
            var data = await _rolePermissionRepository.GetAllAsync();
            return Ok(new ApiResponse<object>(true, "Lấy toàn bộ RolePermissions thành công", data));
        }

        [HttpPost("RolePermission")]
        public async Task<IActionResult> AddRolePermission([FromBody] RolePermission rolePermission)
        {
            await _rolePermissionRepository.AddAsync(rolePermission);
            return Ok(new ApiResponse<bool>(true, "Thêm RolePermission thành công", true));
        }
        [HttpPost("AddRolePermissionByMajor")]
        public async Task<IActionResult> AddRolePermissionByMajor([FromBody] AddRolePermissionByMajorDto input)
        {
            if (string.IsNullOrEmpty(input.RoleId) ||
                string.IsNullOrEmpty(input.MajorId) ||
                input.PermissionIds == null ||
                !input.PermissionIds.Any())
            {
                return Ok(new ApiResponse<bool>(false, "Dữ liệu không hợp lệ", false));
            }

            try
            {
                var userId = User.Identity?.Name ?? "system";

                await _rolePermissionRepository.AddByMajorAsync(
                    input.RoleId,
                    input.MajorId,
                    input.PermissionIds,
                    input.GroupId,
                    userId
                );

                return Ok(new ApiResponse<bool>(true, "Gán quyền theo nghiệp vụ thành công", true));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<bool>(false, ex.Message, false));
            }
        }


        [HttpDelete("RolePermission")]
        public async Task<IActionResult> RemoveRolePermission([FromBody] RolePermission rolePermission)
        {
            await _rolePermissionRepository.RemoveAsync(rolePermission);
            return Ok(new ApiResponse<bool>(true, "Xóa RolePermission thành công", true));
        }

    }
}
