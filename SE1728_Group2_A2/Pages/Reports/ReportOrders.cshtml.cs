using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SE1728_Group2_A2.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SE1728_Group2_A2.Pages.Reports
{
    public class ReportOrdersModel : PageModel
    {
        private readonly MyStoreContext _context;

        public ReportOrdersModel(MyStoreContext context)
        {
            _context = context;
        }

        [BindProperty, Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [BindProperty, Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        public void OnGet()
        {
            EndDate = DateTime.Now;
            StartDate = DateTime.Now.AddMonths(-1);
            LoadAllOrder(StartDate, EndDate);
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            LoadAllOrder(StartDate, EndDate);
            return Page();
        }

        private void LoadAllOrder(DateTime startDate, DateTime endDate)
        {
            var account = 0; // Placeholder for actual account logic

            if (account == 0) // Admin
            {
                LoadAllOrderForAdmin(startDate, endDate);
            }
            else
            {
                LoadAllOrderStaff(account, startDate, endDate);
            }
        }

        private void LoadAllOrderStaff(int staffID, DateTime startDate, DateTime endDate)
        {
            DateTime endDateAdjusted = endDate.AddDays(1).AddSeconds(-1);
            var orders = _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDateAdjusted && o.StaffId == staffID)
                .Join(_context.Staffs,
                      o => o.StaffId,
                      s => s.StaffId,
                      (o, s) => new OrderInfo
                      {
                          OrderId = o.OrderId,
                          OrderDate = o.OrderDate,
                          StaffId = o.StaffId,
                          StaffName = s.Name,
                          RoleName = s.Role == 1 ? "Staff" : "Admin"
                      })
                .ToList();

            ViewData["Orders"] = orders;
        }

        private void LoadAllOrderForAdmin(DateTime startDate, DateTime endDate)
        {
            DateTime endDateAdjusted = endDate.AddDays(1).AddSeconds(-1);
            var orders = _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDateAdjusted)
                .Join(_context.Staffs,
                      o => o.StaffId,
                      s => s.StaffId,
                      (o, s) => new OrderInfo
                      {
                          OrderId = o.OrderId,
                          OrderDate = o.OrderDate,
                          StaffId = o.StaffId,
                          StaffName = s.Name,
                          RoleName = s.Role == 1 ? "Staff" : "Admin"
                      })
                .ToList();

            ViewData["Orders"] = orders;
        }

        public IActionResult OnGetOrderDetails(int orderId)
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

            return RedirectToPage("ReportDetails", new { orderId, orderDetails = query });
        }
    }
}
