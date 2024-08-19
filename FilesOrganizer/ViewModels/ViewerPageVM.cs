using FilesOrganizer.Commands;
using FilesOrganizer.Core;
using FilesOrganizer.Helpers;
using FilesOrganizer.Models;
using Google.Apis.Drive.v3;
using NPOI.OpenXmlFormats.Dml.Diagram;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace FilesOrganizer.ViewModels.Commands;

public class ViewerPageVM : Core.ViewModel, INotifyPropertyChanged
{
    private IList _selectedItems = new ObservableCollection<object>();
    private Settings _settingsDatas;
    TransmittedData _currentData = new TransmittedData();
    private ObservableCollection<string> _priorityList = new ObservableCollection<string>() { "All Items", "None", "High", "Medium", "Low" };
    private ObservableCollection<string> _languages = new ObservableCollection<string>()
    {
        "All Items" ,"None", "Afrikaans", "Arabic", "Armenian", "Azerbaijani", "Belarusian", "Bosnian", "Bulgarian",
        "Catalan", "Chinese", "Croatian", "Czech", "Danish", "Dutch", "English", "Estonian",
        "Finnish", "French", "Galician", "German", "Greek", "Hebrew", "Hindi", "Hungarian",
        "Icelandic", "Indonesian", "Italian", "Japanese", "Kannada", "Kazakh", "Korean", "Latvian",
        "Lithuanian", "Macedonian", "Malay", "Marathi", "Maori", "Nepali", "Norwegian", "Persian",
        "Polish", "Portuguese", "Romanian", "Russian", "Serbian", "Slovak", "Slovenian", "Spanish",
        "Swahili", "Swedish", "Tagalog", "Tamil", "Thai", "Turkish", "Ukrainian", "Urdu", "Vietnamese",
        "Welsh"
    };
    private ObservableCollection<string> _codeLanguages = new ObservableCollection<string>()
    {
        "All Items", "None", "C", "C#", "C++", "HTML", "Java", "Python", 
    };
    private ViewerPageCommands _commands;
    public ObservableCollection<Category> ColorsAvailable { get; set; }
    private Category _definedCategory;
    private string _categoryName;
    private Category _selectedCategory; // = new Category { Name = "White", SolidColorBrushColor = Brushes.White, CategoryName = "All Items", TextColor="Black" };
    private string _selectedPriority; // = "All Items";
    private string _selectedLanguage; // = "All Items";
    private string _selectedCodeLanguage; // = "All Items";
    private string _conversionName;
    private Visibility _conversionNameWattermark;
    private Visibility _conversionPathWattermark;
    private Visibility _categoryNameWattermark;
    private Visibility _pathWatermark;
    private Visibility _searchingWordWatermark;
    private string _currentPath;
    private int _pozInList;
    private string _currentPathDisplayed;
    private string _savingConversionPath;
    private Element _selectedItem;
    private Category _selectedItemCategory;
    private ObservableCollection<MyCheckbox> _conversionOptions;
    private string _searchingWord;
    private bool _isByNameChecked;
    private bool _isByContentChecked;
    private bool _searchApplied = false;

    public IList SelectedItems
    {
        get { return _selectedItems; }
        set
        {
            _selectedItems = value;
            OnPropertyChanged(nameof(SelectedItems));
        }
    }

    public bool IsLastCategory(Category category)
    {
        if (CurrentData.CategoriesWithoutNone.Any())
        {
            return CurrentData.CategoriesWithoutNone.Last() == category;
        }
        return false;
    }
    public bool IsSelectedFolder
    {
        get
        {
            return SelectedItem != null && SelectedItem.Extension == "Folder";
        }
    }

