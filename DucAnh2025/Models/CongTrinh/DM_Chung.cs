using System.ComponentModel.DataAnnotations;
public class CT_DM_TrangThaiThiCong : DM_Chung { }
public class CT_DM_TrangThaiThiCong_Log : DM_Chung_Log { }
public class CT_DM_HinhThucDayHoGa : DM_Chung { }
public class CT_DM_HinhThucDayHoGa_Log : DM_Chung_Log { }
public class CT_DM_LoaiDauNoi : DM_Chung { }
public class CT_DM_LoaiDauNoi_Log : DM_Chung_Log { }
public class CT_DM_HinhThucDapTra : DM_Chung { }
public class CT_DM_HinhThucDapTra_Log : DM_Chung_Log { }

public class CT_DM_TenLoaiThep: DM_Chung { }
public class CT_DM_TenLoaiThep_Log : DM_Chung_Log { }

public class CT_DM_HangMucCongViec : DM_Chung { }
public class CT_DM_HangMucCongViec_Log : DM_Chung_Log { }
public class CT_DM_HangMucKhoiLuong : DM_Chung { }
public class CT_DM_HangMucKhoiLuong_Log : DM_Chung_Log { }
public class CT_DM_LoaiCauKien : DM_Chung { }
public class CT_DM_LoaiCauKien_Log : DM_Chung_Log { }
public class CT_DM_LoaiKhoiLuong : DM_Chung { }
public class CT_DM_LoaiKhoiLuong_Log : DM_Chung_Log { }
public class CT_DM_TuyenDuong : DM_Chung { }
public class CT_DM_TuyenDuong_Log : DM_Chung_Log { }
public class CT_DM_LyTrinh : DM_Chung { }
public class CT_DM_LyTrinh_Log : DM_Chung_Log { }


public partial class DM_Chung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required(ErrorMessage = "Bạn phải nhập tên danh mục")]
    public string TenDanhMuc { get; set; } = "";

    public string GroupId { get; set; } = "";
    public int Ordinarily { get; set; } = 0;
    public DateTime? CreateAt { get; set; } = DateTime.Now;
    public string CreateBy { get; set; } = "";
    public int IsActive { get; set; } = 0;
    public string? ApprovalUserId { get; set; }
    public DateTime? DateApproval { get; set; } = DateTime.Now;
    public string? ApprovalDept { get; set; }
    public string? DepartmentId { get; set; }
    public int DepartmentOrder { get; set; } = 0;
    public int ApprovalOrder { get; set; } = 0;
    public string? ApprovalId { get; set; }
    public string? LastApprovalId { get; set; }
    public string IsStatus { get; set; } = "";
}
public partial class DM_Chung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenDanhMuc { get; set; } = "";

    public string GroupId { get; set; } = "";
    public int Ordinarily { get; set; } = 0;
    public DateTime? CreateAt { get; set; } = DateTime.Now;
    public string CreateBy { get; set; } = "";
    public int IsActive { get; set; } = 0;
    public string? ApprovalUserId { get; set; }
    public DateTime? DateApproval { get; set; } = DateTime.Now;
    public string? ApprovalDept { get; set; }
    public string? DepartmentId { get; set; }
    public int DepartmentOrder { get; set; } = 0;
    public int ApprovalOrder { get; set; } = 0;
    public string? ApprovalId { get; set; }
    public string? LastApprovalId { get; set; }
    public string IsStatus { get; set; } = "";
    public string IdChung { get; set; } = "";
    public bool IsValid { get; set; } = false;
}



public partial class Chung
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string GroupId { get; set; } = "";
    public int Ordinarily { get; set; } = 0;
    public DateTime? CreateAt { get; set; } = DateTime.Now;
    public string CreateBy { get; set; } = "";
    public int IsActive { get; set; } = 0;
    public string? ApprovalUserId { get; set; }
    public DateTime? DateApproval { get; set; } = DateTime.Now;
    public string? ApprovalDept { get; set; }
    public string? DepartmentId { get; set; }
    public int DepartmentOrder { get; set; } = 0;
    public int ApprovalOrder { get; set; } = 0;
    public string? ApprovalId { get; set; }
    public string? LastApprovalId { get; set; }
    public string IsStatus { get; set; } = "";
}

public partial class Chung_Log
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string GroupId { get; set; } = "";
    public int Ordinarily { get; set; } = 0;
    public DateTime? CreateAt { get; set; } = DateTime.Now;
    public string CreateBy { get; set; } = "";
    public int IsActive { get; set; } = 0;
    public string? ApprovalUserId { get; set; }
    public DateTime? DateApproval { get; set; } = DateTime.Now;
    public string? ApprovalDept { get; set; }
    public string? DepartmentId { get; set; }
    public int DepartmentOrder { get; set; } = 0;
    public int ApprovalOrder { get; set; } = 0;
    public string? ApprovalId { get; set; }
    public string? LastApprovalId { get; set; }
    public string IsStatus { get; set; } = "";
    public string IdChung { get; set; } = "";
    public bool IsValid { get; set; } = false;
}
