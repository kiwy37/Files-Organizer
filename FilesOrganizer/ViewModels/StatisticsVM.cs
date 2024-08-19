using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Windows;
using FilesOrganizer.Models;
using LiveCharts;
using LiveCharts.Wpf;
using System.Linq;
using System;
using FilesOrganizer.Commands;

namespace FilesOrganizer.ViewModels.Commands;

public class StatisticsVM : Core.ViewModel, INotifyPropertyChanged
{
    public SeriesCollection FileSizeByExtension { get; set; }
    private ObservableCollection<Element> _elementsRecentAccessed = new ObservableCollection<Element>();
    private ObservableCollection<Element> _elementsLeastAccessed = new ObservableCollection<Element>();
    ObservableCollection<Element> _allElements = new ObservableCollection<Element>();
    private Element _selectedElement;
    private int _integerUpDown;
    public Dictionary<string, List<Element>> FileExtension { get; set; }
    private ViewerPageCommands _commands;
    private bool _isCountChecked;
    private bool _isSizeChecked;
    public bool DriveOrLocal { set; get; }
    public long TotalData
    {
        get
        {
            if (IsSizeChecked)
            {
                return _allElements.Sum(e => e.Size);
            }
            else
            {
                return _allElements.Count;
            }
        }
    }



    public StatisticsVM(bool driveOrLocal, Element selectedItem, ObservableCollection<Element> allElements)
    {
        var path = selectedItem.Path + "\\" + selectedItem.Name;
        AllElements = new ObservableCollection<Element>(allElements.Where(e => e.Path.StartsWith(path)));

        FileSizeByExtension = new SeriesCollection();
        FileExtension = GetFileExtensionCounts(path, AllElements);

        foreach (var extensionCount in FileExtension)
        {
            if (extensionCount.Key != "Folder")
                FileSizeByExtension.Add(new PieSeries
                {
                    Title = extensionCount.Key,
                    Values = new ChartValues<int> { extensionCount.Value.Count },
                    StrokeThickness = 0.5 // Change this value to adjust the thickness
                });
        }
        DriveOrLocal = driveOrLocal;
        if (!DriveOrLocal)
        LastAccessedTime();
        IntegerUpDown = 3;
        _isSizeChecked = false;
        _isCountChecked = true;
    }

    public bool IsCountChecked
    {
        get { return _isCountChecked; }
        set
        {
            if (_isCountChecked != value)
            {
                _isCountChecked = value;
                IsSizeChecked = !value; 
                OnPropertyChanged(nameof(IsCountChecked));
                OnPropertyChanged(nameof(TotalData));
                UpdateChart();
            }
        }
    }

