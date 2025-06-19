using System;
using System.Collections.Generic;

namespace TheCoffeeShop.Models;

public partial class SanPham
{
    public int MaSanPham { get; set; }

    public string TenSanPham { get; set; } = null!;

    public int MaDanhMuc { get; set; }

    public decimal DonGia { get; set; }

    public string? MoTa { get; set; }

    public string? HinhAnh { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual DanhMuc MaDanhMucNavigation { get; set; } = null!;
}
