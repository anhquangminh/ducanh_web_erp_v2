namespace DucAnh2025.Models.Excel
{
    public class ExcelImportRequest
    {
        public string ObjectName { get; set; }
        public int StartRow { get; set; }
        public Dictionary<string, string> ColumnMapping { get; set; }
    }
}
