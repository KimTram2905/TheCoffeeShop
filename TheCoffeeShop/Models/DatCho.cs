using System;
using System.Collections.Generic;

namespace TheCoffeeShop.Models;

public partial class DatCho
{
    public int MaDatCho { get; set; }

    public int MaNguoiDung { get; set; }

    public int MaBan { get; set; }

    public DateTime ThoiGianDat { get; set; }

    public DateTime ThoiGianDen { get; set; }

    public int SoNguoi { get; set; }

    public string? GhiChu { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime ThoiGianCapNhat { get; set; }

    public virtual Ban MaBanNavigation { get; set; } = null!;

    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
