using FilesOrganizer.Commands;
using FilesOrganizer.Helpers;
using FilesOrganizer.Models;
using MediaToolkit.Options;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace FilesOrganizer.ViewModels.Commands;

public class CreateFolderVM : Core.ViewModel, INotifyPropertyChanged
{
    TransmittedData _currentData;
    private CreateFolderCommands _commands;
    private Element _selectedElement;
    private string _initPath;
    private int _pozInList = 0;
    private List<string> _backStack = new List<string>();
    private Category _selectedCategory; // = new Category { Name = "White", SolidColorBrushColor = Brushes.White, CategoryName = "All Items", TextColor="Black" };
    private string _selectedPriority; // = "All Items";
    private string _selectedLanguage; // = "All Items";
    private string _selectedCodeLanguage; // = "All Items";
    private bool? _filterType;
    private bool _searchApplied;
    public ObservableCollection<Element> AllItems { get; set; } 
    public string NewFolderPath { get; set; }

    public CreateFolderVM(TransmittedData currentData, int poz, string initPath, bool? filter, Category category, string priority, string language, string codeLanguage, bool search)
    {
        CurrentData = new TransmittedData(currentData);
        InitPath = initPath;
        BackStack.Add(CurrentData.BackStack.ElementAt(poz));
        FilterType = filter;
        SelectedCategory = category;
        SelectedPriority = priority;
        SelectedLanguage = language;
        SelectedCodeLanguage = codeLanguage;
        SearchApplied = search;
        AllItems = new ObservableCollection<Element>(CurrentData.AllItems);

        //apdatez toate elementele sa ramana doar cele necesare
        bool allFiltersAreDefault = (SelectedPriority == "All Items" || SelectedPriority == null) &&
                        (SelectedCategory == null || SelectedCategory.CategoryName == "All Items") &&
                        (SelectedLanguage == "All Items" || SelectedLanguage == null) &&
                        (SelectedCodeLanguage == "All Items" || SelectedCodeLanguage == null);

        if (!allFiltersAreDefault)
        {
            CurrentData.AllItems = ViewerPageHelper.ReturnAllItemsToCreateFolder(this);
        }
        else
        {
            CurrentData.AllItems = new ObservableCollection<Element>(CurrentData.AllItems.Where(e => e.Path.Contains(InitPath)).ToList());
        }
    }

    public bool SearchApplied
    {
        get { return _searchApplied; }
        set
        {
            if (_searchApplied != value)
            {
                _searchApplied = value;
                OnPropertyChanged(nameof(SearchApplied));
            }
        }
    }
    public bool? FilterType
    {
        get => _filterType;
        set
        {
            _filterType = value;
            OnPropertyChanged(nameof(FilterType));
        }
    }

    public Category SelectedCategory
    {
        get { return _selectedCategory; }
        set
        {
            if (_selectedCategory != value)
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }
    }

    public string SelectedPriority
    {
        get { return _selectedPriority; }
        set
        {
            if (_selectedPriority != value)
            {
                _selectedPriority = value;
                OnPropertyChanged(nameof(SelectedPriority));
            }
        }
    }

    public string SelectedLanguage
    {
        get { return _selectedLanguage; }
        set
        {
            if (_selectedLanguage != value)
            {
                _selectedLanguage = value;
                OnPropertyChanged(nameof(SelectedLanguage));
            }
        }
    }

    public string SelectedCodeLanguage
    {
        get { return _selectedCodeLanguage; }
        set
        {
            if (_selectedCodeLanguage != value)
            {
                _selectedCodeLanguage = value;
                OnPropertyChanged(nameof(SelectedCodeLanguage));
            }
        }
    }

    public string InitPath
    {
        get => _initPath;
        set
        {
            _initPath = value;
            OnPropertyChanged(nameof(InitPath));
        }
    }

    public List<string> BackStack
    {
        get => _backStack;
        set
        {
            _backStack = value;
            OnPropertyChanged(nameof(BackStack));
        }
    }

    public int PozInList
    {
        get => _pozInList;
        set
        {
            _pozInList = value;
            OnPropertyChanged(nameof(PozInList));
        }
    }

    public Element SelectedElement
    {
        get { return _selectedElement; }
        set
        {
            if (_selectedElement != value)
            {
                _selectedElement = value;
                OnPropertyChanged(nameof(SelectedElement));
            }
        }
    }
    public TransmittedData CurrentData
    {
        get => _currentData;
        set
        {
            _currentData = value;
            OnPropertyChanged(nameof(CurrentData));
        }
    }
    public CreateFolderCommands Commands
    {
        get
        {
            if (_commands == null)
            {
                _commands = new CreateFolderCommands(this);
            }
            return _commands;
        }
    }
}
