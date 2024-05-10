using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace FilesOrganizer.ViewModels.Commands;

public class CreateFolderNameAndPathVM: Core.ViewModel, INotifyPropertyChanged
{
    private string _folderName;
    private string _folderPath;
    private Visibility _folderNameWattermark;
    private Visibility _folderPathWattermark;
    private Commands _commands;
    public Action CloseAction { get; set; }

    public CreateFolderNameAndPathVM()
    {
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
            if(_folderName != value)
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
        CloseAction?.Invoke();
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
