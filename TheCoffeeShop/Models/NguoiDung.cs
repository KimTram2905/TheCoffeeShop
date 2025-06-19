using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace TheCoffeeShop.Models
{
    public partial class NguoiDung
    {
        public int MaNguoiDung { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Tên đăng nhập từ 5-50 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Tên đăng nhập chỉ chứa chữ cái và số")]
        public string TenDangNhap { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = null!;

        public string VaiTro { get; set; } = "Khách hàng"; // Giá trị mặc định

        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Họ tên không quá 100 ký tự")]
        public string HoTen { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(15, ErrorMessage = "Số điện thoại không quá 15 ký tự")]
        public string SoDienThoai { get; set; } = null!;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(100, ErrorMessage = "Email không quá 100 ký tự")]
        public string Email { get; set; } = null!;

        public string? DiaChi { get; set; }

        public string? Avatar { get; set; }

        public virtual ICollection<DatCho> DatChos { get; set; } = new List<DatCho>();
        public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
    }
}