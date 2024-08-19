using FilesOrganizer.Commands;
using FilesOrganizer.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace FilesOrganizer.ViewModels.Commands;

public class CreateFolderNameAndPathVM : Core.ViewModel, INotifyPropertyChanged
{
    bool DriveOrLocal { set; get; }
    string _spacePath;
    int _pozInList;
    TransmittedData _currentData;
    private string _folderName;
    private string _folderPath;
    private Visibility _folderNameWattermark;
    private Visibility _folderPathWattermark;
    private ViewerPageCommands _commands;
    public Action CloseAction { get; set; }

    public CreateFolderNameAndPathVM(bool driveOrLocal, string spacePath, TransmittedData currentData, int pozInList, ObservableCollection<Element> allItems)
    {
        DriveOrLocal = driveOrLocal;
        SpacePath = spacePath;
        CurrentData = new TransmittedData(currentData);
        PozInList = pozInList;
        CurrentData.AllItems = new ObservableCollection<Element> (allItems);
    }

    public string SpacePath
    {
        get => _spacePath;
        set
        {
            _spacePath = value;
            OnPropertyChanged(nameof(SpacePath));
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

    public int PozInList
    {
        get => _pozInList;
        set
        {
            _pozInList = value;
            OnPropertyChanged(nameof(PozInList));
        }
    }

    public Visibility FolderNameWattermark
    {
        get => _folderNameWattermark;
        set
        {
            _folderNameWattermark = value;
            OnPropertyChanged(nameof(FolderNameWattermark));
        }
    }

    public Visibility FolderPathWattermark
    {
        get => _folderPathWattermark;
        set
        {
            _folderPathWattermark = value;
            OnPropertyChanged(nameof(FolderPathWattermark));
        }
    }

    public string FolderName
    {
        get => _folderName;
        set
        {
            if (_folderName != value)
            {
                _folderName = value;
                if (_folderName == "")
                {
                    FolderNameWattermark = Visibility.Visible;
                }
                else
                {
                    FolderNameWattermark = Visibility.Hidden;
                }
                OnPropertyChanged(nameof(FolderName));
            }
        }
    }

    public string FolderPath
    {
        get => _folderPath;
        set
        {
            if (_folderPath != value)
            {
                _folderPath = value;
                if (_folderPath == "")
                {
                    FolderPathWattermark = Visibility.Visible;
                }
                else
                {
                    FolderPathWattermark = Visibility.Hidden;
                }
                OnPropertyChanged(nameof(FolderPath));
            }
        }
    }

    public void ClosePage()
    {
        // Check if the folder name is not null or empty
        if (string.IsNullOrWhiteSpace(FolderName))
        {
            MessageBox.Show("Folder name cannot be empty.");
            return;
        }

        // Check if the folder name contains invalid characters
        char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
        if (FolderName.IndexOfAny(invalidChars) >= 0)
        {
            MessageBox.Show("Folder name contains invalid characters.");
            return;
        }

        // Check if the folder path is not null or empty
        if (string.IsNullOrWhiteSpace(FolderPath))
        {
            MessageBox.Show("Folder path cannot be empty.");
            return;
        }

        if (DriveOrLocal)
        {
            bool ok = false;
            for (int i = 0; i < CurrentData.AllItems.Count; i++)
            {
                if ((CurrentData.AllItems[i].Path + "\\" + CurrentData.AllItems[i].Name == FolderPath && CurrentData.AllItems[i].Extension == "Folder") || CurrentData.AllItems[i].Path == FolderPath)
                {
                    ok = true;
                }

                if (CurrentData.AllItems[i].Path + "\\" + CurrentData.AllItems[i].Name == FolderPath +"\\"+ FolderName && CurrentData.AllItems[i].Extension == "Folder")
                {
                    MessageBox.Show("A folder with this name already exists in the specified path.");
                    return;
                }
            }


            if (ok == false)
            {
                MessageBox.Show("Folder path does not exist.");
                return;
            }

            //string fullFolderPath = Path.Combine(FolderPath, FolderName);
            //if (Directory.Exists(fullFolderPath))
            //{
            //    MessageBox.Show("A folder with this name already exists in the specified path.");
            //    return;
            //}
        }
        else
        {
            // Check if the folder path exists
            if (!Directory.Exists(FolderPath))
            {
                MessageBox.Show("Folder path does not exist.");
                return;
            }

            string fullFolderPath = Path.Combine(FolderPath, FolderName);
            if (Directory.Exists(fullFolderPath))
            {
                MessageBox.Show("A folder with this name already exists in the specified path.");
                return;
            }

            // If all checks pass, invoke the CloseAction
        }
        CloseAction?.Invoke();
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
