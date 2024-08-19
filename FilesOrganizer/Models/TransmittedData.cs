using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace FilesOrganizer.Models;

public class TransmittedData: INotifyPropertyChanged
{
    private ObservableCollection<Element> _allItems = new ObservableCollection<Element>();
    private ObservableCollection<Category> _categories = new ObservableCollection<Category>();
    private List<string> _backStack = new List<string>();
    private ObservableCollection<Element> _currentListBoxSource;
    private bool _driveOrLocal = false;
    private string _spacePath;


    #region Properties

    public bool DriveOrLocal
    {
        get { return _driveOrLocal; }
        set
        {
            if (_driveOrLocal != value)
            {
                _driveOrLocal = value;
                OnPropertyChanged(nameof(DriveOrLocal));
            }
        }
    }

    //public ObservableCollection<Element> FiltersResults
    //{
    //    get { return _filtersResults; }
    //    set
    //    {
    //        if (_filtersResults != value)
    //        {
    //            _filtersResults = value;
    //            OnPropertyChanged(nameof(FiltersResults));
    //        }
    //    }
    //}
    //public ObservableCollection<Element> SearchResults
    //{
    //    get { return _searchResults; }
    //    set
    //    {
    //        if (_searchResults != value)
    //        {
    //            _searchResults = value;
    //            OnPropertyChanged(nameof(SearchResults));
    //        }
    //    }
    //}
    public ObservableCollection<Element> CurrentListBoxSource
    {
        get { return _currentListBoxSource; }
        set
        {
            if (_currentListBoxSource != value)
            {
                _currentListBoxSource = value;
                OnPropertyChanged(nameof(CurrentListBoxSource));
            }
        }
    }
    //public bool IsItemsListBoxVisible
    //{
    //    get { return _isItemsListBoxVisible; }
    //    set
    //    {
    //        if (_isItemsListBoxVisible != value)
    //        {
    //            _isItemsListBoxVisible = value;
    //            OnPropertyChanged(nameof(IsItemsListBoxVisible));

    //            IsFilteredItemsListBoxVisible = !value;
    //        }
    //    }
    //}

    //public bool IsFilteredItemsListBoxVisible
    //{
    //    get { return _isFilteredItemsListBoxVisible; }
    //    set
    //    {
    //        if (_isFilteredItemsListBoxVisible != value)
    //        {
    //            _isFilteredItemsListBoxVisible = value;
    //            OnPropertyChanged(nameof(IsFilteredItemsListBoxVisible));

    //            //IsItemsListBoxVisible = !value;
    //        }
    //    }
    //}

    //public ObservableCollection<Element> FilteredItems
    //{
    //    get { return _filteredItems; }
    //    set
    //    {
    //        if (_filteredItems != value)
    //        {
    //            _filteredItems = value;
    //            OnPropertyChanged(nameof(FilteredItems));
    //        }
    //    }
    //}
    public ObservableCollection<Category> Categories
    {
        get { return _categories; }
        set
        {
            if (_categories != value)
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }
    }
    public ObservableCollection<Category> CategoriesWithoutNone
    {
        get
        {
            return new ObservableCollection<Category>(_categories.Where(c => c.CategoryName != "None" && c.CategoryName!= "All Items"));
        }
    }

    //public ObservableCollection<Element> Items
    //{
    //    get { return _items; }
    //    set
    //    {
    //        if (_items != value)
    //        {
    //            _items = value;
    //            CurrentListBoxSource = value;
    //            OnPropertyChanged(nameof(Items));
    //        }
    //    }
    //}

    public ObservableCollection<Element> AllItems
    {
        get { return _allItems; }
        set
        {
            if (_allItems != value)
            {
                _allItems = value;
                OnPropertyChanged(nameof(AllItems));
            }
        }
    }

    public List<string> BackStack
    {
        get { return _backStack; }
        set
        {
            if (_backStack != value)
            {
                _backStack = value;
                OnPropertyChanged(nameof(BackStack));
            }
        }
    }

    public string SpacePath
    {
        get { return _spacePath; }
        set
        {
            if (_spacePath != value)
            {
                _spacePath = value;
                OnPropertyChanged(nameof(SpacePath));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public TransmittedData()
    {
        
    }
    public TransmittedData(ObservableCollection<Element> items, ObservableCollection<Element> allItems, ObservableCollection<Category> categories, ObservableCollection<Element> filteredItems, List<string> backStack, string spacePath, string currentPath, bool isItemsListBoxVisible, bool isFilteredItemsListBoxVisible, ObservableCollection<Element> currentListBoxSource)
    {
        AllItems = allItems;
        Categories = categories;
        //FilteredItems = filteredItems;
        BackStack = backStack;
        SpacePath = spacePath;
        //IsItemsListBoxVisible = isItemsListBoxVisible;
        //IsFilteredItemsListBoxVisible = isFilteredItemsListBoxVisible;
        CurrentListBoxSource = currentListBoxSource;
    }

    public TransmittedData(TransmittedData currentData)
    {
        this.DriveOrLocal = currentData.DriveOrLocal;
        this.AllItems = new ObservableCollection<Element>(currentData.AllItems);
        this.Categories = new ObservableCollection<Category>(currentData.Categories);
        this.BackStack = new List<string>(currentData.BackStack);
        this.SpacePath = currentData.SpacePath;
        //this.IsItemsListBoxVisible = currentData.IsItemsListBoxVisible;
        //this.IsFilteredItemsListBoxVisible = currentData.IsFilteredItemsListBoxVisible;
        this.CurrentListBoxSource = new ObservableCollection<Element>(currentData.CurrentListBoxSource);
    }
    #endregion
}
