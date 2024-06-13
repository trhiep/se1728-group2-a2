using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SE1728_Group2_A2.Models;
using MySessionExtensions = SE1728_Group2_A2.Utils.SessionHelper;

namespace SE1728_Group2_A2.Pages.Staffs
{
    public class EditModel : PageModel
    {
        private readonly SE1728_Group2_A2.Models.MyStoreContext _context;

        public EditModel(SE1728_Group2_A2.Models.MyStoreContext context)
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
                Staff = staff;
                return Page();
            }
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


        public async Task<IActionResult> OnPostAsync()
        {

            if (!IsUserAuthenticated())
            {
                return RedirectToLoginPage();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                _context.Attach(Staff).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                    var staff = MySessionExtensions.SessionExtensions.GetObjectFromJson<Staff>(HttpContext.Session, "Staff");
                    staff.Name = Staff.Name;
                    MySessionExtensions.SessionExtensions.SetObjectAsJson(HttpContext.Session, "Staff", staff);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(Staff.StaffId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToPage("./Details", new { id = Staff.StaffId });
            }

        }

        private bool StaffExists(int id)
        {
            return (_context.Staffs?.Any(e => e.StaffId == id)).GetValueOrDefault();
        }
    }
}
