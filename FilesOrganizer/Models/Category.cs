using System;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Media;

namespace FilesOrganizer.Models;

public class Category : Core.ViewModel
{
    private string _categoryName;
    private string _name;
    private string _textColor;
    private SolidColorBrush _solidColorBrushColor;

    private ObservableCollection<string> brightCols = new ObservableCollection<string> { "AliceBlue", "Azure", "Beige", "Cornsilk", "FloralWhite", "GhostWhite", "Honeydew", "Ivory", "Lavender", "LavenderBlush", "LightCyan", "LightGoldenrodYellow", "LightYellow", "Linen", "MintCream", "OldLace", "SeaShell", "Snow", "Transparent", "White", "WhiteSmoke" };
    public string CategoryName
    {
        get { return _categoryName; }
        set
        {
            if (_categoryName != value)
            {
                _categoryName = value;
                OnPropertyChanged(nameof(CategoryName));
            }
        }
    }
    public string Name
    {
        get { return _name; }
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public string TextColor
    {
        get { return _textColor; }
        set
        {
            if (_textColor != value)
            {
                _textColor = value;
                OnPropertyChanged(nameof(TextColor));
            }
        }
    }
    public SolidColorBrush SolidColorBrushColor
    {
        get { return _solidColorBrushColor; }
        set
        {
            if (_solidColorBrushColor != value)
            {
                _solidColorBrushColor = value;
                OnPropertyChanged(nameof(SolidColorBrushColor));
            }
        }
    }
    public Category()
    {
        Name = "";
        SolidColorBrushColor = new SolidColorBrush();
    }

    public Category(string categoryName, string name, SolidColorBrush col, string textColor)
    {
        CategoryName = categoryName;
        Name = name;
        SolidColorBrushColor = col;

        if(brightCols.Contains(textColor))
        {
            textColor = "Black";
        }
        TextColor = textColor;
    }


    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Category other = (Category)obj;
        return CategoryName == other.CategoryName && Name == other.Name && SolidColorBrushColor.Color == other.SolidColorBrushColor.Color && TextColor == other.TextColor;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CategoryName, Name, SolidColorBrushColor.Color, TextColor);
    }

    public static bool operator ==(Category a, Category b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if ((a is null) || (b is null))
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Category a, Category b)
    {
        return !(a == b);
    }
}