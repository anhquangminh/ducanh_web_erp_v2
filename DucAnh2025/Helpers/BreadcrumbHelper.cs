using System.Collections.Generic;
using System.Linq;
using DucAnh2025.Models;

public static class BreadcrumbHelper
{
    public static List<BreadcrumbItem> GetBreadcrumb(string controller, string action)
    {
        // Tìm trong tất cả các menu
        foreach (var menuGroup in MenuConfig.Menus)
        {
            string? heading = null;
            foreach (var item in menuGroup.Value)
            {
                if (!string.IsNullOrEmpty(item.Heading))
                {
                    heading = item.Heading;
                    continue;
                }

                // Nếu có children
                if (item.Children != null && item.Children.Any())
                {
                    var child = item.Children.FirstOrDefault(c =>
                        string.Equals(c.Controller, controller, System.StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(c.Action, action, System.StringComparison.OrdinalIgnoreCase)
                    );
                    if (child != null)
                    {
                        var breadcrumbs = new List<BreadcrumbItem>();
                        if (!string.IsNullOrEmpty(heading))
                            breadcrumbs.Add(new BreadcrumbItem { Title = heading, Url = "#", IsActive = false });
                        breadcrumbs.Add(new BreadcrumbItem { Title = item.Title, Url = "#", IsActive = false });
                        breadcrumbs.Add(new BreadcrumbItem { Title = child.Title, Url = "", IsActive = true });
                        return breadcrumbs;
                    }
                }
                else
                {
                    // Nếu là menu không có children
                    if (string.Equals(item.Controller, controller, System.StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(item.Action, action, System.StringComparison.OrdinalIgnoreCase))
                    {
                        var breadcrumbs = new List<BreadcrumbItem>();
                        if (!string.IsNullOrEmpty(heading))
                            breadcrumbs.Add(new BreadcrumbItem { Title = heading, Url = "#", IsActive = false });
                        breadcrumbs.Add(new BreadcrumbItem { Title = item.Title, Url = "", IsActive = true });
                        return breadcrumbs;
                    }
                }
            }
        }
        // Không tìm thấy thì trả về rỗng
        return new List<BreadcrumbItem>();
    }
}