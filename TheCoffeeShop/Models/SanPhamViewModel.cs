namespace QuanLyQuanCafe.Models
{
    public class SanPhamViewModel
    {
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public int MaDanhMuc { get; set; }
        public decimal DonGia { get; set; }
        public string MoTa { get; set; }

        public IFormFile? AnhUpload { get; set; } // ảnh upload
    }
}
