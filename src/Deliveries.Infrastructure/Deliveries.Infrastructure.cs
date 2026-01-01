using Microsoft.Extensions.Logging;

namespace Infrastructure.ExternalServices;

public class DriverServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DriverServiceClient> _logger;

    public DriverServiceClient(HttpClient httpClient, ILogger<DriverServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> GetDeliveryStatusAsync(string deliveryId)
    {
        try
        {
            _logger.LogInformation("Fetching delivery status for ID: {DeliveryId}", deliveryId);
            var response = await _httpClient.GetAsync($"/deliveries/{deliveryId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching delivery status for ID: {DeliveryId}", deliveryId);
            return false;
        }
    }
}
