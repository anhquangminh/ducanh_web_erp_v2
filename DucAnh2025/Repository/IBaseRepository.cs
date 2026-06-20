namespace DucAnh2025.Repository
{
    public interface IBaseRepository<T>
    {
        Task<T> GetById(string id);
        Task<List<T>> GetAll(string groupId);
        Task Update(T entity, string userId);
        Task UpdateMulti(T[] entities);
        Task DeleteById(string id, string userId);
        Task<bool> CheckExclusive(string[] ids, DateTime baseTime);
        Task<bool> CheckStatus(string ids, string name);
        Task Insert(T entity, string userId);
    }
}