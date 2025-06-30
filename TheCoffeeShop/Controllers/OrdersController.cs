using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuanLyQuanCafe.Models;
using System.Security.Claims;
using TheCoffeeShop.Models;

namespace QuanLyQuanCafe.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DbquanLyQuanCafeContext _context;
        //private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(DbquanLyQuanCafeContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(string search, int? maDanhMuc)
        {
            var danhSachSanPham = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .ToListAsync();

            if (maDanhMuc.HasValue)
            {
                danhSachSanPham = danhSachSanPham.Where(s => s.MaDanhMuc == maDanhMuc).ToList();
            }

            ViewBag.DanhSachSanPham = danhSachSanPham;
            ViewBag.DanhMucs = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc");
            ViewBag.SelectedDanhMuc = maDanhMuc;

            var cart = HttpContext.Session.GetObject<List<ChiTietDonHang>>("GioHang") ?? new List<ChiTietDonHang>();
            ViewBag.GioHang = cart;

            return View(danhSachSanPham);
        }


        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .FirstOrDefaultAsync(m => m.MaSanPham == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: Orders/Create
        public IActionResult Create(int? id)
        {
            DonHang? donHang = null;

            if (id.HasValue)
            {
                donHang = _context.DonHangs
                    .Include(d => d.ChiTietDonHangs)
                    .FirstOrDefault(d => d.MaDonHang == id);

                if (donHang != null)
                {
                    //HttpContext.Session.SetInt32("DonHangTam", donHang.MaDonHang);
                    //HttpContext.Session.SetObject("GioHang", donHang.ChiTietDonHangs.ToList());
                    var chiTietList = donHang.ChiTietDonHangs.ToList();

                    foreach (var ct in chiTietList)
                    {
                        var sp = _context.SanPhams.FirstOrDefault(p => p.MaSanPham == ct.MaSanPham);
                        ct.TenSanPham = sp?.TenSanPham;
                    }

                    HttpContext.Session.SetObject("GioHang", chiTietList);
                }
            }
            else if (HttpContext.Session.GetInt32("DonHangTam") is int maDon)
            {
                donHang = _context.DonHangs
                    .Include(d => d.ChiTietDonHangs)
                    .FirstOrDefault(d => d.MaDonHang == maDon);

                if (donHang != null)
                {
                    HttpContext.Session.SetObject("GioHang", donHang.ChiTietDonHangs.ToList());
                }
            }

            ViewBag.Ban = _context.Bans.Where(b => b.TrangThai == "Trống").ToList();
            ViewBag.GioHang = HttpContext.Session.GetObject<List<ChiTietDonHang>>("GioHang") ?? new List<ChiTietDonHang>();
            ViewBag.NguoiDung = new SelectList(_context.NguoiDungs, "MaNguoiDung", "HoTen");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSanPham,TenSanPham,MaDanhMuc,DonGia,MoTa,HinhAnh")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            ViewData["TenSanPham"] = new SelectList(_context.SanPhams, "TenSanPham", "TenSanPham", sanPham.TenSanPham);
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }
        
        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaSanPham,TenSanPham,MaDanhMuc,DonGia,MoTa,HinhAnh")] SanPham sanPham)
        {
            if (id != sanPham.MaSanPham)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSanPham))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "MaDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }

        private bool SanPhamExists(int maSanPham)
        {
            return _context.SanPhams.Any(e => e.MaSanPham == maSanPham);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .FirstOrDefaultAsync(m => m.MaSanPham == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: Orders/Delete/5
        [HttpPost]
        public IActionResult AddToOrder(int maSanPham, int soLuong, string? ghiChu)
        {
            var product = _context.SanPhams.FirstOrDefault(p => p.MaSanPham == maSanPham);//Tìm sản phẩm trong database theo mã sản phẩm (maSanPham).
            if (product == null) return NotFound();

            var cart = HttpContext.Session.GetObject<List<ChiTietDonHang>>("GioHang") ?? new List<ChiTietDonHang>();

            var existing = cart.FirstOrDefault(c => c.MaSanPham == maSanPham);
            if (existing != null)
            {
                existing.SoLuong += soLuong;
                if (!string.IsNullOrEmpty(ghiChu))
                {
                    existing.GhiChu = ghiChu;
                }
            }
            else
            {
                cart.Add(new ChiTietDonHang
                {
                    MaSanPham = product.MaSanPham,
                    SoLuong = soLuong,
                    DonGia = product.DonGia,
                    TenSanPham = product.TenSanPham,
                    MaDanhMuc = product.MaDanhMuc,
                    GhiChu = ghiChu
                });
            }

            HttpContext.Session.SetObject("GioHang", cart);
            return RedirectToAction("Create"); // hoặc quay về Index
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder([Bind("LoaiDonHang,TrangThai,MaBan")] DonHang donHang, string PhuongThucThanhToan, int? MaNguoiDung)
        {
            var gioHang = HttpContext.Session.GetObject<List<ChiTietDonHang>>("GioHang") ?? new List<ChiTietDonHang>();
            foreach (var item in gioHang)
            {
                // Chỉ gán tên sản phẩm để hiển thị, không gán navigation
                var sp = _context.SanPhams.AsNoTracking().FirstOrDefault(s => s.MaSanPham == item.MaSanPham);
                item.TenSanPham = sp?.TenSanPham;

                // Tránh lỗi EF tracking khi gán navigation properties
                item.MaSanPhamNavigation = null;
                item.MaDonHangNavigation = null;
                item.MaChiTiet = 0;
            }

            if (!gioHang.Any())
            {
                ModelState.AddModelError("", "Không có sản phẩm trong đơn hàng.");
                ViewBag.Ban = _context.Bans.Where(b => b.TrangThai == "Trống").ToList();
                ViewBag.GioHang = gioHang;
                ViewBag.NguoiDung = new SelectList(_context.NguoiDungs.ToList(), "MaNguoiDung", "HoTen");//
                return View("Create", donHang);
            }

            donHang.ThoiGianTao = DateTime.Now;
            donHang.ThoiGianCapNhat = DateTime.Now;
            donHang.TrangThai = Request.Form.ContainsKey("btnThanhToan") ? "Đã thanh toán" : "Chưa thanh toán";

            if (MaNguoiDung.HasValue)
            {
                donHang.MaNguoiDung = MaNguoiDung.Value; // chọn từ dropdown
            }
            else if (User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdStr, out int userId))
                {
                    donHang.MaNguoiDung = null; // lấy người dùng hiện tại
                }
            }

            donHang.ChiTietDonHangs = gioHang;
            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            // Nếu đã thanh toán → tạo hóa đơn
            if (donHang.TrangThai == "Đã thanh toán" && !string.IsNullOrEmpty(PhuongThucThanhToan) && PhuongThucThanhToan != "Chưa thanh toán")
            {
                _context.HoaDons.Add(new HoaDon
                {
                    MaDonHang = donHang.MaDonHang,
                    NgayThanhToan = DateTime.Now,
                    PhuongThucThanhToan = PhuongThucThanhToan
                });
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("GioHang"); // Xóa giỏ hàng
            }
            else
            {
                // Đơn chưa thanh toán → lưu mã đơn vào session để chỉnh sau
                HttpContext.Session.SetInt32("DonHangTam", donHang.MaDonHang);
            }
            return RedirectToAction("Index", "QuanLyDonHangs");
        }
    }
}
