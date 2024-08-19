using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FilesOrganizer.Core;

public class SizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is long size)
        {
            if (size < 1024) return $"{size} bytes";
            else if (size < 1024 * 1024) return $"{size / 1024.0:#.##} KB";
            else if (size < 1024 * 1024 * 1024) return $"{size / (1024.0 * 1024):#.##} MB";
            else return $"{size / (1024.0 * 1024 * 1024):#.##} GB";
        }
        return "0 bytes";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
