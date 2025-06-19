using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheCoffeeShop.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace TheCoffeeShop.Controllers
{
    public class NguoiDungController : Controller
    {
        private readonly DbquanLyQuanCafeContext _context;

        public NguoiDungController(DbquanLyQuanCafeContext context)
        {
            _context = context;
        }

        // Đăng nhập
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string tenDangNhap, string matKhau)
        {
            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => (u.TenDangNhap == tenDangNhap || u.Email == tenDangNhap) && u.MatKhau == matKhau);

            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }

            // Tạo claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.MaNguoiDung.ToString()),
                new Claim(ClaimTypes.Name, user.HoTen),
                new Claim(ClaimTypes.Role, user.VaiTro),
                new Claim("Avatar", user.Avatar ?? "/images/default-avatar.jpg")
            };

            // Tạo identity
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Tạo principal
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            // Đăng nhập
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        // Đăng ký
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(NguoiDung model)
        {
            try
            {
                // Thiết lập giá trị mặc định
                model.VaiTro = "Khách hàng";
                model.Avatar = "/images/default-avatar.jpg";

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Kiểm tra trùng tên đăng nhập
                if (await _context.NguoiDungs.AnyAsync(u => u.TenDangNhap == model.TenDangNhap))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã được sử dụng");
                    return View(model);
                }

                // Kiểm tra trùng email
                if (await _context.NguoiDungs.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng");
                    return View(model);
                }

                // Kiểm tra trùng số điện thoại
                if (await _context.NguoiDungs.AnyAsync(u => u.SoDienThoai == model.SoDienThoai))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại đã được sử dụng");
                    return View(model);
                }

                // Thêm người dùng mới
                _context.NguoiDungs.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error: {ex.InnerException?.Message}");
                ModelState.AddModelError("", "Lỗi khi lưu dữ liệu. Vui lòng thử lại.");
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi. Vui lòng thử lại.");
                return View(model);
            }
        }

        // Đăng xuất
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Đổi mật khẩu
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu mới và xác nhận không khớp.";
                return View();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.NguoiDungs.FindAsync(userId);

            if (user.MatKhau != currentPassword)
            {
                ViewBag.Error = "Mật khẩu hiện tại không đúng.";
                return View();
            }

            user.MatKhau = newPassword;
            await _context.SaveChangesAsync();

            ViewBag.Success = "Đổi mật khẩu thành công.";
            return View();
        }

        // Hồ sơ cá nhân
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.NguoiDungs.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(NguoiDung model, IFormFile avatarFile)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.NguoiDungs.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.HoTen = model.HoTen;
            user.SoDienThoai = model.SoDienThoai;
            user.Email = model.Email;
            user.DiaChi = model.DiaChi;

            if (avatarFile != null)
            {
                var fileName = Path.GetFileName(avatarFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }
                user.Avatar = "/images/" + fileName;
            }

            await _context.SaveChangesAsync();

            // Cập nhật claims nếu avatar thay đổi
            if (avatarFile != null)
            {
                await UpdateUserClaims(user);
            }

            TempData["Success"] = "Cập nhật hồ sơ thành công.";
            return RedirectToAction("Profile");
        }

        private async Task UpdateUserClaims(NguoiDung user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.MaNguoiDung.ToString()),
                new Claim(ClaimTypes.Name, user.HoTen),
                new Claim(ClaimTypes.Role, user.VaiTro),
                new Claim("Avatar", user.Avatar ?? "/images/default-avatar.jpg")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

        // Quên mật khẩu
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Error = "Email không tồn tại.";
                return View();
            }

            // Tạo mật khẩu tạm
            string tempPassword = GenerateTempPassword(8); // Tạo mật khẩu tạm dài 8 ký tự
            user.MatKhau = tempPassword; // Cập nhật mật khẩu tạm vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            // Hiển thị mật khẩu tạm (lưu ý: trong thực tế nên gửi qua email)
            ViewBag.Success = $"Mật khẩu tạm của bạn là: {tempPassword}. Vui lòng đăng nhập và đổi mật khẩu ngay.";
            return View();
        }

        // Phương thức tạo mật khẩu ngẫu nhiên
        private string GenerateTempPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Lịch sử đơn hàng
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var orders = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.MaSanPhamNavigation)
                .Where(d => d.MaNguoiDung == userId)
                .ToListAsync();

            return View(orders);
        }

        // Quản lý người dùng (Admin)
        [HttpGet]
        [Authorize(Roles = "Quản lý")]
        public async Task<IActionResult> ManageUsers(string vaiTro, string search)
        {
            var users = _context.NguoiDungs.AsQueryable();

            if (!string.IsNullOrEmpty(vaiTro))
                users = users.Where(u => u.VaiTro == vaiTro);

            if (!string.IsNullOrEmpty(search))
                users = users.Where(u => u.HoTen.Contains(search) || u.Email.Contains(search) ||
                                        u.SoDienThoai.Contains(search) || u.TenDangNhap.Contains(search));

            return View(await users.ToListAsync());
        }

        // Xóa người dùng (Admin)
        [HttpGet]
        [Authorize(Roles = "Quản lý")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Quản lý")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user != null)
            {
                // Kiểm tra không cho xóa chính mình
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (user.MaNguoiDung == currentUserId)
                {
                    TempData["Error"] = "Bạn không thể xóa chính mình!";
                    return RedirectToAction("ManageUsers");
                }

                _context.NguoiDungs.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã xóa người dùng {user.HoTen} thành công.";
            }
            return RedirectToAction("ManageUsers");
        }

        // Thêm người dùng (Admin)
        [HttpGet]
        [Authorize(Roles = "Quản lý")]
        public IActionResult AddUser()
        {
            ViewBag.Roles = new List<string> { "Quản lý", "Nhân viên", "Khách hàng" };
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Quản lý")]
        public async Task<IActionResult> AddUser(NguoiDung model)
        {
            ViewBag.Roles = new List<string> { "Quản lý", "Nhân viên", "Khách hàng" };

            if (ModelState.IsValid)
            {
                if (await _context.NguoiDungs.AnyAsync(u => u.TenDangNhap == model.TenDangNhap))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                if (await _context.NguoiDungs.AnyAsync(u => u.SoDienThoai == model.SoDienThoai))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại đã được sử dụng.");
                    return View(model);
                }

                model.Avatar = "/images/default-avatar.jpg";
                _context.NguoiDungs.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm người dùng thành công.";
                return RedirectToAction("ManageUsers");
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
