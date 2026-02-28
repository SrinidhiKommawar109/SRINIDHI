using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // STEP 5 - Customer View Their Invoices
        [HttpGet("customer/invoices")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyInvoices()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var invoices = await _context.Invoices
                .Where(i => i.CustomerId == userId)
                .ToListAsync();

            return Ok(invoices);
        }
    }
}
