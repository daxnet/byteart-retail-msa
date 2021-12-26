using ByteartRetail.Common.Messaging;
using ByteartRetail.TestClients.Common.Sagas;

namespace ByteartRetail.TestClients.EventPublisherConsole.Sagas;

public class ReserveCreditStep : SagaStep
{
    public int ReservingCredit { get; set; }

    public ReserveCreditStep(string serviceName, int reservingCredit)
        : base(serviceName)
    {
        ReservingCredit = reservingCredit;
    }
    protected override (string, string) GetStepEventDefinitionInternal()
    {
        return ("reserve-credit", $"ReservingCredit={ReservingCredit}");
    }

    protected override (string?, string?) GetStepCompensateEventInternal()
    {
        return ("compensate-reserve-credit", $"ReservingCredit={ReservingCredit}");
    }
}