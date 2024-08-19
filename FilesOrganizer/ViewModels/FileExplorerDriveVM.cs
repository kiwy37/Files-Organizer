using FilesOrganizer.Commands;
using FilesOrganizer.Helpers;
using FilesOrganizer.Models;
using FilesOrganizer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOrganizer.ViewModels.Commands;

public class FileExplorerDriveVM : Core.ViewModel, INotifyPropertyChanged
{
    private FileExplorerDriveCommands _commands;
    private string _currentPath;
    TransmittedData _currentData = new TransmittedData();
    private int _pozInList;
    private Element _selectedItem;
    private string _selectedPath;
    private string _currentPathDisplayed;
    private string _option;

    public string Option
    {
        get => _option;
        set
        {
            _option = value;
            OnPropertyChanged(nameof(Option));
        }
    }

    public string SelectedPath
    {
        get => _selectedPath;
        set
        {
            _selectedPath = value;
            OnPropertyChanged(nameof(SelectedPath));
        }
    }
    public string CurrentPathDisplayed
    {
        get => _currentPathDisplayed;
        set
        {
            _currentPathDisplayed = value;
            OnPropertyChanged(nameof(CurrentPathDisplayed));
        }
    }
    public Element SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
        }
    }
    public int PozInList
    {
        get => _pozInList;
        set
        {
            _pozInList = value;
        }
    }
    public TransmittedData CurrentData
    {
        get => _currentData;
        set
        {
            _currentData = value;
        }
    }
    public string CurrentPath
    {
        get => _currentPath;
        set
        {
            _currentPath = value;
        }
    }

    public FileExplorerDriveVM(string currentPath, TransmittedData currentData, int pozInList, string FileOrFolder, ObservableCollection<Element> allItems)
    {
        if (currentData.DriveOrLocal)
        {
            CurrentData = new TransmittedData(currentData);
            CurrentPath = new string(currentPath);
            CurrentPathDisplayed = new string(currentPath);
            PozInList = pozInList;
            Option = FileOrFolder;
            CurrentData.AllItems = new ObservableCollection<Element>(allItems);
            if (FileOrFolder == "File")
            {
                CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(CurrentData.AllItems.Where(item => item.Path == CurrentPath).ToList());
            }
            else if (FileOrFolder == "Folder")
            {
                CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(CurrentData.AllItems.Where(item => item.Path == CurrentPath && item.Extension == "Folder").ToList());
            }
        }
        else
        {
            ViewerPageVM temp = new ViewerPageVM();
            HelperDrive.LoadFilesFromGoogleDrive(temp);
            var firstParent = temp.CurrentPath.Split('\\')[0];
            temp.CurrentPath = firstParent;
            temp.CurrentPathDisplayed = firstParent;
            ViewerPageHelper.ListElementsInCurrentPath(temp);

            CurrentData = temp.CurrentData;
            CurrentPath = temp.CurrentPath;
            CurrentPathDisplayed = temp.CurrentPathDisplayed;
            PozInList = 0;
            CurrentData.BackStack.Add(temp.CurrentPath);

            if (FileOrFolder == "File")
            {
                CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(CurrentData.AllItems.Where(item => item.Path == CurrentPath).ToList());
            }
            else if (FileOrFolder == "Folder")
            {
                CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(CurrentData.AllItems.Where(item => item.Path == CurrentPath && item.Extension == "Folder").ToList());
            }
        }
    }

    public FileExplorerDriveCommands Commands
    {
        get
        {
            if (_commands == null)
            {
                _commands = new FileExplorerDriveCommands(this);
            }
            return _commands;
        }
    }
}
