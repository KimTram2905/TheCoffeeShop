using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheCoffeeShop.Models;

namespace TheCoffeeShop.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly DbquanLyQuanCafeContext _context;

        public SanPhamController(DbquanLyQuanCafeContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.SanPhams.Include(sp => sp.MaDanhMucNavigation).ToListAsync();
            return View(danhSach);
        }

        public IActionResult Create()
        {
            ViewBag.DanhMucList = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                _context.SanPhams.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.DanhMucList = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null) return NotFound();
            ViewBag.DanhMucList = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                _context.Update(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.DanhMucList = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var sanPham = await _context.SanPhams.Include(sp => sp.MaDanhMucNavigation).FirstOrDefaultAsync(sp => sp.MaSanPham == id);
            if (sanPham == null) return NotFound();
            return View(sanPham);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}