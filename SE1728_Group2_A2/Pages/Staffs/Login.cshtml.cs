using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SE1728_Group2_A2.Models;
using MySessionExtensions = SE1728_Group2_A2.Utils.SessionHelper;

namespace SE1728_Group2_A2.Pages.Staffs
{
    public class LoginModel : PageModel
    {
        private readonly MyStoreContext _context;


        public LoginModel(MyStoreContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            string name, pass;
            var conf = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            name = conf.GetSection("AdminAccount").GetSection("Username").Value!;
            pass = conf.GetSection("AdminAccount").GetSection("Password").Value!;
            Staff st = Authentication(name, pass);
            if (st == null)
            {
                Staff tempStaff = new Staff() { Name = name, Password = pass, Role = 1 };
                _context.Staffs.Add(tempStaff);
                _context.SaveChanges();
            }
            return Page();
        }

        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Clear();

            return Page();
        }

        [BindProperty]
        public Staff Staff { get; set; } = default!;

        private Staff Authentication(string username, string password)
        {
            Staff s = _context.Staffs.Where(s => s.Name == username && s.Password == password).FirstOrDefault();
            return s;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                string name, pass;
                int role;
                var conf = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
                Staff staff;
                name = conf.GetSection("AdminAccount").GetSection("Username").Value!;
                pass = conf.GetSection("AdminAccount").GetSection("Password").Value!;
                role = Int32.Parse(conf.GetSection("AdminAccount").GetSection("Role").Value!);
                if (name == Staff.Name && pass == Staff.Password)
                {
                    staff = new Staff()
                    {
                        StaffId = 0,
                        Name = name,
                        Password = pass,
                        Role = role
                    };
                }
                else
                {
                    staff = Authentication(Staff.Name, Staff.Password);
                }
                if (staff == null) return Page();
                MySessionExtensions.SessionExtensions.SetObjectAsJson(HttpContext.Session, "Staff", staff);
                return RedirectToPage("/Index");
            }
            return Page();
        }

    }
}
