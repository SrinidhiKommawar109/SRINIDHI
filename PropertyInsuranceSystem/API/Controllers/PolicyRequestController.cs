using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Domain.Entities;
using Domain.Enums;
using Application.DTOs;
using System.Security.Claims;

namespace API.Controllers;

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
    // 1️⃣ Customer creates policy request
    // =====================================================
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateRequest(CreatePolicyRequestDto dto)
    {
        if (dto == null)
            return BadRequest("Invalid request data.");

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("User ID not found in token.");

        int userId = int.Parse(userIdClaim.Value);

        var planExists = await _context.PropertyPlans
            .AnyAsync(p => p.Id == dto.PlanId);

        if (!planExists)
            return BadRequest("Invalid Plan ID.");

        var request = new PolicyRequest
        {
            PlanId = dto.PlanId,
            CustomerId = userId,
            Status = PolicyRequestStatus.PendingAdmin
            
        };

        _context.PolicyRequests.Add(request);
        await _context.SaveChangesAsync();

        return Ok("Request sent successfully.");
    }

    // =====================================================
    // 2️⃣ Admin views pending requests
    // =====================================================
    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var requests = await _context.PolicyRequests
            .Include(r => r.Customer)
            .Include(r => r.Plan)
            .Where(r => r.Status == PolicyRequestStatus.PendingAdmin)
            .Select(r => new PolicyRequestResponseDto
            {
                Id = r.Id,
                PlanId = r.PlanId,
                PlanName = r.Plan.PlanName,
                CustomerId = r.CustomerId,
                CustomerName = r.Customer.FullName,
                Status = r.Status,
                AgentId = r.AgentId,
                AgentName = r.Agent != null ? r.Agent.FullName : null,
                PropertyAddress = r.PropertyAddress,
                PropertyValue = r.PropertyValue,
                PropertyAge = r.PropertyAge,
                RiskScore = r.RiskScore,
                PremiumAmount = r.PremiumAmount,
                AgentCommissionAmount = r.AgentCommissionAmount
            })
            .ToListAsync();

        return Ok(requests);
    }

    [HttpGet("my-requests")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyRequests()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var requests = await _context.PolicyRequests
            .Include(r => r.Plan)
            .Where(r => r.CustomerId == userId)
            .Select(r => new PolicyRequestResponseDto
            {
                Id = r.Id,
                PlanId = r.PlanId,
                PlanName = r.Plan.PlanName,
                CustomerId = r.CustomerId,
                Status = r.Status,
                PropertyAddress = r.PropertyAddress,
                PropertyValue = r.PropertyValue,
                PropertyAge = r.PropertyAge,
                RiskScore = r.RiskScore,
                PremiumAmount = r.PremiumAmount,
                // Commission hidden from customer
                AgentCommissionAmount = null 
            })
            .ToListAsync();

        return Ok(requests);
    }

    [HttpGet("agent-requests")]
    [Authorize(Roles = "Agent")]
    public async Task<IActionResult> GetAgentRequests()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var requests = await _context.PolicyRequests
            .Include(r => r.Customer)
            .Include(r => r.Plan)
            .Where(r => r.AgentId == userId)
            .Select(r => new PolicyRequestResponseDto
            {
                Id = r.Id,
                PlanId = r.PlanId,
                PlanName = r.Plan.PlanName,
                CustomerId = r.CustomerId,
                CustomerName = r.Customer.FullName,
                Status = r.Status,
                AgentId = r.AgentId,
                PropertyAddress = r.PropertyAddress,
                PropertyValue = r.PropertyValue,
                PropertyAge = r.PropertyAge,
                RiskScore = r.RiskScore,
                PremiumAmount = r.PremiumAmount,
                AgentCommissionAmount = r.AgentCommissionAmount
            })
            .ToListAsync();

        return Ok(requests);
    }

    // =====================================================
    // 3️⃣ Admin assigns agent
    // =====================================================
    [HttpPut("{id}/assign-agent/{agentId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignAgent(int id, int agentId)
    {
        var request = await _context.PolicyRequests.FindAsync(id);
        if (request == null)
            return NotFound("Request not found.");

        if (request.Status != PolicyRequestStatus.PendingAdmin)
            return BadRequest("Request is not in Pending state.");

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
    // 4️⃣ Agent sends form to customer
    // =====================================================
    [HttpPut("{id}/send-form")]
    [Authorize(Roles = "Agent")]
    public async Task<IActionResult> SendForm(int id)
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
    // 5️⃣ Customer submits property details
    // =====================================================
    [HttpPut("{id}/submit-form")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> SubmitForm(int id, SubmitFormDto dto)
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

        return Ok("Form submitted successfully.");
    }

    // =====================================================
    // 6️⃣ Agent calculates risk + premium
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

        // ===============================
        // 1️⃣ Calculate Risk Score
        // ===============================
        decimal riskScore = 0;

        if (request.PropertyAge > 10)
            riskScore += 30;

        if (request.PropertyValue > 250000)
            riskScore += 40;

        request.RiskScore = riskScore;

        // ===============================
        // 2️⃣ Calculate Premium
        // ===============================
        decimal basePremium = request.Plan.BasePremium;
        decimal riskMultiplier = 500;

        decimal finalPremium = basePremium + (riskScore * riskMultiplier);
        request.PremiumAmount = finalPremium;

        // ===============================
        // 3️⃣ Calculate Agent Commission
        // ===============================
        decimal agentCommission = request.Plan.AgentCommission;
        request.AgentCommissionAmount = agentCommission; // add this property in PolicyRequest

        request.Status = PolicyRequestStatus.RiskCalculated;

        await _context.SaveChangesAsync();

        return Ok(new PolicyRequestResponseDto
        {
            Id = request.Id,
            PlanId = request.PlanId,
            PlanName = request.Plan.PlanName,
            CustomerId = request.CustomerId,
            Status = request.Status,
            RiskScore = riskScore,
            PremiumAmount = finalPremium,
            AgentCommissionAmount = agentCommission
        });
    }
    // 7️⃣ Customer confirms purchase
    // =====================================================
    [HttpPut("{id}/buy")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> BuyPolicy(int id)
    {
        var request = await _context.PolicyRequests.FindAsync(id);
        if (request == null)
            return NotFound();

        if (request.Status != PolicyRequestStatus.RiskCalculated)
            return BadRequest("Risk not calculated yet.");

        request.Status = PolicyRequestStatus.CustomerConfirmed;

        await _context.SaveChangesAsync();

        return Ok("Waiting for admin approval.");

    }

    // =====================================================
    // 8️⃣ Admin final approval
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