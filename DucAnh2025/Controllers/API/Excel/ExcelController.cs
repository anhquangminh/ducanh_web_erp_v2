using DucAnh2025.Models.Excel;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Reflection;

namespace DucAnh2025.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        public ExcelController()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        [HttpPost("UploadExcel")]
        public IActionResult UploadExcel([FromForm] IFormFile file, [FromForm] string objectName, [FromForm] int startRow, [FromForm] string columnMappingJson)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, message = "File không hợp lệ." });

            if (string.IsNullOrWhiteSpace(objectName))
                return BadRequest(new { success = false, message = "ObjectName không được để trống." });

            if (string.IsNullOrWhiteSpace(columnMappingJson))
                return BadRequest(new { success = false, message = "ColumnMapping không được để trống." });

            Dictionary<string, string>? columnMapping;
            try
            {
                columnMapping = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(columnMappingJson);
            }
            catch
            {
                return BadRequest(new { success = false, message = "ColumnMapping không đúng định dạng JSON." });
            }

            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(t => t.Name == objectName);

            if (type == null)
                return BadRequest(new { success = false, message = "Không tìm thấy kiểu dữ liệu." });

            var result = new List<object>();
            try
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                            return BadRequest(new { success = false, message = "Không tìm thấy sheet trong file Excel." });

                        int rowCount = worksheet.Dimension.Rows;
                        int colCount = worksheet.Dimension.Columns;

                        for (int row = startRow; row <= rowCount; row++)
                        {
                            bool isRowEmpty = true;
                            for (int c = 1; c <= colCount; c++)
                            {
                                if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, c].Text))
                                {
                                    isRowEmpty = false;
                                    break;
                                }
                            }
                            if (isRowEmpty) continue;

                            var obj = Activator.CreateInstance(type);

                            foreach (var map in columnMapping)
                            {
                                if (map.Value.StartsWith("col") && int.TryParse(map.Value.Substring(3), out int colIndex))
                                {
                                    if (colIndex <= colCount)
                                    {
                                        var prop = type.GetProperty(map.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                        if (prop != null && prop.CanWrite)
                                        {
                                            var cellValue = worksheet.Cells[row, colIndex].Text?.Trim();
                                            object convertedValue = cellValue;
                                            if (prop.PropertyType != typeof(string))
                                            {
                                                try
                                                {
                                                    convertedValue = Convert.ChangeType(cellValue, prop.PropertyType);
                                                }
                                                catch
                                                {
                                                    convertedValue = null;
                                                }
                                            }
                                            prop.SetValue(obj, convertedValue);
                                        }
                                    }
                                }
                            }

                            result.Add(obj);
                        }
                    }
                }

                return Ok(new
                {
                    success = true,
                    objectName = objectName,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi xử lý file Excel: " + ex.Message });
            }
        }
    }
}
