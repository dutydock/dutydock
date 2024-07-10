using DutyDock.Web.Core;
using Microsoft.AspNetCore.Components;

namespace DutyDock.Web.Shared.Navigation;

public class RedirectNotFound : ComponentBase
{
    [Inject] public required NavigationManager NavManager { get; set; }
    
    protected override void OnInitialized()
    {
        NavManager.NavigateTo(Routes.Error404);
    }
}