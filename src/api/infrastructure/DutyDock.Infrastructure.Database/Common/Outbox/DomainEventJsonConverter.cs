using System.Reflection;
using DutyDock.Domain.Common.Models.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DutyDock.Infrastructure.Database.Common.Outbox;

public class DomainEventJsonConverter : JsonConverter<DomainEvent>
{
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, DomainEvent? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override DomainEvent ReadJson(JsonReader reader, Type objectType, DomainEvent? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);

        var eventAssembly = Assembly.GetAssembly(typeof(DomainEvent));

        if (eventAssembly == null)
        {
            throw new ApplicationException("Assembly for Event type is not defined");
        }

        var eventTypes = eventAssembly.GetTypes()
            .Where(typeof(DomainEvent).IsAssignableFrom)
            .Where(type => typeof(DomainEvent) != type)
            .ToList();

        var action = jObject.Value<string>("action");
        var actionType = eventTypes.FirstOrDefault(type => type.Name == action);

        if (actionType == null)
        {
            throw new ApplicationException("Event type for action name could not be found");
        }

        var eventObject = (DomainEvent)Activator.CreateInstance(actionType)!;
        serializer.Populate(jObject.CreateReader(), eventObject);

        return eventObject;
    }
}