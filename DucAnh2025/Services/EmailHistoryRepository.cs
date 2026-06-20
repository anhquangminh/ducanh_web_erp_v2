using DucAnh2025.Data;
using DucAnh2025.Models;
using DucAnh2025.Repository;
using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services
{
    public class EmailHistoryRepository : IEmailHistoryRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;

        public EmailHistoryRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }

        public Task<bool> CheckExclusive(string[] ids, DateTime baseTime)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckStatus(string ids, string name)
        {
            throw new NotImplementedException();
        }

        public Task DeleteById(string id, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<EmailHistory>> GetAll(string groupId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<EmailUserPermissionModel>> GetUserPermission(string companyId, string approvalStepId)
        {
            using var context = _context.CreateDbContext();
            var emailHistories = await (from p in context.ApprovalStaffSettings
                                        join q in context.ApplicationUsers on p.UserId equals q.Id
                                        where p.CompanyId == companyId && p.ApprovalStepId == approvalStepId
                                        select new EmailUserPermissionModel
                                        {
                                            ParentMajorId = p.ParentMajorId,
                                            MajorId = p.MajorId,
                                            Mail = q.Email,
                                            UserId = p.UserId,
                                        }).Distinct().ToListAsync();

            return emailHistories;
        }

        public async Task<List<(string, string, int)>> GetAllCategoriesByUser(string userId)
        {
            using var context = _context.CreateDbContext();
            var query = from eh in context.EmailHistories
                        join maj in context.Majors on eh.MajorId equals maj.Id into majGroup
                        from maj in majGroup.DefaultIfEmpty()
                        where maj.IsActive != 100 && eh.UserId == userId
                        group new { eh, maj } by new { eh.MajorId, maj.MajorName } into g
                        select new
                        {
                            g.Key.MajorId,
                            g.Key.MajorName,
                            UnreadCount = g.Sum(e => e.eh.IsRead == 0 ? 1 : 0)
                        };

            var result = await query.ToListAsync();
            return result.Select(x => (x.MajorId, x.MajorName, x.UnreadCount)).ToList();
        }

        public async Task<List<NotificationModel>> GetAllNotiByUser(string userId)
        {
            using var context = _context.CreateDbContext();
            var emailHistories = await (from eh in context.EmailHistories
                                        join major in context.Majors on eh.MajorId equals major.Id
                                        where major.IsActive != 100 && eh.UserId == userId
                                        orderby eh.ParentMajorId, eh.MajorId, eh.CreateAt descending
                                        select new NotificationModel
                                        {
                                            Id = eh.Id,
                                            Receiver = eh.Receiver,
                                            ParentName = major.MajorName,
                                            MajorName = major.MajorName,
                                            Subject = eh.Subject,
                                            Content = eh.Content,
                                            GroupId = eh.GroupId,
                                            CompanyId = eh.CompanyId,
                                            MajorId = eh.MajorId,
                                            ParentMajorId = eh.ParentMajorId,
                                            CreateAt = eh.CreateAt,
                                            CreateBy = eh.CreateBy,
                                            IsRead = eh.IsRead

                                        }).ToListAsync();

            return emailHistories;
        }

        public async Task<List<NotificationModel>> GetAllNotiByUser(string userId, int skip = 0, int take = 10)
        {
            using var context = _context.CreateDbContext();
            var emailHistories = await (from eh in context.EmailHistories
                                        join major in context.Majors on eh.MajorId equals major.Id
                                        where major.IsActive != 100 && eh.UserId == userId
                                        orderby eh.CreateAt descending
                                        select new NotificationModel
                                        {
                                            Id = eh.Id,
                                            Receiver = eh.Receiver,
                                            ParentName = major.MajorName,
                                            MajorName = major.MajorName,
                                            Subject = eh.Subject,
                                            Content = eh.Content,
                                            GroupId = eh.GroupId,
                                            CompanyId = eh.CompanyId,
                                            MajorId = eh.MajorId,
                                            ParentMajorId = eh.ParentMajorId,
                                            CreateAt = eh.CreateAt,
                                            CreateBy = eh.CreateBy,
                                            IsRead = eh.IsRead
                                        })
                                        .Skip(skip)
                                        .Take(take)
                                        .ToListAsync();

            return emailHistories;
        }


        public async Task<EmailHistory> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.EmailHistories.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new Exception($"Không tìm thấy bản ghi theo ID: {id}");
            }

            return entity;
        }

        public async Task<int> GetUnreadNotiByUser(string userId)
        {
            using var context = _context.CreateDbContext();
            var notifs = await context.EmailHistories.Where(x => x.UserId == userId && x.IsRead == 0).CountAsync();
            return notifs;
        }

        public async Task Insert(EmailHistory entity, string userId)
        {
            using var context = _context.CreateDbContext();
            if (entity == null)
            {
                throw new Exception("Không có bản ghi nào để thêm!");
            }

            context.EmailHistories.Add(entity);
            await context.SaveChangesAsync();
        }
        public async Task InsertMulti(List<EmailHistory> entity)
        {
            try
            {
                using var context = _context.CreateDbContext();
                if (entity == null)
                {
                    throw new Exception("Không có bản ghi nào được thêm!");
                }
                await context.EmailHistories.AddRangeAsync(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task SendEmail(EmailHistory emailHistory)
        {
            using var context = _context.CreateDbContext();
            if (emailHistory == null)
            {
                throw new Exception("Không có bản ghi nào để thêm!");
            }

            context.EmailHistories.Add(emailHistory);
            await context.SaveChangesAsync();
        }

        public async Task ReadNotifi(EmailHistory emailHistory, string userId)
        {
            using var context = _context.CreateDbContext();
            if (emailHistory == null)
            {
                throw new Exception($"Không tìm thấy bản ghi");
            }
            emailHistory.IsRead = 1;
            context.EmailHistories.Update(emailHistory);
            await context.SaveChangesAsync();
        }

        public async Task Update(EmailHistory emailHistory, string userId)
        {
            using var context = _context.CreateDbContext();
            if (emailHistory == null)
            {
                throw new Exception($"Không tìm thấy bản ghi");
            }

            context.EmailHistories.Update(emailHistory);
            await context.SaveChangesAsync();
        }

        public Task UpdateMulti(EmailHistory[] entities)
        {
            throw new NotImplementedException();
        }
    }
}
