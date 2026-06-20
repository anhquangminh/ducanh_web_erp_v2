using DucAnh2025.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.HeThong
{
    public class MajorModel : PagingParameters
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? ParentId { get; set; } = "";
        public string MajorName { get; set; } = "";
        public int Order { get; set; } = 0;
        public string? Table { get; set; } = "";
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public string CreateBy { get; set; } = "";
        public int IsActive { get; set; } = 0;
        public string ParentName { get; set; } = "";
    }
}
