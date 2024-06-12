using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1728_Group2_A2.Models;
using SE1728_Group2_A2.Utils.SessionHelper;

namespace SE1728_Group2_A2.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly SE1728_Group2_A2.Models.MyStoreContext _context;

        public DetailsModel(SE1728_Group2_A2.Models.MyStoreContext context)
        {
            _context = context;
        }

      public Product Product { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!isAdmin()) { return RedirectToPage("/Index"); }
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            else 
            {
                Product = product;
            }
            return Page();
        }

        bool isAdmin()
        {
            Staff currentStaff = HttpContext.Session.GetObjectFromJson<Staff>("Staff");
            if (currentStaff == null || currentStaff.Role != 0)
            {
                return false;
            }
            return true;
        }
    }
}
