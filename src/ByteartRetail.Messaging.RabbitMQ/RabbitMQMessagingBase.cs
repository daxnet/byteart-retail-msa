using ByteartRetail.Common.Messaging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace ByteartRetail.Messaging.RabbitMQ;

public abstract class RabbitMQMessagingBase : IDisposable
{
    private readonly IConnection _connection;
    private bool disposed;

    ~RabbitMQMessagingBase()
    {
        Dispose(false);
    }

    protected RabbitMQMessagingBase(
        IConnectionFactory connectionFactory,
        string exchangeName,
        string exchangeType,
        ILogger logger)
    {
        _connection = connectionFactory.CreateConnection();
        ExchangeName = exchangeName;
        ExchangeType = exchangeType;
        Channel = _connection.CreateModel();
        Channel.ExchangeDeclare(ExchangeName, ExchangeType,  true);
        Logger = logger;
    }

    protected IModel Channel { get; }
    
    protected ILogger Logger { get; }
    
    protected string ExchangeName { get; }
    
    protected string ExchangeType { get; }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                Channel.Dispose();
                _connection.Dispose();
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }
}