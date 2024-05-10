using DiffPlex.Wpf.Controls;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using FilesOrganizer.Commands;
using FilesOrganizer.Core;
using FilesOrganizer.Models;
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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Formatting = Newtonsoft.Json.Formatting;

namespace FilesOrganizer.ViewModels.Commands
{
    public class Commands
    {
        private readonly SimilarFilesVM _similarFilesVM;
        private readonly CreateFolderNameAndPathVM _createFolderNameAndPathVM;
        private ViewerPageVM _viewerPageVM;
        private readonly AddCategoryVM _addCategoryVM;
        private readonly SavesVM _savesVM;
        private SavesWindow _savesWindow;
        private readonly SettingsVM _settingsVM;
        private readonly CreateFolderVM _createFolderVM;
        private readonly StatisticsVM _statisticsVM;
        private readonly LocalOrDriverVM _localOrDriverVM;

        public SavesWindow SavesWindow
        {
            get { return _savesWindow; }
            set { _savesWindow = value; }
        }
        public Commands(LocalOrDriverVM localOrDriverVM)
        {
            _localOrDriverVM = localOrDriverVM;
        }
        public Commands(StatisticsVM statisticsVM)
        {
            _statisticsVM = statisticsVM;
        }
        public Commands(SimilarFilesVM similarFilesVM)
        {
            _similarFilesVM = similarFilesVM;
        }
        public Commands(CreateFolderNameAndPathVM createFolderNameAndPathVM)
        {
            _createFolderNameAndPathVM = createFolderNameAndPathVM;
        }
        public Commands(ViewerPageVM viewerPageVM)
        {
            _viewerPageVM = viewerPageVM;
        }
        public Commands(AddCategoryVM addCategoryVM)
        {
            _addCategoryVM = addCategoryVM;
        }
        public Commands(SavesVM savesVM, ViewerPageVM viewerPageVM)
        {
            _savesVM = savesVM;
            _viewerPageVM = viewerPageVM;
        }
        public Commands(SettingsVM settingsVM)
        {
            _settingsVM = settingsVM;
        }
        public Commands(CreateFolderVM createFolderVM)
        {
            _createFolderVM = createFolderVM;
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
            Helper.DetectCodeLanguagePics(_viewerPageVM);
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
            var localOrDriverVM = new LocalOrDriverVM();
            LocalOrDriveWindow localOrDriveWindow = new LocalOrDriveWindow { DataContext = localOrDriverVM };
            localOrDriverVM.CloseAction = localOrDriveWindow.Close; // Pass the Close action to the ViewModel
            localOrDriveWindow.ShowDialog();
            HelperDrive.ClearFolder();

            if (localOrDriverVM.ButtonClicked == "Local")
            {
                _viewerPageVM.CurrentData.DriveOdLocal = false;
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
                        Helper.LanguageAndCodeForElements(_viewerPageVM, _viewerPageVM.SettingsDatas);
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
            else if (localOrDriverVM.ButtonClicked == "Drive")
            {
                _viewerPageVM.CurrentData.DriveOdLocal = true;
                _viewerPageVM = HelperDrive.LoadFilesFromGoogleDrive(_viewerPageVM);
                ListElementsInCurrentPath();
                _viewerPageVM.CurrentData.SpacePath = new string(_viewerPageVM.CurrentPath);
                _viewerPageVM.CurrentData.BackStack.Add(_viewerPageVM.CurrentPath);
            }
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
                    Helper.LanguageAndCodeForElements(_viewerPageVM, _viewerPageVM.SettingsDatas);
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
        private void LoadItems(string path)     //path - cel de unde trebuie sa ia tot contentul
        {
            string[] files = System.IO.Directory.GetFiles(path);
            string[] folders = System.IO.Directory.GetDirectories(path);

            foreach (var file in files)
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                string extension = System.IO.Path.GetExtension(file);

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
                string folderName = System.IO.Path.GetFileName(folder);
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
            else
            {
                if (!(bool)_createFolderVM.FilterType)
                {
                    _createFolderVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(_createFolderVM.CurrentData.AllItems.Where(item => item.Path == actualPath &&
                        ((_createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || _createFolderVM.SearchApplied == false) &&
                        ((_createFolderVM.SelectedPriority == null || _createFolderVM.SelectedPriority == "All Items") || (_createFolderVM.SelectedPriority != "All Items" && item.Priority == _createFolderVM.SelectedPriority)) &&
                        ((_createFolderVM.SelectedCategory == null || _createFolderVM.SelectedCategory.CategoryName == "All Items") || (_createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.Col.ToString() == _createFolderVM.SelectedCategory.Col.ToString() && c.CategoryName == _createFolderVM.SelectedCategory.CategoryName))) &&
                        ((_createFolderVM.SelectedLanguage == null || _createFolderVM.SelectedLanguage == "All Items") || (_createFolderVM.SelectedLanguage != "All Items" && item.Language == _createFolderVM.SelectedLanguage)) &&
                        ((_createFolderVM.SelectedCodeLanguage == null || _createFolderVM.SelectedCodeLanguage == "All Items") || (_createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == _createFolderVM.SelectedCodeLanguage))
                    ).ToList());
                }
                else
                {
                    _createFolderVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(_createFolderVM.CurrentData.AllItems.Where(item => item.Path == actualPath &&
                        ((_createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || _createFolderVM.SearchApplied == false) &&
                        ((_createFolderVM.SelectedPriority == null || _createFolderVM.SelectedPriority == "All Items") || (_createFolderVM.SelectedPriority != "All Items" && item.Priority == _createFolderVM.SelectedPriority)) &&
                        ((_createFolderVM.SelectedCategory == null || _createFolderVM.SelectedCategory.CategoryName == "All Items") || (_createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.Col.ToString() == _createFolderVM.SelectedCategory.Col.ToString() && c.CategoryName == _createFolderVM.SelectedCategory.CategoryName))) &&
                        ((_createFolderVM.SelectedLanguage == null || _createFolderVM.SelectedLanguage == "All Items") || (_createFolderVM.SelectedLanguage != "All Items" && item.Language == _createFolderVM.SelectedLanguage)) &&
                        ((_createFolderVM.SelectedCodeLanguage == null || _createFolderVM.SelectedCodeLanguage == "All Items") || (_createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == _createFolderVM.SelectedCodeLanguage))
                    ).ToList());
                }

                //if (!(bool)viewerPageVM.SettingsDatas.FilterType) //cu reuniune 
                //{
                //    results = list.Where(item =>
                //        ((item.Path == viewerPageVM.CurrentPath) || item.Path.Contains(viewerPageVM.CurrentPath)) && ((viewerPageVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || viewerPageVM.SearchApplied == false) &&
                //        ((viewerPageVM.SelectedPriority == null || viewerPageVM.SelectedPriority == "All Items") || (viewerPageVM.SelectedPriority != "All Items" && item.Priority == viewerPageVM.SelectedPriority)) &&
                //        ((viewerPageVM.SelectedCategory == null || viewerPageVM.SelectedCategory.CategoryName == "All Items") || (viewerPageVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.Col.ToString() == viewerPageVM.SelectedCategory.Col.ToString() && c.CategoryName == viewerPageVM.SelectedCategory.CategoryName))) &&
                //        ((viewerPageVM.SelectedLanguage == null || viewerPageVM.SelectedLanguage == "All Items") || (viewerPageVM.SelectedLanguage != "All Items" && item.Language == viewerPageVM.SelectedLanguage)) &&
                //        ((viewerPageVM.SelectedCodeLanguage == null || viewerPageVM.SelectedCodeLanguage == "All Items") || (viewerPageVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == viewerPageVM.SelectedCodeLanguage))
                //    ).ToList();
                //}
                //else
                //{
                //    results = list.Where(item =>
                //        ((item.Path == viewerPageVM.CurrentPath) || item.Path.Contains(viewerPageVM.CurrentPath)) && ((viewerPageVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || viewerPageVM.SearchApplied == false) &&
                //        (((viewerPageVM.SelectedPriority == null || viewerPageVM.SelectedPriority == "All Items") || (viewerPageVM.SelectedPriority != "All Items" && item.Priority == viewerPageVM.SelectedPriority)) ||
                //        ((viewerPageVM.SelectedCategory == null || viewerPageVM.SelectedCategory.CategoryName == "All Items") || (viewerPageVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.Col.ToString() == viewerPageVM.SelectedCategory.Col.ToString() && c.CategoryName == viewerPageVM.SelectedCategory.CategoryName))) ||
                //        ((viewerPageVM.SelectedLanguage == null || viewerPageVM.SelectedLanguage == "All Items") || (viewerPageVM.SelectedLanguage != "All Items" && item.Language == viewerPageVM.SelectedLanguage)) ||
                //        ((viewerPageVM.SelectedCodeLanguage == null || viewerPageVM.SelectedCodeLanguage == "All Items") || (viewerPageVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == viewerPageVM.SelectedCodeLanguage)))
                //    ).ToList();
                //}

            }
        }
        #endregion
        #region OpenFolder Command
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
                    //_viewerPageVM.CurrentData.DriveOdLocal = true;
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
                        else if (_viewerPageVM.CurrentData.DriveOdLocal)
                        {
                            //conditie ca poate exista
                            string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                            solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
                            solutionDirectory = Path.Combine(solutionDirectory, clickedElement.Path);
                            if (clickedElement.Status == FileStatus.Undownloaded)
                            {
                                HelperDrive.DownloadFile(_viewerPageVM.Service, clickedElement.Id, solutionDirectory);
                                clickedElement.Status = FileStatus.Downloaded;
                            }
                            var path = "";

                            string extension = Path.GetExtension(clickedElement.Name);

                            if (string.IsNullOrEmpty(extension))
                            {
                                path = Path.Combine(solutionDirectory, clickedElement.Name + clickedElement.Extension);
                            }
                            else
                            {
                                path = Path.Combine(solutionDirectory, clickedElement.Name);
                            }

                            if (System.IO.File.Exists(path))
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

                    _viewerPageVM.SelectedCategory = _viewerPageVM.CurrentData.Categories.FirstOrDefault(c => c.CategoryName == elements[3] && c.Col.ToString() == elements[4]);

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

                    Helper.FilterItems(_viewerPageVM, true);
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

                String currentDir;
                if (_viewerPageVM.CurrentData.BackStack.ElementAt(_viewerPageVM.PozInList).StartsWith("Filter results"))
                {
                    currentDir = _viewerPageVM.CurrentData.BackStack.ElementAt(_viewerPageVM.PozInList);
                    var elements = currentDir.Split('-');

                    _viewerPageVM.SettingsDatas.FilterType = bool.Parse(elements[1]);
                    _viewerPageVM.SelectedPriority = elements[2] == "null" ? null : elements[2];

                    //buba
                    _viewerPageVM.SelectedCategory = _viewerPageVM.CurrentData.Categories.FirstOrDefault(c => c.CategoryName == elements[3] && c.Col.ToString() == elements[4]);


                    _viewerPageVM.SelectedLanguage = elements[5] == "null" ? null : elements[5];
                    _viewerPageVM.SelectedCodeLanguage = elements[6] == "null" ? null : elements[6];
                    _viewerPageVM.CurrentPathDisplayed = elements[0];

                    Helper.FilterItems(_viewerPageVM, true);
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
                        _viewerPageVM.SelectedCategory = _viewerPageVM.CurrentData.Categories.FirstOrDefault(c => c.CategoryName == elements[3] && c.Col.ToString() == elements[4]);


                        _viewerPageVM.SelectedLanguage = elements[5] == "null" ? null : elements[5];
                        _viewerPageVM.SelectedCodeLanguage = elements[6] == "null" ? null : elements[6];
                        _viewerPageVM.CurrentPathDisplayed = elements[0];

                        _viewerPageVM.CurrentPath = _viewerPageVM.CurrentData.BackStack.ElementAt(_viewerPageVM.PozInList);
                        Helper.FilterItems(_viewerPageVM, true);
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

            if (_viewerPageVM.CurrentData.Categories.Any(c => c.CategoryName == _viewerPageVM.CategoryName && c.Col == _viewerPageVM.DefinedCategory.Col && c.Name == _viewerPageVM.DefinedCategory.Name))
            {
                MessageBox.Show("Category with the same name and color already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Category category = new Category(_viewerPageVM.CategoryName, _viewerPageVM.DefinedCategory.Name, _viewerPageVM.DefinedCategory.Col, _viewerPageVM.DefinedCategory.Name);
            _viewerPageVM.CurrentData.Categories.Add(category);
            _viewerPageVM.CurrentData.OnPropertyChanged((nameof(_viewerPageVM.CurrentData.Categories)));
            _viewerPageVM.CurrentData.OnPropertyChanged((nameof(_viewerPageVM.CurrentData.CategoriesWithoutNone)));
            _viewerPageVM.CategoryName = string.Empty;
            _viewerPageVM.DefinedCategory = null;
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
            if (obj is object[] array)
            {
                if (array.Length == 2)
                {
                    var element = array[1] as Element;
                    var category = array[0] as Category;

                    //and if contains the element with CategoryName == "None" && Name == "White"
                    if (element.Category.Count == 1 && element.Category.FirstOrDefault().CategoryName == "None" && element.Category.FirstOrDefault().Name == "White")
                    {
                        //remove the element with priority.CategoryName == "None" && priority.Name == "White"
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
                                    if (_viewerPageVM.CurrentData.AllItems[i].Path.Contains(element.Path + "\\" + element.Name) && _viewerPageVM.CurrentData.AllItems[i].Path.Count() > (element.Path.Length + element.Name.Length))
                                    {
                                        if (_viewerPageVM.CurrentData.AllItems[i].Category.Count() == 1 && _viewerPageVM.CurrentData.AllItems[i].Category.FirstOrDefault().CategoryName == "None" && _viewerPageVM.CurrentData.AllItems[i].Category.FirstOrDefault().Name == "White")
                                        {
                                            _viewerPageVM.CurrentData.AllItems[i].Category.Remove(_viewerPageVM.CurrentData.AllItems[i].Category.FirstOrDefault(c => c.CategoryName == "None" && c.Name == "White"));
                                        }

                                        _viewerPageVM.CurrentData.AllItems[i].Category.Add(category);
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Element is already in the priority.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (obj is object[] array)
            {
                if (array.Length == 2)
                {
                    var element = array[1] as Element;
                    var priority = array[0] as string;

                    if (element != null && priority != null)
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
            try
            {
                string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                string saveFolderPath = Path.Combine(solutionDirectory, "Saves");
                Directory.CreateDirectory(saveFolderPath);

                if (_viewerPageVM.CurrentData.SpacePath == null)
                {
                    throw new Exception("Cannot save an empty space.");
                }

                string savingName = _viewerPageVM.CurrentData.SpacePath.Split('\\').Last();
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(savingName);
                string fileName = $"{fileNameWithoutExtension}.json";

                string filePath = Path.Combine(saveFolderPath, fileName);

                try
                {
                    string jsonData = JsonConvert.SerializeObject(_viewerPageVM.CurrentData, Formatting.Indented);

                    File.WriteAllText(filePath, jsonData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string savesFilePath = Path.Combine(saveFolderPath, "saves.txt");

                try
                {
                    if (!File.Exists(savesFilePath))
                    {
                        File.Create(savesFilePath).Close();
                    }

                    string existingSave = File.ReadLines(savesFilePath).FirstOrDefault(line => line.Trim() == fileNameWithoutExtension);

                    if (existingSave == null)
                    {
                        File.AppendAllText(savesFilePath, fileNameWithoutExtension + Environment.NewLine);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating 'saves' file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //MessageBox.Show($"Data saved successfully in the folder 'Save' with filename: {fileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            HelperDrive.PushChanges(_viewerPageVM);
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
                    if (!Directory.Exists(loadedData.SpacePath))
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

                _viewerPageVM.Commands.TryLoadPath(loadedData.SpacePath);

                if (loadedData != null && Helper.AreObjectsEqual(loadedData, _viewerPageVM.CurrentData))
                {
                    _viewerPageVM.CurrentData = loadedData;
                    _viewerPageVM.Commands.SavesWindow?.Close();
                }
                else
                {
                    MessageBox.Show("The content changed over time. We reload the content with the changes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _viewerPageVM.Commands.SavesWindow?.Close();
                    _viewerPageVM.CurrentData = Helper.TakeChanges(loadedData, _viewerPageVM.CurrentData);
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
        private void DeleteImplementation(object obj)
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

                _savesVM.Saves = Helper.LoadSavesFromTextFile();

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
            if (!_viewerPageVM.CurrentData.DriveOdLocal)
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
                var fileExplorerDriveVM = new FileExplorerDriveVM(_viewerPageVM.CurrentData.SpacePath, _viewerPageVM.CurrentData, _viewerPageVM.PozInList);
                var fileExplorerDriveWindow = new FileExplorerDriveWindow { DataContext = fileExplorerDriveVM };
                fileExplorerDriveWindow.ShowDialog();

                _viewerPageVM.SavingConversionPath = fileExplorerDriveVM.SelectedItem.Path + "\\" + fileExplorerDriveVM.SelectedItem.Name;
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


            if (_viewerPageVM.CurrentData.DriveOdLocal)
            {
                string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
                var endPath = Path.Combine(solutionDirectory, _viewerPageVM.SavingConversionPath);
                solutionDirectory = Path.Combine(solutionDirectory, _viewerPageVM.SelectedItem.Path);
                HelperDrive.DownloadFile(_viewerPageVM.Service, _viewerPageVM.SelectedItem.Id, solutionDirectory);
                var path = "";

                string ext = Path.GetExtension(_viewerPageVM.SelectedItem.Name);
                var driveInitPath = _viewerPageVM.SelectedItem.Path + "\\" + _viewerPageVM.SelectedItem.Name;
                if (string.IsNullOrEmpty(ext))
                {
                    path = Path.Combine(solutionDirectory, _viewerPageVM.SelectedItem.Name + _viewerPageVM.SelectedItem.Extension);
                    driveInitPath += _viewerPageVM.SelectedItem.Extension;
                }
                else
                {
                    path = Path.Combine(solutionDirectory, _viewerPageVM.SelectedItem.Name);
                }

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
            //if (_viewerPageVM.SelectedCategory != null && _viewerPageVM.SelectedCategory.CategoryName != "None"
            //    || _viewerPageVM.SelectedPriority != null && _viewerPageVM.SelectedPriority != "None")
            //{
            //    list = _viewerPageVM.CurrentData.FiltersResults;
            //}
            //else
            //{
            list = _viewerPageVM.CurrentData.AllItems;
            //}

            _viewerPageVM.CurrentData.SearchResults = new ObservableCollection<Element>(
                list
                    .Select(path =>
                    {
                        int appearances = Helper.FileMatchesCriteria(_viewerPageVM.CurrentData.DriveOdLocal, path, _viewerPageVM.SearchingWord, _viewerPageVM.IsByContentChecked, _viewerPageVM.IsByNameChecked);
                        path.Appearance = appearances > 1 ? $"{appearances} appearances" : $"{appearances} appearance";
                        return path;
                    })
                    .Where(path => int.Parse(path.Appearance.Split(' ')[0]) > 0)
                    .OrderByDescending(path => int.Parse(path.Appearance.Split(' ')[0]))
                    .ToList()
            );
            _viewerPageVM.CurrentData.CurrentListBoxSource = _viewerPageVM.CurrentData.SearchResults;
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
            var similarFilesVM = new SimilarFilesVM(_viewerPageVM.CurrentData, _viewerPageVM.SettingsDatas.SimilarityThreshold);

            SimilarFilesWindow similarFilesWindow = new SimilarFilesWindow { DataContext = similarFilesVM };
            if (similarFilesVM.SimilarElements.Count == 0)
            {
                MessageBox.Show("No similar files found");
            }
            else
            {
                similarFilesWindow.ShowDialog();
            }

        }
        #endregion

        #region Settings Window
        private ICommand _recheckLanguageCommand;
        public ICommand RecheckLanguageCommand
        {
            get
            {
                if (_recheckLanguageCommand == null)
                {
                    _recheckLanguageCommand = new RelayCommand(RecheckLanguageImplementation);
                }
                return _recheckLanguageCommand;
            }
        }
        private void RecheckLanguageImplementation(object obj)
        {
            Helper.RecheckLanguage(_settingsVM);
        }
        private ICommand _recheckCodeCommand;
        public ICommand RecheckCodeCommand
        {
            get
            {
                if (_recheckCodeCommand == null)
                {
                    _recheckCodeCommand = new RelayCommand(RecheckCodeImplementation);
                }
                return _recheckCodeCommand;
            }
        }
        private void RecheckCodeImplementation(object obj)
        {
            Helper.RecheckCode(_settingsVM);
        }
        private ICommand _checkWorkingSpacesCommand;
        public ICommand CheckWorkingSpacesCommand
        {
            get
            {
                if (_checkWorkingSpacesCommand == null)
                {
                    _checkWorkingSpacesCommand = new RelayCommand(CheckWorkingSpacesImplementation);
                }
                return _checkWorkingSpacesCommand;
            }
        }
        private void CheckWorkingSpacesImplementation(object obj)
        {
        }
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
            var createFolderVM = new CreateFolderVM(_viewerPageVM.CurrentData, _viewerPageVM.PozInList, _viewerPageVM.CurrentPath,
                _viewerPageVM.SettingsDatas.FilterType,
                _viewerPageVM.SelectedCategory,
                _viewerPageVM.SelectedPriority,
                _viewerPageVM.SelectedLanguage,
                _viewerPageVM.SelectedCodeLanguage,
                _viewerPageVM.SearchApplied);

            CreateFolderWindow createFolderWindow = new CreateFolderWindow { DataContext = createFolderVM };
            createFolderWindow.ShowDialog();
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
            Helper.AddFileToList(_createFolderVM);
        }

        private ICommand _addNewFolderCommand;
        public ICommand AddNewFolderCommand
        {
            get
            {
                if (_addNewFolderCommand == null)
                {
                    _addNewFolderCommand = new RelayCommand(AddNewFolderImplementation);
                }
                return _addNewFolderCommand;
            }
        }
        private void AddNewFolderImplementation(object obj)
        {
            Helper.AddFolderToList(_createFolderVM);
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
            Helper.FilterItems(_viewerPageVM, false);
        }

        #region Create new folder nav - open
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
                    Helper.FilterItemsCreateFolder(_createFolderVM, _createFolderVM.InitPath);
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
        #endregion

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
                            MessageBox.Show($"Access to the path '{_viewerPageVM.CurrentPath}' is denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        //else if(_viewerPageVM.CurrentData.DriveOdLocal)
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
            var createFolderNameAndPathVM = new CreateFolderNameAndPathVM();

            CreateFolderNameAndPathWindow createFolderNameAndPathWindow = new CreateFolderNameAndPathWindow { DataContext = createFolderNameAndPathVM };
            createFolderNameAndPathVM.CloseAction = new Action(() =>
            {
                createFolderNameAndPathWindow.Close();
                Helper.CreateNewFolder(_createFolderVM, createFolderNameAndPathVM.FolderName, createFolderNameAndPathVM.FolderPath);
            });

            createFolderNameAndPathWindow.ShowDialog();
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
            var statisticsVM = new StatisticsVM(_viewerPageVM.SelectedItem, _viewerPageVM.CurrentData.AllItems);
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
                        var viewPieElementsVM = new ViewPieElementsVM(elements);
                        ViewPieElementsWindow viewPieElementsWindow = new ViewPieElementsWindow { DataContext = viewPieElementsVM };
                        viewPieElementsWindow.ShowDialog();
                    }
                }
            }
            else
            {
                if (_statisticsVM.FileExtension.TryGetValue(chartpoint.SeriesView.Title, out var elements))
                {
                    var viewPieElementsVM = new ViewPieElementsVM(elements);
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
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                _createFolderNameAndPathVM.FolderPath = folderBrowserDialog.SelectedPath;
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

            if (_similarFilesVM.plainTextFileExtensions.Contains(element1.Extension))
            {
                _similarFilesVM.PicsVisibility = false;
                _similarFilesVM.DiffplexVisibility = true;
                if (diffView == null)
                    diffView = new DiffViewer();
                diffView.OldText = Helper.GetFileContent(element1);
                diffView.NewText = Helper.GetFileContent(element2);
            }
            else
            {
                _similarFilesVM.PicsVisibility = true;
                _similarFilesVM.DiffplexVisibility = false;
                if (element1 != null && element1.Path != null && element1.Name != null)
                {
                    var imagePath1 = Path.Combine(element1.Path, element1.Name);
                    _similarFilesVM.ImageSource1 = new BitmapImage(new Uri(imagePath1));
                }

                if (element2 != null && element2.Path != null && element2.Name != null)
                {
                    var imagePath2 = Path.Combine(element2.Path, element2.Name);
                    _similarFilesVM.ImageSource2 = new BitmapImage(new Uri(imagePath2));
                }

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



        #endregion
    }
}
