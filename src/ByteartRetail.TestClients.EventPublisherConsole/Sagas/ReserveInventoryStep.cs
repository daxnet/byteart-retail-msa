using ByteartRetail.Common.Messaging;
using ByteartRetail.TestClients.Common.Sagas;

namespace ByteartRetail.TestClients.EventPublisherConsole.Sagas;

public class ReserveInventoryStep : SagaStep
{
    public float ReservingAmount { get; set; }

    public ReserveInventoryStep(string serviceName, float reservingAmount)
        : base(serviceName)
    {
        ReservingAmount = reservingAmount;
    }
    
    protected override (string, string) GetStepEventDefinitionInternal()
    {
        return ("reserve-inventory", $"ReservingAmount={ReservingAmount}");
    }
}