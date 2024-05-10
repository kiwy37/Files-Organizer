using DiffPlex.WindowsForms.Controls;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
using FilesOrganizer.Core;
using FilesOrganizer.Models;
using ImageMagick;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FilesOrganizer.ViewModels.Commands;

public class SimilarFilesVM: Core.ViewModel, INotifyPropertyChanged
{
    public readonly List<string> plainTextFileExtensions = new List<string> { ".rtf", ".txt", ".json", ".odt", ".doc", ".docx", ".xls", ".xlsx", ".html", ".htm", ".pdf", ".xml", ".ppt", ".pptx" };
    public readonly List<string> imageExtensions = new List<string> { ".avif", ".gif", ".heif", ".jpeg", ".jpg", ".png", ".raw", ".svg", ".tiff", ".heic" };
    private int _positionInList;
    private TransmittedData _currentData;
    private Commands _commands;
    private Element _selectedElement;
    private ObservableCollection<ObservableCollection<Element>> _similarElements;
    private IList _selectedItems;
    private bool _allFileVisibility = true;
    private bool _diffplexVisibility = false;
    private bool _picsVisibility = false;
    private ImageSource _imageSource1 =null;
    private ImageSource _imageSource2 =null;
    private double _zoomLevel = 1;
    private float _similarityThreshold = 0;

    public DiffViewer DiffView { get; set; }

    public SimilarFilesVM(TransmittedData transmittedData, int similarityThreshold)
    {
        SimilarityThreshold = (float)(100-similarityThreshold)/100;
        _currentData = new TransmittedData(transmittedData);

        //spargem in fisiere cu text si imagini
        //var textFiles = new ObservableCollection<Element>(_currentData.AllItems.Where(x => plainTextFileExtensions.Contains(x.Extension)).ToList());
        //var imageFiles = new ObservableCollection<Element>(_currentData.AllItems.Where(x => imageExtensions.Contains(x.Extension)).ToList());

        //    var clusters = new ObservableCollection<ObservableCollection<Element>>(allItems
        //.Where(item => item.Extension != "Folder")
        //.Select(item => new ObservableCollection<Element> { item }));

        var textFiles = new ObservableCollection<ObservableCollection<Element>>(_currentData.AllItems.Where(x => plainTextFileExtensions.Contains(x.Extension)).Select(x => new ObservableCollection<Element> { x }).ToList());
        var imageFiles = new ObservableCollection<ObservableCollection<Element>>(_currentData.AllItems.Where(x => imageExtensions.Contains(x.Extension)).Select(x => new ObservableCollection<Element> { x }).ToList());

        var resultText = HelperSimilarFiles.HierarchicalClustering(textFiles, SimilarityThreshold, false);          // 0 - identic     1 - different
        var resultImages = HelperSimilarFiles.HierarchicalClustering(imageFiles, SimilarityThreshold, true);          // 0 - identic     1 - different

        SimilarElements = new ObservableCollection<ObservableCollection<Element>>(resultText.Concat(resultImages));

        if (SimilarElements.Count != 0)
        {
            CurrentData.CurrentListBoxSource = SimilarElements[0];
        }
        else
        {
            CurrentData.CurrentListBoxSource = null;
        }
        PositionInList = 0;
        //HelperSimilarFiles.ShowSimilarElements(SimilarElements);
    }
    public float SimilarityThreshold
    {
        get { return _similarityThreshold; }
        set
        {
            if (_similarityThreshold != value)
            {
                _similarityThreshold = value;
                OnPropertyChanged(nameof(SimilarityThreshold));
            }
        }
    }
    public double ZoomLevel
    {
        get { return _zoomLevel; }
        set
        {
            if (_zoomLevel != value)
            {
                _zoomLevel = value;
                OnPropertyChanged(nameof(ZoomLevel));
            }
        }
    }
    public ImageSource ImageSource1
    {
        get { return _imageSource1; }
        set
        {
            if (_imageSource1 != value)
            {
                _imageSource1 = value;
                OnPropertyChanged(nameof(ImageSource1));
            }
        }
    }

    public ImageSource ImageSource2
    {
        get { return _imageSource2; }
        set
        {
            if (_imageSource2 != value)
            {
                _imageSource2 = value;
                OnPropertyChanged(nameof(ImageSource2));
            }
        }
    }

    public bool AllFileVisibility
    {
        get { return _allFileVisibility; }
        set
        {
            if (_allFileVisibility != value)
            {
                _allFileVisibility = value;
                OnPropertyChanged(nameof(AllFileVisibility));
            }
        }
    }

    public bool DiffplexVisibility
    {
        get { return _diffplexVisibility; }
        set
        {
            if (_diffplexVisibility != value)
            {
                _diffplexVisibility = value;
                OnPropertyChanged(nameof(DiffplexVisibility));
            }
        }
    }

    public bool PicsVisibility
    {
        get { return _picsVisibility; }
        set
        {
            if (_picsVisibility != value)
            {
                _picsVisibility = value;
                OnPropertyChanged(nameof(PicsVisibility));
            }
        }
    }

    public IList SelectedItems
    {
        get { return _selectedItems; }
        set
        {
            if (_selectedItems != value)
            {
                _selectedItems = value;
                OnPropertyChanged(nameof(SelectedItems));
            }
        }
    }

    public int PositionInList
    {
        get { return _positionInList; }
        set
        {
            if (_positionInList != value)
            {
                _positionInList = value;
                OnPropertyChanged(nameof(PositionInList));
            }
        }
    }

    public ObservableCollection<ObservableCollection<Element>> SimilarElements
    {
        get { return _similarElements; }
        set
        {
            if (_similarElements != value)
            {
                _similarElements = value;
                OnPropertyChanged(nameof(SimilarElements));
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
}
