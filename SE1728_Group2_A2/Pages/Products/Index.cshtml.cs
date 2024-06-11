using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1728_Group2_A2.Models;

namespace SE1728_Group2_A2.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly SE1728_Group2_A2.Models.MyStoreContext _context;

        public IndexModel(SE1728_Group2_A2.Models.MyStoreContext context)
        {
            _context = context;
        }
        public IList<Product> Product { get;set; } = default!;
        [BindProperty]
        public String SearchName { get; set; }
        [BindProperty]
        public String SearchPrice { get; set; }
        public async Task OnGetAsync()
        {
            if (!isAdmin()) { return; }
            if (_context.Products != null)
            {
                Product = await _context.Products
                .Include(p => p.Category).ToListAsync();
            }
        }
        public async Task OnPostAsync()
        {
            if (!isAdmin()) { return; }
            Product = await _context.Products
                .Include(p =>p.Category)
                .Where(p => p.ProductName.Contains(SearchName ?? ""))
                .ToListAsync();
            if (SearchPrice!=null)
            {
                Product = Product.Where(p => p.UnitPrice==int.Parse(SearchPrice)).ToList();
            }
        }
        bool isAdmin()
        {
            return true;
        }
    }
}
