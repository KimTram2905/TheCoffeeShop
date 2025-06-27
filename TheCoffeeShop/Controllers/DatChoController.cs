using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheCoffeeShop.Models;

namespace TheCoffeeShop.Controllers
{
    public class DatChoController : Controller
    {
        private readonly DbquanLyQuanCafeContext _context;

        public DatChoController(DbquanLyQuanCafeContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách đặt chỗ
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.DatChos
                .Include(d => d.MaBanNavigation)
                .Include(d => d.MaNguoiDungNavigation)
                .ToListAsync();

            return View(danhSach);
        }

        // GET: Tạo mới đặt chỗ
        public IActionResult Create()
        {
            ViewBag.BanList = new SelectList(_context.Bans, "MaBan", "TenBan");
            ViewBag.NguoiDungList = new SelectList(_context.NguoiDungs, "MaNguoiDung", "HoTen");
            return View();
        }

        // POST: Tạo mới đặt chỗ
        [HttpPost]
        public async Task<IActionResult> Create(DatCho datCho)
        {
            if (ModelState.IsValid)
            {
                datCho.TrangThai = "Chờ xác nhận";
                datCho.ThoiGianCapNhat = DateTime.Now;

                _context.Add(datCho);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BanList = new SelectList(_context.Bans, "MaBan", "TenBan", datCho.MaBan);
            ViewBag.NguoiDungList = new SelectList(_context.NguoiDungs, "MaNguoiDung", "HoTen", datCho.MaNguoiDung);
            return View(datCho);
        }

        // GET: Sửa đặt chỗ
        public async Task<IActionResult> Edit(int id)
        {
            var datCho = await _context.DatChos.FindAsync(id);
            if (datCho == null) return NotFound();

            ViewBag.BanList = new SelectList(_context.Bans, "MaBan", "TenBan", datCho.MaBan);
            ViewBag.NguoiDungList = new SelectList(_context.NguoiDungs, "MaNguoiDung", "HoTen", datCho.MaNguoiDung);
            return View(datCho);
        }

        // POST: Sửa đặt chỗ
        [HttpPost]
        public async Task<IActionResult> Edit(DatCho datCho)
        {
            if (ModelState.IsValid)
            {
                datCho.ThoiGianCapNhat = DateTime.Now;
                _context.Update(datCho);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BanList = new SelectList(_context.Bans, "MaBan", "TenBan", datCho.MaBan);
            ViewBag.NguoiDungList = new SelectList(_context.NguoiDungs, "MaNguoiDung", "HoTen", datCho.MaNguoiDung);
            return View(datCho);
        }

        // GET: Xóa đặt chỗ
        public async Task<IActionResult> Delete(int id)
        {
            var datCho = await _context.DatChos
                .Include(d => d.MaBanNavigation)
                .Include(d => d.MaNguoiDungNavigation)
                .FirstOrDefaultAsync(d => d.MaDatCho == id);

            if (datCho == null) return NotFound();

            return View(datCho);
        }

        // POST: Xác nhận xóa
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var datCho = await _context.DatChos.FindAsync(id);
            if (datCho != null)
            {
                _context.DatChos.Remove(datCho);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Xác nhận đặt chỗ
        public async Task<IActionResult> Confirm(int id)
        {
            var datCho = await _context.DatChos.FindAsync(id);
            if (datCho != null)
            {
                datCho.TrangThai = "Đã xác nhận";
                datCho.ThoiGianCapNhat = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Hủy đặt chỗ
        public async Task<IActionResult> Cancel(int id)
        {
            var datCho = await _context.DatChos.FindAsync(id);
            if (datCho != null)
            {
                datCho.TrangThai = "Đã hủy";
                datCho.ThoiGianCapNhat = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
