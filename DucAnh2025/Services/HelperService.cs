using System.Text;

namespace DucAnh2025.Services
{
    public class HelperService : IHelperService
    {

        public string GeneratePassword(int length)
        {
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string special = "!@#$%^&*";

            var allChars = lowercase + uppercase + numbers + special;
            var random = new Random();
            var password = new StringBuilder();

            // Ensure the password contains at least one of each character type
            password.Append(lowercase[random.Next(lowercase.Length)]);
            password.Append(uppercase[random.Next(uppercase.Length)]);
            password.Append(numbers[random.Next(numbers.Length)]);
            password.Append(special[random.Next(special.Length)]);

            // Fill the rest of the password length with random characters from all categories
            for (int i = 4; i < length; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the characters to ensure randomness
            return new string(password.ToString().OrderBy(_ => random.Next()).ToArray());
        }
    }
}
