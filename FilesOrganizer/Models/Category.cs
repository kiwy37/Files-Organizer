using System;
using System.Windows.Media;

namespace FilesOrganizer.Models;

public class Category : Core.ViewModel
{
    private string _categoryName;
    private string _name;
    private string _textColor;
    private SolidColorBrush _col;


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
    public SolidColorBrush Col
    {
        get { return _col; }
        set
        {
            if (_col != value)
            {
                _col = value;
                OnPropertyChanged(nameof(Col));
            }
        }
    }
    public Category()
    {
        Name = "";
        Col = new SolidColorBrush();
    }

    public Category(string categoryName, string name, SolidColorBrush col, string textColor)
    {
        CategoryName = categoryName;
        Name = name;
        Col = col;
        TextColor = textColor;
    }


    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Category other = (Category)obj;
        return CategoryName == other.CategoryName && Name == other.Name && Col.Color == other.Col.Color && TextColor == other.TextColor;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CategoryName, Name, Col.Color, TextColor);
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