using DiffPlex.Wpf.Controls;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using FilesOrganizer.Core;
using FilesOrganizer.Helpers;
using FilesOrganizer.Models;
using FilesOrganizer.ViewModels;
using FilesOrganizer.ViewModels.Commands;
using FilesOrganizer.Views;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using NPOI.POIFS.FileSystem;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit.Primitives;
using Formatting = Newtonsoft.Json.Formatting;

namespace FilesOrganizer.Commands
{
    public class ViewerPageCommands
    {
        #region Additional Strings For Comboboxes changes
        private string _previousLanguage;
        private string _previousCodeLanguage;
        private Category _previousCategory;
        private string _previousPriority;
        #endregion 


        private readonly SimilarFilesVM _similarFilesVM;
        private readonly CreateFolderNameAndPathVM _createFolderNameAndPathVM;
        private ViewerPageVM _viewerPageVM;
        private readonly SavesVM _savesVM;
        private SavesWindow _savesWindow;
        private readonly StatisticsVM _statisticsVM;
        private readonly LocalOrDriverVM _localOrDriverVM;

        public SavesWindow SavesWindow
        {
            get { return _savesWindow; }
            set { _savesWindow = value; }
        }
        public ViewerPageCommands(LocalOrDriverVM localOrDriverVM)
        {
            _localOrDriverVM = localOrDriverVM;
        }
        public ViewerPageCommands(StatisticsVM statisticsVM)
        {
            _statisticsVM = statisticsVM;
        }
        public ViewerPageCommands(SimilarFilesVM similarFilesVM)
        {
            _similarFilesVM = similarFilesVM;
        }
        public ViewerPageCommands(CreateFolderNameAndPathVM createFolderNameAndPathVM)
        {
            _createFolderNameAndPathVM = createFolderNameAndPathVM;
        }
        public ViewerPageCommands(ViewerPageVM viewerPageVM)
        {
            _viewerPageVM = viewerPageVM;
        }

        public ViewerPageCommands(SavesVM savesVM, ViewerPageVM viewerPageVM)
        {
            _savesVM = savesVM;
            _viewerPageVM = viewerPageVM;
        }

        #region Test Command
        private ICommand _testCommand;
        public ICommand TestCommand
        {
            get
            {
                if (_testCommand == null)
                {
                    _testCommand = new RelayCommand(TestImplementation);
                }
                return _testCommand;
            }
        }
        private void TestImplementation(object obj)
        {
            var test = _viewerPageVM;
            ViewerPageHelper.DetectCodeLanguagePics(_viewerPageVM);
            _viewerPageVM.CurrentData.OnPropertyChanged(nameof(_viewerPageVM.CurrentData.CurrentListBoxSource));
        }
        #endregion
        #region LoadPath Command
        private ICommand _loadPathCommand;

        public ICommand LoadPathCommand
        {
            get
            {
                if (_loadPathCommand == null)
                {
                    _loadPathCommand = new RelayCommand(LoadPathImplementation);
                }
                return _loadPathCommand;
            }
        }

        private void LoadPathImplementation(object obj)
        {
            ViewerPageHelper.LoadPathImplementation(_viewerPageVM);
        }

        private ICommand _loadLocalPathCommand;
        public ICommand LoadLocalPathCommand
        {
            get
            {
                if (_loadLocalPathCommand == null)
                {
                    _loadLocalPathCommand = new RelayCommand(LoadLocalPathImplementation);
                }
                return _loadLocalPathCommand;
            }
        }
        private void LoadLocalPathImplementation(object obj)
        {
            var folderBrowser = new VistaFolderBrowserDialog();
            if (folderBrowser.ShowDialog() == true)
            {
                string selectedPath = folderBrowser.SelectedPath;

                _viewerPageVM.CurrentData.AllItems.Clear();

                try
                {
                    LoadItems(selectedPath);
                    UpdateTextBox(selectedPath);
                    ListElementsInCurrentPath();
                    _viewerPageVM.CurrentData.SpacePath = new string(_viewerPageVM.CurrentPath);
                    _viewerPageVM.CurrentData.BackStack.Add(_viewerPageVM.CurrentPath);
                    ViewerPageHelper.LanguageAndCodeForElements(_viewerPageVM, _viewerPageVM.SettingsDatas);
                }
                catch (UnauthorizedAccessException)
                {
                    _viewerPageVM.CurrentData.AllItems.Clear();
                    MessageBox.Show($"Access to the path '{selectedPath}' is denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    _viewerPageVM.CurrentData.AllItems.Clear();
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //public static void LoadPathImplementation(ViewerPageVM _viewerPageVM)
        //{


        //    if (localOrDriverVM.ButtonClicked == "Local")
        //    {
        //        _viewerPageVM.CurrentData.DriveOrLocal = false;
        //        var folderBrowser = new VistaFolderBrowserDialog();
        //        if (folderBrowser.ShowDialog() == true)
        //        {
        //            string selectedPath = folderBrowser.SelectedPath;

        //            ViewerPageHelper.LoadItemsAndUpdatePath(_viewerPageVM, selectedPath);
        //        }
        //    }
        //    else if (localOrDriverVM.ButtonClicked == "Drive")
        //    {
        //        _viewerPageVM.CurrentData.DriveOrLocal = true;
        //        _viewerPageVM = HelperDrive.LoadFilesFromGoogleDrive(_viewerPageVM);
        //        var firstParent = _viewerPageVM.CurrentPath.Split('\\')[0];
        //        UpdateTextBox(_viewerPageVM, firstParent);
        //        ListElementsInCurrentPath(_viewerPageVM);
        //        _viewerPageVM.CurrentData.SpacePath = new string(_viewerPageVM.CurrentPath);
        //        _viewerPageVM.CurrentData.BackStack.Add(_viewerPageVM.CurrentPath);
        //        ViewerPageHelper.LanguageAndCodeForElementsDrive(_viewerPageVM, _viewerPageVM.SettingsDatas);
        //    }
        //}

        private void TryLoadPath(string selectedPath)
        {
            _viewerPageVM.CurrentData.AllItems.Clear();

            try
            {
                LoadItems(selectedPath);
                UpdateTextBox(selectedPath);
                ListElementsInCurrentPath();
                _viewerPageVM.CurrentData.SpacePath = new string(_viewerPageVM.CurrentPath);
                _viewerPageVM.CurrentData.BackStack.Add(_viewerPageVM.CurrentPath);
            }
            catch (UnauthorizedAccessException)
            {
                _viewerPageVM.CurrentData.AllItems.Clear();
                MessageBox.Show($"Access to the path '{selectedPath}' is denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                _viewerPageVM.CurrentData.AllItems.Clear();
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void TryLoadPathDrive(string selectedPath)
        {
            _viewerPageVM.CurrentData.AllItems.Clear();

            try
            {
                ViewerPageHelper.ReloadDriveFiles(_viewerPageVM);
            }
            catch (UnauthorizedAccessException)
            {
                _viewerPageVM.CurrentData.AllItems.Clear();
                MessageBox.Show($"Access to the path '{selectedPath}' is denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                _viewerPageVM.CurrentData.AllItems.Clear();
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadItems(string path)     //path - cel de unde trebuie sa ia tot contentul
        {
            string[] files = Directory.GetFiles(path);
            string[] folders = Directory.GetDirectories(path);

            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string extension = Path.GetExtension(file);

                //make extension lower case
                extension = extension.ToLower();

                switch (extension)
                {
                    //audio
                    case ".m4a":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".mp3":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".mpga":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".wav":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".mpeg":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;

                    //video
                    case ".mp4":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".avi":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".mov":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".flv":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".wmv":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".webm":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".mpg":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".3gp":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;

                    //orher files
                    case ".txt":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), extension));
                        break;
                    case ".pdf":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FilePdfBox", new SolidColorBrush(Colors.Red), extension));
                        break;
                    case ".png":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FilePngBox", new SolidColorBrush(Colors.Fuchsia), extension));
                        break;
                    case ".jpg":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "ImageJpgBox", new SolidColorBrush(Colors.Red), extension));
                        break;
                    case ".gif":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileGifBox", new SolidColorBrush(Colors.Green), extension));
                        break;
                    case ".zip":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FolderZip", new SolidColorBrush(Colors.DarkMagenta), extension));
                        break;
                    case ".xls":
                    case ".xlsx":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileExcel", new SolidColorBrush(Colors.DarkGreen), extension));
                        break;
                    case ".ppt":
                    case ".pptx":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FilePowerpoint", new SolidColorBrush(Colors.DarkOrange), extension));
                        break;
                    case ".exe":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "Application", new SolidColorBrush(Colors.DarkBlue), extension));
                        break;
                    case ".doc":
                    case ".docx":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileWord", new SolidColorBrush(Colors.DarkBlue), extension));
                        break;
                    case ".rar":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FolderZip", new SolidColorBrush(Colors.MediumPurple), extension));
                        break;
                    case ".jpeg":
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "ImageJpegBox", new SolidColorBrush(Colors.DarkRed), extension));
                        break;
                    default:
                        _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "File", new SolidColorBrush(Colors.LightSlateGray), extension));
                        break;
                }
            }

