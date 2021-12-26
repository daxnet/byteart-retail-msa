using ByteartRetail.Common.Messaging;
using ByteartRetail.TestClients.Common.Sagas;

namespace ByteartRetail.TestClients.EventPublisherConsole.Sagas;

public class CheckCustomerValidityStep : SagaStep
{
    public string CustomerName { get; set; }

    public CheckCustomerValidityStep(string serviceName, string customerName)
        : base(serviceName)
    {
        CustomerName = customerName;
    }
    
    protected override (string, string) GetStepEventDefinitionInternal()
    {
        return ("check-customer-validity", $"customerName={CustomerName}");
    }
}