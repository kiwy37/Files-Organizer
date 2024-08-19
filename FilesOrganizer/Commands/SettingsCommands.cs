using FilesOrganizer.Core;
using FilesOrganizer.Helpers;
using FilesOrganizer.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FilesOrganizer.Commands;

public class SettingsCommands
{
    private readonly SettingsVM _settingsVM;

    public SettingsCommands(SettingsVM settingsVM)
    {
        _settingsVM = settingsVM;
    }

    private ICommand _recheckLanguageCommand;
    public ICommand RecheckLanguageCommand
    {
        get
        {
            if (_recheckLanguageCommand == null)
            {
                _recheckLanguageCommand = new RelayCommand(RecheckLanguageImplementation);
            }
            return _recheckLanguageCommand;
        }
    }
    private void RecheckLanguageImplementation(object obj)
    {
        ViewerPageHelper.RecheckLanguage(_settingsVM);
    }
    private ICommand _recheckCodeCommand;
    public ICommand RecheckCodeCommand
    {
        get
        {
            if (_recheckCodeCommand == null)
            {
                _recheckCodeCommand = new RelayCommand(RecheckCodeImplementation);
            }
            return _recheckCodeCommand;
        }
    }
    private void RecheckCodeImplementation(object obj)
    {
        ViewerPageHelper.RecheckCode(_settingsVM);
    }
    private ICommand _checkWorkingSpacesCommand;
    public ICommand CheckWorkingSpacesCommand
    {
        get
        {
            if (_checkWorkingSpacesCommand == null)
            {
                _checkWorkingSpacesCommand = new RelayCommand(CheckWorkingSpacesImplementation);
            }
            return _checkWorkingSpacesCommand;
        }
    }
    private void CheckWorkingSpacesImplementation(object obj)
    {
        ViewerPageHelper.CheckWorkingSpaces(_settingsVM);
    }
}
