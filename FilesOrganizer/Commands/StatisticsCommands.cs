using FilesOrganizer.Core;
using FilesOrganizer.Helpers;
using FilesOrganizer.Models;
using FilesOrganizer.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FilesOrganizer.Commands;

public class StatisticsCommands
{
    private readonly StatisticsVM _statisticsVM;

    public StatisticsCommands(StatisticsVM statisticsVM)
    {
        _statisticsVM = statisticsVM;
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

