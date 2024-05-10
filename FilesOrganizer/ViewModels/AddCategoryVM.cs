using FilesOrganizer.Core;
using FilesOrganizer.Models;
using FilesOrganizer.Services;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace FilesOrganizer.ViewModels.Commands;

public class AddCategoryVM : Core.ViewModel
{
    private INavigationService _navigation;
    public ObservableCollection<Category> ColorList { get; set; }
    private ViewerPageVM _viewerPageVM;
    private bool _isRightPartVisible;
    private Commands _commands;
    private Category _definedCategory;
    private string _categoryName;

    TransmittedData _submittedData;

    public TransmittedData SubmittedData
    {
        get => _submittedData;
        set
        {
            _submittedData = value;
            OnPropertyChanged();

            ViewerPageVM.UpdateData(_submittedData);
        }
    }

    public string CategoryName
    {
        get => _categoryName;
        set
        {
            _categoryName = value;
            OnPropertyChanged();
        }
    }
    public Category DefinedCategory
    {
        get => _definedCategory;
        set
        {
            _definedCategory = value;
            OnPropertyChanged(nameof(DefinedCategory));
        }
    }


    public bool IsRightPartVisible
    {
        get => _isRightPartVisible;
        set
        {
            _isRightPartVisible = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand ToggleVisibilityCommand { get; set; }

    public INavigationService Navigation
    {
        get => _navigation;
        set
        {
            _navigation = value;
            OnPropertyChanged();
        }
    }

    public ViewerPageVM ViewerPageVM
    {
        get => _viewerPageVM;
        set
        {
            _viewerPageVM = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand NavigateToViewerPageCommand { get; set; }

    public AddCategoryVM(INavigationService navigation, ViewerPageVM homeViewModel)
    {
        IsRightPartVisible = true;

        ToggleVisibilityCommand = new RelayCommand(
            execute: o => { IsRightPartVisible = !IsRightPartVisible; },
            canExecute: o => true
        );

        Navigation = navigation;
        ViewerPageVM = homeViewModel;

        NavigateToViewerPageCommand = new RelayCommand(
            execute: o => { Navigation.NavigateTo<ViewerPageVM>(SubmittedData); },
            canExecute: o => true
        );

        ColorList = new ObservableCollection<Category>();

        foreach (var property in typeof(Brushes).GetProperties())
        {
            if (property.PropertyType == typeof(SolidColorBrush))
            {
                SolidColorBrush colorBrush = (SolidColorBrush)property.GetValue(null, null);
                string colorName = property.Name;

                ColorList.Add(new Category { Name = colorName, Col = colorBrush });
            }
        }
        DefinedCategory = new Category();
    }

    public Commands Commands
    {
        get
        {
            if (_commands == null)
            {
                _commands = new Commands(this);
            }
            return _commands;
        }
    }
}
