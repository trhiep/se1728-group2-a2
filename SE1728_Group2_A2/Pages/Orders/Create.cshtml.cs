using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SE1728_Group2_A2.Models;
using SE1728_Group2_A2.Utils.SessionHelper;

namespace SE1728_Group2_A2.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly SE1728_Group2_A2.Models.MyStoreContext _context;

        public CreateModel(SE1728_Group2_A2.Models.MyStoreContext context)
        {
            _context = context;
        }

        private bool IsValidAccount(Staff staff)
        {
            if (staff == null || staff.Role != 0)
            {
                return false;
            }
            return true;
        }

        public IActionResult OnGet()
        {
            Staff currentStaff = HttpContext.Session.GetObjectFromJson<Staff>("Staff");
            if (IsValidAccount(currentStaff))
            {
                ViewData["Products"] = JsonConvert.SerializeObject(_context.Products.ToList());
                return Page();
            }
            else
            {
                return RedirectToPage("/Index");
            }
            
            
        }

        [BindProperty]
        public Order Order { get; set; } = default!;

        [BindProperty]
        public string? OrderDetailsJson { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            Staff currentStaff = HttpContext.Session.GetObjectFromJson<Staff>("Staff");
            if (IsValidAccount(currentStaff))
            {
                if (!string.IsNullOrEmpty(OrderDetailsJson))
                {
                    OrderDetails = JsonConvert.DeserializeObject<List<OrderDetail>>(OrderDetailsJson);
                    // Create a new order
                    Order newOrder = new Order()
                    {
                        OrderDate = DateTime.Now,
                        StaffId = currentStaff.StaffId
                    };

                    // Add order to DB
                    _context.Orders.Add(newOrder);
                    await _context.SaveChangesAsync();

                    // Add all order details in list to DB
                    if (OrderDetails != null)
                    {
                        foreach (var item in OrderDetails)
                        {
                            OrderDetail newOrderDetail = new OrderDetail()
                            {
                                OrderId = newOrder.OrderId,
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                UnitPrice = item.UnitPrice * item.Quantity
                            };
                            _context.OrderDetails.Add(newOrderDetail);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                return RedirectToPage("./Index");
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
    }
}
