using SE1728_Group2_A2.Models;
using System.Text;

namespace SE1728_Group2_A2.Utils.ModelHelper
{
    public class OrdersHelper
    {
        public static string CalculateOrderTotal(Order order)
        {
            int total = 0;
            foreach (var orderDetail in order.OrderDetails)
            {
                total += orderDetail.Quantity * orderDetail.UnitPrice;
            }
            return total.ToString();
        }

        public static string GetFormatedCurrency(string amount)
        {
            StringBuilder formattedAmount = new StringBuilder();
            int endPoint = amount.IndexOf(".");
            if (endPoint < 0)
            {
                endPoint = amount.Length;
            }
            int count = 0;
            for (int i = endPoint - 1; i >= 0; i--)
            {
                formattedAmount.Insert(0, amount[i]);
                count++;

                if (count % 3 == 0 && i > 0)
                {
                    formattedAmount.Insert(0, ",");
                }
            }

            var conf = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            string currencyUnit = conf.GetSection("CurrencyUnit").Value;
            if (endPoint == amount.Length)
            {
                return formattedAmount.ToString() + " " + currencyUnit;
            }
            else
            {
                return formattedAmount.ToString() + amount.Substring(endPoint) + " " + currencyUnit;
            }
        }

        public static string GetFormatedDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public static string GetFormatedDate(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }
    }
}
