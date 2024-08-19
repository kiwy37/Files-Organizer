using DiffPlex.WindowsForms.Controls;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
using FilesOrganizer.Commands;
using FilesOrganizer.Helpers;
using FilesOrganizer.Models;
using ImageMagick;
using iTextSharp.text.pdf;
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
    private ViewerPageCommands _commands;
    private Element _selectedElement;
    private ObservableCollection<ObservableCollection<Element>> _similarElements;
    private IList _selectedItems;
    private bool _allFileVisibility = true;
    private bool _diffplexVisibility = false;
    private bool _picsVisibility = false;
    private ImageSource _imageSource1 =null;
    private ImageSource _imageSource2 =null;
    private double _zoomLevel = 1;
    private float _similarityThreshold;

    public DiffViewer DiffView { get; set; }

    public SimilarFilesVM(TransmittedData transmittedData, int similarityThreshold, string similarCase)
    {
        _currentData = new TransmittedData(transmittedData);
        if (similarCase == "SimilarFiles")
        {
            SimilarityThreshold = (float)(100 - similarityThreshold) / 100;

            var textFiles = new ObservableCollection<Element>(_currentData.AllItems.Where(x => plainTextFileExtensions.Contains(x.Extension)).ToList());

            var clustering = HelperSimilarFiles.ClusterDocuments(transmittedData.DriveOrLocal, SimilarityThreshold, textFiles);

            if (clustering.Count > 0)
            {
                SimilarElements = new ObservableCollection<ObservableCollection<Element>>(clustering);
            }
            else
            {
                SimilarElements = new ObservableCollection<ObservableCollection<Element>>();
            }
        }
        if (similarCase == "EditedPhotos")
        {
            var imageFiles = new ObservableCollection<Element>(_currentData.AllItems.Where(x => imageExtensions.Contains(x.Extension)).ToList());

            SimilarityThreshold = (float)similarityThreshold / 100;

            // Group the images based on the SSIM
            var clusteredImages = HelperSimilarFiles.ClusterPhotosSSIM(SimilarityThreshold, 1, imageFiles);


            if (clusteredImages.Count > 0)
            {
                SimilarElements = new ObservableCollection<ObservableCollection<Element>>(clusteredImages);
            }
            else
            {
                SimilarElements = new ObservableCollection<ObservableCollection<Element>>();
            }
        }
        if (similarCase == "CroppedPhotos")
        {
            var imageFiles = new ObservableCollection<Element>(_currentData.AllItems.Where(x => imageExtensions.Contains(x.Extension)).ToList());

            SimilarityThreshold = (float)similarityThreshold / 100;

            // Group the images based on the SSIM
            var clusteredImages = HelperSimilarFiles.ClusterPhotosCropped(SimilarityThreshold, imageFiles);

            // Assign the clustered images to SimilarElements
            //SimilarElements = clusteredImages;


            if (clusteredImages.Count > 0)
            {
                SimilarElements = new ObservableCollection<ObservableCollection<Element>>(clusteredImages);
            }
            else
            {
                SimilarElements = new ObservableCollection<ObservableCollection<Element>>();
            }
        }
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
