public class MenuItem
{
    public string Title { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Icon { get; set; }
    public string StateIcon { get; set; }
    public List<MenuItem> Children { get; set; }
    public string Heading { get; set; }
    public bool IsPublic { get; set; } = false;
    public string MajorId { get; set; }

    // Deep clone method to avoid reference issues when filtering menus
    public MenuItem Clone()
    {
        return new MenuItem
        {
            Title = this.Title,
            Controller = this.Controller,
            Action = this.Action,
            Icon = this.Icon,
            StateIcon = this.StateIcon,
            Heading = this.Heading,
            IsPublic = this.IsPublic,
            MajorId = this.MajorId,
            Children = this.Children != null
                ? this.Children.Select(child => child.Clone()).ToList()
                : null
        };
    }
}