    public Settings SettingsDatas
    {
        get => _settingsDatas;
        set
        {
            _settingsDatas = value;
            OnPropertyChanged(nameof(SettingsDatas));
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
    public bool IsByNameChecked
    {
        get { return _isByNameChecked; }
        set
        {
            if (_isByNameChecked != value)
            {
                _isByNameChecked = value;
                OnPropertyChanged(nameof(IsByNameChecked));
            }
        }
    }

    public bool IsByContentChecked
    {
        get { return _isByContentChecked; }
        set
        {
            if (_isByContentChecked != value)
            {
                _isByContentChecked = value;
                OnPropertyChanged(nameof(IsByContentChecked));
            }
        }
    }
    public string SearchingWord
    {
        get { return _searchingWord; }
        set
        {
            if (_searchingWord != value)
            {
                _searchingWord = value;
                if (_searchingWord == "")
                {
                    SearchingWordWatermark = Visibility.Visible;
                }
                else
                {
                    SearchingWordWatermark = Visibility.Hidden;
                }
                OnPropertyChanged(nameof(SearchingWord));
            }
        }
    }

    public Category SelectedItemCategory
    {
        get { return _selectedItemCategory; }
        set
        {
            if (_selectedItemCategory != value)
            {
                _selectedItemCategory = value;
                OnPropertyChanged(nameof(SelectedItemCategory));
            }
        }
    }

    public Element SelectedItem
    {
        get { return _selectedItem; }
        set
        {
            if (_selectedItem != value)
            {
                _selectedItem = value;
                if (SelectedItem != null)
                {
                    var extension = SelectedItem.Extension;
                    extension = extension.Substring(1, extension.Length - 1).ToUpper();
                    ObservableCollection<MyCheckbox> conversionOptions;

                    if (Conversions.Texts.Any(item => item.Content == extension) && extension != "PDF")
                    {
                        conversionOptions = new ObservableCollection<MyCheckbox>(Conversions.Texts);
                    }
                    else if (Conversions.Images.Any(item => item.Content == extension))
                    {
                        conversionOptions = new ObservableCollection<MyCheckbox>(Conversions.Images);
                    }
                    else if (Conversions.Videos.Any(item => item.Content == extension))
                    {
                        conversionOptions = new ObservableCollection<MyCheckbox>(Conversions.Videos);
                    }
                    else if (Conversions.Audios.Any(item => item.Content == extension))
                    {
                        conversionOptions = new ObservableCollection<MyCheckbox>(Conversions.Audios);
                    }
                    else
                    {
                        conversionOptions = Conversions.Unknown;
                    }

                    conversionOptions.Remove(conversionOptions.FirstOrDefault(item => item.Content == extension));
                    ConversionOptions = conversionOptions;
                }
                else
                {
                    ConversionOptions = new ObservableCollection<MyCheckbox>();
                }
                OnPropertyChanged(nameof(ConversionOptions));
                OnPropertyChanged(nameof(SelectedItem));
                OnPropertyChanged(nameof(IsSelectedFolder));
            }
        }
    }
    public ObservableCollection<MyCheckbox> ConversionOptions
    {
        get { return _conversionOptions; }
        set
        {
            if (_conversionOptions != value)
            {
                _conversionOptions = value;
                OnPropertyChanged(nameof(ConversionOptions));
            }
        }
    }
    public string SavingConversionPath
    {
        get { return _savingConversionPath; }
        set
        {
            if (_savingConversionPath != value)
            {
                _savingConversionPath = value;
                if (_savingConversionPath == "")
                {
                    ConversionPathWattermark = Visibility.Visible;
                }
                else
                {
                    ConversionPathWattermark = Visibility.Hidden;
                }
                OnPropertyChanged(nameof(SavingConversionPath));
            }
        }
    }
    public string ConversionName
    {
        get { return _conversionName; }
        set
        {
            if (_conversionName != value)
            {
                _conversionName = value;
                if (_conversionName == "")
                {
                    ConversionNameWattermark = Visibility.Visible;
                }
                else
                {
                    ConversionNameWattermark = Visibility.Hidden;
                }
                OnPropertyChanged(nameof(ConversionName));
            }
        }
    }

    public string CurrentPath
    {
        get { return _currentPath; }
        set
        {
            if (_currentPath != value)
            {
                _currentPath = value;
                if (_currentPath == "")
                {
                    PathWatermark = Visibility.Visible;
                }
                else
                {
                    PathWatermark = Visibility.Hidden;
                }
                OnPropertyChanged(nameof(CurrentPath));
            }
        }
    }

    public int PozInList
    {
        get { return _pozInList; }
        set
        {
            if (_pozInList != value)
            {
                _pozInList = value;
                OnPropertyChanged(nameof(PozInList));
            }
        }
    }
    public string CurrentPathDisplayed
    {
        get { return _currentPathDisplayed; }
        set
        {
            if (_currentPathDisplayed != value)
            {
                _currentPathDisplayed = value;
                if (_currentPathDisplayed == "")
                {
                    PathWatermark = Visibility.Visible;
                }
                else
                {
                    PathWatermark = Visibility.Hidden;
                }
                OnPropertyChanged(nameof(CurrentPathDisplayed));
            }
        }
    }

    public Visibility ConversionNameWattermark
    {
        get { return _conversionNameWattermark; }
        set
        {
            if (_conversionNameWattermark != value)
            {
                _conversionNameWattermark = value;
                OnPropertyChanged(nameof(ConversionNameWattermark));
            }
        }
    }

    public Visibility ConversionPathWattermark
    {
        get { return _conversionPathWattermark; }
        set
        {
            if (_conversionPathWattermark != value)
            {
                _conversionPathWattermark = value;
                OnPropertyChanged(nameof(ConversionPathWattermark));
            }
        }
    }
    public Visibility CategoryNameWattermark
    {
        get { return _categoryNameWattermark; }
        set
        {
            if (_categoryNameWattermark != value)
            {
                _categoryNameWattermark = value;
                OnPropertyChanged(nameof(CategoryNameWattermark));
            }
        }
    }

    public Visibility PathWatermark
    {
        get { return _pathWatermark; }
        set
        {
            if (_pathWatermark != value)
            {
                _pathWatermark = value;
                OnPropertyChanged(nameof(PathWatermark));
            }
        }
    }

    public Visibility SearchingWordWatermark
    {
        get { return _searchingWordWatermark; }
        set
        {
            if (_searchingWordWatermark != value)
            {
                _searchingWordWatermark = value;
                OnPropertyChanged(nameof(SearchingWordWatermark));
            }
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

    #region Switch views
    private RelayCommand _toggleGridVisibilityCommand;

    public RelayCommand ToggleGridVisibilityCommand
    {
        get
        {
            if (_toggleGridVisibilityCommand == null)
            {
                _toggleGridVisibilityCommand = new RelayCommand(ToggleGridVisibility);
            }
            return _toggleGridVisibilityCommand;
        }
    }

    private Tuple<bool, bool, bool, bool> _isGridVisible = new Tuple<bool, bool, bool, bool>(false, false, false, false);

    public Tuple<bool, bool, bool, bool> IsGridVisible
    {
        get => _isGridVisible;
        set
        {
            if (_isGridVisible != value)
            {
                _isGridVisible = value;
                OnPropertyChanged(nameof(IsGridVisible));
            }
        }
    }
    private void ToggleGridVisibility(object obj)
    {
        if (obj is string gridName)
        {
            if (gridName == "ConverterSpace")
            {
                IsGridVisible = new Tuple<bool, bool, bool, bool>(!IsGridVisible.Item1, false, false, false);
            }
            else if (gridName == "CategorySpace")
            {
                IsGridVisible = new Tuple<bool, bool, bool, bool>(false, !IsGridVisible.Item2, false, false);
            }
            else if (gridName == "FilterSpace")
            {
                IsGridVisible = new Tuple<bool, bool, bool, bool>(false, false, !IsGridVisible.Item3, false);
            }
            else if (gridName == "LanguageSpace")
            {
                IsGridVisible = new Tuple<bool, bool, bool, bool>(false, false, false, !IsGridVisible.Item4);
            }
        }
    }


    #endregion

    public string CategoryName
    {
        get => _categoryName;
        set
        {
            if (_categoryName != value)
            {
                _categoryName = value;
                if (_categoryName == "")
                {
                    CategoryNameWattermark = Visibility.Visible;
                }
                else
                {
                    CategoryNameWattermark = Visibility.Hidden;
                }
                OnPropertyChanged(nameof(CategoryName));
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
    public Category DefinedCategory
    {
        get => _definedCategory;
        set
        {
            _definedCategory = value;
            OnPropertyChanged(nameof(DefinedCategory));
        }
    }
    public ObservableCollection<string> PriorityList
    {
        get { return _priorityList; }
        set
        {
            if (_priorityList != value)
            {
                _priorityList = value;
                OnPropertyChanged(nameof(PriorityList));
            }
        }
    }
    public ObservableCollection<string> PriorityWithoutNone
    {
        get
        {
            return new ObservableCollection<string>(_priorityList.Where(c => c != "None" && c != "All Items"));
        }
    }

    public ObservableCollection<string> Languages
    {
        get { return _languages; }
        set
        {
            if (_languages != value)
            {
                _languages = value;
                OnPropertyChanged(nameof(Languages));
            }
        }
    }

    public ObservableCollection<string> CodeLanguages
    {
        get { return _codeLanguages; }
        set
        {
            if (_codeLanguages != value)
            {
                _codeLanguages = value;
                OnPropertyChanged(nameof(CodeLanguages));
            }
        }
    }

    public ObservableCollection<string> PriorityListWithoutNone
    {
        get
        {
            return new ObservableCollection<string>(_priorityList.Where(c => c != "None"));
        }
        set
        {
            if (_priorityList != value)
            {
                _priorityList = value;
                OnPropertyChanged(nameof(PriorityListWithoutNone));
            }
        }
    }

    public void UpdateData(TransmittedData data)
    {
        CurrentData = data;
    }
    public RelayCommand NavigateToAddCategoryPageCommand { get; set; }
    public ViewerPageVM()
    {
        //if (CurrentData.IsFilteredItemsListBoxVisible)
        //{
        //    CurrentData.CurrentListBoxSource = CurrentData.CurrentListBoxSource;
        //}
        //else
        //{
        //    CurrentData.CurrentListBoxSource = CurrentData.CurrentListBoxSource;
        //}
        ColorsAvailable = new ObservableCollection<Category>();

        foreach (var property in typeof(Brushes).GetProperties())
        {
            if (property.PropertyType == typeof(SolidColorBrush))
            {
                SolidColorBrush colorBrush = (SolidColorBrush)property.GetValue(null, null);
                string colorName = property.Name;

                ColorsAvailable.Add(new Category { Name = colorName, SolidColorBrushColor = colorBrush });
            }
        }
        DefinedCategory = new Category();
        SettingsDatas = new Settings(ViewerPageHelper.TakeInfosForSettings(this));
        CurrentData.Categories.Add(new Category { Name = "White", SolidColorBrushColor = Brushes.White, CategoryName = "All Items" });
        CurrentData.Categories.Add(new Category { Name = "White", SolidColorBrushColor = Brushes.White, CategoryName = "None" });
    }
    public ViewerPageCommands Commands
    {
        get
        {
            if (_commands == null)
            {
                _commands = new ViewerPageCommands(this);
            }
            return _commands;
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;
    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
