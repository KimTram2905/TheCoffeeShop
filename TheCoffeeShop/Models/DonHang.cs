using System;
using System.Collections.Generic;

namespace TheCoffeeShop.Models;

public partial class DonHang
{
    public int MaDonHang { get; set; }

    public int? MaNguoiDung { get; set; }

    public int? MaBan { get; set; }

    public DateTime ThoiGianTao { get; set; }

    public DateTime ThoiGianCapNhat { get; set; }

    public string LoaiDonHang { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual HoaDon? HoaDon { get; set; }

    public virtual Ban? MaBanNavigation { get; set; }

    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
}
