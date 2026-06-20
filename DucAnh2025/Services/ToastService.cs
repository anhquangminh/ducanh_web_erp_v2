namespace DucAnh2025.Services
{
    public class ToastService
    {
        private readonly List<ToastMessage> _toasts = new();
        public event Func<Task> OnChange;

        public IReadOnlyList<ToastMessage> Toasts => _toasts.AsReadOnly();

        public void ShowToast(string title, string message, string cssClass = "toast-info", string iconClass = "fa fa-check-circle", string progressBarClass = "bg-info", int duration = 5000)
        {
            var toast = new ToastMessage(title, message, cssClass, iconClass, progressBarClass, duration);
            _toasts.Add(toast);
            NotifyStateChanged();
            // Optionally: Automatically remove the toast after a specified duration
            Task.Delay(duration).ContinueWith(_ => RemoveToast(toast.Id));
        }

        //public void RemoveToast(Guid toastId)
        //{
        //    var toast = _toasts.FirstOrDefault(t => t.Id == toastId);
        //    if (toast != null)
        //    {
        //        _toasts.Remove(toast);
        //        NotifyStateChanged();
        //    }
        //}
        private readonly object _lock = new object();

        public void RemoveToast(Guid toastId)
        {
            lock (_lock)
            {
                var toast = _toasts.FirstOrDefault(t => t.Id == toastId);
                if (toast != null)
                {
                    _toasts.Remove(toast);
                    NotifyStateChanged();
                }
            }
        }


        private async void NotifyStateChanged()
        {
            if (OnChange != null)
            {
                await OnChange.Invoke();
            }
        }
    }
}
