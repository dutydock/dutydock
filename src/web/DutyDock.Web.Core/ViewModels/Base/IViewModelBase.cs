using System.ComponentModel;

namespace DutyDock.Web.Core.ViewModels.Base;

public interface IViewModelBase : INotifyPropertyChanged, IDisposable
{
    Task OnInitializedAsync();
}