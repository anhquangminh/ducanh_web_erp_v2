using DucAnh2025.Helpers;
using DucAnh2025.Repository;
using DucAnh2025.Repository.HeThong;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class SidebarViewComponent : ViewComponent
{
    private readonly IApplicationUserRepository _userRepo;
    private readonly IMajorUserPermissionRepository _permissionRepo;
    private readonly IRolePermissionRepository _rolePermissionRepo;

    public SidebarViewComponent(
        IApplicationUserRepository userRepo,
        IMajorUserPermissionRepository permissionRepo,
        IRolePermissionRepository rolePermissionRepo)
    {
        _userRepo = userRepo;
        _permissionRepo = permissionRepo;
        _rolePermissionRepo = rolePermissionRepo;
    }

    public async Task<IViewComponentResult> InvokeAsync(string menuKey)
    {
        var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return View(new List<MenuItem>());

        var user = await _userRepo.GetById(userId);
        if (user == null || string.IsNullOrEmpty(user.GroupId))
            return View(new List<MenuItem>());

        // 1. Major từ phân quyền nghiệp vụ
        var majorFromBusiness = await _permissionRepo
            .GetAccessibleMajorIds(userId, user.GroupId);

        // 2. Major từ Role → Permission
        var majorFromRole = await _rolePermissionRepo
            .GetMajorIdsByUserAsync(userId);

        // 3. Gộp lại
        var allowedMajors = majorFromBusiness
            .Union(majorFromRole)
            .Distinct()
            .ToList();

        // CLONE MENU GỐC
        var menus = MenuConfig.Menus.ContainsKey(menuKey)
            ? MenuConfig.Menus[menuKey].Select(m => m.Clone()).ToList()
            : new List<MenuItem>();

        // FILTER
        var filtered = MenuHelper.FilterMenuByPermission(
            menus,
            allowedMajors.ToHashSet()
        );

        return View(filtered);
    }
}
