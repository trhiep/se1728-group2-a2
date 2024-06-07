using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1728_Group2_A2.Models;

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
            OrderDetail = await _context.OrderDetails.Where(od => od.OrderId == orderId)
                    .Include(p => p.Product)
                    .ToListAsync();
            return Page();
        }
    }
}
