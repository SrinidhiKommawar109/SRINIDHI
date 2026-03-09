using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class PolicyRequestService : IPolicyRequestService
{
    private readonly IRepository<PolicyRequest> _policyRequestRepository;
    private readonly IRepository<Notification> _notificationRepository;
    private readonly IPolicyRequestRepository _policyRequestReadRepository;
    private readonly IRepository<ApplicationUser> _userRepository;

    public PolicyRequestService(
        IRepository<PolicyRequest> policyRequestRepository,
        IRepository<Notification> notificationRepository,
        IPolicyRequestRepository policyRequestReadRepository,
        IRepository<ApplicationUser> userRepository)
    {
        _policyRequestRepository = policyRequestRepository;
        _notificationRepository = notificationRepository;
        _policyRequestReadRepository = policyRequestReadRepository;
        _userRepository = userRepository;
    }

    public async Task CreateRequestAsync(CreatePolicyRequestDto dto, int customerId)
    {
        var plan = await _policyRequestReadRepository.GetPlanByIdAsync(dto.PlanId);
        if (plan == null)
            throw new InvalidOperationException("Invalid Plan ID.");

        var request = new PolicyRequest
        {
            PlanId = dto.PlanId,
            CustomerId = customerId,
            Status = PolicyRequestStatus.PendingAdmin
        };

        await _policyRequestRepository.AddAsync(request);
        await _policyRequestRepository.SaveChangesAsync();
    }

    public Task<List<PolicyRequest>> GetPendingRequestsAsync() =>
        _policyRequestReadRepository.GetPendingRequestsAsync();

    public async Task AssignAgentAsync(int requestId, int agentId, string? adminNotes)
    {
        var request = await _policyRequestReadRepository.GetByIdAsync(requestId);
        if (request == null)
            throw new InvalidOperationException("Request not found.");

        var agentExists = await _userRepository.AnyAsync(u => u.Id == agentId && u.Role == UserRole.Agent);
        if (!agentExists)
            throw new InvalidOperationException("Invalid Agent ID.");

        request.AgentId = agentId;
        request.AdminNotes = adminNotes;
        request.Status = PolicyRequestStatus.AgentAssigned;

        await _notificationRepository.AddAsync(new Notification
        {
            UserId = agentId,
            Title = "New Request Assigned",
            Message = $"Admin assigned a new policy request (ID: {request.Id}) to you.",
            Type = "info"
        });

        await _notificationRepository.AddAsync(new Notification
        {
            UserId = request.CustomerId,
            Title = "Agent Assigned",
            Message = $"An agent has been assigned to your policy request (ID: {request.Id}).",
            Type = "success"
        });

        await _policyRequestRepository.SaveChangesAsync();
    }

    public Task<List<PolicyRequest>> GetAssignedRequestsAsync(int agentId) =>
        _policyRequestReadRepository.GetAssignedForAgentAsync(agentId);

    public async Task SendFormToCustomerAsync(int requestId)
    {
        var request = await _policyRequestReadRepository.GetByIdAsync(requestId);
        if (request == null)
            throw new InvalidOperationException("Request not found.");

        if (request.Status != PolicyRequestStatus.AgentAssigned)
            throw new InvalidOperationException("Agent not assigned yet.");

        request.Status = PolicyRequestStatus.FormSent;

        await _notificationRepository.AddAsync(new Notification
        {
            UserId = request.CustomerId,
            Title = "Action Required: Property Details",
            Message = "Please submit your property details as requested by the agent.",
            Type = "info"
        });

        await _policyRequestRepository.SaveChangesAsync();
    }

    public async Task SubmitPropertyDetailsAsync(int requestId, SubmitPropertyDto dto)
    {
        var request = await _policyRequestReadRepository.GetByIdAsync(requestId);
        if (request == null)
            throw new InvalidOperationException("Request not found.");

        if (request.Status != PolicyRequestStatus.FormSent)
            throw new InvalidOperationException("Form not sent yet.");

        if (string.IsNullOrWhiteSpace(dto.PropertyAddress))
            throw new InvalidOperationException("Property address is required.");
        if (dto.PropertyValue <= 0)
            throw new InvalidOperationException("Property value must be greater than zero.");
        if (dto.PropertyAge < 0)
            throw new InvalidOperationException("Property age cannot be negative.");

        request.PropertyAddress = dto.PropertyAddress;
        request.PropertyValue = dto.PropertyValue;
        request.PropertyAge = dto.PropertyAge;
        request.Status = PolicyRequestStatus.FormSubmitted;

        if (request.AgentId.HasValue)
        {
            await _notificationRepository.AddAsync(new Notification
            {
                UserId = request.AgentId.Value,
                Title = "Form Submitted",
                Message = $"Customer has submitted property details for request ID {request.Id}.",
                Type = "info"
            });
        }

        await _policyRequestRepository.SaveChangesAsync();
    }

    public async Task<CalculateRiskResultDto> CalculateRiskAsync(int requestId)
    {
        var request = await _policyRequestReadRepository.GetByIdWithPlanAsync(requestId);
        if (request == null)
            throw new InvalidOperationException("Request not found.");

        if (request.Status != PolicyRequestStatus.FormSubmitted)
            throw new InvalidOperationException("Form not submitted yet.");

        decimal riskScore = 0;
        if (request.PropertyAge.HasValue && request.PropertyAge > 10)
            riskScore += 30;
        if (request.PropertyValue.HasValue && request.PropertyValue > 250000)
            riskScore += 40;

        request.RiskScore = riskScore;

        decimal basePremium = request.Plan.BasePremium;
        decimal riskMultiplier = 500;
        decimal finalPremium = basePremium + (riskScore * riskMultiplier);

        request.PremiumAmount = finalPremium;
        request.TotalPremium = finalPremium;
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
        request.AgentCommissionAmount = request.Plan.AgentCommission;
        request.Status = PolicyRequestStatus.RiskCalculated;

        await _notificationRepository.AddAsync(new Notification
        {
            UserId = request.CustomerId,
            Title = "Premium Calculated",
            Message = $"Your premium has been calculated: {request.TotalPremium:C}. Please review and confirm purchase.",
            Type = "success"
        });

        await _policyRequestRepository.SaveChangesAsync();

        return new CalculateRiskResultDto
        {
            Id = request.Id,
            PlanId = request.PlanId,
            PlanName = request.Plan.PlanName,
            RiskScore = request.RiskScore,
            TotalPremium = request.TotalPremium,
            Frequency = request.Frequency,
            InstallmentCount = request.InstallmentCount,
            InstallmentAmount = request.InstallmentAmount,
            AgentCommissionAmount = request.AgentCommissionAmount,
            Status = request.Status
        };
    }

    public async Task BuyPolicyAsync(int requestId)
    {
        var request = await _policyRequestReadRepository.GetByIdAsync(requestId);
        if (request == null)
            throw new InvalidOperationException("Request not found.");

        if (request.Status != PolicyRequestStatus.RiskCalculated)
            throw new InvalidOperationException("Risk not calculated yet.");

        request.Status = PolicyRequestStatus.CustomerConfirmed;

        var admins = await _policyRequestReadRepository.GetAdminsAsync();
        foreach (var admin in admins)
        {
            await _notificationRepository.AddAsync(new Notification
            {
                UserId = admin.Id,
                Title = "Action Required: Final Approval",
                Message = $"Customer has confirmed purchase for request ID {request.Id}. Please provide final approval.",
                Type = "info"
            });
        }

        await _policyRequestRepository.SaveChangesAsync();
    }

    public async Task AdminApproveAsync(int requestId)
    {
        var request = await _policyRequestReadRepository.GetByIdAsync(requestId);
        if (request == null)
            throw new InvalidOperationException("Request not found.");

        if (request.Status != PolicyRequestStatus.CustomerConfirmed)
            throw new InvalidOperationException("Customer has not confirmed yet.");

        request.Status = PolicyRequestStatus.PolicyApproved;

        await _notificationRepository.AddAsync(new Notification
        {
            UserId = request.CustomerId,
            Title = "Policy Approved",
            Message = "Your policy has been officially approved. Congratulations!",
            Type = "success"
        });

        await _policyRequestRepository.SaveChangesAsync();
    }

    public Task<List<PolicyRequest>> GetMyRequestsAsync(int customerId) =>
        _policyRequestReadRepository.GetMyRequestsAsync(customerId);

    public Task<List<PolicyRequest>> GetApprovedForAgentAsync(int agentId) =>
        _policyRequestReadRepository.GetApprovedForAgentAsync(agentId);

    public async Task<List<PolicyRequestResponseDto>> GetAllRequestsAsync()
    {
        var requests = await _policyRequestReadRepository.GetAllRequestsWithClaimsAsync();
        return requests.Select(r =>
        {
            var latestClaim = r.Claims.OrderByDescending(c => c.Id).FirstOrDefault();
            return new PolicyRequestResponseDto
            {
                Id = r.Id,
                PlanId = r.PlanId,
                PlanName = r.Plan.PlanName,
                CustomerId = r.CustomerId,
                CustomerName = r.Customer.FullName,
                AgentId = r.AgentId,
                AgentName = r.Agent?.FullName,
                Status = r.Status,
                PropertyAddress = r.PropertyAddress,
                PropertyValue = r.PropertyValue,
                PropertyAge = r.PropertyAge,
                RiskScore = r.RiskScore,
                PremiumAmount = r.PremiumAmount,
                AgentCommissionAmount = r.AgentCommissionAmount,
                ClaimId = latestClaim?.Id,
                ClaimStatus = latestClaim?.Status.ToString(),
                ClaimsOfficerId = latestClaim?.AssignedOfficerId,
                ClaimsOfficerName = latestClaim?.AssignedOfficer?.FullName
            };
        }).ToList();
    }
}
