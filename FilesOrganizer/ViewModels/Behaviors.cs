using FilesOrganizer.ViewModels.Commands;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FilesOrganizer.ViewModels;

public class SaveOnCloseBehavior : Behavior<Window>
{
    protected override void OnAttached()
    {
        AssociatedObject.Closing += OnWindowClosing;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.Closing -= OnWindowClosing;
    }

    private void OnWindowClosing(object sender, CancelEventArgs e)
    {
        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        solutionDirectory = Path.Combine(solutionDirectory, "Saves");
        solutionDirectory = Path.Combine(solutionDirectory, "Settings.txt");
        var window = sender as Window;
        if (window != null)
        {
            var viewModel = window.DataContext as SettingsVM;
            if (viewModel != null)
            {
                File.WriteAllText(solutionDirectory,
                    viewModel.SettingsDatas.ImagesCode.ToString() + "\n" +
                    viewModel.SettingsDatas.VideosCode.ToString() + "\n" +
                    viewModel.SettingsDatas.ImagesLanguage.ToString() + "\n" +
                    viewModel.SettingsDatas.VideosLanguage.ToString() + "\n" +
                    viewModel.SettingsDatas.FilterType.ToString() + "\n" +
                    viewModel.SettingsDatas.TextFilesCode.ToString() + "\n" +
                    viewModel.SettingsDatas.TextFilesLanguage.ToString() + "\n" +
                    viewModel.SettingsDatas.AudiosLanguage.ToString() + "\n" +
                    viewModel.SettingsDatas.DistanceBetweenClusters.ToString() + "\n" +
                    viewModel.SettingsDatas.MinValueSSIM.ToString() + "\n" +
                    viewModel.SettingsDatas.MinValueArea.ToString() + "\n"
                    );
            }
        }
    }
}
