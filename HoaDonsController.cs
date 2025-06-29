using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanCafe.Models;
using Rotativa.AspNetCore;

namespace QuanLyQuanCafe.Controllers
{
    public class HoaDonsController : Controller
    {
        private readonly DbquanLyQuanCafeContext _context;

        public HoaDonsController(DbquanLyQuanCafeContext context)
        {
            _context = context;
        }

        // GET: HoaDons
        public async Task<IActionResult> Index()
        {
            var dbquanLyQuanCafeContext = _context.HoaDons.Include(h => h.MaDonHangNavigation);
            return View(await dbquanLyQuanCafeContext.ToListAsync());
        }

        // GET: HoaDons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var hoaDon = await _context.HoaDons
            .Include(h => h.MaDonHangNavigation)
                .ThenInclude(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.MaSanPhamNavigation)
            .Include(h => h.MaDonHangNavigation.MaNguoiDungNavigation)
            .FirstOrDefaultAsync(h => h.MaHoaDon == id);

            if (hoaDon == null) return NotFound();
            foreach (var item in hoaDon.MaDonHangNavigation.ChiTietDonHangs)
            {
                item.TenSanPham = item.MaSanPhamNavigation?.TenSanPham;
            }

            return View(hoaDon);
        }

        // GET: HoaDons/Create
        public IActionResult Create(int maDonHang)
        {
            var donHang = _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .FirstOrDefault(d => d.MaDonHang == maDonHang);

            if (donHang == null)
            {
                return NotFound();
            }

            // Tính tổng tiền từ chi tiết
            decimal tongTien = donHang.ChiTietDonHangs.Sum(ct => ct.SoLuong * ct.DonGia);

            var hoaDon = new HoaDon
            {
                MaDonHang = maDonHang,
                NgayThanhToan = DateTime.Now,
                TongTien = tongTien
            };
            ViewData["PhuongThucThanhToan"] = new SelectList(new[] { "Tiền mặt", "Chuyển khoản", "QR Code" });
            return View(hoaDon);
        }

