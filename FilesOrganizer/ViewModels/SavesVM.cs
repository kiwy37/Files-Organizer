using FilesOrganizer.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FilesOrganizer.ViewModels.Commands
{
    public class SavesVM : INotifyPropertyChanged
    {
        private ObservableCollection<string> _saves;
        private Commands _commands;
        private string _selectedSave;
        private ViewerPageVM _viewerPageVM;
        private SavesWindow _savesWindow; 

        public SavesWindow SavesWindow
        {
            get => _savesWindow;
            set
            {
                _savesWindow = value;
                OnPropertyChanged(nameof(SavesWindow));
            }
        }

        public ViewerPageVM ViewerPageVM
        {
            get => _viewerPageVM;
            set
            {
                _viewerPageVM = value;
                OnPropertyChanged(nameof(ViewerPageVM));
            }
        }
        public Commands Commands
        {
            get
            {
                if (_commands == null)
                {
                    _commands = new Commands(this, _viewerPageVM);
                }
                return _commands;
            }
        }

        public ObservableCollection<string> Saves
        {
            get => _saves;
            set
            {
                if (_saves != value)
                {
                    _saves = value;
                    OnPropertyChanged(nameof(Saves));
                }
            }
        }

        public string SelectedSave
        {
            get { return _selectedSave; }
            set
            {
                if (_selectedSave != value)
                {
                    _selectedSave = value;
                    OnPropertyChanged(nameof(SelectedSave));
                }
            }
        }

        public SavesVM(ViewerPageVM viewerPageVM)
        {
            Saves = Helper.LoadSavesFromTextFile();
            ViewerPageVM = viewerPageVM;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
