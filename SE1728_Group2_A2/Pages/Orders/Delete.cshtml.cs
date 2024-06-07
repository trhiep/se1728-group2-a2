using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SE1728_Group2_A2.Models;

namespace SE1728_Group2_A2.Pages.Orders
{
    public class DeleteModel : PageModel
    {
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
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);

            if (order != null)
            {
                Order = order;
                _context.Orders.Remove(Order);
                await _context.SaveChangesAsync();
            }
            DateTime parsedDate = DateTime.ParseExact(searchedDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            TempData["SearchedDate"] = parsedDate.ToString("dd/MM/yyyy"); ;
            return RedirectToPage("./Index");
        }
    }
}
