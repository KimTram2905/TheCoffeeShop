using System;
using System.Collections.Generic;

namespace TheCoffeeShop.Models;

public partial class Ban
{
    public int MaBan { get; set; }

    public string TenBan { get; set; } = null!;

    public string? TrangThai { get; set; }

    public int SucChua { get; set; }

    public virtual ICollection<DatCho> DatChos { get; set; } = new List<DatCho>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
