using FilesOrganizer.Core;
using FilesOrganizer.Helpers;
using FilesOrganizer.Models;
using FilesOrganizer.ViewModels.Commands;
using FilesOrganizer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FilesOrganizer.Commands;

public class CreateFolderCommands
{
    private readonly CreateFolderVM _createFolderVM;

    public CreateFolderCommands(CreateFolderVM createFolderVM)
    {
        _createFolderVM = createFolderVM;
    }

    private ICommand _addNewFileCommand;
    public ICommand AddNewFileCommand
    {
        get
        {
            if (_addNewFileCommand == null)
            {
                _addNewFileCommand = new RelayCommand(AddNewFileImplementation);
            }
            return _addNewFileCommand;
        }
    }
    private void AddNewFileImplementation(object obj)
    {
        if (_createFolderVM.PozInList != 0)
        {
            MessageBox.Show("You need to be in the root to be able to add items.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        ViewerPageHelper.AddFileToList(_createFolderVM);
    }    
    
    private ICommand _addNewFileDriveCommand;
    public ICommand AddNewFileDriveCommand
    {
        get
        {
            if (_addNewFileDriveCommand == null)
            {
                _addNewFileDriveCommand = new RelayCommand(AddNewFileDriveImplementation);
            }
            return _addNewFileDriveCommand;
        }
    }
    private void AddNewFileDriveImplementation(object obj)
    {
        if (_createFolderVM.PozInList != 0)
        {
            MessageBox.Show("You need to be in the root to be able to add items.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        ViewerPageHelper.AddFileToListDrive(_createFolderVM);
    }

    private ICommand _navigateOpenFolderinCreateFolderCommand;

    public ICommand NavigateOpenFolderInCreateFolderCommand
    {
        get
        {
            if (_navigateOpenFolderinCreateFolderCommand == null)
            {
                _navigateOpenFolderinCreateFolderCommand = new RelayCommand(NavigateOpenFolderInCreateFolderImplementation);
            }
            return _navigateOpenFolderinCreateFolderCommand;
        }
    }

    private void NavigateOpenFolderInCreateFolderImplementation(object obj)
    {
        if (obj is Element clickedElement)
        {
            if (clickedElement.Icon == "Folder")
            {
                if (_createFolderVM != null)
                {
                    try
                    {
                        if (_createFolderVM.PozInList == _createFolderVM.BackStack.Count - 1)
                        {
                            _createFolderVM.BackStack.Add(clickedElement.Path + "\\" + clickedElement.Name);
                            _createFolderVM.PozInList++;
                            ListElementsInCurrentPath(_createFolderVM.BackStack.Last());
                        }
                        else
                        {
                            //daca nu e ultimul am doua cazuri, in care pathul urmator e egal cu ce am dat click si fac salt
                            //sau nu e agal si starg tot pana la pozitia curenta si adaug noul path
                            if (_createFolderVM.BackStack.ElementAt(_createFolderVM.PozInList + 1) == clickedElement.Path + "\\" + clickedElement.Name)
                            {
                                _createFolderVM.PozInList++;
                                ListElementsInCurrentPath(_createFolderVM.BackStack.Last());
                            }
                            else
                            {
                                //remove until pozInList
                                while (_createFolderVM.PozInList + 1 != _createFolderVM.BackStack.Count)
                                {
                                    _createFolderVM.BackStack.RemoveAt(_createFolderVM.PozInList + 1);
                                }
                                _createFolderVM.BackStack.Add(clickedElement.Path + "\\" + clickedElement.Name);
                                _createFolderVM.PozInList++;
                                ListElementsInCurrentPath(_createFolderVM.BackStack.Last());
                            }

                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        _createFolderVM.CurrentData.AllItems.Clear();
                        MessageBox.Show($"Access to the path is denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        _createFolderVM.CurrentData.AllItems.Clear();
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("You cant open folders here.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                try
                {
                    if (System.IO.File.Exists(clickedElement.Path + "\\" + clickedElement.Name + clickedElement.Extension))
                    {
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = clickedElement.Path + "\\" + clickedElement.Name + clickedElement.Extension,
                            UseShellExecute = true
                        };

                        // Start the process
                        System.Diagnostics.Process.Start(startInfo);
                    }
                    //else if(_viewerPageVM.CurrentData.DriveOrLocal)
                    //{
                    //    string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                    //    solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
                    //    solutionDirectory = Path.Combine(solutionDirectory, clickedElement.Path);
                    //    HelperDrive.DownloadFile(_viewerPageVM.Service, clickedElement.Id, solutionDirectory);
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private ICommand _navigateBackInCreateFolderCommand;

    public ICommand NavigateBackInCreateFolderCommand
    {
        get
        {
            if (_navigateBackInCreateFolderCommand == null)
            {
                _navigateBackInCreateFolderCommand = new RelayCommand(NavigateBackInCreateFolderImplementation);
            }
            return _navigateBackInCreateFolderCommand;
        }
    }

    private void NavigateBackInCreateFolderImplementation(object obj)
    {
        try
        {
            if (_createFolderVM.PozInList == 0)
            {
                MessageBox.Show("Cannot go back further. This is the initial path.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _createFolderVM.PozInList--;

            if (_createFolderVM.BackStack.ElementAt(_createFolderVM.PozInList).StartsWith("Filter results"))
            {
                ViewerPageHelper.FilterItemsCreateFolder(_createFolderVM, _createFolderVM.InitPath);
            }
            else
            {
                ListElementsInCurrentPath(_createFolderVM.BackStack.ElementAt(_createFolderVM.PozInList));
            }
        }
        catch (UnauthorizedAccessException)
        {
            MessageBox.Show($"Access to the path is denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ListElementsInCurrentPath(string actualPath = "")
    {
        if (actualPath != "")
        {
            if (!(bool)_createFolderVM.FilterType)
            {
                _createFolderVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(_createFolderVM.CurrentData.AllItems.Where(item => item.Path == actualPath &&
                    ((_createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || _createFolderVM.SearchApplied == false) &&
                    ((_createFolderVM.SelectedPriority == null || _createFolderVM.SelectedPriority == "All Items") || (_createFolderVM.SelectedPriority != "All Items" && item.Priority == _createFolderVM.SelectedPriority)) &&
                    ((_createFolderVM.SelectedCategory == null || _createFolderVM.SelectedCategory.CategoryName == "All Items") || (_createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.SolidColorBrushColor.ToString() == _createFolderVM.SelectedCategory.SolidColorBrushColor.ToString() && c.CategoryName == _createFolderVM.SelectedCategory.CategoryName))) &&
                    ((_createFolderVM.SelectedLanguage == null || _createFolderVM.SelectedLanguage == "All Items") || (_createFolderVM.SelectedLanguage != "All Items" && item.Language == _createFolderVM.SelectedLanguage)) &&
                    ((_createFolderVM.SelectedCodeLanguage == null || _createFolderVM.SelectedCodeLanguage == "All Items") || (_createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == _createFolderVM.SelectedCodeLanguage))
                ).ToList());
            }
            else
            {
                _createFolderVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(_createFolderVM.CurrentData.AllItems.Where(item => item.Path == actualPath &&
                    ((_createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || _createFolderVM.SearchApplied == false) &&
                    ((_createFolderVM.SelectedPriority == null || _createFolderVM.SelectedPriority == "All Items") || (_createFolderVM.SelectedPriority != "All Items" && item.Priority == _createFolderVM.SelectedPriority)) &&
                    ((_createFolderVM.SelectedCategory == null || _createFolderVM.SelectedCategory.CategoryName == "All Items") || (_createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.SolidColorBrushColor.ToString() == _createFolderVM.SelectedCategory.SolidColorBrushColor.ToString() && c.CategoryName == _createFolderVM.SelectedCategory.CategoryName))) &&
                    ((_createFolderVM.SelectedLanguage == null || _createFolderVM.SelectedLanguage == "All Items") || (_createFolderVM.SelectedLanguage != "All Items" && item.Language == _createFolderVM.SelectedLanguage)) &&
                    ((_createFolderVM.SelectedCodeLanguage == null || _createFolderVM.SelectedCodeLanguage == "All Items") || (_createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == _createFolderVM.SelectedCodeLanguage))
                ).ToList());
            }

        }
    }

    private ICommand _nextCreateFolderCommand;
    public ICommand NextCreateFolderCommand
    {
        get
        {
            if (_nextCreateFolderCommand == null)
            {
                _nextCreateFolderCommand = new RelayCommand(NextCreateFolderImplementation);
            }
            return _nextCreateFolderCommand;
        }
    }
    private void NextCreateFolderImplementation(object obj)
    {
        var createFolderNameAndPathVM = new CreateFolderNameAndPathVM(_createFolderVM.CurrentData.DriveOrLocal,_createFolderVM.CurrentData.SpacePath, _createFolderVM.CurrentData, _createFolderVM.PozInList, _createFolderVM.AllItems);

        //check _createFolderVM.CurrentData.AllItems si AllItems
        CreateFolderNameAndPathWindow createFolderNameAndPathWindow = new CreateFolderNameAndPathWindow { DataContext = createFolderNameAndPathVM };
        createFolderNameAndPathVM.CloseAction = new Action(() =>
        {
            createFolderNameAndPathWindow.Close();
            ViewerPageHelper.CreateNewFolder(_createFolderVM, createFolderNameAndPathVM.FolderName, createFolderNameAndPathVM.FolderPath);
            _createFolderVM.NewFolderPath = createFolderNameAndPathVM.FolderPath + "\\" + createFolderNameAndPathVM.FolderName;
        });

        createFolderNameAndPathWindow.ShowDialog();
    }

    private ICommand _deleteElementCommand;

    public ICommand DeleteElementCommand
    {
        get
        {
            if (_deleteElementCommand == null)
            {
                _deleteElementCommand = new RelayCommand(DeleteElementImplementation);
            }
            return _deleteElementCommand;
        }
    }

    private void DeleteElementImplementation(object obj)
    {
        _createFolderVM.CurrentData.CurrentListBoxSource.Remove(_createFolderVM.SelectedElement);
    }
}