        // POST: HoaDons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHoaDon,MaDonHang,NgayThanhToan,TongTien,PhuongThucThanhToan")] HoaDon hoaDon)
        {
            var donHang = await _context.DonHangs
        .Include(d => d.ChiTietDonHangs)
        .FirstOrDefaultAsync(d => d.MaDonHang == hoaDon.MaDonHang);

            if (donHang == null)
            {
                ModelState.AddModelError("", "Không tìm thấy đơn hàng.");
                ViewData["MaDonHang"] = new SelectList(_context.DonHangs, "MaDonHang", "MaDonHang", hoaDon.MaDonHang);
                return View(hoaDon);
            }

            // Tính tổng tiền
            decimal tongTien = donHang.ChiTietDonHangs.Sum(c => c.SoLuong * c.DonGia);

            // Gán các thuộc tính còn lại
            hoaDon.NgayThanhToan = DateTime.Now;
            hoaDon.TongTien = tongTien;

            // Cập nhật trạng thái đơn hàng sang "Đã thanh toán"
            donHang.TrangThai = "Đã thanh toán";

            if (ModelState.IsValid)
            {
                _context.Update(donHang); // cập nhật trạng thái đơn hàng
                _context.Add(hoaDon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaDonHang"] = new SelectList(_context.DonHangs, "MaDonHang", "MaDonHang", hoaDon.MaDonHang);
            return View(hoaDon);
        }*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HoaDon hoaDon)
        {
            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .FirstOrDefaultAsync(d => d.MaDonHang == hoaDon.MaDonHang);

            if (donHang == null)
            {
                ModelState.AddModelError("", "Không tìm thấy đơn hàng.");
                return View(hoaDon);
            }
            // Tính lại tổng tiền cho chính xác
            hoaDon.TongTien = donHang.ChiTietDonHangs.Sum(ct => ct.SoLuong * ct.DonGia);
            hoaDon.NgayThanhToan = DateTime.Now;

            // Cập nhật trạng thái đơn hàng
            donHang.TrangThai = "Đã thanh toán";

            if (ModelState.IsValid)
            {
                _context.HoaDons.Add(hoaDon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaDonHang"] = new SelectList(_context.DonHangs, "MaDonHang", "MaDonHang", hoaDon.MaDonHang);
            return View(hoaDon);
        }

        // GET: HoaDons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }
            ViewData["MaDonHang"] = new SelectList(_context.DonHangs, "MaDonHang", "MaDonHang", hoaDon.MaDonHang);
            ViewData["PhuongThucThanhToan"] = new SelectList(new[] { "Tiền mặt", "Chuyển khoản", "QR Code" });
            return View(hoaDon);
        }

        // POST: HoaDons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaHoaDon,MaDonHang,NgayThanhToan,TongTien,PhuongThucThanhToan")] HoaDon hoaDon)
        {
            if (id != hoaDon.MaHoaDon)
            {
                return NotFound();
            }

            /*if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonExists(hoaDon.MaHoaDon))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }*/
            if (ModelState.IsValid)
            {
                var hoaDonInDb = await _context.HoaDons.FindAsync(id);
                if (hoaDonInDb == null)
                {
                    return NotFound();
                }

                // Gán thủ công các giá trị cần cập nhật
                hoaDonInDb.NgayThanhToan = hoaDon.NgayThanhToan;
                hoaDonInDb.PhuongThucThanhToan = hoaDon.PhuongThucThanhToan;
                hoaDonInDb.TongTien = hoaDon.TongTien; // Nếu bạn cho phép chỉnh sửa tổng tiền

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["MaDonHang"] = new SelectList(_context.DonHangs, "MaDonHang", "MaDonHang", hoaDon.MaDonHang);
            ViewData["PhuongThucThanhToan"] = new SelectList(new[] { "Tiền mặt", "Chuyển khoản", "QR Code" });
            return View(hoaDon);
        }

        // GET: HoaDons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var hoaDon = await _context.HoaDons
        .Include(h => h.MaDonHangNavigation)
            .ThenInclude(d => d.ChiTietDonHangs)
            .ThenInclude(c => c.MaSanPhamNavigation)
        .Include(h => h.MaDonHangNavigation.MaNguoiDungNavigation)
        .FirstOrDefaultAsync(h => h.MaHoaDon == id);

            if (hoaDon == null) return NotFound();

            // Gán thủ công TenSanPham
            foreach (var item in hoaDon.MaDonHangNavigation.ChiTietDonHangs)
            {
                item.TenSanPham = item.MaSanPhamNavigation?.TenSanPham;
            }

            return View(hoaDon);
        }

        // POST: HoaDons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null)
            {
                // Cập nhật trạng thái đơn hàng về "Chưa thanh toán"
                var donHang = await _context.DonHangs.FindAsync(hoaDon.MaDonHang);
                if (donHang != null)
                {
                    donHang.TrangThai = "Chưa thanh toán";
                }

                _context.HoaDons.Remove(hoaDon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonExists(int id)
        {
            return _context.HoaDons.Any(e => e.MaHoaDon == id);
        }
        // Lịch sử theo khách hàng
        public async Task<IActionResult> LichSuKhachHang(int? maNguoiDung)
        {
            if (maNguoiDung == null)
            {
                return BadRequest("Thiếu mã người dùng.");
            }
            var hoaDons = _context.HoaDons
                .Include(h => h.MaDonHangNavigation)
                .ThenInclude(d => d.MaNguoiDungNavigation)
                .Where(h => h.MaDonHangNavigation.MaNguoiDung == maNguoiDung);

            return View(await hoaDons.ToListAsync());
        }

        // Lịch sử theo bàn
        public async Task<IActionResult> LichSuTheoBan(int? maBan)
        {
            var hoaDons = _context.HoaDons
                .Include(h => h.MaDonHangNavigation)
                .Where(h => h.MaDonHangNavigation.MaBan == maBan);

            return View(await hoaDons.ToListAsync());
        }
        public IActionResult XuatHoaDonPDF(int id)
        {
            var hoaDon = _context.HoaDons
                .Include(h => h.MaDonHangNavigation)
                .ThenInclude(d => d.ChiTietDonHangs)
                .FirstOrDefault(h => h.MaHoaDon == id);

            if (hoaDon == null) return NotFound();

            return new ViewAsPdf("HoaDonPDF", hoaDon)
            {
                FileName = $"HoaDon_{hoaDon.MaHoaDon}.pdf"
            };
        }
    }
}
