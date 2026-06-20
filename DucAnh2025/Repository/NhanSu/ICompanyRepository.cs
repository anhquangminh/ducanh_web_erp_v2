using DucAnh2025.Models.NhanSu;
using DucAnh2025.ViewModels.HeThong;

namespace DucAnh2025.Repository.NhanSu
{
    public interface ICompanyRepository : IBaseRepository<MCompany>
    {
        Task<List<MCompany>> GetAllCompanies();
        string CheckCondition(MCompany mcompanie, int InputSave);
        Task<List<MCompanyModel>> GetAllByVM();
    }
}