    public bool IsSizeChecked
    {
        get { return _isSizeChecked; }
        set
        {
            if (_isSizeChecked != value)
            {
                _isSizeChecked = value;
                IsCountChecked = !value;
                OnPropertyChanged(nameof(IsSizeChecked));
                OnPropertyChanged(nameof(TotalData));
                UpdateChart();
            }
        }
    }
    public void SetElementSizes()
    {
        foreach (var element in AllElements)
        {
            string filePath = Path.Combine(element.Path, element.Name + element.Extension);
            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                element.Size = fileInfo.Length;
            }
        }
    }

    private void UpdateChart()
    {
        FileSizeByExtension.Clear();

        if (IsCountChecked)
        {
            // Populate the chart based on the count of files
            foreach (var extensionCount in FileExtension)
            {
                if (extensionCount.Key != "Folder")
                    FileSizeByExtension.Add(new PieSeries
                    {
                        Title = extensionCount.Key,
                        Values = new ChartValues<int> { extensionCount.Value.Count },
                        StrokeThickness = 0.5 // Change this value to adjust the thickness
                    });
            }
        }
        else if (IsSizeChecked)
        {
            SetElementSizes();
            // Populate the chart based on the size of files
            foreach (var extensionCount in FileExtension)
            {
                if (extensionCount.Key != "Folder")
                {
                    long totalSize = extensionCount.Value.Sum(e => e.Size);
                    FileSizeByExtension.Add(new PieSeries
                    {
                        Title = extensionCount.Key,
                        Values = new ChartValues<long> { totalSize },
                        StrokeThickness = 0.5 // Change this value to adjust the thickness
                    });
                }
            }
        }
    }


    public int IntegerUpDown
    {
        get { return _integerUpDown; }
        set
        {
            // Calculate the maximum allowed value for IntegerUpDown
            int maxAllowedValue = CalculateValidElementsCount();
            int newValue = Math.Min(value, maxAllowedValue);

            if (_integerUpDown != newValue)
            {
                _integerUpDown = newValue;
                TakeFirstElements();
                OnPropertyChanged(nameof(IntegerUpDown));
            }
        }
    }

    private int CalculateValidElementsCount()
    {
        return AllElements.Count(e => e.LastAccessed != "Unknown");
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
    public ObservableCollection<Element> AllElements
    {
        get { return _allElements; }
        set
        {
            if (_allElements != value)
            {
                _allElements = value;
                OnPropertyChanged(nameof(AllElements));
            }
        }
    }
    public ObservableCollection<Element> ElementsRecentAccessed
    {
        get { return _elementsRecentAccessed; }
        set
        {
            if (_elementsRecentAccessed != value)
            {
                _elementsRecentAccessed = value;
                OnPropertyChanged(nameof(ElementsRecentAccessed));
            }
        }
    }

    public ObservableCollection<Element> ElementsLeastAccessed
    {
        get { return _elementsLeastAccessed; }
        set
        {
            if (_elementsLeastAccessed != value)
            {
                _elementsLeastAccessed = value;
                OnPropertyChanged(nameof(ElementsLeastAccessed));
            }
        }
    }

    public string GetLastAccessedTime(string path)
    {
        try
        {
            var lastAccessedTime = File.GetLastAccessTime(path);
            return lastAccessedTime.ToString();
        }
        catch
        {
            return "Unknown";
        }
    }

    public void TakeFirstElements()
    {
        var sortedElementsAscending = AllElements
            .Where(e => e.LastAccessed != "Unknown")
            .OrderBy(e => DateTime.Parse(e.LastAccessed))
            .ToList();

        var sortedElementsDescending = AllElements
            .Where(e => e.LastAccessed != "Unknown")
            .OrderByDescending(e => DateTime.Parse(e.LastAccessed))
            .ToList();

        ElementsRecentAccessed = new ObservableCollection<Element>(sortedElementsDescending.Take(IntegerUpDown));
        ElementsLeastAccessed = new ObservableCollection<Element>(sortedElementsAscending.Take(IntegerUpDown));
    }
    public void LastAccessedTime()
    {
        foreach (var element in AllElements)
        {
            element.LastAccessed = GetLastAccessedTime(element.Path + "\\" + element.Name + element.Extension);
        }

        var sortedElementsAscending = AllElements
            .Where(e => e.LastAccessed != "Unknown")
            .OrderBy(e => DateTime.Parse(e.LastAccessed))
            .ToList();

        var sortedElementsDescending = AllElements
            .Where(e => e.LastAccessed != "Unknown")
            .OrderByDescending(e => DateTime.Parse(e.LastAccessed))
            .ToList();

        ElementsRecentAccessed = new ObservableCollection<Element>(sortedElementsDescending.Take(IntegerUpDown));
        ElementsLeastAccessed = new ObservableCollection<Element>(sortedElementsAscending.Take(IntegerUpDown));
    }


    public Dictionary<string, List<Element>> GetFileExtensionCounts(string root, ObservableCollection<Element> allElements)
    {
        var result = new Dictionary<string, List<Element>>();

        foreach (var element in allElements)
        {
            if (element.Path.StartsWith(root) && element.Extension != "Folder")
            {
                var extension = string.IsNullOrEmpty(element.Extension) ? "No extension" : element.Extension;

                if (result.ContainsKey(extension))
                {
                    result[extension].Add(element);
                }
                else
                {
                    result[extension] = new List<Element> { element };
                }
            }
        }

        return result;
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
}