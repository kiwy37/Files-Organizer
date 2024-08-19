﻿using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace FilesOrganizer.Core;

public class ArrayMultiValueConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.Clone();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return (object[])value;
    }
}