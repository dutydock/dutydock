using Newtonsoft.Json;

namespace DutyDock.Domain.Iam.User.Events;

public sealed class UserInvitedEvent : UserEvent
{
    [JsonProperty] public string InviterId { get; set; } = null!;

    [JsonProperty] public string OrganizationId { get; set; } = null!;

    public UserInvitedEvent()
    {
    }

    public UserInvitedEvent(string userId, string inviterId, string organizationId) : base(userId, nameof(UserInvitedEvent))
    {
        InviterId = inviterId;
        OrganizationId = organizationId;
    }
}