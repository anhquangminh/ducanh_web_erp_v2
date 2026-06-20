using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DucAnh2025.Models.QLNV
{
    [Table("QLNV_CongViecActivities")]
    public class QLNV_CongViecActivity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Id_CongViec { get; set; } = "";
        public string EventType { get; set; } = "";
        public string? FieldName { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string Description { get; set; } = "";
        public string? ActorUserId { get; set; }
        public string ActorUserName { get; set; } = "";
        public string? MetadataJson { get; set; }
        public string CompanyId { get; set; } = "";
        public string GroupId { get; set; } = "";
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public int IsActive { get; set; } = 1;
    }
}
