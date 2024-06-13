using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1728_Group2_A2.Models;
using MySession = SE1728_Group2_A2.Utils.SessionHelper;

namespace SE1728_Group2_A2.Pages.Staffs
{
    public class DetailsModel : PageModel
    {
        private readonly SE1728_Group2_A2.Models.MyStoreContext _context;

        public DetailsModel(SE1728_Group2_A2.Models.MyStoreContext context)
        {
            _context = context;
        }

        public Staff Staff { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            if (!IsUserAuthenticated())
            {
                return RedirectToLoginPage();
            }

            if (id == null || _context.Staffs == null)
            {
                return NotFound();
            }

            if (id == 0)
            {
                var staff = MySession.SessionExtensions.GetObjectFromJson<Staff>(HttpContext.Session, "Staff");
                Staff = staff;
            }
            else
            {
                var staff = await _context.Staffs.FirstOrDefaultAsync(m => m.StaffId == id);
                if (staff == null)
                {
                    return NotFound();
                }
                else
                {
                    Staff = staff;
                }
            }

            return Page();
        }
        
        private bool IsUserAuthenticated()
        {
            var account = SE1728_Group2_A2.Utils.SessionHelper.SessionExtensions.GetObjectFromJson<Staff>(HttpContext.Session, "Staff");
            if (account != null)
            {
                return true;
            }

            return false;
        }

        private IActionResult RedirectToLoginPage()
        {
            return RedirectToPage("/Staffs/Login");
        }
            
    }
}
