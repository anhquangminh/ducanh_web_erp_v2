public static class MenuHelper
{
    public static List<MenuItem> FilterMenuByPermission(
        List<MenuItem> menus,
        HashSet<string> allowedMajors)
    {
        var result = new List<MenuItem>();

        foreach (var item in menus)
        {
            // ===== HEADING =====
            if (!string.IsNullOrEmpty(item.Heading))
            {
                result.Add(item);
                continue;
            }

            // ===== MENU CHA =====
            if (item.Children?.Any() == true)
            {
                var children = item.Children
                    .Where(c =>
                        c.IsPublic ||
                        (!string.IsNullOrEmpty(c.MajorId)
                         && allowedMajors.Contains(c.MajorId))
                    )
                    .ToList();

                if (children.Any())
                {
                    result.Add(new MenuItem
                    {
                        Title = item.Title,
                        Icon = item.Icon,
                        StateIcon = item.StateIcon,
                        Children = children
                    });
                }
            }
            else
            {
                // ===== MENU ĐƠN =====
                if (item.IsPublic ||
                    (!string.IsNullOrEmpty(item.MajorId)
                     && allowedMajors.Contains(item.MajorId)))
                {
                    result.Add(item);
                }
            }
        }

        return result;
    }
}
