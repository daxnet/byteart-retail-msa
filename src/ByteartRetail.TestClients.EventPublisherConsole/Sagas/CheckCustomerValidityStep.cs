using ByteartRetail.Common.Messaging;
using ByteartRetail.TestClients.Common.Sagas;

namespace ByteartRetail.TestClients.EventPublisherConsole.Sagas;

public class CheckCustomerValidityStep : SagaStep
{
    private readonly string _customerName;

    public CheckCustomerValidityStep(string serviceName, string customerName)
        : base(serviceName)
    {
        _customerName = customerName;
    }
    
    protected override (string, string) GetStepEventDefinitionInternal()
    {
        return ("check-customer-validity", $"customerName={_customerName}");
    }
}