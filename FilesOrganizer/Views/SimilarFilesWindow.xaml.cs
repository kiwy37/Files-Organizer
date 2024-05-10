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
    /// Interaction logic for SimilarFilesWindow.xaml
    /// </summary>
    public partial class SimilarFilesWindow : Window
    {
        public SimilarFilesWindow()
        {
            InitializeComponent();
        }
        private void ZoomSlider_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Slider slider = (Slider)sender;
            if (e.Delta > 0)
            {
                slider.Value += slider.TickFrequency;
            }
            else
            {
                slider.Value -= slider.TickFrequency;
            }
            e.Handled = true;
        }

    }
}
