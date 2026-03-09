using API.Controllers;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Xunit;
using SecurityClaim = System.Security.Claims.Claim;

namespace API.Tests.Controllers;

public class PolicyRequestsControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly PolicyRequestsController _controller;

    public PolicyRequestsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        var policyRequestRepo = new Repository<PolicyRequest>(_context);
        var notificationRepo = new Repository<Notification>(_context);
        var policyRequestReadRepo = new PolicyRequestRepository(_context);
        var userRepo = new Repository<ApplicationUser>(_context);
        IPolicyRequestService policyRequestService = new PolicyRequestService(policyRequestRepo, notificationRepo, policyRequestReadRepo, userRepo);
        _controller = new PolicyRequestsController(policyRequestService);
    }

    private void SetupUser(string userId, string role)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new SecurityClaim[]
        {
            new SecurityClaim(ClaimTypes.NameIdentifier, userId),
            new SecurityClaim(ClaimTypes.Role, role),
        }, "mock"));
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }

    [Fact]
    public async Task CreateRequest_ReturnsOk_WhenValid()
    {
        SetupUser("1", "Customer");
        _context.PropertyPlans.Add(new PropertyPlans { Id = 1, PlanName = "Plan1" });
        await _context.SaveChangesAsync();

        var dto = new CreatePolicyRequestDto { PlanId = 1 };

        var result = await _controller.CreateRequest(dto);

        Assert.IsType<OkObjectResult>(result);
        Assert.True(await _context.PolicyRequests.AnyAsync(r => r.PlanId == 1));
    }

    [Fact]
    public async Task AdminApprove_UpdatesStatus()
    {
        var request = new PolicyRequest { Id = 1, Status = PolicyRequestStatus.CustomerConfirmed, CustomerId = 1 };
        _context.PolicyRequests.Add(request);
        await _context.SaveChangesAsync();

        var result = await _controller.AdminApprove(1);

        Assert.IsType<OkObjectResult>(result);
        var updated = await _context.PolicyRequests.FindAsync(1);
        Assert.NotNull(updated);
        Assert.Equal(PolicyRequestStatus.PolicyApproved, updated.Status);
    }
}
