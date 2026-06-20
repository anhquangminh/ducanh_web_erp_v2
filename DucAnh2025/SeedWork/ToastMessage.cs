public class ToastMessage
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Title { get; }
    public string Message { get; }
    public string CssClass { get; }
    public string IconClass { get; }
    public string ProgressBarClass { get; }
    public DateTime Timestamp { get; }
    public int Duration { get; }

    public ToastMessage(string title, string message, string cssClass, string iconClass, string progressBarClass, int duration)
    {
        Title = title;
        Message = message;
        CssClass = cssClass;
        IconClass = iconClass;
        ProgressBarClass = progressBarClass;
        Timestamp = DateTime.Now;
        Duration = duration;
    }
}