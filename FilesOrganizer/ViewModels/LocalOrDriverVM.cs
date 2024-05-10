using FilesOrganizer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FilesOrganizer.ViewModels.Commands;

public class LocalOrDriverVM
{
    private Commands _commands;
    public string ButtonClicked { get; set; }
    public ICommand LocalButtonCommand { get; set; }
    public ICommand DriveButtonCommand { get; set; }
    public Action CloseAction { get; set; }
    public LocalOrDriverVM()
    {
        LocalButtonCommand = new RelayCommand(o => { ButtonClicked = "Local"; CloseAction?.Invoke(); });
        DriveButtonCommand = new RelayCommand(o => { ButtonClicked = "Drive"; CloseAction?.Invoke(); });
    }

    public Commands Commands
    {
        get
        {
            if (_commands == null)
            {
                _commands = new Commands(this);
            }
            return _commands;
        }
    }
}
