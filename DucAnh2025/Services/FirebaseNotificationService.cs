using DucAnh2025.Data;
using DucAnh2025.Models;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace DucAnh2025.Services
{
    public class FirebaseNotificationService
    {
        private readonly string _firebaseProjectId = "ducanherp";
        private readonly string _serviceAccountJsonPath;
        private readonly IDbContextFactory<ApplicationDbContext> _context;

        public FirebaseNotificationService(IWebHostEnvironment env, IDbContextFactory<ApplicationDbContext> context)
        {
            _serviceAccountJsonPath = Path.Combine(env.WebRootPath, "files", "ducanherp-firebase-adminsdk-fbsvc-78bd260d8e.json");
            _context = context;
        }

        public async Task SendNotificationAsync(string targetFcmToken, string title, string body ,string targetPage = "", string targetId = "")
        {
            // Tạo object theo chuẩn Firebase v1
            var message = new FirebaseNotification
            {
                Message = new Message
                {
                    Token = targetFcmToken,
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    },
                    Data = new Dictionary<string, string>
                    {
                        {
                            "page",
                            targetPage
                        },
                        {
                            "id",
                            targetId
                        }
                    },
                    Android = new Android
                    {
                        Priority = "high"
                    },
                    Apns = new Apns
                    {
                        Headers = new ApnsHeaders
                        {
                            ApnsPriority = "10"
                        }
                    }
                }
            };
            string accessToken = await GetAccessTokenAsync();
            var log = new NotificationLog
            {
                TargetToken = targetFcmToken,
                Title = title,
                Body = body,
                CreatedAt = DateTime.UtcNow,
                TargetPage = targetPage,
                TargetId = targetId
            };
            try
            {

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var json = JsonConvert.SerializeObject(message);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"https://fcm.googleapis.com/v1/projects/{_firebaseProjectId}/messages:send";
                var response = await client.PostAsync(url, content);

                Console.WriteLine($"----sending notification: {response.StatusCode}");
                if (response.IsSuccessStatusCode)
                {
                    log.ResponseStatus = "Success";
                }
                else
                {
                    log.ResponseStatus = "Fail";
                    log.ErrorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"----Error sending notification: {log.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                log.ResponseStatus = "Exception";
                log.ErrorMessage = ex.Message;
                Console.WriteLine($"----Exception sending notification: {ex.Message}");
            }
            finally
            {
                using var context = _context.CreateDbContext();
                context.NotificationLogs.Add(log);
                await context.SaveChangesAsync();
            }


        }
        private async Task<string> GetAccessTokenAsync()
        {
            try
            {
                var googleCredential = GoogleCredential
                    .FromFile(_serviceAccountJsonPath)
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

                var token = await googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync();
                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAccessTokenAsync error: {ex.Message}");
                throw;
            }
        }
        public async Task SendNotificationToUserAsync(string userId, string title, string body, string targetPage="", string targetId = "")
        {
            using var context = _context.CreateDbContext();
            // Tạo danh sách thông báo để lưu vào DB
            var notificationFireBase = new NotificationFireBase
            {
                Id = Guid.NewGuid().ToString(),
                ReciverId = userId,
                Title = title,
                Body = body,
                IsRead = 0,
                Creatby = "system",
                CreatedAt = DateTime.UtcNow,
                TargetPage= targetPage,
                TargetId = targetId
            };
            await context.NotificationFireBases.AddRangeAsync(notificationFireBase);
            await context.SaveChangesAsync();
            var tokens = await context.UserFcmTokens
                .Where(x => x.UserId == userId && x.IsActive)
                .Select(x => x.Token)
                .ToListAsync();

            foreach (var token in tokens)
            {
                await SendNotificationAsync(token, title, body);
            }
        }

        //Gửi thông báo đến nhiều người dùng
        public async Task SendNotificationToMultipleAsync(List<string> userIds, string title, string body, string targetPage = "", string targetId = "")
        {
            using var context = _context.CreateDbContext();

            // Tạo danh sách thông báo để lưu vào DB
            var notificationFireBases = userIds.Select(userId => new NotificationFireBase
            {
                Id = Guid.NewGuid().ToString(),
                ReciverId = userId,
                Title = title,
                Body = body,
                IsRead = 0,
                Creatby = "system",
                CreatedAt = DateTime.UtcNow,
                TargetPage = targetPage,
                TargetId = targetId
            }).ToList();
            await context.NotificationFireBases.AddRangeAsync(notificationFireBases);
            await context.SaveChangesAsync();

            // Gửi FCM
            var tokens = await context.UserFcmTokens
                .Where(x => userIds.Contains(x.UserId) && x.IsActive)
                .Select(x => x.Token)
                .Distinct()
                .ToListAsync();

            foreach (var token in tokens)
            {
                await SendNotificationAsync(token, title, body);
            }
        }


        //lấy thông báo theo userId
        public async Task<List<NotificationFireBase>> GetNotificationsByReciverIdAsync(string reciverId, int skip = 0, int take = 10)
        {
            using var context = _context.CreateDbContext();

            var notifications = await context.NotificationFireBases
                .Where(n => n.ReciverId == reciverId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return notifications;
        }
        public async Task<int> GetUnreadNotificationCountAsync(string reciverId)
        {
            using var context = _context.CreateDbContext();

            var count = await context.NotificationFireBases
                .Where(x => x.ReciverId == reciverId && x.IsRead == 0 && x.IsActive == 1)
                .CountAsync();

            return count;
        }

        public async Task UpdateIsReadAsync(string notificationId)
        {
            using var context = _context.CreateDbContext();

            var noti = await context.NotificationFireBases.FirstOrDefaultAsync(x => x.Id == notificationId);
            if (noti != null)
            {
                noti.IsRead = 1;
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateIsReadManyAsync(List<string> notificationIds)
        {
            using var context = _context.CreateDbContext();

            var notifications = await context.NotificationFireBases
                .Where(x => notificationIds.Contains(x.Id))
                .ToListAsync();

            foreach (var noti in notifications)
            {
                noti.IsRead = 1;
            }

            await context.SaveChangesAsync();
        }
        public async Task UpdateIsActiveAsync(string notificationId, int isActive)
        {
            using var context = _context.CreateDbContext();

            var noti = await context.NotificationFireBases.FirstOrDefaultAsync(x => x.Id == notificationId);
            if (noti != null)
            {
                noti.IsActive = isActive;
                await context.SaveChangesAsync();
            }
        }



        //public async Task SendNotificationToMultipleAsync(List<string> userIds, string title, string body)
        //{
        //    using var context = _context.CreateDbContext();

        //    var tokens = await context.UserFcmTokens
        //        .Where(x => userIds.Contains(x.UserId) && x.IsActive)
        //        .Select(x => x.Token)
        //        .Distinct()
        //        .ToListAsync();

        //    foreach (var token in tokens)
        //    {
        //        await SendNotificationAsync(token, title, body);
        //    }
        //}

        //Gửi thông báo đến tất cả người dùng
        //public async Task SendNotificationToAllAsync(string title, string body)
        //{
        //    using var context = _context.CreateDbContext();

        //    var tokens = await context.UserFcmTokens
        //        .Where(x => x.IsActive)
        //        .Select(x => x.Token)
        //        .Distinct()
        //        .ToListAsync();

        //    foreach (var token in tokens)
        //    {
        //        await SendNotificationAsync(token, title, body);
        //    }
        //}
    }
}
