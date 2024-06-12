using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SE1728_Group2_A2.Models;

namespace SE1728_Group2_A2.Pages.Staffs
{
    public class CreateModel : PageModel
    {
        private readonly SE1728_Group2_A2.Models.MyStoreContext _context;

        public CreateModel(SE1728_Group2_A2.Models.MyStoreContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Staff Staff { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLoginPage();
            }
            else
            {
                if (!ModelState.IsValid || _context.Staffs == null || Staff == null)
                {
                    return Page();
                }

                _context.Staffs.Add(Staff);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }
        }


        private bool IsUserAuthenticated()
        {
            var account = SE1728_Group2_A2.Utils.SessionHelper.SessionExtensions.GetObjectFromJson<Staff>(HttpContext.Session, "Staff");
            if (account != null)
            {
                if (account.Role == 1)
                {
                    return true;
                }
            }

            return false;
        }

        private IActionResult RedirectToLoginPage()
        {
            return RedirectToPage("/Staffs/Login");
        }
    }
}
