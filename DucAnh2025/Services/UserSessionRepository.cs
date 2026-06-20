using DucAnh2025.Data;
using DucAnh2025.Models;
using DucAnh2025.Repository;

using Microsoft.EntityFrameworkCore;

namespace DucAnh2025.Services
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;

        public UserSessionRepository(IDbContextFactory<ApplicationDbContext> context)
        {
            _context = context;
        }

        public Task<bool> CheckExclusive(string[] ids, DateTime baseTime)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteById(string id, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = await GetById(id);

            if (entity == null)
            {
                throw new Exception($"Không tìm thấy bản ghi theo ID: {id}");
            }
            entity.IsActive = 0;
            context.UserSessions.Update(entity);
            await context.SaveChangesAsync();
        }

        public Task<List<UserSession>> GetAll(string groupId)
        {
            throw new NotImplementedException();
        }

        public async Task<UserSession> GetById(string id)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.UserSessions.Where(x => x.Id.Equals(id) && x.IsActive == 1).FirstOrDefaultAsync();
            return entity;
        }

        public async Task Insert(UserSession entity, string userId)
        {
            using var context = _context.CreateDbContext();
            if (entity == null)
            {
                throw new Exception("Không có bản ghi nào để thêm!");
            }

            context.UserSessions.Add(entity);
            await context.SaveChangesAsync();
        }

        public async Task<UserSession> GetByUserName(string userName)
        {
            using var context = _context.CreateDbContext();
            var entity = await context.UserSessions.Where(x => x.UserName.Equals(userName) && x.IsActive == 1).FirstOrDefaultAsync();
            return entity;
        }

        public async Task Update(UserSession userSession, string userId)
        {
            using var context = _context.CreateDbContext();
            var entity = GetById(userSession.Id);

            if (entity == null)
            {
                throw new Exception($"Không tìm thấy bản ghi theo ID: {userSession.Id}");
            }

            context.UserSessions.Update(userSession);
            await context.SaveChangesAsync();
        }

        public Task UpdateMulti(UserSession[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckStatus(string[] ids, DateTime baseTime)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckStatus(string ids, string name)
        {
            throw new NotImplementedException();
        }
    }
}