            foreach (var folder in folders)
            {
                string folderName = Path.GetFileName(folder);
                _viewerPageVM.CurrentData.AllItems.Add(new Element(path, folderName, "Folder", new SolidColorBrush(Colors.DodgerBlue), "Folder"));

                LoadItems(folder);
            }
        }
        private void UpdateTextBox(string path)
        {
            _viewerPageVM.CurrentPath = path;
            _viewerPageVM.CurrentPathDisplayed = path;
        }
        private void ListElementsInCurrentPath(string actualPath = "")
        {
            if (actualPath == "")
            {
                _viewerPageVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(_viewerPageVM.CurrentData.AllItems.Where(item => item.Path == _viewerPageVM.CurrentPath).ToList());
            }
        }
        #endregion
        #region OpenFolder Command
        private ICommand _selectionChangedCommand;
        public ICommand SelectionChangedCommand
        {
            get
            {
                if (_selectionChangedCommand == null)
                {
                    _selectionChangedCommand = new RelayCommand(SelectionChangedImplementation);
                }
                return _selectionChangedCommand;
            }
        }
        private void SelectionChangedImplementation(object obj)
        {
            // Assuming _viewerPageVM.SelectedItems is already correctly populated
            // through the binding with the UI (e.g., using ListBoxExtensions for multi-selection binding).

            // Check if there are selected items
            if (_viewerPageVM.SelectedItems != null)
            {
                // If exactly one item is selected, set it as the SelectedItem
                if (_viewerPageVM.SelectedItems.Count == 1)
                {
                    _viewerPageVM.SelectedItem = _viewerPageVM.SelectedItems[0] as Element; // Assuming the items are of type Element
                }
                else
                {
                    // If there are multiple items selected, set SelectedItem to null
                    _viewerPageVM.SelectedItem = null;
                }
            }
            else
            {
                // If for some reason SelectedItems is null, ensure SelectedItem is also set to null
                _viewerPageVM.SelectedItem = null;
            }
            _viewerPageVM.OnPropertyChanged(nameof(_viewerPageVM.SelectedItem));
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
                    if (_viewerPageVM != null)
                    {
                        try
                        {
                            //sunt pe ultima pozitie, doar adaug in lista
                            if (_viewerPageVM.PozInList == _viewerPageVM.CurrentData.BackStack.Count - 1)
                            {
                                _viewerPageVM.CurrentData.BackStack.Add(clickedElement.Path + "\\" + clickedElement.Name);
                                _viewerPageVM.CurrentPath = clickedElement.Path + "\\" + clickedElement.Name;
                                _viewerPageVM.PozInList++;
                                ListElementsInCurrentPath();
                                UpdateTextBox(_viewerPageVM.CurrentPath);
                            }
                            else
                            {
                                //daca nu e ultimul am doua cazuri, in care pathul urmator e egal cu ce am dat click si fac salt
                                //sau nu e agal si starg tot pana la pozitia curenta si adaug noul path
                                if (_viewerPageVM.CurrentData.BackStack.ElementAt(_viewerPageVM.PozInList + 1) == clickedElement.Path + "\\" + clickedElement.Name)
                                {
                                    _viewerPageVM.PozInList++;
                                    _viewerPageVM.CurrentPath = clickedElement.Path + "\\" + clickedElement.Name;
                                    ListElementsInCurrentPath();
                                    UpdateTextBox(_viewerPageVM.CurrentPath);
                                }
                                else
                                {
                                    //remove until pozInList
                                    while (_viewerPageVM.PozInList != _viewerPageVM.CurrentData.BackStack.Count - 1)
                                    {
                                        _viewerPageVM.CurrentData.BackStack.RemoveAt(_viewerPageVM.PozInList + 1);
                                    }
                                    _viewerPageVM.CurrentData.BackStack.Add(clickedElement.Path + "\\" + clickedElement.Name);
                                    _viewerPageVM.PozInList++;
                                    _viewerPageVM.CurrentPath = clickedElement.Path + "\\" + clickedElement.Name;
                                    ListElementsInCurrentPath();
                                    UpdateTextBox(_viewerPageVM.CurrentPath);
                                }

                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                            _viewerPageVM.CurrentData.AllItems.Clear();
                            MessageBox.Show($"Access to the path '{_viewerPageVM.CurrentPath}' is denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        catch (Exception ex)
                        {
                            _viewerPageVM.CurrentData.AllItems.Clear();
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
                        if (File.Exists(clickedElement.Path + "\\" + clickedElement.Name + clickedElement.Extension))
                        {
                            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = clickedElement.Path + "\\" + clickedElement.Name + clickedElement.Extension,
                                UseShellExecute = true
                            };

                            // Start the process
                            System.Diagnostics.Process.Start(startInfo);
                        }
                        else if (File.Exists(clickedElement.Path + "\\" + clickedElement.Name))
                        {
                            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = clickedElement.Path + "\\" + clickedElement.Name + clickedElement.Extension,
                                UseShellExecute = true
                            };

                            // Start the process
                            System.Diagnostics.Process.Start(startInfo);
                        }
                        else if (_viewerPageVM.CurrentData.DriveOrLocal)
                        {
                            if (clickedElement.Id == null)
                            {
                                string solutionDirectoryy = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                                solutionDirectoryy = Path.Combine(solutionDirectoryy, "DriveDownloads");
                                //var nam = Path.GetFileNameWithoutExtension(clickedElement.Name);


                                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = solutionDirectoryy + "\\" + clickedElement.Path + "\\" + clickedElement.Name + clickedElement.Extension,
                                    UseShellExecute = true
                                };

                                // Start the process
                                System.Diagnostics.Process.Start(startInfo);
                            }
                            else
                            {
                                //conditie ca poate exista
                                string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                                solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
                                solutionDirectory = Path.Combine(solutionDirectory, clickedElement.Path);
                                if (clickedElement.Status == FileStatus.Undownloaded)
                                {
                                    HelperDrive.DownloadFile(clickedElement.Id, solutionDirectory);
                                    clickedElement.Status = FileStatus.Downloaded;
                                }
                                var path = "";

                                //string extension = Path.GetExtension(clickedElement.Name);

                                //if (string.IsNullOrEmpty(extension))
                                //{
                                //    path = Path.Combine(solutionDirectory, clickedElement.Name + clickedElement.Extension);
                                //}
                                //else
                                //{
                                //    path = Path.Combine(solutionDirectory, clickedElement.Name);
                                //}

                                path = Path.Combine(solutionDirectory, clickedElement.Id + clickedElement.Extension);


                                if (File.Exists(path))
                                {
                                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                                    {
                                        FileName = path,
                                        UseShellExecute = true
                                    };

                                    // Start the process
                                    System.Diagnostics.Process.Start(startInfo);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        #endregion
        #region Home Command
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
            _viewerPageVM.PozInList = 0;
            _viewerPageVM.SearchingWord = "";
            _viewerPageVM.SearchApplied = false;
            _viewerPageVM.SelectedCodeLanguage = _viewerPageVM.CodeLanguages[0];
            _viewerPageVM.SelectedLanguage = _viewerPageVM.Languages[0];
            _viewerPageVM.SelectedCategory = _viewerPageVM.CurrentData.Categories[0];
            _viewerPageVM.SelectedPriority = _viewerPageVM.PriorityList[0];
            foreach (var item in _viewerPageVM.CurrentData.AllItems)
            {
                item.Appearance = "";
            }
            _viewerPageVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(
                _viewerPageVM.CurrentData.AllItems.Where(item => item.Path == _viewerPageVM.CurrentData.SpacePath)
            );
            _viewerPageVM.CurrentPath = _viewerPageVM.CurrentData.SpacePath;
            _viewerPageVM.CurrentPathDisplayed = _viewerPageVM.CurrentData.SpacePath;
            _viewerPageVM.CurrentData.BackStack.Clear();
            _viewerPageVM.CurrentData.BackStack.Add(_viewerPageVM.CurrentPath);
        }
        #endregion
        #region Back Command
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
                if (_viewerPageVM.PozInList == 0)
                {
                    MessageBox.Show("Cannot go back further. This is the initial path.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                _viewerPageVM.PozInList--;
                if (_viewerPageVM.CurrentData.BackStack.ElementAt(_viewerPageVM.PozInList).StartsWith("Filter results"))
                {
                    var elements = _viewerPageVM.CurrentData.BackStack.ElementAt(_viewerPageVM.PozInList).Split('-');

                    _viewerPageVM.SettingsDatas.FilterType = bool.Parse(elements[1]);
                    _viewerPageVM.SelectedPriority = elements[2] == "null" ? null : elements[2];

                    _viewerPageVM.SelectedCategory = _viewerPageVM.CurrentData.Categories.FirstOrDefault(c => c.CategoryName == elements[3] && c.SolidColorBrushColor.ToString() == elements[4]);

                    _viewerPageVM.SelectedLanguage = elements[5] == "null" ? null : elements[5];
                    _viewerPageVM.SelectedCodeLanguage = elements[6] == "null" ? null : elements[6];
                    _viewerPageVM.CurrentPathDisplayed = elements[0];

                    //iau current din lista primul element de la capat la coada fara filtrare si dupa pozitie
                    for (int i = _viewerPageVM.PozInList; i >= 0; i--)
                    {
                        if (!_viewerPageVM.CurrentData.BackStack.ElementAt(i).StartsWith("Filter results"))
                        {
                            _viewerPageVM.CurrentPath = _viewerPageVM.CurrentData.BackStack.ElementAt(i);
                            break;
                        }
                    }

                    ViewerPageHelper.FilterItems(_viewerPageVM, true);
                }
                else
                {
                    _viewerPageVM.CurrentPath = _viewerPageVM.CurrentData.BackStack.ElementAt(_viewerPageVM.PozInList);
                    ListElementsInCurrentPath();
                    UpdateTextBox(_viewerPageVM.CurrentPath);
                }
            }
            catch (UnauthorizedAccessException)
            {
                _viewerPageVM.CurrentData.AllItems.Clear();
                MessageBox.Show($"Access to the path '{_viewerPageVM.CurrentPath}' is denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                _viewerPageVM.CurrentData.AllItems.Clear();
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
        #region Forward Command
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
                if (_viewerPageVM.PozInList == _viewerPageVM.CurrentData.BackStack.Count - 1)
                {
                    MessageBox.Show("Already on the end of path.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                _viewerPageVM.PozInList++;

                string currentDir;
                if (_viewerPageVM.CurrentData.BackStack.ElementAt(_viewerPageVM.PozInList).StartsWith("Filter results"))
                {
                    currentDir = _viewerPageVM.CurrentData.BackStack.ElementAt(_viewerPageVM.PozInList);
                    var elements = currentDir.Split('-');

                    _viewerPageVM.SettingsDatas.FilterType = bool.Parse(elements[1]);
                    _viewerPageVM.SelectedPriority = elements[2] == "null" ? null : elements[2];

                    //buba
                    _viewerPageVM.SelectedCategory = _viewerPageVM.CurrentData.Categories.FirstOrDefault(c => c.CategoryName == elements[3] && c.SolidColorBrushColor.ToString() == elements[4]);


                    _viewerPageVM.SelectedLanguage = elements[5] == "null" ? null : elements[5];
                    _viewerPageVM.SelectedCodeLanguage = elements[6] == "null" ? null : elements[6];
                    _viewerPageVM.CurrentPathDisplayed = elements[0];

                    ViewerPageHelper.FilterItems(_viewerPageVM, true);
                }
                else
                {
                    currentDir = _viewerPageVM.CurrentData.BackStack.ElementAt(_viewerPageVM.PozInList);
                    if (currentDir.StartsWith("Filter results"))
                    {
                        var elements = currentDir.Split('-');

                        _viewerPageVM.SettingsDatas.FilterType = bool.Parse(elements[1]);
                        _viewerPageVM.SelectedPriority = elements[2] == "null" ? null : elements[2];

                        //buba
                        _viewerPageVM.SelectedCategory = _viewerPageVM.CurrentData.Categories.FirstOrDefault(c => c.CategoryName == elements[3] && c.SolidColorBrushColor.ToString() == elements[4]);


                        _viewerPageVM.SelectedLanguage = elements[5] == "null" ? null : elements[5];
                        _viewerPageVM.SelectedCodeLanguage = elements[6] == "null" ? null : elements[6];
                        _viewerPageVM.CurrentPathDisplayed = elements[0];

                        _viewerPageVM.CurrentPath = _viewerPageVM.CurrentData.BackStack.ElementAt(_viewerPageVM.PozInList);
                        ViewerPageHelper.FilterItems(_viewerPageVM, true);
                        _viewerPageVM.CurrentPath = _viewerPageVM.CurrentData.BackStack.ElementAt(_viewerPageVM.PozInList);
                    }
                    else
                    {
                        currentDir = _viewerPageVM.CurrentData.BackStack.ElementAt(_viewerPageVM.PozInList);

                        if (currentDir.Length < _viewerPageVM.CurrentData.SpacePath.Length && !_viewerPageVM.CurrentPath.StartsWith("Filter results"))
                        {
                            MessageBox.Show("Cannot go back further. This is the initial path.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        _viewerPageVM.CurrentPath = currentDir;
                        ListElementsInCurrentPath();
                        UpdateTextBox(_viewerPageVM.CurrentPath);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                _viewerPageVM.CurrentData.AllItems.Clear();
                MessageBox.Show($"Access to the path '{_viewerPageVM.CurrentPath}' is denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                _viewerPageVM.CurrentData.AllItems.Clear();
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
        #region Add Category Command
        private ICommand _addCategoryCommand;
        public ICommand AddCategoryCommand
        {
            get
            {
                if (_addCategoryCommand == null)
                {
                    _addCategoryCommand = new RelayCommand(AddCategoryImplementation);
                }
                return _addCategoryCommand;
            }
        }
        private void AddCategoryImplementation(object obj)
        {
            if (string.IsNullOrEmpty(_viewerPageVM.CategoryName) || _viewerPageVM.DefinedCategory == null)
            {
                MessageBox.Show("Category name and color cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_viewerPageVM.CurrentData.Categories.Any(c => c.CategoryName == _viewerPageVM.CategoryName && c.SolidColorBrushColor == _viewerPageVM.DefinedCategory.SolidColorBrushColor && c.Name == _viewerPageVM.DefinedCategory.Name))
            {
                MessageBox.Show("Category with the same name and color already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Category category = new Category(_viewerPageVM.CategoryName, _viewerPageVM.DefinedCategory.Name, _viewerPageVM.DefinedCategory.SolidColorBrushColor, _viewerPageVM.DefinedCategory.Name);
            _viewerPageVM.CurrentData.Categories.Add(category);
            _viewerPageVM.CurrentData.OnPropertyChanged(nameof(_viewerPageVM.CurrentData.Categories));
            _viewerPageVM.CurrentData.OnPropertyChanged(nameof(_viewerPageVM.CurrentData.CategoriesWithoutNone));
            _viewerPageVM.CategoryName = string.Empty;
            _viewerPageVM.DefinedCategory = null;
        }

        private ICommand _deleteCategoryCommand;

        public ICommand DeleteCategoryCommand
        {
            get
            {
                if (_deleteCategoryCommand == null)
                {
                    _deleteCategoryCommand = new RelayCommand(DeleteCategoryImplementation);
                }
                return _deleteCategoryCommand;
            }
        }

        private void DeleteCategoryImplementation(object obj)
        {
            var category = _viewerPageVM.SelectedItemCategory;
            _viewerPageVM.CurrentData.Categories.Remove(category);
            foreach (var element in _viewerPageVM.CurrentData.AllItems)
            {
                if (category.CategoryName == "None" && category.Name == "White")
                {
                    MessageBox.Show("Cannot remove this priority.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (element != null && category != null)
                {
                    element.Category.Remove(category);
                    if (element.Extension == "Folder")
                    {
                        for (int i = 0; i < _viewerPageVM.CurrentData.AllItems.Count; i++)
                        {
                            if (_viewerPageVM.CurrentData.AllItems[i].Path.Contains(element.Path) && _viewerPageVM.CurrentData.AllItems[i].Path.Count() > element.Path.Length)
                            {
                                _viewerPageVM.CurrentData.AllItems[i].Category.Remove(category);
                                if (_viewerPageVM.CurrentData.AllItems[i].Category.Count == 0)
                                {
                                    _viewerPageVM.CurrentData.AllItems[i].Category.Add(new Category("None", "White", new SolidColorBrush(Colors.White), "Black"));
                                }
                            }
                        }
                    }
                }

                if (element.Category.Count == 0)
                {
                    element.Category.Add(new Category("None", "White", new SolidColorBrush(Colors.White), "Black"));
                }
            }
            foreach (var element in _viewerPageVM.CurrentData.CurrentListBoxSource)
            {
                if (category.CategoryName == "None" && category.Name == "White")
                {
                    MessageBox.Show("Cannot remove this priority.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (element != null && category != null)
                {
                    element.Category.Remove(category);
                    if (element.Extension == "Folder")
                    {
                        for (int i = 0; i < _viewerPageVM.CurrentData.AllItems.Count; i++)
                        {
                            if (_viewerPageVM.CurrentData.AllItems[i].Path.Contains(element.Path) && _viewerPageVM.CurrentData.AllItems[i].Path.Count() > element.Path.Length)
                            {
                                _viewerPageVM.CurrentData.AllItems[i].Category.Remove(category);
                                if (_viewerPageVM.CurrentData.AllItems[i].Category.Count == 0)
                                {
                                    _viewerPageVM.CurrentData.AllItems[i].Category.Add(new Category("None", "White", new SolidColorBrush(Colors.White), "Black"));
                                }
                            }
                        }
                    }
                }

                if (element.Category.Count == 0)
                {
                    element.Category.Add(new Category("None", "White", new SolidColorBrush(Colors.White), "Black"));
                }
            }
            _viewerPageVM.CurrentData.OnPropertyChanged(nameof(_viewerPageVM.CurrentData.Categories));
            _viewerPageVM.CurrentData.OnPropertyChanged(nameof(_viewerPageVM.CurrentData.CategoriesWithoutNone));
        }

        #endregion
        #region Add To Category
        private ICommand _addToCategoryCommand;
        public ICommand AddToCategoryCommand
        {
            get
            {
                if (_addToCategoryCommand == null)
                {
                    _addToCategoryCommand = new RelayCommand(AddToCategoryImplementation);
                }
                return _addToCategoryCommand;
            }
        }
        private void AddToCategoryImplementation(object obj)
        {

            if (obj is object[] array && array.Length == 2)
            {
                var category = array[0] as Category;
                var items = array[1] as IList;

                if (items != null && category != null)
                {
                    var names = new System.Text.StringBuilder();
                    foreach (var item in items)
                    {
                        var element = item as Element;

                        if (element.Category.Count == 1 && element.Category.FirstOrDefault().CategoryName == "None" && element.Category.FirstOrDefault().Name == "White")
                        {
                            element.Category.Remove(element.Category.FirstOrDefault(c => c.CategoryName == "None" && c.Name == "White"));
                        }

                        if (element != null && category != null)
                        {
                            if (!element.Category.Contains(category))
                            {
                                element.Category.Add(category);
                                if (element.Extension == "Folder")
                                {
                                    for (int i = 0; i < _viewerPageVM.CurrentData.AllItems.Count; i++)
                                    {
                                        if (_viewerPageVM.CurrentData.AllItems[i].Path.Contains(element.Path + "\\" + element.Name) && _viewerPageVM.CurrentData.AllItems[i].Path.Count() > element.Path.Length + element.Name.Length)
                                        {
                                            if (_viewerPageVM.CurrentData.AllItems[i].Category.Count() == 1 && _viewerPageVM.CurrentData.AllItems[i].Category.FirstOrDefault().CategoryName == "None" && _viewerPageVM.CurrentData.AllItems[i].Category.FirstOrDefault().Name == "White")
                                            {
                                                _viewerPageVM.CurrentData.AllItems[i].Category.Remove(_viewerPageVM.CurrentData.AllItems[i].Category.FirstOrDefault(c => c.CategoryName == "None" && c.Name == "White"));
                                            }

                                            _viewerPageVM.CurrentData.AllItems[i].Category.Add(category);
                                            //sort ascending
                                            _viewerPageVM.CurrentData.AllItems[i].Category = new ObservableCollection<Category>(_viewerPageVM.CurrentData.AllItems[i].Category.OrderBy(c => c.CategoryName).ToList());
                                        }
                                    }
                                }
                                element.Category = new ObservableCollection<Category>(element.Category.OrderBy(c => c.CategoryName).ToList());
                            }
                            else
                            {
                                MessageBox.Show("Element is already in the priority.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region Remove From Category
        private ICommand _removeFromCategoryCommand;
        public ICommand RemoveFromCategoryCommand
        {
            get
            {
                if (_removeFromCategoryCommand == null)
                {
                    _removeFromCategoryCommand = new RelayCommand(RemoveFromCategoryImplementation);
                }
                return _removeFromCategoryCommand;
            }
        }
        private void RemoveFromCategoryImplementation(object obj)
        {
            if (obj is object[] array)
            {
                if (array.Length == 2)
                {
                    var element = array[1] as Element;
                    var category = array[0] as Category;

                    if (category.CategoryName == "None" && category.Name == "White")
                    {
                        MessageBox.Show("Cannot remove this priority.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (element != null && category != null)
                    {
                        element.Category.Remove(category);
                        if (element.Extension == "Folder")
                        {
                            for (int i = 0; i < _viewerPageVM.CurrentData.AllItems.Count; i++)
                            {
                                if (_viewerPageVM.CurrentData.AllItems[i].Path.Contains(element.Path) && _viewerPageVM.CurrentData.AllItems[i].Path.Count() > element.Path.Length)
                                {
                                    _viewerPageVM.CurrentData.AllItems[i].Category.Remove(category);
                                    if (_viewerPageVM.CurrentData.AllItems[i].Category.Count == 0)
                                    {
                                        _viewerPageVM.CurrentData.AllItems[i].Category.Add(new Category("None", "White", new SolidColorBrush(Colors.White), "Black"));
                                    }
                                }
                            }
                        }
                    }

                    if (element.Category.Count == 0)
                    {
                        element.Category.Add(new Category("None", "White", new SolidColorBrush(Colors.White), "Black"));
                    }
                }
            }
        }
        #endregion
        #region Add Priority
        private ICommand _addPriorityCommand;
        public ICommand AddPriorityCommand
        {
            get
            {
                if (_addPriorityCommand == null)
                {
                    _addPriorityCommand = new RelayCommand(AddPriorityImplementation);
                }
                return _addPriorityCommand;
            }
        }
        private void AddPriorityImplementation(object obj)
        {
            if (obj is object[] array && array.Length == 2)
            {
                var priority = array[0] as string;
                var items = array[1] as IList;

                if (items != null && priority != null)
                {
                    var names = new System.Text.StringBuilder();
                    foreach (var item in items)
                    {
                        var element = item as Element;
                        if (element != null)
                        {
                            element.Priority = priority;

                            if (element.Extension == "Folder")
                            {
                                for (int i = 0; i < _viewerPageVM.CurrentData.AllItems.Count; i++)
                                {
                                    if (_viewerPageVM.CurrentData.AllItems[i].Path.Contains(element.Path + "\\" + element.Name))
                                    {
                                        _viewerPageVM.CurrentData.AllItems[i].Priority = priority;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region Remove Priority
        private ICommand _removePriorityCommand;
        public ICommand RemovePriorityCommand
        {
            get
            {
                if (_removePriorityCommand == null)
                {
                    _removePriorityCommand = new RelayCommand(RemovePriorityImplementation);
                }
                return _removePriorityCommand;
            }
        }
        private void RemovePriorityImplementation(object obj)
        {
            if (obj is object[] array)
            {
                if (array.Length == 1)
                {
                    var element = array[0] as Element;

                    if (element.Priority == "None")
                    {
                        MessageBox.Show("Cannot remove this priority.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (element != null && element.Priority != null)
                    {
                        element.Priority = "None";
                        if (element.Extension == "Folder")
                        {
                            for (int i = 0; i < _viewerPageVM.CurrentData.AllItems.Count; i++)
                            {
                                if (_viewerPageVM.CurrentData.AllItems[i].Path.Contains(element.Path + "\\" + element.Name))
                                {
                                    _viewerPageVM.CurrentData.AllItems[i].Priority = "None";
                                }
                            }
                        }
                    }

                }
            }
        }
        #endregion
        #region Save
        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(SaveImplementation);
                }
                return _saveCommand;
            }
        }
        private void SaveImplementation(object obj)
        {
            ViewerPageHelper.SaveData(_viewerPageVM);
        }

        private ICommand _pushChangesCommand;

        public ICommand PushChangesCommand
        {
            get
            {
                if (_pushChangesCommand == null)
                {
                    _pushChangesCommand = new RelayCommand(PushChangesImplementation);
                }
                return _pushChangesCommand;
            }
        }

        private void PushChangesImplementation(object obj)
        {
            _viewerPageVM = HelperDrive.PushChanges(_viewerPageVM);
            _viewerPageVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(_viewerPageVM.CurrentData.AllItems.Where(e => e.Path == _viewerPageVM.CurrentPath).ToList());

            HelperDrive.ClearFolder();
        }

        #endregion
        #region  Open Load Space Window
        private ICommand _openLoadSpaceWindowCommand;
        public ICommand OpenLoadSpaceCommand
        {
            get
            {
                if (_openLoadSpaceWindowCommand == null)
                {
                    _openLoadSpaceWindowCommand = new RelayCommand(OpenLoadSpaceWindowImplementation);
                }
                return _openLoadSpaceWindowCommand;
            }
        }
        private void OpenLoadSpaceWindowImplementation(object obj)
        {
            _savesWindow = new SavesWindow(_viewerPageVM);

            _viewerPageVM.Commands.SavesWindow = _savesWindow;

            _savesWindow.ShowDialog();
        }
        #endregion
        #region LoadJson Command
        private ICommand _loadJsonCommand;
        public ICommand LoadJsonCommand
        {
            get
            {
                if (_loadJsonCommand == null)
                {
                    _loadJsonCommand = new RelayCommand(LoadJsonImplementation);
                }
                return _loadJsonCommand;
            }
        }
        private void LoadJsonImplementation(object obj)
        {
            try
            {
                string fileName = obj as string;

                string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                string filePath = Path.Combine(solutionDirectory, "Saves", fileName);
                filePath = filePath + ".json";

                string jsonData = File.ReadAllText(filePath);
                TransmittedData loadedData = null;
                try
                {
                    loadedData = JsonConvert.DeserializeObject<TransmittedData>(jsonData);
                    if (!Directory.Exists(loadedData.SpacePath) && !loadedData.DriveOrLocal)
                    {
                        MessageBox.Show($"The file {loadedData.SpacePath} does not exist. It will be deleted from Saves.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        DeleteImplementation(fileName);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deserializing data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (loadedData.DriveOrLocal)
                {
                    solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
                    var correctPath = Path.Combine(solutionDirectory, loadedData.SpacePath);

                    // logica de incarcare drive
                    _viewerPageVM.Commands.TryLoadPathDrive(loadedData.SpacePath);
                }
                else
                {
                    _viewerPageVM.Commands.TryLoadPath(loadedData.SpacePath);
                }

                if (loadedData != null && ViewerPageHelper.AreObjectsEqual(loadedData, _viewerPageVM.CurrentData))
                {
                    _viewerPageVM.CurrentData = loadedData;
                    _viewerPageVM.Commands.SavesWindow?.Close();
                }
                else
                {
                    MessageBox.Show("The content changed over time. We reload the content with the changes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _viewerPageVM.Commands.SavesWindow?.Close();
                    _viewerPageVM.CurrentData = ViewerPageHelper.TakeChanges(loadedData, _viewerPageVM.CurrentData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            _viewerPageVM.CurrentData.CurrentListBoxSource = _viewerPageVM.CurrentData.CurrentListBoxSource;
        }
        #endregion
        #region Delete 
        private ICommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(DeleteImplementation);
                }
                return _deleteCommand;
            }
        }
        public void DeleteImplementation(object obj)
        {
            try
            {
                string fileName = obj as string;
                if (fileName == null)
                {
                    throw new Exception("Invalid file name.");
                }

                string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                string saveFolderPath = Path.Combine(solutionDirectory, "Saves");

                string filePath = Path.Combine(saveFolderPath, $"{fileName}.json");
                if (!File.Exists(filePath))
                {
                    throw new Exception($"File '{fileName}.json' does not exist.");
                }

                File.Delete(filePath);

                string savesFilePath = Path.Combine(saveFolderPath, "saves.txt");
                if (!File.Exists(savesFilePath))
                {
                    throw new Exception("'saves.txt' does not exist.");
                }

                var lines = File.ReadAllLines(savesFilePath).ToList();
                lines.Remove(fileName);
                File.WriteAllLines(savesFilePath, lines);

                _savesVM.Saves = ViewerPageHelper.LoadSavesFromTextFile();

                //MessageBox.Show($"File '{fileName}.json' deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
        #region Choose Path For Saving
        private ICommand _saveConversionPathCommand;
        public ICommand SaveConversionPathCommand
        {
            get
            {
                if (_saveConversionPathCommand == null)
                {
                    _saveConversionPathCommand = new RelayCommand(SaveConversionPathImplementation);
                }
                return _saveConversionPathCommand;
            }
        }
        private void SaveConversionPathImplementation(object obj)
        {
            if (_viewerPageVM.CurrentData.SpacePath == null)
            {
                MessageBox.Show("Please select a workspace first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!_viewerPageVM.CurrentData.DriveOrLocal)
            {
                var folderBrowser = new VistaFolderBrowserDialog();
                folderBrowser.SelectedPath = _viewerPageVM.CurrentData.SpacePath; // Set the initial directory to the given path
                folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;
                folderBrowser.Description = "Select a subfolder";
                folderBrowser.ShowNewFolderButton = false;

                if (folderBrowser.ShowDialog() == true)
                {

                    if (!folderBrowser.SelectedPath.StartsWith(_viewerPageVM.CurrentData.SpacePath))
                    {
                        MessageBox.Show("Please select a subfolder of the initial path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    _viewerPageVM.SavingConversionPath = folderBrowser.SelectedPath;
                }
            }
            else
            {
                var fileExplorerDriveVM = new FileExplorerDriveVM(_viewerPageVM.CurrentData.SpacePath, _viewerPageVM.CurrentData, 0, "Folder", _viewerPageVM.CurrentData.AllItems);
                var fileExplorerDriveWindow = new FileExplorerDriveWindow { DataContext = fileExplorerDriveVM };
                fileExplorerDriveWindow.ShowDialog();

                _viewerPageVM.SavingConversionPath = fileExplorerDriveVM.SelectedPath;
            }
        }
        #endregion
        #region Save Conversion
        private ICommand _saveConversionCommand;
        public ICommand SaveConversionCommand
        {
            get
            {
                if (_saveConversionCommand == null)
                {
                    _saveConversionCommand = new RelayCommand(SaveConversionImplementation);
                }
                return _saveConversionCommand;
            }
        }
        private void SaveConversionImplementation(object obj)
        {
            List<string> selectedItems = _viewerPageVM.ConversionOptions.Where(item => item.IsChecked).Select(item => item.Content).ToList();
            var extension = _viewerPageVM.SelectedItem.Extension;
            extension = extension.Substring(1, extension.Length - 1).ToUpper();

            string endPathCheck = _viewerPageVM.SavingConversionPath;

            foreach (var item in selectedItems)
            {
                string fileNameCheck = _viewerPageVM.ConversionName;
                fileNameCheck += "." + item.ToLower();
                if (File.Exists(Path.Combine(endPathCheck, fileNameCheck)))
                {
                    MessageBox.Show("A file with the same name and extension already exists at the specified path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (_viewerPageVM.CurrentData.DriveOrLocal)
            {
                string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
                var endPath = Path.Combine(solutionDirectory, _viewerPageVM.SavingConversionPath);
                solutionDirectory = Path.Combine(solutionDirectory, _viewerPageVM.SelectedItem.Path);
                if (_viewerPageVM.SelectedItem.Status == FileStatus.Undownloaded)
                    HelperDrive.DownloadFile(_viewerPageVM.SelectedItem.Id, solutionDirectory);
                var path = "";

                string ext = Path.GetExtension(_viewerPageVM.SelectedItem.Name);
                var driveInitPath = _viewerPageVM.SelectedItem.Path + "\\" + _viewerPageVM.SelectedItem.Name;
                if (string.IsNullOrEmpty(ext))
                {
                    //path = Path.Combine(solutionDirectory, _viewerPageVM.SelectedItem.Name + _viewerPageVM.SelectedItem.Extension);
                    driveInitPath += _viewerPageVM.SelectedItem.Extension;
                }
                //else
                //{
                //    path = Path.Combine(solutionDirectory, _viewerPageVM.SelectedItem.Name);
                //}

                path = Path.Combine(solutionDirectory, _viewerPageVM.SelectedItem.Id + _viewerPageVM.SelectedItem.Extension);

                _viewerPageVM.SelectedItem.Status = FileStatus.ForConversion;

                ConvertItemsHelper temp = new ConvertItemsHelper();
                var driveEndPath = _viewerPageVM.SavingConversionPath;
                temp.ConvertItems(selectedItems, extension, _viewerPageVM, path, endPath, driveInitPath, driveEndPath);
            }
            else
            {
                ConvertItemsHelper temp = new ConvertItemsHelper();
                var initPath = _viewerPageVM.SelectedItem.Path + "\\" + _viewerPageVM.SelectedItem.Name + _viewerPageVM.SelectedItem.Extension;
                var endPath = _viewerPageVM.SavingConversionPath;
                temp.ConvertItems(selectedItems, extension, _viewerPageVM, initPath, endPath, "", "");
            }
        }
        #endregion
        #region Search
        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(SearchImplementation);
                }
                return _searchCommand;
            }
        }
        private void SearchImplementation(object obj)
        {
            _viewerPageVM.SearchApplied = true;
            var list = new ObservableCollection<Element>();

            list = _viewerPageVM.CurrentData.AllItems;

            _viewerPageVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(
                list
                    .Select(path =>
                    {
                        int appearances = ViewerPageHelper.FileMatchesCriteria(_viewerPageVM.CurrentData.DriveOrLocal, path, _viewerPageVM.SearchingWord, _viewerPageVM.IsByContentChecked, _viewerPageVM.IsByNameChecked);
                        path.Appearance = appearances > 1 ? $"{appearances} appearances" : $"{appearances} appearance";
                        return path;
                    })
                    .Where(path => int.Parse(path.Appearance.Split(' ')[0]) > 0)
                    .OrderByDescending(path => int.Parse(path.Appearance.Split(' ')[0]))
                    .ToList()
            );
            //_viewerPageVM.CurrentData.CurrentListBoxSource = _viewerPageVM.CurrentData.SearchResults;
        }
        #endregion
        #region Settings Command
        private ICommand _settingsCommand;
        public ICommand SettingsCommand
        {
            get
            {
                if (_settingsCommand == null)
                {
                    _settingsCommand = new RelayCommand(SettingsImplementation);
                }
                return _settingsCommand;
            }
        }
        private void SettingsImplementation(object obj)
        {
            //var settingsVM = new SettingsVM { SettingsDatas = _viewerPageVM.SettingsDatas, Datas = _viewerPageVM.CurrentData };

            //SettingsWindow settingsWindow = new SettingsWindow { DataContext = settingsVM };
            //settingsWindow.Show();

            var settingsVM = new SettingsVM(_viewerPageVM);
            var settingsWindow = new SettingsWindow { DataContext = settingsVM };
            settingsWindow.Show();
        }
        #endregion

        #region See difference between two files
        private ICommand _similarFilesCommand;
        public ICommand SimilarFilesCommand
        {
            get
            {
                if (_similarFilesCommand == null)
                {
                    _similarFilesCommand = new RelayCommand(SimilarFilesImplementation);
                }
                return _similarFilesCommand;
            }
        }
        private void SimilarFilesImplementation(object obj)
        {
            var similarFilesVM = new SimilarFilesVM(_viewerPageVM.CurrentData, _viewerPageVM.SettingsDatas.DistanceBetweenClusters, "SimilarFiles");

            SimilarFilesWindow similarFilesWindow = new SimilarFilesWindow { DataContext = similarFilesVM };
            if (similarFilesVM.SimilarElements.Count == 0)
            {
                MessageBox.Show("No similar files found");
            }
            else
            {
                similarFilesVM.CurrentData.CurrentListBoxSource = similarFilesVM.SimilarElements[0];
                similarFilesWindow.ShowDialog();
            }

        }

        private ICommand _croppedPhotoCommand;
        public ICommand CroppedPhotoCommand
        {
            get
            {
                if (_croppedPhotoCommand == null)
                {
                    _croppedPhotoCommand = new RelayCommand(CroppedPhotoImplementation);
                }
                return _croppedPhotoCommand;
            }
        }

        private void CroppedPhotoImplementation(object obj)
        {
            var similarFilesVM = new SimilarFilesVM(_viewerPageVM.CurrentData, _viewerPageVM.SettingsDatas.MinValueArea, "CroppedPhotos");

            SimilarFilesWindow similarFilesWindow = new SimilarFilesWindow { DataContext = similarFilesVM };
            if (similarFilesVM.SimilarElements.Count == 0)
            {
                MessageBox.Show("No similar files found");
            }
            else
            {
                similarFilesVM.CurrentData.CurrentListBoxSource = similarFilesVM.SimilarElements[0];
                similarFilesWindow.ShowDialog();
            }
        }

        private ICommand _editedPhotoCommand;

        public ICommand EditedPhotoCommand
        {
            get
            {
                if (_editedPhotoCommand == null)
                {
                    _editedPhotoCommand = new RelayCommand(EditedPhotoImplementation);
                }
                return _editedPhotoCommand;
            }
        }

        private void EditedPhotoImplementation(object obj)
        {
            var similarFilesVM = new SimilarFilesVM(_viewerPageVM.CurrentData, _viewerPageVM.SettingsDatas.MinValueSSIM, "EditedPhotos");

            SimilarFilesWindow similarFilesWindow = new SimilarFilesWindow { DataContext = similarFilesVM };
            if (similarFilesVM.SimilarElements.Count == 0)
            {
                MessageBox.Show("No similar files found");
            }
            else
            {
                similarFilesVM.CurrentData.CurrentListBoxSource = similarFilesVM.SimilarElements[0];
                similarFilesWindow.ShowDialog();
            }
        }
        #endregion

        #region Settings Window
        //private ICommand _recheckLanguageCommand;
        //public ICommand RecheckLanguageCommand
        //{
        //    get
        //    {
        //        if (_recheckLanguageCommand == null)
        //        {
        //            _recheckLanguageCommand = new RelayCommand(RecheckLanguageImplementation);
        //        }
        //        return _recheckLanguageCommand;
        //    }
        //}
        //private void RecheckLanguageImplementation(object obj)
        //{
        //    ViewerPageHelper.RecheckLanguage(_viewPieElementsVM);
        //}
        //private ICommand _recheckCodeCommand;
        //public ICommand RecheckCodeCommand
        //{
        //    get
        //    {
        //        if (_recheckCodeCommand == null)
        //        {
        //            _recheckCodeCommand = new RelayCommand(RecheckCodeImplementation);
        //        }
        //        return _recheckCodeCommand;
        //    }
        //}
        //private void RecheckCodeImplementation(object obj)
        //{
        //    ViewerPageHelper.RecheckCode(_viewPieElementsVM);
        //}
        //private ICommand _checkWorkingSpacesCommand;
        //public ICommand CheckWorkingSpacesCommand
        //{
        //    get
        //    {
        //        if (_checkWorkingSpacesCommand == null)
        //        {
        //            _checkWorkingSpacesCommand = new RelayCommand(CheckWorkingSpacesImplementation);
        //        }
        //        return _checkWorkingSpacesCommand;
        //    }
        //}
        //private void CheckWorkingSpacesImplementation(object obj)
        //{
        //}
        #endregion

        private ICommand _newFolderCommand;
        public ICommand NewFolderCommand
        {
            get
            {
                if (_newFolderCommand == null)
                {
                    _newFolderCommand = new RelayCommand(NewFolderImplementation);
                }
                return _newFolderCommand;
            }
        }

        private void NewFolderImplementation(object obj)
        {
            if (_viewerPageVM.CurrentData.SpacePath == null)
            {
                MessageBox.Show("Please select a workspace first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var createFolderVM = new CreateFolderVM(_viewerPageVM.CurrentData, _viewerPageVM.PozInList, _viewerPageVM.CurrentPath,
                _viewerPageVM.SettingsDatas.FilterType,
                _viewerPageVM.SelectedCategory,
                _viewerPageVM.SelectedPriority,
                _viewerPageVM.SelectedLanguage,
                _viewerPageVM.SelectedCodeLanguage,
                _viewerPageVM.SearchApplied);

            //e bine pana aici 
            CreateFolderWindow createFolderWindow = new CreateFolderWindow { DataContext = createFolderVM };
            createFolderWindow.ShowDialog();

            var temp = createFolderVM.NewFolderPath;
            if (temp == null)
            {
                return;
            }
            var folderName = temp.Split('\\').Last();
            temp = temp.Substring(0, temp.LastIndexOf('\\'));

            if (temp.Contains(_viewerPageVM.CurrentData.SpacePath))
            {
                Element temporary = new Element(temp, folderName, "Folder", new SolidColorBrush(Colors.DodgerBlue), "Folder");
                temporary.Status = FileStatus.ConversionResult;
                _viewerPageVM.CurrentData.AllItems.Add(temporary);
            }

            if (_viewerPageVM.CurrentPathDisplayed == temp)
            {
                Element temporary = new Element(temp, folderName, "Folder", new SolidColorBrush(Colors.DodgerBlue), "Folder");
                temporary.Status = FileStatus.ConversionResult;
                _viewerPageVM.CurrentData.CurrentListBoxSource.Add(temporary);
            }

            //take all elements that in path contains temp
            var elements = new ObservableCollection<Element>(createFolderVM.CurrentData.AllItems.Where(e => e.Path.Contains(createFolderVM.NewFolderPath)).ToList());
            for (int i = 0; i < elements.Count; i++)
            {
                Element temporary = new Element(elements[i].Path, elements[i].Name, elements[i].Icon, elements[i].Color, elements[i].Extension);
                temporary.Status = FileStatus.ConversionResult;
                _viewerPageVM.CurrentData.AllItems.Add(temporary);
            }
        }

        private ICommand _updateFiltersCommand;
        public ICommand UpdateFiltersCommand
        {
            get
            {
                if (_updateFiltersCommand == null)
                {
                    _updateFiltersCommand = new RelayCommand(UpdateFiltersImplementation);
                }
                return _updateFiltersCommand;
            }
        }

        private void UpdateFiltersImplementation(object obj)
        {
            if (_viewerPageVM.SelectedLanguage == _previousLanguage &&
                _viewerPageVM.SelectedCodeLanguage == _previousCodeLanguage &&
                _viewerPageVM.SelectedCategory == _previousCategory &&
                _viewerPageVM.SelectedPriority == _previousPriority
                )
            {
                return;
            }

            _previousLanguage = _viewerPageVM.SelectedLanguage;
            _previousCodeLanguage = _viewerPageVM.SelectedCodeLanguage;
            _previousCategory = _viewerPageVM.SelectedCategory;
            _previousPriority = _viewerPageVM.SelectedPriority;

            ViewerPageHelper.FilterItems(_viewerPageVM, false);
        }

        private ICommand _openStatisticsWindowCommand;

        public ICommand OpenStatisticsWindowCommand
        {
            get
            {
                if (_openStatisticsWindowCommand == null)
                {
                    _openStatisticsWindowCommand = new RelayCommand(OpenStatisticsWindowImplementation);
                }
                return _openStatisticsWindowCommand;
            }
        }

        private void OpenStatisticsWindowImplementation(object obj)
        {
            var statisticsVM = new StatisticsVM(_viewerPageVM.CurrentData.DriveOrLocal, _viewerPageVM.SelectedItem, _viewerPageVM.CurrentData.AllItems);
            StatisticsWindow statisticsWindow = new StatisticsWindow { DataContext = statisticsVM };
            statisticsWindow.ShowDialog();
        }

        public static ICommand _openElementsWindowComman;

        public ICommand OpenElementsWindowCommand
        {
            get
            {
                if (_openElementsWindowComman == null)
                {
                    _openElementsWindowComman = new RelayCommand(OpenElementsWindowImplementation);
                }
                return _openElementsWindowComman;
            }
        }
        private void OpenElementsWindowImplementation(object obj)
        {
            var chartpoint = obj as ChartPoint;
            if (chartpoint == null)
            {
                var pie = obj as PieSeries;
                if (pie != null)
                {
                    if (_statisticsVM.FileExtension.TryGetValue(pie.Title, out var elements))
                    {
                        var viewPieElementsVM = new ViewPieElementsVM(_statisticsVM.DriveOrLocal, elements);
                        ViewPieElementsWindow viewPieElementsWindow = new ViewPieElementsWindow { DataContext = viewPieElementsVM };
                        viewPieElementsWindow.ShowDialog();
                    }
                }
            }
            else
            {
                if (_statisticsVM.FileExtension.TryGetValue(chartpoint.SeriesView.Title, out var elements))
                {
                    var viewPieElementsVM = new ViewPieElementsVM(_statisticsVM.DriveOrLocal, elements);
                    ViewPieElementsWindow viewPieElementsWindow = new ViewPieElementsWindow { DataContext = viewPieElementsVM };
                    viewPieElementsWindow.ShowDialog();
                }
            }
        }

        #region Commands for CreateFolderNameAndPath

        private ICommand _selectPathCreateFolderNameAndPathCommand;
        public ICommand SelectPathCreateFolderNameAndPathCommand
        {
            get
            {
                if (_selectPathCreateFolderNameAndPathCommand == null)
                {
                    _selectPathCreateFolderNameAndPathCommand = new RelayCommand(SelectPathCreateFolderNameAndPathImplementation);
                }
                return _selectPathCreateFolderNameAndPathCommand;
            }
        }
        private void SelectPathCreateFolderNameAndPathImplementation(object obj)
        {
            if (_createFolderNameAndPathVM.CurrentData.DriveOrLocal)
            {
                var fileExplorerDriveVM = new FileExplorerDriveVM(_createFolderNameAndPathVM.CurrentData.SpacePath, _createFolderNameAndPathVM.CurrentData, _createFolderNameAndPathVM.PozInList, "Folder", _createFolderNameAndPathVM.CurrentData.AllItems);
                var fileExplorerDriveWindow = new FileExplorerDriveWindow { DataContext = fileExplorerDriveVM };
                fileExplorerDriveWindow.ShowDialog();

                _createFolderNameAndPathVM.FolderPath = fileExplorerDriveVM.SelectedPath;
            }
            else
            {
                System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

                System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    _createFolderNameAndPathVM.FolderPath = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private ICommand _nextCreateFolderNameAndPathCommand;
        public ICommand NextCreateFolderNameAndPathCommand
        {
            get
            {
                if (_nextCreateFolderNameAndPathCommand == null)
                {
                    _nextCreateFolderNameAndPathCommand = new RelayCommand(NextCreateFolderNameAndPathImplementation);
                }
                return _nextCreateFolderNameAndPathCommand;
            }
        }
        private void NextCreateFolderNameAndPathImplementation(object obj)
        {
            _createFolderNameAndPathVM.ClosePage();
        }

        #endregion
        #region Similar Files

        private ICommand _navigateBackInSimilarFilesCommand;

        public ICommand NavigateBackInSimilarFilesCommand
        {
            get
            {
                if (_navigateBackInSimilarFilesCommand == null)
                {
                    _navigateBackInSimilarFilesCommand = new RelayCommand(NavigateBackInSimilarFilesImplementation);
                }
                return _navigateBackInSimilarFilesCommand;
            }
        }
        private void NavigateBackInSimilarFilesImplementation(object obj)
        {
            if (_similarFilesVM.PositionInList == 0)
            {
                return;
            }
            _similarFilesVM.PositionInList--;
            _similarFilesVM.CurrentData.CurrentListBoxSource = _similarFilesVM.SimilarElements[_similarFilesVM.PositionInList];
        }

        private ICommand _navigateForwardInSimilarFilesCommand;

        public ICommand NavigateForwardInSimilarFilesCommand
        {
            get
            {
                if (_navigateForwardInSimilarFilesCommand == null)
                {
                    _navigateForwardInSimilarFilesCommand = new RelayCommand(NavigateForwardInSimilarFilesImplementation);
                }
                return _navigateForwardInSimilarFilesCommand;
            }
        }
        private void NavigateForwardInSimilarFilesImplementation(object obj)
        {
            if (_similarFilesVM.PositionInList == _similarFilesVM.SimilarElements.Count - 1)
            {
                return;
            }
            _similarFilesVM.PositionInList++;
            _similarFilesVM.CurrentData.CurrentListBoxSource = _similarFilesVM.SimilarElements[_similarFilesVM.PositionInList];
        }

        private ICommand _navigateHomeInSimilarFilesCommand;

        public ICommand NavigateHomeInSimilarFilesCommand
        {
            get
            {
                if (_navigateHomeInSimilarFilesCommand == null)
                {
                    _navigateHomeInSimilarFilesCommand = new RelayCommand(NavigateHomeInSimilarFilesImplementation);
                }
                return _navigateHomeInSimilarFilesCommand;
            }
        }
        private void NavigateHomeInSimilarFilesImplementation(object obj)
        {
            _similarFilesVM.PositionInList = 0;
            _similarFilesVM.CurrentData.CurrentListBoxSource = _similarFilesVM.SimilarElements[_similarFilesVM.PositionInList];
        }




        private ICommand _showSelectedNamesCommand;

        public ICommand ShowSelectedNamesCommand
        {
            get
            {
                if (_showSelectedNamesCommand == null)
                {
                    _showSelectedNamesCommand = new RelayCommand(ShowSelectedNamesImplementation);
                }
                return _showSelectedNamesCommand;
            }
        }

        private void ShowSelectedNamesImplementation(object obj)
        {
            var parameters = obj as object[];
            if (parameters.Count() != 2)
                return;
            var selectedItems = parameters[0] as IList;
            if (selectedItems.Count != 2)
                return;
            var element1 = selectedItems[0] as Element;
            var element2 = selectedItems[1] as Element;
            var diffView = parameters[1] as DiffViewer;
            if (selectedItems.Count != 2)
            {
                MessageBox.Show("Please select two items.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _similarFilesVM.AllFileVisibility = false;
            if (_similarFilesVM.plainTextFileExtensions.Contains(element1.Extension) && _similarFilesVM.CurrentData.DriveOrLocal)
            {
                _similarFilesVM.PicsVisibility = false;
                _similarFilesVM.DiffplexVisibility = true;
                if (diffView == null)
                    diffView = new DiffViewer();
                diffView.OldText = ViewerPageHelper.GetFileContent(_similarFilesVM.CurrentData.DriveOrLocal, element1);
                diffView.NewText = ViewerPageHelper.GetFileContent(_similarFilesVM.CurrentData.DriveOrLocal, element2);
            }
            else if (_similarFilesVM.plainTextFileExtensions.Contains(element1.Extension))
            {
                _similarFilesVM.PicsVisibility = false;
                _similarFilesVM.DiffplexVisibility = true;
                if (diffView == null)
                    diffView = new DiffViewer();
                diffView.OldText = ViewerPageHelper.GetFileContent(_similarFilesVM.CurrentData.DriveOrLocal, element1);
                diffView.NewText = ViewerPageHelper.GetFileContent(_similarFilesVM.CurrentData.DriveOrLocal, element2);
            }
            else
            {
                _similarFilesVM.DiffplexVisibility = false;
                _similarFilesVM.PicsVisibility = true;
                // Create a new BitmapImage.
                BitmapImage bitmap1 = new BitmapImage();

                // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                bitmap1.BeginInit();
                bitmap1.UriSource = new Uri(element1.Path + "\\" + element1.Name + element1.Extension, UriKind.Absolute);
                bitmap1.EndInit();

                // Assign the BitmapImage to ImageSource1.
                _similarFilesVM.ImageSource1 = bitmap1;

                // Repeat for ImageSource2.
                BitmapImage bitmap2 = new BitmapImage();
                bitmap2.BeginInit();
                bitmap2.UriSource = new Uri(element2.Path + "\\" + element2.Name + element2.Extension, UriKind.Absolute);
                bitmap2.EndInit();
                _similarFilesVM.ImageSource2 = bitmap2;

            }
        }


        private ICommand _showBackFromViewingCommand;

        public ICommand ShowBackFromViewingCommand
        {
            get
            {
                if (_showBackFromViewingCommand == null)
                {
                    _showBackFromViewingCommand = new RelayCommand(ShowBackFromViewingImplementation);
                }
                return _showBackFromViewingCommand;
            }
        }

        private void ShowBackFromViewingImplementation(object obj)
        {
            _similarFilesVM.AllFileVisibility = true;
            _similarFilesVM.DiffplexVisibility = false;
            _similarFilesVM.PicsVisibility = false;
        }

        private ICommand _listUpdate;

        public ICommand ListUpdate
        {
            get
            {
                if (_listUpdate == null)
                {
                    _listUpdate = new RelayCommand(ListUpdateImplementation);
                }
                return _listUpdate;
            }
        }

        private void ListUpdateImplementation(object obj)
        {
            var selectedItems = obj as IList;
            if (selectedItems != null)
            {
                _viewerPageVM.SelectedItems = selectedItems;
            }
        }

        private ICommand _deleteItemCommand;

        public ICommand DeleteItemCommand
        {
            get
            {
                if (_deleteItemCommand == null)
                {
                    _deleteItemCommand = new RelayCommand(DeleteItemImplementation);
                }
                return _deleteItemCommand;
            }
        }

        private void DeleteItemImplementation(object obj)
        {
            var items = obj as IList;

            if (items != null)
            {
                // Create a copy of the items to delete
                var itemsToDelete = new List<object>(items.Cast<object>());

                foreach (var item in itemsToDelete)
                {
                    var element = item as Element;
                    if (element != null)
                    {
                        if (_viewerPageVM.CurrentData.DriveOrLocal)
                        {
                            if (element.Extension == "Folder")
                            {
                                HelperDrive.DeleteFile(element.Id);
                                var elements = new ObservableCollection<Element>(_viewerPageVM.CurrentData.AllItems.Where(e => e.Path.Contains(element.Path + "\\" + element.Name)).ToList());
                                for (int i = 0; i < elements.Count; i++)
                                {
                                    _viewerPageVM.CurrentData.AllItems.Remove(elements[i]);
                                }
                                elements = new ObservableCollection<Element>(_viewerPageVM.CurrentData.CurrentListBoxSource.Where(e => e.Path.Contains(element.Path + "\\" + element.Name)).ToList());
                                for (int i = 0; i < elements.Count; i++)
                                {
                                    _viewerPageVM.CurrentData.CurrentListBoxSource.Remove(elements[i]);
                                }
                                _viewerPageVM.CurrentData.AllItems.Remove(element);
                                _viewerPageVM.CurrentData.CurrentListBoxSource.Remove(element);
                            }
                            else
                            {
                                HelperDrive.DeleteFile(element.Id);
                                _viewerPageVM.CurrentData.AllItems.Remove(element);
                                _viewerPageVM.CurrentData.CurrentListBoxSource.Remove(element);
                            }
                        }
                        else
                        {
                            if (File.Exists(element.Path + "\\" + element.Name + element.Extension))
                            {
                                File.Delete(element.Path + "\\" + element.Name + element.Extension);
                                _viewerPageVM.CurrentData.AllItems.Remove(element);
                                _viewerPageVM.CurrentData.CurrentListBoxSource.Remove(element);
                            }
                            else if (Directory.Exists(element.Path + "\\" + element.Name))
                            {
                                Directory.Delete(element.Path + "\\" + element.Name, true); // true to remove directories, subdirectories, and files in path
                                var elements = new ObservableCollection<Element>(_viewerPageVM.CurrentData.AllItems.Where(e => e.Path.Contains(element.Path + "\\" + element.Name)).ToList());
                                for (int i = 0; i < elements.Count; i++)
                                {
                                    _viewerPageVM.CurrentData.AllItems.Remove(elements[i]);
                                }
                                elements = new ObservableCollection<Element>(_viewerPageVM.CurrentData.CurrentListBoxSource.Where(e => e.Path.Contains(element.Path + "\\" + element.Name)).ToList());
                                for (int i = 0; i < elements.Count; i++)
                                {
                                    _viewerPageVM.CurrentData.CurrentListBoxSource.Remove(elements[i]);
                                }
                                _viewerPageVM.CurrentData.AllItems.Remove(element);
                                _viewerPageVM.CurrentData.CurrentListBoxSource.Remove(element);
                            }
                        }
                    }
                }
            }
        }

        #endregion


        private ICommand _openFileFromList;

        public ICommand OpenFileFromList
        {
            get
            {
                if (_openFileFromList == null)
                {
                    _openFileFromList = new RelayCommand(OpenFileFromListImplementation);
                }
                return _openFileFromList;
            }
        }

        private void OpenFileFromListImplementation(object obj)
        {
            if (obj is Element clickedElement)
            {
                if (_statisticsVM.DriveOrLocal)
                {
                    if (clickedElement.Id == null)
                    {
                        string solutionDirectoryy = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                        solutionDirectoryy = Path.Combine(solutionDirectoryy, "DriveDownloads");
                        var nam = Path.GetFileNameWithoutExtension(clickedElement.Name);


                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = solutionDirectoryy + "\\" + clickedElement.Path + "\\" + nam + clickedElement.Extension,
                            UseShellExecute = true
                        };

                        // Start the process
                        System.Diagnostics.Process.Start(startInfo);
                    }
                    else
                    {
                        //conditie ca poate exista
                        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                        solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
                        solutionDirectory = Path.Combine(solutionDirectory, clickedElement.Path);
                        if (clickedElement.Status == FileStatus.Undownloaded)
                        {
                            HelperDrive.DownloadFile(clickedElement.Id, solutionDirectory);
                            clickedElement.Status = FileStatus.Downloaded;
                        }
                        var path = "";

                        //string extension = Path.GetExtension(clickedElement.Name);

                        //if (string.IsNullOrEmpty(extension))
                        //{
                        //    path = Path.Combine(solutionDirectory, clickedElement.Name + clickedElement.Extension);
                        //}
                        //else
                        //{
                        //    path = Path.Combine(solutionDirectory, clickedElement.Name);
                        //}

                        path = Path.Combine(solutionDirectory, clickedElement.Id + clickedElement.Extension);


                        if (File.Exists(path))
                        {
                            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = path,
                                UseShellExecute = true
                            };

                            // Start the process
                            System.Diagnostics.Process.Start(startInfo);
                        }
                    }
                }
                {
                    var path = Path.Combine(clickedElement.Path, clickedElement.Name + clickedElement.Extension);
                    if (File.Exists(path))
                    {
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = path,
                            UseShellExecute = true
                        };

                        // Start the process
                        System.Diagnostics.Process.Start(startInfo);
                    }
                }
            }
        }
    }
}