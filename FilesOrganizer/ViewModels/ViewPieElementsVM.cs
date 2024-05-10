using FilesOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOrganizer.ViewModels;

public class ViewPieElementsVM : Core.ViewModel, INotifyPropertyChanged
{
    private ObservableCollection<Element> _elements = new ObservableCollection<Element>();
    private Element _selectedItem;
    public ViewPieElementsVM(List<Element> elements)
    {
        Elements = new ObservableCollection<Element>(elements);
    }

    public ObservableCollection<Element> Elements
    {
        get { return _elements; }
        set
        {
            if (_elements != value)
            {
                _elements = value;
                OnPropertyChanged(nameof(Elements));
            }
        }
    }

    public Element SelectedItem
    {
        get { return _selectedItem; }
        set
        {
            if (_selectedItem != value)
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
    }
}
