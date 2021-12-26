using ByteartRetail.TestClients.Common.Sagas;

namespace ByteartRetail.TestClients.EventPublisherConsole.Sagas;

public class CreateSalesOrderStep : SagaStep
{
    public string CustomerName { get; set; }

    public CreateSalesOrderStep(string serviceName, string customerName)
        : base(serviceName)
    {
        CustomerName = customerName;
    }
    
    protected override (string, string) GetStepEventDefinitionInternal()
    {
        return ("create-sales-order", $"CustomerName={CustomerName}");
    }

    protected override (string?, string?) GetStepCompensateEventInternal()
    {
        return ("compensate-create-sales-order", $"CustomerName={CustomerName}");
    }
}