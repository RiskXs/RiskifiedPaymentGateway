using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RiskifiedPaymentGateway.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskifiedPaymentGateway.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargeStatusesController : ControllerBase
    {
        private readonly IChargingService _chargingService;

        public ChargeStatusesController(IChargingService chargingService)
        {
            _chargingService = chargingService;
        }
        [HttpGet]
        public async Task<IActionResult> GetChargeStatuses([FromHeader(Name ="merchant-identifier")] string merchantId)
        {
            var results = await _chargingService.GetChargeStatuses(merchantId);
            return Ok(results);
        }
    }
}
