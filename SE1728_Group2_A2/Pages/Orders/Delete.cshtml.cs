using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1728_Group2_A2.Models;

namespace SE1728_Group2_A2.Pages.Orders
{
    public class DeleteModel : PageModel
    {
        Staff currentStaff = new Staff()
        {
            StaffId = 3,
            Name = "hieptv2",
            Password = "password",
            Role = 0
        };
        public void CheckAccount()
        {
            
        }
        private readonly SE1728_Group2_A2.Models.MyStoreContext _context;

        public DeleteModel(SE1728_Group2_A2.Models.MyStoreContext context)
        {
            _context = context;
        }

        [TempData]
        public string SearchedDate { get; set; }

        [BindProperty]
        public Order Order { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id, string searchedDate)
        {
            if (currentStaff == null || currentStaff.Role != 0)
            {
                Response.Redirect("/Index");
            }
            else
            {
                if (id == null || _context.Orders == null)
                {
                    return RedirectToPage("./Index");
                }

                var order = await _context.Orders.FirstOrDefaultAsync(m => m.OrderId == id);
                if (order == null || order.StaffId != currentStaff.StaffId)
                {
                    return RedirectToPage("./Index");
                }

                if (order != null)
                {
                    Order = order;
                    _context.Orders.Remove(Order);
                    await _context.SaveChangesAsync();
                }
                DateTime parsedDate = DateTime.ParseExact(searchedDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                TempData["SearchedDate"] = parsedDate.ToString("dd/MM/yyyy"); ;
            }
            return RedirectToPage("./Index");
        }
    }
}
