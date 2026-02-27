using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Domain.Entities;
using Domain.Enums;
using Application.DTOs;
using System.Security.Claims;
using ClaimEntity = Domain.Entities.Claim;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClaimsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================================
        // 1️⃣ Customer files a claim
        // ================================
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> FileClaim(CreateClaimDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID not found in token.");

            var userId = int.Parse(userIdClaim.Value);

            var policy = await _context.PolicyRequests
                .FirstOrDefaultAsync(p => p.Id == dto.PolicyRequestId);

            if (policy == null || policy.Status != PolicyRequestStatus.PolicyApproved)
                return BadRequest("Invalid or unapproved policy.");

            var claim = new ClaimEntity
            {
                PolicyRequestId = dto.PolicyRequestId,
                PropertyAddress = dto.PropertyAddress,
                PropertyValue = dto.PropertyValue,
                PropertyAge = dto.PropertyAge,
                ClaimAmount = policy.PremiumAmount,
                Status = ClaimStatus.Pending,
                Remarks = ""
            };

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            return Ok("Claim request sent successfully.");
        }

        // ================================
        // 2️⃣ Claims Officer views pending claims
        // ================================
        [HttpGet("pending")]
        [Authorize(Roles = "ClaimsOfficer")]
        public async Task<IActionResult> GetPendingClaims()
        {
            var claims = await _context.Claims
                .Include(c => c.PolicyRequest)
                .Where(c => c.Status == ClaimStatus.Pending)
                .ToListAsync();

            return Ok(claims);
        }

        // ================================
        // 3️⃣ Claims Officer verifies claim
        // ================================
        [HttpPut("{id}/verify")]
        [Authorize(Roles = "ClaimsOfficer")]
        public async Task<IActionResult> VerifyClaim(int id, [FromBody] VerifyClaimDto dto)
        {
            var claim = await _context.Set<ClaimEntity>().FindAsync(id);
            if (claim == null)
                return NotFound();

            if (claim.Status != ClaimStatus.Pending)
                return BadRequest("Claim already processed.");

            if (dto.IsAccepted)
            {
                claim.Status = ClaimStatus.Approved;
                claim.Remarks = dto.Remarks ?? "Claim accepted";
                await _context.SaveChangesAsync();
                return Ok("Insurance has been claimed ✅");
            }
            else
            {
                claim.Status = ClaimStatus.Rejected;
                claim.Remarks = dto.Remarks ?? "Claim rejected";
                await _context.SaveChangesAsync();
                return Ok("Claim rejected ❌");
            }
        }
    }
}