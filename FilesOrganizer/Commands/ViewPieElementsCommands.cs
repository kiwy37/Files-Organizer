using FilesOrganizer.Core;
using FilesOrganizer.Helpers;
using FilesOrganizer.Models;
using FilesOrganizer.ViewModels;
using FilesOrganizer.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FilesOrganizer.Commands;

public class ViewPieElementsCommands
{
    private readonly ViewPieElementsVM _viewPieElementsVM;

    public ViewPieElementsCommands(ViewPieElementsVM viewPieElementsVM)
    {
        _viewPieElementsVM = viewPieElementsVM;
    }

    private ICommand _open;

    public ICommand Open
    {
        get
        {
            if (_open == null)
            {
                _open = new RelayCommand(OpenImplementation);
            }
            return _open;
        }
    }

    private void OpenImplementation(object obj)
    {
        if (obj is Element clickedElement)
        {
            if(_viewPieElementsVM.DriveOrLocal)
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

                string extension = Path.GetExtension(clickedElement.Name);

                if (string.IsNullOrEmpty(extension))
                {
                    path = Path.Combine(solutionDirectory, clickedElement.Name + clickedElement.Extension);
                }
                else
                {
                    path = Path.Combine(solutionDirectory, clickedElement.Name);
                }

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