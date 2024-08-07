﻿using System;
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
using SE1728_Group2_A2.Utils.SessionHelper;

namespace SE1728_Group2_A2.Pages.Orders
{
    public class EditModel : PageModel
    {
        string editingOrderSessionKey = "editingOrderSessionKey";

        private bool IsValidAccount(Staff staff)
        {
            if (staff == null || staff.Role != 0)
            {
                return false;
            }
            return true;
        }

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
            Staff currentStaff = HttpContext.Session.GetObjectFromJson<Staff>("Staff");
            if (IsValidAccount(currentStaff))
            {
                // Check is received orderid valid
                if (id == null || _context.Orders == null)
                {
                    return RedirectToPage("./Index");
                }

                // get order
                var order = await _context.Orders.FirstOrDefaultAsync(m => m.OrderId == id);
                if (order == null || order.StaffId != currentStaff.StaffId)
                {
                    return RedirectToPage("./Index");
                }
                Order = order;

                // Save editing order to session
                HttpContext.Session.SetObjectAsJson("editingOrderSessionKey", order);

                // get order details of selected order
                List<OrderDetail> OrderDetails = await _context.OrderDetails.Where(od => od.OrderId == id)
                        .Include(p => p.Product)
                        .ToListAsync();

                // settings for json parse
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                // check is this order contains order details
                if (OrderDetails.Any())
                {
                    OrderDetailsDTO = new List<OrderDetailsDTO>();
                    // Map order details to custom DTO
                    foreach (var item in OrderDetails)
                    {
                        OrderDetailsDTO newOrderDetails = new OrderDetailsDTO
                        {
                            productId = item.ProductId,
                            productName = item.Product.ProductName,
                            unitPrice = item.UnitPrice,
                            quantity = item.Quantity,
                            totalPrice = (long )item.UnitPrice * item.Quantity
                        };
                        OrderDetailsDTO.Add(newOrderDetails);
                    }

                    // parse list to json
                    OrderDetailsJson = JsonConvert.SerializeObject(OrderDetailsDTO, settings);
                }

                // get products json
                ViewData["Products"] = JsonConvert.SerializeObject(_context.Products.ToList(), settings);
                return Page();

            }
            else
            {
                return RedirectToPage("/Index");
            }
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Staff currentStaff = HttpContext.Session.GetObjectFromJson<Staff>("Staff");
            if(IsValidAccount(currentStaff))
            {
                // get editing orderId
                int id = Order.OrderId;

                // check is valid order id
                var editingOrder = HttpContext.Session.GetObjectFromJson<Order>("editingOrderSessionKey");
                if (editingOrder == null || editingOrder.OrderId != id)
                {
                    return RedirectToPage("./Index");
                }

                try
                {
                    // check is user received an json string from font-end
                    if (OrderDetailsJson != null)
                    {
                        // Remove all old order details
                        List<OrderDetail> oldOrderDetails = await _context.OrderDetails
                            .Where(x => x.OrderId == id)
                            .ToListAsync();
                        foreach (var item in oldOrderDetails)
                        {
                            _context.OrderDetails.Remove(item);
                            await _context.SaveChangesAsync();
                        }

                        // parse received json to list
                        List<OrderDetail> orderDetails = JsonConvert.DeserializeObject<List<OrderDetail>>(OrderDetailsJson);
                        if (orderDetails != null)
                        {
                            // add all order details to list
                            foreach (var item in orderDetails)
                            {
                                OrderDetail newOrderDetail = new OrderDetail()
                                {
                                    OrderId = id,
                                    ProductId = item.ProductId,
                                    Quantity = item.Quantity,
                                    UnitPrice = item.UnitPrice
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
            else
            {
                return RedirectToPage("/Index");
            }
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
        public long totalPrice { get; set; }
    }
}
