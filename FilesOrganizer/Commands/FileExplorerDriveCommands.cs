using FilesOrganizer.Core;
using FilesOrganizer.Models;
using FilesOrganizer.ViewModels;
using FilesOrganizer.ViewModels.Commands;
using FilesOrganizer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FilesOrganizer.Commands;

public class FileExplorerDriveCommands
{
    private readonly FileExplorerDriveVM _fileExplorerDriveVM;

    public FileExplorerDriveCommands(FileExplorerDriveVM fileExplorerDriveVM)
    {
        _fileExplorerDriveVM = fileExplorerDriveVM;
    }

    private ICommand _homeCommand;
    public ICommand HomeCommand
    {
        get
        {
            if (_homeCommand == null)
            {
                _homeCommand = new RelayCommand(HomeCommandImplementation);
            }
            return _homeCommand;
        }
    }
    private void HomeCommandImplementation(object obj)
    {
        _fileExplorerDriveVM.PozInList = 0;
        _fileExplorerDriveVM.CurrentPath = _fileExplorerDriveVM.CurrentData.SpacePath;
        _fileExplorerDriveVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(_fileExplorerDriveVM.CurrentData.AllItems.Where(item => item.Path == _fileExplorerDriveVM.CurrentPath && item.Extension == "Folder").ToList());
        _fileExplorerDriveVM.CurrentData.BackStack.Clear();
        _fileExplorerDriveVM.CurrentData.BackStack.Add(_fileExplorerDriveVM.CurrentPath);
        _fileExplorerDriveVM.CurrentPathDisplayed = _fileExplorerDriveVM.CurrentPath;
    }

