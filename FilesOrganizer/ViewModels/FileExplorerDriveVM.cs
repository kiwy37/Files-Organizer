using FilesOrganizer.Commands;
using FilesOrganizer.Models;
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
    private string _currentPathDisplayed;

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

    public FileExplorerDriveVM(string currentPath, TransmittedData currentData, int pozInList)
    {
        CurrentData = new TransmittedData(currentData);
        CurrentPath = new string(currentPath);
        CurrentPathDisplayed = new string(currentPath);
        PozInList = pozInList;
        CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(CurrentData.AllItems.Where(item => item.Path == CurrentPath && item.Extension == "Folder").ToList());
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
