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
    public class IndexModel : PageModel
    {
        private readonly SE1728_Group2_A2.Models.MyStoreContext _context;

        public IndexModel(SE1728_Group2_A2.Models.MyStoreContext context)
        {
            _context = context;
        }

        private bool IsUserAuthenticated()
        {
            var account = SE1728_Group2_A2.Utils.SessionHelper.SessionExtensions.GetObjectFromJson<Staff>(HttpContext.Session, "Staff");
            if(account != null)
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

        public IList<Staff> Staff { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string SearchByName { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {



            if (!IsUserAuthenticated())
            {
                return RedirectToLoginPage();
            }else
            {
                var staffQuery = from s in _context.Staffs
                                 select s;


                if (!string.IsNullOrEmpty(SearchByName))
                {
                    staffQuery = staffQuery.Where(s => s.Name.Contains(SearchByName));
                }

                Staff = await staffQuery.ToListAsync();
                return Page();
            }
            

        }
    }
}