    private ICommand _backCommand;
    public ICommand BackCommand
    {
        get
        {
            if (_backCommand == null)
            {
                _backCommand = new RelayCommand(BackCommandImplementation);
            }
            return _backCommand;
        }
    }
    private void BackCommandImplementation(object obj)
    {
        try
        {
            if (_fileExplorerDriveVM.PozInList == 0)
            {
                MessageBox.Show("Cannot go back further. This is the initial path.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _fileExplorerDriveVM.PozInList--;

            _fileExplorerDriveVM.CurrentPath = _fileExplorerDriveVM.CurrentData.BackStack.ElementAt(_fileExplorerDriveVM.PozInList);
            _fileExplorerDriveVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(_fileExplorerDriveVM.CurrentData.AllItems.Where(item => item.Path == _fileExplorerDriveVM.CurrentPath && item.Extension == "Folder").ToList());
            _fileExplorerDriveVM.CurrentPathDisplayed = _fileExplorerDriveVM.CurrentPath;
        }
        catch (UnauthorizedAccessException)
        {
            _fileExplorerDriveVM.CurrentData.AllItems.Clear();
            MessageBox.Show($"Access to the path '{_fileExplorerDriveVM.CurrentPath}' is denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            _fileExplorerDriveVM.CurrentData.AllItems.Clear();
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private ICommand _forwardCommand;
    public ICommand ForwardCommand
    {
        get
        {
            if (_forwardCommand == null)
            {
                _forwardCommand = new RelayCommand(ForwardCommandImplementation);
            }
            return _forwardCommand;
        }
    }
    private void ForwardCommandImplementation(object obj)
    {
        try
        {
            if (_fileExplorerDriveVM.PozInList == _fileExplorerDriveVM.CurrentData.BackStack.Count - 1)
            {
                MessageBox.Show("Already on the end of path.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _fileExplorerDriveVM.PozInList++;

            String currentDir;

            currentDir = _fileExplorerDriveVM.CurrentData.BackStack.ElementAt(_fileExplorerDriveVM.PozInList);

            currentDir = _fileExplorerDriveVM.CurrentData.BackStack.ElementAt(_fileExplorerDriveVM.PozInList);

            if (currentDir.Length < _fileExplorerDriveVM.CurrentData.SpacePath.Length && !_fileExplorerDriveVM.CurrentPath.StartsWith("Filter results"))
            {
                MessageBox.Show("Cannot go back further. This is the initial path.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            _fileExplorerDriveVM.CurrentPath = currentDir;
            _fileExplorerDriveVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(_fileExplorerDriveVM.CurrentData.AllItems.Where(item => item.Path == _fileExplorerDriveVM.CurrentPath && item.Extension == "Folder").ToList());
            _fileExplorerDriveVM.CurrentPathDisplayed = _fileExplorerDriveVM.CurrentPath;
        }
        catch (UnauthorizedAccessException)
        {
            _fileExplorerDriveVM.CurrentData.AllItems.Clear();
            MessageBox.Show($"Access to the path '{_fileExplorerDriveVM.CurrentPath}' is denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            _fileExplorerDriveVM.CurrentData.AllItems.Clear();
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private ICommand _openFolderCommand;
    public ICommand OpenFolderCommand
    {
        get
        {
            if (_openFolderCommand == null)
            {
                _openFolderCommand = new RelayCommand(OpenFolderImplementation);
            }
            return _openFolderCommand;
        }
    }
    private void OpenFolderImplementation(object obj)
    {
        if (obj is Element clickedElement)
        {
            if (clickedElement.Icon == "Folder")
            {
                //_viewerPageVM.CurrentData.DriveOdLocal = false;
                if (_fileExplorerDriveVM != null)
                {
                    try
                    {
                        //sunt pe ultima pozitie, doar adaug in lista
                        if (_fileExplorerDriveVM.PozInList == _fileExplorerDriveVM.CurrentData.BackStack.Count - 1)
                        {
                            _fileExplorerDriveVM.CurrentData.BackStack.Add(clickedElement.Path + "\\" + clickedElement.Name);
                            _fileExplorerDriveVM.CurrentPath = clickedElement.Path + "\\" + clickedElement.Name;
                            _fileExplorerDriveVM.PozInList++;
                            _fileExplorerDriveVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(_fileExplorerDriveVM.CurrentData.AllItems.Where(item => item.Path == _fileExplorerDriveVM.CurrentPath && item.Extension == "Folder").ToList());
                            _fileExplorerDriveVM.CurrentPathDisplayed = _fileExplorerDriveVM.CurrentPath;
                        }
                        else
                        {
                            //daca nu e ultimul am doua cazuri, in care pathul urmator e egal cu ce am dat click si fac salt
                            //sau nu e agal si starg tot pana la pozitia curenta si adaug noul path
                            if (_fileExplorerDriveVM.CurrentData.BackStack.ElementAt(_fileExplorerDriveVM.PozInList + 1) == clickedElement.Path + "\\" + clickedElement.Name)
                            {
                                _fileExplorerDriveVM.PozInList++;
                                _fileExplorerDriveVM.CurrentPath = clickedElement.Path + "\\" + clickedElement.Name;
                                _fileExplorerDriveVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(_fileExplorerDriveVM.CurrentData.AllItems.Where(item => item.Path == _fileExplorerDriveVM.CurrentPath && item.Extension == "Folder").ToList());
                                _fileExplorerDriveVM.CurrentPathDisplayed = _fileExplorerDriveVM.CurrentPath;
                            }
                            else
                            {
                                //remove until pozInList
                                while (_fileExplorerDriveVM.PozInList != _fileExplorerDriveVM.CurrentData.BackStack.Count - 1)
                                {
                                    _fileExplorerDriveVM.CurrentData.BackStack.RemoveAt(_fileExplorerDriveVM.PozInList + 1);
                                }
                                _fileExplorerDriveVM.CurrentData.BackStack.Add(clickedElement.Path + "\\" + clickedElement.Name);
                                _fileExplorerDriveVM.PozInList++;
                                _fileExplorerDriveVM.CurrentPath = clickedElement.Path + "\\" + clickedElement.Name;
                                _fileExplorerDriveVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(_fileExplorerDriveVM.CurrentData.AllItems.Where(item => item.Path == _fileExplorerDriveVM.CurrentPath && item.Extension=="Folder").ToList());
                                _fileExplorerDriveVM.CurrentPathDisplayed = _fileExplorerDriveVM.CurrentPath;
                            }

                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        _fileExplorerDriveVM.CurrentData.AllItems.Clear();
                        MessageBox.Show($"Access to the path '{_fileExplorerDriveVM.CurrentPath}' is denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        _fileExplorerDriveVM.CurrentData.AllItems.Clear();
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("You cant open folders here.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
