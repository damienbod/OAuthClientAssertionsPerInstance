// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleDPoPClientAssertions;

public class DPoPClient : BackgroundService
{
    private readonly ILogger<DPoPClient> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public DPoPClient(ILogger<DPoPClient> logger, IHttpClientFactory factory)
    {
        _logger = logger;
        _clientFactory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(2000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("\n\n");
            _logger.LogInformation("DPoPClient running at: {time}", DateTimeOffset.UtcNow);


            // Onobarding User API
            var onboardingClient = _clientFactory.CreateClient("onboarding-user-client");
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("sessionId", "teste3e")
            };
            var content = new FormUrlEncodedContent(formData);
            var onboardingClientResponse = await onboardingClient.PostAsync("api/OnboardingUser/StartEmailVerification", content, stoppingToken);



            // Call mobile API
            var client = _clientFactory.CreateClient("mobile-dpop-client");
            var response = await client.GetAsync("api/values", stoppingToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(stoppingToken);
                _logger.LogInformation("API response: {response}", responseContent);
            }
            else
            {
                _logger.LogError("API returned: {statusCode}", response.StatusCode);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}