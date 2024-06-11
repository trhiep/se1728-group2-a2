using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SE1728_Group2_A2.Models;
using SE1728_Group2_A2.Utils.ModelHelper;
using SE1728_Group2_A2.Utils.SessionHelper;
using MySessionHelper = SE1728_Group2_A2.Utils.SessionHelper;

namespace SE1728_Group2_A2.Pages.Orders
{
    public class IndexModel : PageModel
    {

        private readonly SE1728_Group2_A2.Models.MyStoreContext _context;
        public bool IsValidAccount(Staff staff)
        {
            if (staff == null || staff.Role != 0)
            {
                return false;
            }
            return true;
        }

        public IndexModel(SE1728_Group2_A2.Models.MyStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string SearchDate { get; set; }

        [TempData]
        public string SearchedDate { get; set; }

        public string DisplayDate { get; set; }

        public IList<Order> Order { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            Staff currentStaff = HttpContext.Session.GetObjectFromJson<Staff>("Staff");
            if (IsValidAccount(currentStaff))
            {
                DateTime searchDateVal = OrdersHelper.GetFormatedDateTimeFromString(SearchedDate);
                if (searchDateVal == default(DateTime))
                {
                    searchDateVal = DateTime.Today;
                }
                var tomorrow = searchDateVal.AddDays(1);

                if (_context.Orders != null)
                {
                    Order = await _context.Orders
                        .Where(o => o.StaffId == currentStaff.StaffId).Where(o => o.OrderDate >= searchDateVal && o.OrderDate < tomorrow)
                        .Include(o => o.Staff)
                        .Include(od => od.OrderDetails)
                        .OrderByDescending(o => o.OrderId)
                        .ToListAsync();
                    TransferMessageToPage(searchDateVal);
                }
                return Page();
            } else
            {
                return RedirectToPage("/Index");
            }

            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Staff currentStaff = HttpContext.Session.GetObjectFromJson<Staff>("Staff");
            if (IsValidAccount(currentStaff))
            {
                DateTime searchDateVal = OrdersHelper.GetFormatedDateTimeFromString(SearchDate);
                var tomorrow = searchDateVal.AddDays(1);
                if (_context.Orders != null)
                {
                    Order = await _context.Orders
                        .Where(o => o.StaffId == currentStaff.StaffId).Where(o => o.OrderDate >= searchDateVal && o.OrderDate < tomorrow)
                        .Include(o => o.Staff)
                        .Include(od => od.OrderDetails)
                        .OrderByDescending(o => o.OrderId)
                        .ToListAsync();
                    TransferMessageToPage(searchDateVal);
                }
                return Page();
            } 
            else
            {
                return RedirectToPage("/Index");
            }
                
        }

        private void TransferMessageToPage(DateTime searchDateVal)
        {
            Staff currentStaff = HttpContext.Session.GetObjectFromJson<Staff>("Staff");
            //  Message of page heading
            if (searchDateVal == DateTime.Today)
            {
                ViewData["PageHeading"] = "Orders of staff " + currentStaff.StaffId + " today";
            }
            else
            {
                ViewData["PageHeading"] = "Orders of staff " + currentStaff.StaffId + " on " + OrdersHelper.GetFormatedDate(searchDateVal);
            }

            // Message of total order on selected date
            if (Order.Count > 0)
            {
                string totalOrder = OrdersHelper.GetFormatedCurrency(Order.SelectMany(order => order.OrderDetails)
                    .Sum(orderDetail => orderDetail.UnitPrice).ToString());
                ViewData["OrderTotalInDay"] = "Total order: " + totalOrder;
            }
            else
            {
                ViewData["OrderTotalInDay"] = "The staff has no orders on this date.";
            }

            // Transfer selected date to page
            DisplayDate = searchDateVal.ToString("yyyy/MM/dd");
        }

    }
}
