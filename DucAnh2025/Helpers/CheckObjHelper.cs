using System.Reflection;
using System.Text.RegularExpressions;

namespace DucAnh2025.Helpers
{
    public class CheckObjHelper
    {
        // Trả về true nếu tất cả các thuộc tính double/double? là 0 hoặc null
        public bool AreAllDoublePropertiesZeroOrNull(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
                {
                    var value = property.GetValue(obj);
                    if (value != null && (double)value != 0)
                        return false;
                }
            }
            return true;
        }

        public static string UpdateSoHieu(string soHieu, int delta = -1)
        {
            if (string.IsNullOrEmpty(soHieu)) return soHieu;
            var match = Regex.Match(soHieu, @"^([A-Za-z]*)(\d+)$");
            if (match.Success)
            {
                string prefix = match.Groups[1].Value;
                string numberPart = match.Groups[2].Value;
                int number = int.Parse(numberPart);
                int newNumber = number + delta;
                if (newNumber < 0)
                    throw new ArgumentException($"Giá trị số hiệu không thể nhỏ hơn 0: {soHieu}");
                string formattedNumber = newNumber.ToString(new string('0', numberPart.Length));
                return $"{prefix}{formattedNumber}";
            }
            return soHieu;
        }
    }
}
