namespace DucAnh2025.Helpers
{
    public class Format
    {
        public static string FormatCurrency(object amount)
        {
            return double.TryParse(amount.ToString(), out double number)
                ? string.Format("{0:N2}", number)
                : amount.ToString();
        }

        public static string FormatVND(object amount, string cultureCode = "vi-VN")
        {
            if (double.TryParse(amount.ToString(), out double number))
            {
                var cultureInfo = new System.Globalization.CultureInfo(cultureCode);
                string formatted = number.ToString("C", cultureInfo);
                return formatted.Replace(" ₫", "").Replace("₫", "");
            }
            return amount.ToString();
        }

        public static string FormatDate(DateTime? date, string format = "dd/MM/yyyy")
        {
            return date.HasValue ? date.Value.ToString(format) : "";
        }
    }
}
