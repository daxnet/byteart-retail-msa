// See https://aka.ms/new-console-template for more information

using System.Runtime.Loader;
using ByteartRetail.Common.DataAccess;
using ByteartRetail.Common.Messaging;
using ByteartRetail.DataAccess.Mongo;
using ByteartRetail.Messaging.RabbitMQ;
using ByteartRetail.TestClients.Common.Sagas;
using ByteartRetail.TestClients.EventPublisherConsole.Sagas;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;
using RabbitMQ.Client;

var connectionFactory = new ConnectionFactory { HostName = "localhost" };
var eventPublisher = new RabbitMQMessagePublisher(
    connectionFactory, 
    "sagaExchange", 
    "direct", 
    NullLogger<RabbitMQMessagePublisher>.Instance);

var eventHandlingContext = new DefaultEventHandlingContext();
eventHandlingContext.RegisterHandler<SagaEvent, SagaEventHandler>();
var eventSubscriber = new RabbitMQMessageSubscriber(
    eventHandlingContext,
    connectionFactory,
    "sagaReturnExchange",
    "direct",
    NullLogger<RabbitMQMessageSubscriber>.Instance
);
eventSubscriber.Subscribe("shopping-cart");

var dao = new MongoDataAccessObject(MongoUrl.Create("mongodb://localhost:27017"), "sagas");
var sagaManager = new SagaManager(dao, eventPublisher);
var saga = await sagaManager.CreateAsync(new SagaStep[]
{
    new CreateSalesOrderStep("sales-order", "daxnet"),
    new CheckCustomerValidityStep("customers", "daxnet"),
    new ReserveCreditStep("customers", 100),
    new ReserveInventoryStep("product-catalog", 100)
});

await sagaManager.StartAsync(saga);
Console.WriteLine("Saga started, press ENTER to exit.");
Console.ReadLine();

class SagaEventHandler : IEventHandler<SagaEvent>
{
    private readonly SagaManager _sagaManager;

    public SagaEventHandler()
    {
        var connectionFactory = new ConnectionFactory { HostName = "localhost" };
        var eventPublisher = new RabbitMQMessagePublisher(
            connectionFactory, 
            "sagaExchange", 
            "direct", 
            NullLogger<RabbitMQMessagePublisher>.Instance);
        var dao = new MongoDataAccessObject(MongoUrl.Create("mongodb://localhost:27017"), "sagas");
        _sagaManager = new SagaManager(dao, eventPublisher);
    }
    
    public async Task<bool> HandleAsync(SagaEvent @event, CancellationToken cancellationToken = default)
    {
        await _sagaManager.TransitAsync(@event, cancellationToken);
        return true;
    }
}
