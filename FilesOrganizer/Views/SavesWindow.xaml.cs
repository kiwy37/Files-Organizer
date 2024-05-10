﻿using FilesOrganizer.ViewModels;
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
    /// Interaction logic for SavesWindow.xaml
    /// </summary>
    public partial class SavesWindow : Window
    {
        public SavesWindow(ViewerPageVM viewerPageVM)
        {
            InitializeComponent();
            DataContext = new SavesVM(viewerPageVM)
            {
                SavesWindow = this // Use the correct property name
            };
        }
    }
}
