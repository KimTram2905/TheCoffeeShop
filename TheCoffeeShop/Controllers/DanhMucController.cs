using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheCoffeeShop.Models;

public class DanhMucController : Controller
{
    private readonly DbquanLyQuanCafeContext _context;

    public DanhMucController(DbquanLyQuanCafeContext context)
    {
        _context = context;
    }

    // Danh sách + tìm kiếm + lọc
    public async Task<IActionResult> Index(string search)
    {
        var danhMucs = _context.DanhMucs.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            danhMucs = danhMucs.Where(dm =>( dm.TenDanhMuc != null && dm.TenDanhMuc.Contains(search)) || (dm.MoTa != null && dm.MoTa.Contains(search)));
        }

        return View(await danhMucs.ToListAsync());
    }

    // GET: Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DanhMuc model)
    {
        if (ModelState.IsValid)
        {
            _context.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Edit
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _context.DanhMucs.FindAsync(id);
        if (model == null) return NotFound();
        return View(model);
    }

    // POST: Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DanhMuc model)
    {
        if (ModelState.IsValid)
        {
            _context.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Delete
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _context.DanhMucs.FindAsync(id);
        if (model == null) return NotFound();
        return View(model);
    }

    // POST: Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var model = await _context.DanhMucs.FindAsync(id);
        if (model != null)
        {
            _context.DanhMucs.Remove(model);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
