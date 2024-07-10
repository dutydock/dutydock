using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DutyDock.Web.Core.ViewModels.Base;

public partial class ViewModelBase : ObservableObject, IViewModelBase
{
    [RelayCommand]
    public virtual async Task OnInitializedAsync()
    {
        await Task.CompletedTask.ConfigureAwait(false);
    }

    public virtual void Dispose()
    {
    }
}