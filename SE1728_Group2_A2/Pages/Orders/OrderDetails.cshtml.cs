using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1728_Group2_A2.Models;
using SE1728_Group2_A2.Utils.SessionHelper;

namespace SE1728_Group2_A2.Pages.Orders
{
    public class OrderDetailsModel : PageModel
    {
        private readonly SE1728_Group2_A2.Models.MyStoreContext _context;
        public OrderDetailsModel(SE1728_Group2_A2.Models.MyStoreContext context)
        {
            _context = context;
        }

        public IList<OrderDetail> OrderDetail { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? orderId)
        {
            Staff currentStaff = HttpContext.Session.GetObjectFromJson<Staff>("Staff");
            if (currentStaff == null || currentStaff.Role != 0)
            {
                return RedirectToPage("/Error/401");
            }
            else
            {
                var order = await _context.Orders.FirstOrDefaultAsync(m => m.OrderId == orderId);
                if (order == null || order.StaffId != currentStaff.StaffId)
                {
                    return RedirectToPage("./Index");
                }

                OrderDetail = await _context.OrderDetails.Where(od => od.OrderId == orderId)
                        .Include(p => p.Product)
                        .ToListAsync();

                return Page();
            }
        }
    }
}
