using Domain.Delivery;

namespace Application.Delivery;

public class DeliveryQuery
{
    public DeliveryStatus Execute()
    {
        return new DeliveryStatus("UP", DateTime.UtcNow);
    }
}
