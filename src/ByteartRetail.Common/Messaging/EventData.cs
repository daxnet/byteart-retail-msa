using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ByteartRetail.Common.Messaging;

public sealed class EventData
{
    public Guid EventId { get; set; }
    
    public DateTime EventTimestamp { get; set; }
    
    public string? ClrTypeName { get; set; }

    public Dictionary<string, object?> EventContent { get; set; } = new();

    public EventData()
    {
    }

    public EventData(IEvent evnt)
    {
        EventId = evnt.Id;
        EventTimestamp = evnt.Timestamp;
        ClrTypeName = evnt.GetType().AssemblyQualifiedName;
        var properties = from prop in evnt.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            where prop.CanRead && prop.Name != nameof(evnt.Id) && prop.Name != nameof(evnt.Timestamp) && prop.PropertyType.IsSimpleType()
            select prop;
        foreach (var prop in properties)
        {
            EventContent.Add(prop.Name, prop.GetValue(evnt));
        }
    }

    public IEvent? AsEvent()
    {
        if (string.IsNullOrEmpty(ClrTypeName))
        {
            return null;
        }
        
        var eventType = Type.GetType(ClrTypeName);
        if (eventType == null)
        {
            return null;
        }
        
        var evnt = (IEvent?)Activator.CreateInstance(eventType);
        if (evnt == null)
        {
            return null;
        }

        evnt.Id = EventId;
        evnt.Timestamp = EventTimestamp;
        foreach (var (key, value) in EventContent)
        {
            var propertyInfo = eventType.GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(evnt, InferObjectValue(propertyInfo.PropertyType, value));
            }
        }

        return evnt;
    }

    private object? InferObjectValue(Type type, object? val)
    {
        if (type == typeof(string))
        {
            return val?.ToString();
        }
        if (type == typeof(Guid))
        {
            return new Guid(val?.ToString() ?? string.Empty);
        }

        if (type == typeof(int))
        {
            return Convert.ToInt32(val);
        }

        if (type == typeof(float))
        {
            return Convert.ToSingle(val);
        }

        if (type == typeof(double))
        {
            return Convert.ToDouble(val);
        }

        return val;
    }
}