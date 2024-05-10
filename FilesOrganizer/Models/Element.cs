using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;

namespace FilesOrganizer.Models
{
    public class Element : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        private SolidColorBrush _color;
        public string Path { get; set; }
        public string Icon { get; set; }
        public string _priority;
        private ObservableCollection<Category> _category { get; set; }
        private string _language;
        public string _codeLanguage;
        public string _appearance;
        private string _lastAccess;
        private long _size;
        private string _id;
        private FileStatus _status;

        public FileStatus Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }
        public string Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public long Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    OnPropertyChanged(nameof(Size));
                }
            }
        }
        public ObservableCollection<Category> Category
        {
            get { return _category; }
            set
            {
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged(nameof(Category));
                }
            }
        }

        public string Priority
        {
            get { return _priority; }
            set
            {
                if (_priority != value)
                {
                    _priority = value;
                    OnPropertyChanged(nameof(Priority));
                }
            }
        }
        public string Appearance
        {
            get { return _appearance; }
            set
            {
                if (_appearance != value)
                {
                    _appearance = value;
                    OnPropertyChanged(nameof(Appearance));
                }
            }
        }

        public SolidColorBrush Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged(nameof(Color));
                }
            }
        }

        public string Language
        {
            get { return _language; }
            set
            {
                if (_language != value)
                {
                    _language = value;
                    OnPropertyChanged(nameof(Language));
                }
            }
        }

        public string CodeLanguage
        {
            get { return _codeLanguage; }
            set
            {
                if (_codeLanguage != value)
                {
                    _codeLanguage = value;
                    OnPropertyChanged(nameof(CodeLanguage));
                }
            }
        }

        public string LastAccessed
        {
            get { return _lastAccess; }
            set
            {
                if (_lastAccess != value)
                {
                    _lastAccess = value;
                    OnPropertyChanged(nameof(LastAccessed));
                }
            }
        }

        public Element()
        {
        }

        public Element(string path, string name, string icon, SolidColorBrush color, string extension)
        {
            Name = name;
            Extension = extension;
            Path = path;
            Icon = icon;
            Color = color;
            Priority = "None";
            //put an element in the category
            Category = new ObservableCollection<Category>{ new Category("None", "White", new SolidColorBrush(Colors.White), "Black") };
            Appearance = "";
            Language = "None";
            CodeLanguage = "None";
            LastAccessed = "Unknown";
            Status = FileStatus.Undownloaded;
        }
        public Element( FileStatus fileStatus, string path, string name, string icon, SolidColorBrush color, string extension)
        {
            Name = name;
            Extension = extension;
            Path = path;
            Icon = icon;
            Color = color;
            Priority = "None";
            //put an element in the category
            Category = new ObservableCollection<Category>{ new Category("None", "White", new SolidColorBrush(Colors.White), "Black") };
            Appearance = "";
            Language = "None";
            CodeLanguage = "None";
            LastAccessed = "Unknown";
            Status = fileStatus;
        }
        public Element(string id, string path, string name, string icon, SolidColorBrush color, string extension)
        {
            Id = id;
            Name = name;
            Extension = extension;
            Path = path;
            Icon = icon;
            Color = color;
            Priority = "None";
            //put an element in the category
            Category = new ObservableCollection<Category>{ new Category("None", "White", new SolidColorBrush(Colors.White), "Black") };
            Appearance = "";
            Language = "None";
            CodeLanguage = "None";
            LastAccessed = "Unknown";
            Status = FileStatus.Undownloaded;
        }

        //public Element(string name, string extension, SolidColorBrush color, string path, string icon)
        //{
        //    Name = name;
        //    Extension = extension;
        //    Color = color;
        //    Path = path;
        //    Icon = icon;
        //    Priority = "None";
        //    Category = new ObservableCollection<Category> { new Category("None", "White", new SolidColorBrush(Colors.White), "Black") };
        //    Appearance = "";
        //    Language = "None";
        //    CodeLanguage = "None";
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
