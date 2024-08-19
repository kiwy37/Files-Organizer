using FilesOrganizer.Commands;
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
    public bool DriveOrLocal { set; get; }
    private ObservableCollection<Element> _elements = new ObservableCollection<Element>();
    private Element _selectedItem;
    private ViewPieElementsCommands _commands;

    public ViewPieElementsCommands Commands
    {
        get
        {
            if (_commands == null)
            {
                _commands = new ViewPieElementsCommands(this);
            }
            return _commands;
        }
    }

    public ViewPieElementsVM(bool driveOrLocal, List<Element> elements)
    {
        DriveOrLocal = driveOrLocal;
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
