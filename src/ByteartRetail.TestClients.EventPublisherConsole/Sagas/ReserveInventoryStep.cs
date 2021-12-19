using ByteartRetail.Common.Messaging;
using ByteartRetail.TestClients.Common.Sagas;

namespace ByteartRetail.TestClients.EventPublisherConsole.Sagas;

public class ReserveInventoryStep : SagaStep
{
    private readonly float _reservingAmount;

    public ReserveInventoryStep(string serviceName, float reservingAmount)
    {
        _reservingAmount = reservingAmount;
    }
    
    protected override (string, string) GetStepEventDefinitionInternal()
    {
        return ("reserve-inventory", $"ReservingAmount={_reservingAmount}");
    }
}