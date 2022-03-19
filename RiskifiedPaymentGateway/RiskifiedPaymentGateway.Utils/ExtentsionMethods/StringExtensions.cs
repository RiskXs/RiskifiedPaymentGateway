using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.Utils.ExtentsionMethods.String
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return str == null || str.Trim().Length == 0;
        }
    }
}
