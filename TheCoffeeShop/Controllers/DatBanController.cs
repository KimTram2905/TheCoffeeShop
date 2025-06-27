using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheCoffeeShop.Models;

namespace TheCoffeeShop.Controllers
{
    public class DatBanController : Controller
    {
        private readonly DbquanLyQuanCafeContext _context;

        public DatBanController(DbquanLyQuanCafeContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.Bans.ToListAsync();
            return View(danhSach);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ban = await _context.Bans.FindAsync(id);
            if (ban == null) return NotFound();
            return View(ban);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Ban ban)
        {
            if (ModelState.IsValid)
            {
                _context.Update(ban);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ban);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var ban = await _context.Bans.FindAsync(id);
            if (ban == null) return NotFound();
            return View(ban);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ban = await _context.Bans.FindAsync(id);
            if (ban != null)
            {
                _context.Bans.Remove(ban);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}