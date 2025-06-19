using System;
using System.Collections.Generic;

namespace TheCoffeeShop.Models;

public partial class HoaDon
{
    public int MaHoaDon { get; set; }

    public int MaDonHang { get; set; }

    public DateTime NgayThanhToan { get; set; }

    public decimal TongTien { get; set; }

    public string PhuongThucThanhToan { get; set; } = null!;

    public virtual DonHang MaDonHangNavigation { get; set; } = null!;
}
