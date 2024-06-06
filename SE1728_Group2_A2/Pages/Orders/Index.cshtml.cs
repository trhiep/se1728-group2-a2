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
using static NuGet.Packaging.PackagingConstants;

namespace SE1728_Group2_A2.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly SE1728_Group2_A2.Models.MyStoreContext _context;

        public IndexModel(SE1728_Group2_A2.Models.MyStoreContext context)
        {
            _context = context;
        }

        int staffId = 3; // CHANGE STAFF ID HERE AFTER IMPLEMENTED LOGIN


        [BindProperty]
        public string SearchDate { get; set; }

        public IList<Order> Order { get; set; } = default! ;

        public async Task OnGetAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            if (_context.Orders != null)
            {
                Order = await _context.Orders
                    .Where(o => o.StaffId == staffId).Where(o => o.OrderDate >= today && o.OrderDate < tomorrow)
                    .Include(o => o.Staff)
                    .Include(od => od.OrderDetails)
                    .OrderByDescending(o => o.OrderId)
                    .ToListAsync();

                // Calculate order total to display in page
                if (Order != null)
                {
                    String totalOrder = OrdersHelper.GetFormatedCurrency(Order.SelectMany(order => order.OrderDetails)
                    .Sum(orderDetail => orderDetail.Quantity * orderDetail.UnitPrice).ToString());
                    ViewData["OrderTotalInDay"] = "Total order today: " + totalOrder;
                    ViewData["PageHeading"] = "Orders of staff " + staffId + " today";
                }
            }

        }

        public async Task OnPostAsync()
        {
            DateTime searchDateVal = DateTime.Today;
            if (DateTime.TryParseExact(SearchDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                searchDateVal = parsedDate;
            }
            var tomorrow = searchDateVal.AddDays(1);
            if (_context.Orders != null)
            {
                Order = await _context.Orders
                    .Where(o => o.StaffId == staffId).Where(o => o.OrderDate >= searchDateVal && o.OrderDate < tomorrow)
                    .Include(o => o.Staff)
                    .Include(od => od.OrderDetails)
                    .OrderByDescending(o => o.OrderId)
                    .ToListAsync();

                // Calculate order total to display in page
                if (Order != null)
                {
                    String totalOrder = OrdersHelper.GetFormatedCurrency(Order.SelectMany(order => order.OrderDetails)
                    .Sum(orderDetail => orderDetail.Quantity * orderDetail.UnitPrice).ToString());
                    ViewData["PageHeading"] = "Orders of staff " + staffId + " on " + OrdersHelper.GetFormatedDate(searchDateVal);
                    ViewData["OrderTotalInDay"] = "Total order on " + OrdersHelper.GetFormatedDate(searchDateVal) + ": " + totalOrder;
                }
            }

        }
    }
}
