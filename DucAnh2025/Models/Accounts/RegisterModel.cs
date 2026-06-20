using System.ComponentModel.DataAnnotations;

namespace DucAnh2025.Models.Accounts
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Bạn phải nhập họ!")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập tên!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập email!")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập ngày tháng năm sinh!")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Dob { get; set; }


        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        [Required(ErrorMessage = "Bạn phải nhập số điện thoại!")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập tên doanh nghiệp!")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập địa chỉ!")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Bạn phải nhập mã số thuế!")]
        public string Tax { get; set; }
    }
}
