﻿using System;
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
using static NuGet.Packaging.PackagingConstants;

namespace SE1728_Group2_A2.Pages.Orders
{
    public class IndexModel : PageModel
    {
        Staff currentStaff = new Staff()
        {
            StaffId = 3,
            Name = "hieptv2",
            Password = "password",
            Role = 0
        };

        private readonly SE1728_Group2_A2.Models.MyStoreContext _context;
        public void CheckAccount()
        {
            if (currentStaff == null || currentStaff.Role != 0)
            {
                Response.Redirect("./Index");
            }
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

        public async Task OnGetAsync()
        {
            CheckAccount();

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
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CheckAccount();
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

        private void TransferMessageToPage(DateTime searchDateVal)
        {
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
