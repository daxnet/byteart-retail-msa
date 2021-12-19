using ByteartRetail.Common.Messaging;
using ByteartRetail.TestClients.Common.Sagas;

namespace ByteartRetail.TestClients.EventPublisherConsole.Sagas;

public class ReserveCreditStep : SagaStep
{
    private readonly int _reservingCredit;

    public ReserveCreditStep(string serviceName, int reservingCredit)
        : base(serviceName)
    {
        _reservingCredit = reservingCredit;
    }
    protected override (string, string) GetStepEventDefinitionInternal()
    {
        return ("reserve-credit", $"ReservingCredit={_reservingCredit}");
    }
}