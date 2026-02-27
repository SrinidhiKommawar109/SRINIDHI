using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Domain.Entities;
using Domain.Enums;
using Application.DTOs;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolicyRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PolicyRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // 1️⃣ CUSTOMER CREATES POLICY REQUEST
        // =====================================================
        [HttpPost("create")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateRequest(CreatePolicyRequestDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request data.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("User ID not found in token.");

            int userId = int.Parse(userIdClaim.Value);

            var plan = await _context.PropertyPlans
                .FirstOrDefaultAsync(p => p.Id == dto.PlanId);

            if (plan == null)
                return BadRequest("Invalid Plan ID.");

            var request = new PolicyRequest
            {
                PlanId = dto.PlanId,
                CustomerId = userId,
                Status = PolicyRequestStatus.PendingAdmin
            };

            _context.PolicyRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok("Policy request created successfully.");
        }

        // =====================================================
        // 2️⃣ ADMIN - VIEW PENDING REQUESTS
        // =====================================================
        [HttpGet("admin/pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var requests = await _context.PolicyRequests
                .Include(r => r.Plan)
                .Include(r => r.Customer)
                .Where(r => r.Status == PolicyRequestStatus.PendingAdmin)
                .ToListAsync();

            return Ok(requests);
        }

        // =====================================================
        // 3️⃣ ADMIN - ASSIGN AGENT
        // =====================================================
        [HttpPut("{id}/assign-agent/{agentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignAgent(int id, int agentId)
        {
            var request = await _context.PolicyRequests.FindAsync(id);
            if (request == null)
                return NotFound("Request not found.");

            var agentExists = await _context.Users
     .AnyAsync(u => u.Id == agentId && u.Role == UserRole.Agent);

            if (!agentExists)
                return BadRequest("Invalid Agent ID.");

            request.AgentId = agentId;
            request.Status = PolicyRequestStatus.AgentAssigned;

            await _context.SaveChangesAsync();

            return Ok("Agent assigned successfully.");
        }

        // =====================================================
        // 4️⃣ AGENT - SEND FORM TO CUSTOMER
        // =====================================================
        [HttpPut("{id}/send-form")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> SendFormToCustomer(int id)
        {
            var request = await _context.PolicyRequests.FindAsync(id);
            if (request == null)
                return NotFound("Request not found.");

            if (request.Status != PolicyRequestStatus.AgentAssigned)
                return BadRequest("Agent not assigned yet.");

            request.Status = PolicyRequestStatus.FormSent;

            await _context.SaveChangesAsync();

            return Ok("Form sent to customer.");
        }

        // =====================================================
        // 5️⃣ CUSTOMER SUBMITS PROPERTY DETAILS
        // =====================================================
        [HttpPut("{id}/submit-form")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> SubmitPropertyDetails(int id, SubmitPropertyDto dto)
        {
            var request = await _context.PolicyRequests.FindAsync(id);

            if (request == null)
                return NotFound("Request not found.");

            if (request.Status != PolicyRequestStatus.FormSent)
                return BadRequest("Form not sent yet.");

            request.PropertyAddress = dto.PropertyAddress;
            request.PropertyValue = dto.PropertyValue;
            request.PropertyAge = dto.PropertyAge;

            request.Status = PolicyRequestStatus.FormSubmitted;

            await _context.SaveChangesAsync();

            return Ok("Property details submitted successfully.");
        }

        // =====================================================
        // 6️⃣ AGENT CALCULATES RISK + PREMIUM + INSTALLMENTS
        // =====================================================
        [HttpPut("{id}/calculate-risk")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> CalculateRisk(int id)
        {
            var request = await _context.PolicyRequests
                .Include(r => r.Plan)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                return NotFound("Request not found.");

            if (request.Status != PolicyRequestStatus.FormSubmitted)
                return BadRequest("Form not submitted yet.");

            // Risk Calculation
            decimal riskScore = 0;

            if (request.PropertyAge.HasValue && request.PropertyAge > 10)
                riskScore += 30;

            if (request.PropertyValue.HasValue && request.PropertyValue > 250000)
                riskScore += 40;

            request.RiskScore = riskScore;

            // Premium Calculation
            decimal basePremium = request.Plan.BasePremium;
            decimal riskMultiplier = 500;

            decimal finalPremium = basePremium + (riskScore * riskMultiplier);

            request.PremiumAmount = finalPremium;
            request.TotalPremium = finalPremium;

            // Installment Calculation
            request.Frequency = request.Plan.Frequency;

            int installmentCount = request.Plan.Frequency switch
            {
                PremiumFrequency.Quarterly => 4,
                PremiumFrequency.HalfYearly => 2,
                PremiumFrequency.Yearly => 1,
                _ => 1
            };

            request.InstallmentCount = installmentCount;
            request.InstallmentAmount = finalPremium / installmentCount;

            // Agent Commission
            request.AgentCommissionAmount = request.Plan.AgentCommission;

            request.Status = PolicyRequestStatus.RiskCalculated;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                request.Id,
                request.PlanId,
                PlanName = request.Plan.PlanName,
                request.RiskScore,
                request.TotalPremium,
                request.Frequency,
                request.InstallmentCount,
                request.InstallmentAmount,
                request.AgentCommissionAmount,
                request.Status
            });
        }

        // =====================================================
        // 7️⃣ CUSTOMER CONFIRMS PURCHASE
        // =====================================================
        [HttpPut("{id}/buy")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> BuyPolicy(int id)
        {
            var request = await _context.PolicyRequests.FindAsync(id);

            if (request == null)
                return NotFound("Request not found.");

            if (request.Status != PolicyRequestStatus.RiskCalculated)
                return BadRequest("Risk not calculated yet.");

            request.Status = PolicyRequestStatus.CustomerConfirmed;

            await _context.SaveChangesAsync();

            return Ok("Customer confirmed. Waiting for admin approval.");
        }

        // =====================================================
        // 8️⃣ ADMIN FINAL APPROVAL
        // =====================================================
        [HttpPut("{id}/admin-approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminApprove(int id)
        {
            var request = await _context.PolicyRequests.FindAsync(id);

            if (request == null)
                return NotFound("Request not found.");

            if (request.Status != PolicyRequestStatus.CustomerConfirmed)
                return BadRequest("Customer has not confirmed yet.");

            request.Status = PolicyRequestStatus.PolicyApproved;

            await _context.SaveChangesAsync();

            return Ok("Policy officially approved.");
        }
    }
}