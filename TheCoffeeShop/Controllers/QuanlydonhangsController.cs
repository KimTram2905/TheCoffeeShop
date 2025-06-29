using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanCafe.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace QuanLyQuanCafe.Controllers
{
    public class QuanlydonhangsController : Controller
    {
        private readonly DbquanLyQuanCafeContext _context;

        public object TrangThai { get; private set; }

        public QuanlydonhangsController(DbquanLyQuanCafeContext context)
        {
            _context = context;
        }

        // GET: Quanlydonhangs
        public async Task<IActionResult> Index(string search, DateTime? fromDate, DateTime? toDate, int? year, int? month, int? day, string paymentStatus)
        {
            var query = _context.DonHangs
                .Include(d => d.MaNguoiDungNavigation)
                .Include(d => d.HoaDon)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int maDon))
                {
                    query = query.Where(d => d.MaDonHang == maDon ||
                                             d.MaNguoiDungNavigation.TenDangNhap.Contains(search));
                }
                else
                {
                    query = query.Where(d => d.MaNguoiDungNavigation.TenDangNhap.Contains(search));
                }
            }

            if (fromDate.HasValue)
            {
                query = query.Where(d => d.ThoiGianTao >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(d => d.ThoiGianTao <= toDate.Value);
            }

            // Lọc theo ngày/tháng/năm riêng biệt
            if (year.HasValue)
            {
                query = query.Where(d => d.ThoiGianTao.Year == year.Value);
            }
            if (month.HasValue)
            {
                query = query.Where(d => d.ThoiGianTao.Month == month.Value);
            }
            if (day.HasValue)
            {
                query = query.Where(d => d.ThoiGianTao.Day == day.Value);
            }

            var donHangs = await query.OrderByDescending(d => d.ThoiGianTao).ToListAsync();
            /*if (TempData["TongTienMoi"] != null)
            {
                ViewBag.TongTienMoi = TempData["TongTienMoi"];
            }*/
            ViewBag.Search = search;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.Day = day;
            ViewBag.Month = month;
            ViewBag.Year = year;
            ViewBag.PaymentStatus = paymentStatus;

            return View(donHangs);
        }

        // GET: Quanlydonhangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var donHang = await _context.DonHangs
                .Include(d => d.MaNguoiDungNavigation)
                .Include(d => d.MaBanNavigation)
                .Include(d => d.ChiTietDonHangs).ThenInclude(c => c.MaSanPhamNavigation)
                .FirstOrDefaultAsync(m => m.MaDonHang == id);

            if (donHang == null) return NotFound();

            return View(donHang);
        }

        // GET: Quanlydonhangs/Create
        public IActionResult Create()
        {
            ViewData["MaBan"] = new SelectList(_context.Bans, "MaBan", "MaBan");
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung");
            ViewData["LoaiDonHang"] = new SelectList(new List<string> { "Mang đi", "Tại quán", "Đặt trước" });
            ViewData["TrangThai"] = new SelectList(new List<string> { "Đã thanh toán", "Chưa thanh toán", "Hủy" });
            return View();
        }

        // POST: Quanlydonhangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDonHang,MaNguoiDung,MaBan,ThoiGianTao,ThoiGianCapNhat,LoaiDonHang,TrangThai")] DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(donHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaBan"] = new SelectList(_context.Bans, "MaBan", "MaBan", donHang.MaBan);
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", donHang.MaNguoiDung);
            return View(donHang);
        }*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string LoaiDonHang, string? MaBan, string TrangThai)
        {
            int? maBanParsed = null;
            if (LoaiDonHang == "Tại quán")
            {
                if (string.IsNullOrEmpty(MaBan) || !int.TryParse(MaBan, out int maBanValue))
                {
                    ModelState.AddModelError("MaBan", "Vui lòng chọn bàn hợp lệ cho đơn tại quán.");
                    ViewData["MaBan"] = new SelectList(_context.Bans, "MaBan", "MaBan");
                    ViewData["TrangThai"] = new SelectList(new List<string> { "Đã thanh toán", "Chưa thanh toán", "Hủy" }, TrangThai);
                    ViewData["LoaiDonHang"] = new SelectList(new List<string> { "Mang đi", "Tại quán", "Đặt trước" }, LoaiDonHang);
                    return View(); // hoặc return View(donHang) nếu cần hiển thị lại dữ liệu
                }
                maBanParsed = maBanValue;
            }

            var donHang = new DonHang
            {
                LoaiDonHang = LoaiDonHang,
                MaBan = maBanParsed,
                ThoiGianTao = DateTime.Now,
                ThoiGianCapNhat = DateTime.Now,
                TrangThai = TrangThai
            };

            // Nếu người dùng đã đăng nhập, lưu vào lịch sử
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name;
                var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDangNhap == userName);
                if (nguoiDung != null)
                {
                    donHang.MaNguoiDung = nguoiDung.MaNguoiDung;
                }
            }

            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            // TODO: xử lý giỏ hàng từ session nếu cần

            return RedirectToAction("Index", "Quanlydonhangs");
        }

        // GET: Quanlydonhangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null) return NotFound();
            ViewData["MaBan"] = new SelectList(_context.Bans, "MaBan", "MaBan", donHang.MaBan);
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", donHang.MaNguoiDung);
            ViewData["LoaiDonHang"] = new SelectList(new List<string> { "Mang đi", "Tại quán", "Đặt trước" });
            ViewData["TrangThai"] = new SelectList(new List<string> { "Đã thanh toán", "Chưa thanh toán", "Hủy" });
            
            return View(donHang);
        }

        // POST: Quanlydonhangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DonHang donHang)
        {
            if (id != donHang.MaDonHang) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    donHang.ThoiGianCapNhat = DateTime.Now;
                    _context.Update(donHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DonHangs.Any(e => e.MaDonHang == donHang.MaDonHang))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, gán lại ViewData
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "TenDangNhap", donHang.MaNguoiDung);
            ViewData["MaBan"] = new SelectList(_context.Bans, "MaBan", "TenBan", donHang.MaBan);
            ViewData["LoaiDonHang"] = new SelectList(new List<string> { "Mang đi", "Tại quán", "Đặt trước" }, donHang.LoaiDonHang);
            ViewData["TrangThai"] = new SelectList(new List<string> { "Chưa thanh toán", "Đã thanh toán", "Hủy" }, donHang.TrangThai);
            return View(donHang);
        }

        // GET: Quanlydonhangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var donHang = await _context.DonHangs
                .Include(d => d.MaNguoiDungNavigation)
                .Include(d => d.MaBanNavigation)
                .Include(d => d.ChiTietDonHangs).ThenInclude(c => c.MaSanPhamNavigation)
                .FirstOrDefaultAsync(m => m.MaDonHang == id);

            if (donHang == null) return NotFound();

            return View(donHang);
        }

        // POST: Quanlydonhangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .Include(d => d.HoaDon)
                .FirstOrDefaultAsync(m => m.MaDonHang == id);

            if (donHang == null)
            {
                return NotFound();
            }

            // Xoá các chi tiết đơn hàng
            if (donHang.ChiTietDonHangs != null && donHang.ChiTietDonHangs.Any())
            {
                _context.ChiTietDonHangs.RemoveRange(donHang.ChiTietDonHangs);
            }

            // Xoá hóa đơn nếu có
            if (donHang.HoaDon != null)
            {
                _context.HoaDons.Remove(donHang.HoaDon);
            }

            // Xoá đơn hàng
            _context.DonHangs.Remove(donHang);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool DonHangExists(int id)
        {
            return _context.DonHangs.Any(e => e.MaDonHang == id);
        }
    }
}
