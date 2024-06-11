using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1728_Group2_A2.Models;

namespace SE1728_Group2_A2.Pages.Reports
{
    public class ReportDetailsModel : PageModel
    {
        private readonly MyStoreContext _context;

        public ReportDetailsModel(MyStoreContext context)
        {
            _context = context;
        }

        public int OrderId { get; set; }
        public List<OrderDetailInfo> OrderDetails { get; set; }

        public void OnGet(int orderId, List<OrderDetailInfo> orderDetails)
        {
            var query = (from od in _context.OrderDetails
                         join p in _context.Products on od.ProductId equals p.ProductId
                         join c in _context.Categories on p.CategoryId equals c.CategoryId
                         where od.OrderId == orderId
                         select new OrderDetailInfo
                         {
                             OrderDetailId = od.OrderDetailId,
                             OrderId = od.OrderId,
                             ProductId = od.ProductId,
                             ProductName = p.ProductName,
                             CategoryName = c.CategoryName,
                             Quantity = od.Quantity,
                             UnitPrice = od.UnitPrice
                         }).ToList();

            // Initialize ViewData if not initialized
            ViewData["OrderId"] = orderId;
            Console.WriteLine(query);
            ViewData["OrderDetails"] = query;
                      
        }
    }
}
