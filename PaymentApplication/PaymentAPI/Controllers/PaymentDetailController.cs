using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentAPI.Models;

namespace PaymentAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly PaymentDetailContext _context;
        private readonly ILogger _logger;

        public PaymentController(PaymentDetailContext paymentDetailContext, ILogger<PaymentController> logger)
		{
			_context = paymentDetailContext;
			_logger = logger;
		}


		[HttpGet]
		public async Task<ActionResult<IEnumerable<PaymentDetail>>> GetPaymentDetails()
		{
			_logger.LogInformation($"About page visited at {DateTime.UtcNow.ToLongTimeString()}");
			return await _context.PaymentDetails.ToListAsync();
		}

		[HttpGet("{id}", Name ="GetPaymentDetail")]
		[ProducesResponseType(StatusCodes.Status200OK, Type =typeof(PaymentDetail))]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<PaymentDetail>> GetPaymentDetailById(int id)
		{
			_logger.LogInformation($"About page visited at {DateTime.UtcNow.ToLongTimeString()}");
			
			var payment = await _context.PaymentDetails.FindAsync(id);

			return payment == null ? NotFound() : Ok(payment);
			
		}

		[HttpPost]
		public async Task<ActionResult<PaymentDetail>> CreatePaymentDetail(PaymentDetail paymentDetail)
		{
			_context.PaymentDetails.Add(paymentDetail);
			await _context.SaveChangesAsync();

			return CreatedAtRoute("GetPaymentDetail", new {id = paymentDetail.PaymentDetailId}, paymentDetail);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<PaymentDetail>> UpdatePaymentDetail(int id, PaymentDetail paymentDetail)
		{
			if (id != paymentDetail.PaymentDetailId)
				return BadRequest();

			_context.Entry(paymentDetail).State = EntityState.Modified;

			try
			{
                await _context.SaveChangesAsync();
            }
			catch(DbUpdateConcurrencyException)
			{
				if (!PaymentDetailExists(id))
					return NotFound();
				else
					throw new ArgumentNullException(nameof(DbUpdateConcurrencyException));
			}
			

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePaymentDetail(int id)
		{
			var payment = await _context.PaymentDetails.FindAsync(id);

			if (payment == null)
				return NotFound();

			_context.PaymentDetails.Remove(payment);
			await _context.SaveChangesAsync();

			return NoContent();
		}



		private bool PaymentDetailExists(int id)
		{
			return _context.PaymentDetails.Any(e => e.PaymentDetailId == id);
		}
	}


}

