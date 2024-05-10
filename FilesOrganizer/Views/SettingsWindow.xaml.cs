using FilesOrganizer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FilesOrganizer.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ((ToggleButton)sender).Content = "Difference at filtering";
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ((ToggleButton)sender).Content = "Reunion at filtering";
        }
    }
}
