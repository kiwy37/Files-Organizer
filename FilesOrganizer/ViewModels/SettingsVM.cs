using FilesOrganizer.Core;
using FilesOrganizer.Models;
using System.ComponentModel;

namespace FilesOrganizer.ViewModels.Commands;

public class SettingsVM : Core.ViewModel, INotifyPropertyChanged
{
    private TransmittedData _datas;
    private Settings _settingsDatas;
    private Commands _commands;
    public ViewerPageVM _mainViewModel;

    public SettingsVM(ViewerPageVM mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _datas = mainViewModel.CurrentData;
        _settingsDatas = mainViewModel.SettingsDatas;
    }

    public ViewerPageVM MainViewModel
    {
        get => _mainViewModel;
        set
        {
            _mainViewModel = value;
            OnPropertyChanged(nameof(MainViewModel));
        }
    }
    public TransmittedData Datas
    {
        get => _datas;
        set
        {
            _datas = value;
            OnPropertyChanged(nameof(Datas));
        }
    }
    public Settings SettingsDatas
    {
        get => _settingsDatas;
        set
        {
            _settingsDatas = value;
            OnPropertyChanged(nameof(SettingsDatas));
        }
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