using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1728_Group2_A2.Models;

namespace SE1728_Group2_A2.Pages.Staffs
{
    public class DeleteModel : PageModel
    {
        private readonly SE1728_Group2_A2.Models.MyStoreContext _context;

        public DeleteModel(SE1728_Group2_A2.Models.MyStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Staff Staff { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLoginPage();
            }
            else
            {
                if (id == null || _context.Staffs == null)
                {
                    return NotFound();
                }

                var staff = await _context.Staffs.FirstOrDefaultAsync(m => m.StaffId == id);

                if (staff == null)
                {
                    return NotFound();
                }
                else
                {
                    Staff = staff;
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToLoginPage();
            }
            else
            {
                if (id == null || _context.Staffs == null)
                {
                    return NotFound();
                }
                var staff = await _context.Staffs.FindAsync(id);

                if (staff != null)
                {
                    Staff = staff;
                    _context.Staffs.Remove(Staff);
                    await _context.SaveChangesAsync();
                }

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
