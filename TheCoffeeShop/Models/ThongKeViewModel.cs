// Models/ThongKeViewModel.cs
using System;
using System.Collections.Generic;

namespace TheCoffeeShop.Models
{
    public class ThongKeViewModel
    {
        public int TongKhachHang { get; set; }
        public int TongDonHang { get; set; }
        public decimal TongDoanhThu { get; set; }
        public int TongSanPham { get; set; }

        public int DonHangChuaThanhToan { get; set; }
        public int DonHangDaThanhToan { get; set; }
        public int DonHangHuy { get; set; }

        public int DatChoHomNay { get; set; }
        public int DatChoDaXacNhan { get; set; }
        public int DatChoChuaXacNhan { get; set; }

        public List<SanPhamBanChayViewModel> SanPhamBanChay { get; set; } = new();
        public List<DoanhThuDanhMucViewModel> DoanhThuTheoDanhMuc { get; set; } = new();
        public List<DonHangTheoLoaiViewModel> DonHangTheoLoai { get; set; } = new();
        public List<DoanhThuNgayViewModel> DoanhThu7NgayGanNhat { get; set; } = new();
    }

    public class SanPhamBanChayViewModel
    {
        public string TenSanPham { get; set; }
        public int SoLuongBan { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class DoanhThuDanhMucViewModel
    {
        public string TenDanhMuc { get; set; }
        public decimal DoanhThu { get; set; }
        public int SoLuong { get; set; }
    }

    public class DonHangTheoLoaiViewModel
    {
        public string LoaiDonHang { get; set; }
        public int SoLuong { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class DoanhThuNgayViewModel
    {
        public string Ngay { get; set; }
        public decimal DoanhThu { get; set; }
    }
}