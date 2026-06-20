using DucAnh2025.Data;
using DucAnh2025.Repository;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services
{
    public class LoginService : ILoginService
    {
        private readonly Dictionary<string, (string Code, DateTime CreatedAt)> verificationCodes = new();
        private readonly Random random = new();
        private readonly TimeSpan codeExpiryDuration = TimeSpan.FromMinutes(5); // Đúng với comment
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContext;

        public LoginService(IHttpContextAccessor context, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _contextAccessor = context;
            _dbContext = dbContextFactory;
        }

        public string GenerateVerificationCode(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;
            var code = string.Concat(Enumerable.Range(0, 6).Select(_ => random.Next(0, 10)));
            verificationCodes[email] = (code, DateTime.UtcNow);
            return code;
        }

        public bool VerifyCode(string email, string code)
        {
            //if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            //{
            //    return false;
            //}

            //if (verificationCodes.TryGetValue(email, out var storedCode))
            //{
            //    // Kiểm tra thời hạn hiệu lực của mã
            //    if (DateTime.UtcNow - storedCode.CreatedAt <= codeExpiryDuration)
            //    {
            //        return storedCode.Code == code;
            //    }
            //    else
            //    {
            //        // Xóa mã nếu hết hạn
            //        verificationCodes.Remove(email);
            //    }
            //}
            //return false;
            return true;
        }

        public void ClearVerificationCode(string email)
        {
            verificationCodes.Remove(email);
        }

        public async Task<bool> CheckLogin()
        {
            var context = _contextAccessor.HttpContext;
            if (context != null)
            {
                var sessionId = context.Session.GetString("sessionId");
                var userName = context.Session.GetString("userName");

                if (!string.IsNullOrEmpty(sessionId) && !string.IsNullOrEmpty(userName))
                {
                    using var db = _dbContext.CreateDbContext();
                    var userSession = await db.UserSessions
                        .FirstOrDefaultAsync(s => s.UserName == userName && s.Id == sessionId && s.IsActive == 1);

                    if (userSession == null)
                    {
                        context.Session.Clear();
                        return false;
                    }

                    if (DateTime.UtcNow - userSession.LastActive > TimeSpan.FromMinutes(30))
                    {
                        context.Session.Clear();
                        return false;
                    }

                    userSession.LastActive = DateTime.UtcNow;
                    db.Update(userSession);
                    await db.SaveChangesAsync();
                }
            }
            return true;
        }
    }
}
