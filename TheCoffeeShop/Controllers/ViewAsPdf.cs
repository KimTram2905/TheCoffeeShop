using Microsoft.AspNetCore.Mvc;
using TheCoffeeShop.Models;

namespace TheCoffeeShop.Controllers
{
    internal class ViewAsPdf : IActionResult
    {
        public ViewAsPdf(string v, HoaDon hoaDon)
        {
        }

        public string FileName { get; set; }

        public Task ExecuteResultAsync(ActionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
