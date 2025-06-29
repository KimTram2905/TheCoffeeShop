using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheCoffeeShop.Models;

public partial class ChiTietDonHang
{
    public int MaChiTiet { get; set; }

    public int MaDonHang { get; set; }

    public int MaSanPham { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal? ThanhTien { get; set; }

    public string? GhiChu { get; set; }

    public virtual DonHang MaDonHangNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;

    [NotMapped]
    public string? TenSanPham { get; set; }

    [NotMapped]
    public int? MaDanhMuc { get; set; }

}
