using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DucAnh2025.Models.QLNV
{
    [Table("QLNV_CongViecEvents")]
    public class QLNV_CongViecEvent
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Id_CongViec { get; set; } = "";
        public string EventName { get; set; } = "";
        public string PayloadJson { get; set; } = "";
        public string TargetUserIdsJson { get; set; } = "[]";
        public int IsPublished { get; set; } = 0;
        public DateTime? PublishedAt { get; set; }
        public int RetryCount { get; set; } = 0;
        public string CompanyId { get; set; } = "";
        public string GroupId { get; set; } = "";
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public string CreateBy { get; set; } = "";
        public int IsActive { get; set; } = 1;
    }
}
