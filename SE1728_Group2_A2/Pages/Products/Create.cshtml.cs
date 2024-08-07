﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SE1728_Group2_A2.Models;
using SE1728_Group2_A2.Utils.SessionHelper;

namespace SE1728_Group2_A2.Pages.Products
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
            if (!isAdmin()) { return RedirectToPage("/Index"); }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!isAdmin()) { return RedirectToPage("/Index"); }
            if (Product == null)
            {
                return Page();
            }

            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        bool isAdmin()
        {
            Staff currentStaff = HttpContext.Session.GetObjectFromJson<Staff>("Staff");
            if (currentStaff == null || currentStaff.Role == 0)
            {
                return false;
            }
            return true;
        }
    }
}
