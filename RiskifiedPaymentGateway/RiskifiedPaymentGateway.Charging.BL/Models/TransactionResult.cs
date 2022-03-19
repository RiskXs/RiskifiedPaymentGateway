using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.Charging.BL.Models
{
    public class TransactionResult
    {
        public bool IsSuccess { get; set; }
        public string ReasonForFailure { get; set; }
    }
}
