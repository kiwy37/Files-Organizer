using FilesOrganizer.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FilesOrganizer.Views
{
    /// <summary>
    /// Interaction logic for FileExplorerDriveWindow.xaml
    /// </summary>
    public partial class FileExplorerDriveWindow : Window
    {
        public FileExplorerDriveWindow()
        {
            InitializeComponent();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as FileExplorerDriveVM;
            if (viewModel.Option == "Folder")
            {
                if (viewModel != null && viewModel.SelectedItem == null)
                {
                    viewModel.SelectedPath = viewModel.CurrentPathDisplayed;
                    Close();
                }
                else
                {
                    viewModel.SelectedPath = viewModel.SelectedItem.Path + "\\" + viewModel.SelectedItem.Name;
                    Close();
                }
            }
            else
            {
                if (viewModel != null && viewModel.SelectedItem == null)
                {
                    MessageBox.Show("Please select a file.");
                }
                else if (viewModel.SelectedItem.Extension == "Folder")
                {
                    MessageBox.Show("Please select a file.");
                }
                else
                {
                    viewModel.SelectedItem = viewModel.SelectedItem;
                    Close();
                }
            }
        }
    }
}
