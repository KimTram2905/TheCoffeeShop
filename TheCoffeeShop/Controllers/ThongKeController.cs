using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheCoffeeShop.Models;

namespace TheCoffeeShop.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly DbquanLyQuanCafeContext _context;

        public ThongKeController(DbquanLyQuanCafeContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var thongKe = new ThongKeViewModel();

            // Thiết lập ngày mặc định nếu không có giá trị
            startDate = startDate ?? DateTime.Today.AddDays(-7);
            endDate = endDate ?? DateTime.Today;

            // Lưu ngày vào ViewBag để hiển thị trong form
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            // Lấy chi tiết đơn hàng trong khoảng thời gian
            var chiTietDonHang = await _context.DonHangs
                .Include(d => d.HoaDon)
                .Where(d => d.ThoiGianTao.Date >= startDate.Value.Date && d.ThoiGianTao.Date <= endDate.Value.Date)
                .Select(d => new
                {
                    d.MaDonHang,
                    d.ThoiGianTao,
                    d.TrangThai,
                    d.LoaiDonHang,
                    d.HoaDon
                })
                .OrderByDescending(d => d.ThoiGianTao)
                .ToListAsync();
            ViewBag.ChiTietDonHang = chiTietDonHang;

            // Thống kê tổng quan
            thongKe.TongKhachHang = await _context.NguoiDungs
                .CountAsync(n => n.VaiTro == "Khách hàng");

            thongKe.TongDonHang = await _context.DonHangs.CountAsync();

            // Doanh thu theo khoảng thời gian
            thongKe.TongDoanhThu = await _context.HoaDons
                .Where(h => h.NgayThanhToan.Date >= startDate.Value.Date && h.NgayThanhToan.Date <= endDate.Value.Date)
                .SumAsync(h => (decimal?)h.TongTien) ?? 0;

            thongKe.TongSanPham = await _context.SanPhams.CountAsync();

            // Thống kê đơn hàng theo trạng thái
            thongKe.DonHangChuaThanhToan = await _context.DonHangs
                .CountAsync(d => d.TrangThai == "Chưa thanh toán");

            thongKe.DonHangDaThanhToan = await _context.DonHangs
                .CountAsync(d => d.TrangThai == "Đã thanh toán");

            thongKe.DonHangHuy = await _context.DonHangs
                .CountAsync(d => d.TrangThai == "Hủy");

            // Thống kê sản phẩm bán chạy
            thongKe.SanPhamBanChay = await _context.ChiTietDonHangs
                .Include(ct => ct.MaSanPhamNavigation)
                .GroupBy(ct => new { ct.MaSanPham, ct.MaSanPhamNavigation.TenSanPham })
                .Select(g => new SanPhamBanChayViewModel
                {
                    TenSanPham = g.Key.TenSanPham,
                    SoLuongBan = g.Sum(ct => ct.SoLuong),
                    DoanhThu = g.Sum(ct => ct.ThanhTien) ?? 0
                })
                .OrderByDescending(sp => sp.SoLuongBan)
                .Take(5)
                .ToListAsync();

            // Thống kê doanh thu theo danh mục
            thongKe.DoanhThuTheoDanhMuc = await _context.ChiTietDonHangs
                .Include(ct => ct.MaSanPhamNavigation)
                .ThenInclude(sp => sp.MaDanhMucNavigation)
                .GroupBy(ct => ct.MaSanPhamNavigation.MaDanhMucNavigation.TenDanhMuc)
                .Select(g => new DoanhThuDanhMucViewModel
                {
                    TenDanhMuc = g.Key,
                    DoanhThu = g.Sum(ct => ct.ThanhTien) ?? 0,
                    SoLuong = g.Sum(ct => ct.SoLuong)
                })
                .OrderByDescending(dm => dm.DoanhThu)
                .ToListAsync();

            // Thống kê đơn hàng theo loại
            thongKe.DonHangTheoLoai = await _context.DonHangs
                .GroupBy(d => d.LoaiDonHang)
                .Select(g => new DonHangTheoLoaiViewModel
                {
                    LoaiDonHang = g.Key,
                    SoLuong = g.Count(),
                    DoanhThu = g.Where(d => d.HoaDon != null)
                              .Sum(d => (decimal?)d.HoaDon.TongTien) ?? 0
                })
                .ToListAsync();

            // Thống kê đặt chỗ
            thongKe.DatChoHomNay = await _context.DatChos
                .CountAsync(dc => dc.ThoiGianDen.Date == DateTime.Today);

            thongKe.DatChoDaXacNhan = await _context.DatChos
                .CountAsync(dc => dc.TrangThai == "Đã xác nhận");

            thongKe.DatChoChuaXacNhan = await _context.DatChos
                .CountAsync(dc => dc.TrangThai == "Chưa xác nhận");

            return View(thongKe);
        }

        [HttpGet]
        public async Task<IActionResult> GetDoanhThuTheoThang(int nam)
        {
            var doanhThuTheoThang = new List<object>();

            for (int thang = 1; thang <= 12; thang++)
            {
                var doanhThu = await _context.HoaDons
                    .Where(h => h.NgayThanhToan.Year == nam && h.NgayThanhToan.Month == thang)
                    .SumAsync(h => (decimal?)h.TongTien) ?? 0;

                doanhThuTheoThang.Add(new { thang, doanhThu });
            }

            return Json(doanhThuTheoThang);
        }
    }
}