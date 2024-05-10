using FilesOrganizer.Core;
using FilesOrganizer.Services;
using FilesOrganizer.ViewModels.Commands;

namespace FilesOrganizer.ViewModels;

public class MainViewModel : Core.ViewModel
{
    private INavigationService _navigation;

    public INavigationService Navigation
    {
        get => _navigation;
        set
        {
            _navigation = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand NavigateToViewerPageCommand { get; set; }
    public RelayCommand NavigateToAddCategoryCommand { get; set; }
    public MainViewModel(INavigationService navService)
    {
        Navigation = navService;
        Navigation.CurrentView = new ViewerPageVM(Navigation);
        NavigateToViewerPageCommand = new RelayCommand(execute: o => { Navigation.NavigateTo<ViewerPageVM>(); }, canExecute: o => true);
        NavigateToAddCategoryCommand = new RelayCommand(execute: o => { Navigation.NavigateTo<AddCategoryVM>(); }, canExecute: o => true);
    }
}
