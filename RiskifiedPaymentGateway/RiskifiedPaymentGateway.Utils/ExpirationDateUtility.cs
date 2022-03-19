using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.Utils
{
    public class ExpirationDateUtility
    {
        public static bool TryToConvertCVVDate(string cvvDateStr, out DateTime cvvDate)
        {
            string dayMonthYear = $"1/{cvvDateStr}";
            return DateTime.TryParse(dayMonthYear, out cvvDate);
        }
    }
}
