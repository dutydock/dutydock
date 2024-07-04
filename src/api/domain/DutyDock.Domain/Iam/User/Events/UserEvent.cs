using DutyDock.Domain.Common.Models.Events;
using Newtonsoft.Json;

namespace DutyDock.Domain.Iam.User.Events;

public abstract class UserEvent : DomainEvent
{
    [JsonProperty] public string UserId { get; private set; } = null!;

    protected UserEvent()
    {
    }

    protected UserEvent(string userId, string action) : base(action)
    {
        UserId = userId;
    }
}