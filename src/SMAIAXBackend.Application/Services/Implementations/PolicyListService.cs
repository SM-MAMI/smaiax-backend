﻿using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class PolicyListService(IPolicyRepository policyRepository, ILogger<PolicyListService> logger) : IPolicyListService
{
    public async Task<List<PolicyDto>> GetPoliciesBySmartMeterIdAsync(SmartMeterId smartMeterId)
    {
        var policies = await policyRepository.GetPoliciesBySmartMeterIdAsync(smartMeterId);
        return policies.Select(PolicyDto.FromPolicy).ToList();
    }

    public async Task<List<PolicyDto>> GetPoliciesAsync()
    {
        var policies = await policyRepository.GetPoliciesAsync();
        return policies.Select(PolicyDto.FromPolicy).ToList();
    }
}