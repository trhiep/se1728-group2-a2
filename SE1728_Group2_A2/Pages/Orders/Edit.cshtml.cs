using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Configuration;
using SE1728_Group2_A2.Models;

namespace SE1728_Group2_A2.Pages.Orders
{
    public class EditModel : PageModel
    {
        private readonly SE1728_Group2_A2.Models.MyStoreContext _context;

        public EditModel(SE1728_Group2_A2.Models.MyStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Order Order { get; set; } = default!;
        [BindProperty]
        public string? OrderDetailsJson { get; set; }
        public List<OrderDetailsDTO>? OrderDetailsDTO { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order =  await _context.Orders.FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            Order = order;
            List<OrderDetail> OrderDetails = await _context.OrderDetails.Where(od => od.OrderId == id)
                    .Include(p => p.Product)
                    .ToListAsync();

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            if (OrderDetails.Any())
            {
                OrderDetailsDTO = new List<OrderDetailsDTO>();
                foreach (var item in OrderDetails)
                {
                    OrderDetailsDTO newOrderDetails = new OrderDetailsDTO
                    {
                        productId = item.ProductId,
                        productName = item.Product.ProductName,
                        unitPrice = item.Product.UnitPrice,
                        quantity = item.Quantity,
                        totalPrice = item.Product.UnitPrice * item.Quantity
                    };
                    OrderDetailsDTO.Add(newOrderDetails);
                }
                OrderDetailsJson = JsonConvert.SerializeObject(OrderDetailsDTO, settings);
                Console.WriteLine(OrderDetailsJson);
            }
            ViewData["Products"] = JsonConvert.SerializeObject(_context.Products.ToList(), settings);
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            int id = Order.OrderId;
            try
            {
                if (OrderDetailsJson != null)
                {
                    List<OrderDetail> oldOrderDetails = await _context.OrderDetails
                        .Where(x => x.OrderId == id)
                        .ToListAsync();
                    foreach (var item in oldOrderDetails)
                    {
                        _context.OrderDetails.Remove(item);
                        await _context.SaveChangesAsync();
                    }

                    List<OrderDetail> orderDetails = JsonConvert.DeserializeObject<List<OrderDetail>>(OrderDetailsJson);
                    if (orderDetails != null)
                    {
                        foreach (var item in orderDetails)
                        {
                            OrderDetail newOrderDetail = new OrderDetail()
                            {
                                OrderId = id,
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                UnitPrice = item.UnitPrice * item.Quantity
                            };
                            _context.OrderDetails.Add(newOrderDetail);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(Order.OrderId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }

    public class OrderDetailsDTO
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public int unitPrice { get; set; }
        public int quantity { get; set; }
        public int totalPrice { get; set; }
    }
}
