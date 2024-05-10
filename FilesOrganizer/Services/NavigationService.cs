using FilesOrganizer.Core;
using FilesOrganizer.Models;
using FilesOrganizer.ViewModels.Commands;
using System;

namespace FilesOrganizer.Services;

public interface INavigationService
{
    Core.ViewModel CurrentView { get; set; }

    void NavigateTo<T>(TransmittedData transmittedData=null) where T : Core.ViewModel;
}

public class NavigationService : ObservableObject, INavigationService
{
    private readonly Func<Type, Core.ViewModel> _viewModelFactory;

    private Core.ViewModel _currentView;

    public Core.ViewModel CurrentView
    {
        get => _currentView;
        set
        {
            _currentView = value;
            OnPropertyChanged();
        }
    }

    public NavigationService(Func<Type, Core.ViewModel> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public void NavigateTo<T>(TransmittedData transmittedData) where T : Core.ViewModel
    {
        Core.ViewModel viewModel = _viewModelFactory.Invoke(typeof(T));

        if (viewModel is AddCategoryVM addCategoryVM)
        {
            addCategoryVM.SubmittedData = transmittedData;
        }

        CurrentView = viewModel;
    }
}
