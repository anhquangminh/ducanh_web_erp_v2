using DucAnh2025.Models.Accounts;
using DucAnh2025.Models.QLNV;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using DucAnh2025.ViewModels;

namespace DucAnh2025.Services.HeThong
{
    public class PermissionService
    {
        private readonly IPhanQuyenRepository _phanQuyenService;
        private readonly IApplicationUserRepository _applicationUserService;
      
        public PermissionService(IPhanQuyenRepository phanQuyenRepository, IApplicationUserRepository applicationUserService)
        {
            _phanQuyenService = phanQuyenRepository;
            _applicationUserService = applicationUserService;
        }

        public async Task<bool> HasPermissionAsync(string userName, string parentMajorId, string majorId, int permissionType)
        {
            var _appUser = _applicationUserService.GetByUserName(userName);
            if (_appUser == null)
                return false;

            var permissions = await _phanQuyenService.GetAllPermissionByGroupId(_appUser.GroupId, _appUser.Id, parentMajorId, majorId);
            return permissions.Any(p => p.PermissionType == permissionType && p.IsActive == 3);
        }
    }
}
