using System.Reflection;
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
                propertyInfo.SetValue(evnt, value);
            }
        }

        return evnt;
    }
}