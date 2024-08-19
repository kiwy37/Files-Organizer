using FilesOrganizer.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using AODL.Document.TextDocuments;
using AODL.Document.Content.Text;
using Xceed.Words.NET;
using System.Globalization;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Tesseract;
using System.Collections.Generic;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using DocumentFormat.OpenXml.Wordprocessing;
using FilesOrganizer.Core;
using NPOI.OpenXmlFormats.Spreadsheet;
using System.Drawing;
using Svg;
using ImageMagick;
using FilesOrganizer.Views;
using SixLabors.ImageSharp;
using System.Windows.Forms;
using System.Windows.Media;
using NPOI.HPSF;
using DocumentFormat.OpenXml.Office2013.Excel;
using System.Management;
using FilesOrganizer.ViewModels.Commands;
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;
using Emgu.CV;
using FilesOrganizer.Commands;
using MessageBox = System.Windows.Forms.MessageBox;
using NPOI.POIFS.Crypt.Dsig.Services;

namespace FilesOrganizer.Helpers;

public class ViewerPageHelper
{
    public static void OpenFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            else
            {
                System.Windows.MessageBox.Show($"File not found: {filePath}");
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error opening file: {ex.Message}");
        }
    }
    public static void FilterItems(ViewerPageVM viewerPageVM, bool cond = false)
    {
        var list = viewerPageVM.CurrentData.AllItems;
        List<Element> results;

        if (!(bool)viewerPageVM.SettingsDatas.FilterType) //cu reuniune 
        {
            results = list.Where(item =>
                (item.Path == viewerPageVM.CurrentPath || item.Path.Contains(viewerPageVM.CurrentPath)) && (viewerPageVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0 || viewerPageVM.SearchApplied == false) &&
                (viewerPageVM.SelectedPriority == null || viewerPageVM.SelectedPriority == "All Items" || viewerPageVM.SelectedPriority != "All Items" && item.Priority == viewerPageVM.SelectedPriority) &&
                (viewerPageVM.SelectedCategory == null || viewerPageVM.SelectedCategory.CategoryName == "All Items" || viewerPageVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.SolidColorBrushColor.ToString() == viewerPageVM.SelectedCategory.SolidColorBrushColor.ToString() && c.CategoryName == viewerPageVM.SelectedCategory.CategoryName)) &&
                (viewerPageVM.SelectedLanguage == null || viewerPageVM.SelectedLanguage == "All Items" || viewerPageVM.SelectedLanguage != "All Items" && item.Language == viewerPageVM.SelectedLanguage) &&
                (viewerPageVM.SelectedCodeLanguage == null || viewerPageVM.SelectedCodeLanguage == "All Items" || viewerPageVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == viewerPageVM.SelectedCodeLanguage)
            ).ToList();
        }
        else
        {
            results = list.Where(item =>
                (item.Path == viewerPageVM.CurrentPath || item.Path.Contains(viewerPageVM.CurrentPath)) && (viewerPageVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0 || viewerPageVM.SearchApplied == false) &&
                (viewerPageVM.SelectedPriority == null || viewerPageVM.SelectedPriority == "All Items" || viewerPageVM.SelectedPriority != "All Items" && item.Priority == viewerPageVM.SelectedPriority ||
                viewerPageVM.SelectedCategory == null || viewerPageVM.SelectedCategory.CategoryName == "All Items" || viewerPageVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.SolidColorBrushColor.ToString() == viewerPageVM.SelectedCategory.SolidColorBrushColor.ToString() && c.CategoryName == viewerPageVM.SelectedCategory.CategoryName) ||
                viewerPageVM.SelectedLanguage == null || viewerPageVM.SelectedLanguage == "All Items" || viewerPageVM.SelectedLanguage != "All Items" && item.Language == viewerPageVM.SelectedLanguage ||
                viewerPageVM.SelectedCodeLanguage == null || viewerPageVM.SelectedCodeLanguage == "All Items" || viewerPageVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == viewerPageVM.SelectedCodeLanguage)
            ).ToList();
        }
        viewerPageVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(results);

        if (cond == false)
        {
            string a = "", bunu = "", bdoi = "", c = "", d = "";
            if (viewerPageVM.SelectedPriority == null)
            {
                a = "null";
            }
            else
            {
                a = viewerPageVM.SelectedPriority;
            }
            if (viewerPageVM.SelectedCategory == null)
            {
                bunu = "null";
                bdoi = "null";
            }
            else
            {
                bunu = viewerPageVM.SelectedCategory.CategoryName;
                bdoi = viewerPageVM.SelectedCategory.SolidColorBrushColor.ToString();
            }
            if (viewerPageVM.SelectedLanguage == null)
            {
                c = "null";
            }
            if (viewerPageVM.SelectedCodeLanguage == null)
            {
                d = "null";
            }

            string str = "Filter results-" +
                (bool)viewerPageVM.SettingsDatas.FilterType +
                "-" + a +
                "-" + bunu +
                "-" + bdoi +
                "-" + c +
                "-" + d;

            if (viewerPageVM.CurrentData.BackStack.Count > viewerPageVM.PozInList + 1 && viewerPageVM.CurrentData.BackStack.ElementAt(viewerPageVM.PozInList + 1) != str)
            {
                while (viewerPageVM.PozInList != viewerPageVM.CurrentData.BackStack.Count - 1)
                {
                    viewerPageVM.CurrentData.BackStack.RemoveAt(viewerPageVM.PozInList + 1);
                }
            }

            viewerPageVM.CurrentData.BackStack.Add(str);


            viewerPageVM.PozInList++;
        }

        viewerPageVM.CurrentPathDisplayed = "Filter results";
    }
    public static void FilterItemsCreateFolder(CreateFolderVM viewerPageVM, string actualPath)
    {
        var list = viewerPageVM.CurrentData.AllItems;
        List<Element> results;

        if (!(bool)viewerPageVM.FilterType) //cu reuniune 
        {
            results = list.Where(item =>
                item.Path.Contains(actualPath) && (viewerPageVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0 || viewerPageVM.SearchApplied == false) &&
                (viewerPageVM.SelectedPriority == null || viewerPageVM.SelectedPriority == "All Items" || viewerPageVM.SelectedPriority != "All Items" && item.Priority == viewerPageVM.SelectedPriority) &&
                (viewerPageVM.SelectedCategory == null || viewerPageVM.SelectedCategory.CategoryName == "All Items" || viewerPageVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.SolidColorBrushColor.ToString() == viewerPageVM.SelectedCategory.SolidColorBrushColor.ToString() && c.CategoryName == viewerPageVM.SelectedCategory.CategoryName)) &&
                (viewerPageVM.SelectedLanguage == null || viewerPageVM.SelectedLanguage == "All Items" || viewerPageVM.SelectedLanguage != "All Items" && item.Language == viewerPageVM.SelectedLanguage) &&
                (viewerPageVM.SelectedCodeLanguage == null || viewerPageVM.SelectedCodeLanguage == "All Items" || viewerPageVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == viewerPageVM.SelectedCodeLanguage)
            ).ToList();
        }
        else
        {
            results = list.Where(item =>
                item.Path.Contains(actualPath) && (viewerPageVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0 || viewerPageVM.SearchApplied == false) &&
                (viewerPageVM.SelectedPriority == null || viewerPageVM.SelectedPriority == "All Items" || viewerPageVM.SelectedPriority != "All Items" && item.Priority == viewerPageVM.SelectedPriority ||
                viewerPageVM.SelectedCategory == null || viewerPageVM.SelectedCategory.CategoryName == "All Items" || viewerPageVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.SolidColorBrushColor.ToString() == viewerPageVM.SelectedCategory.SolidColorBrushColor.ToString() && c.CategoryName == viewerPageVM.SelectedCategory.CategoryName) ||
                viewerPageVM.SelectedLanguage == null || viewerPageVM.SelectedLanguage == "All Items" || viewerPageVM.SelectedLanguage != "All Items" && item.Language == viewerPageVM.SelectedLanguage ||
                viewerPageVM.SelectedCodeLanguage == null || viewerPageVM.SelectedCodeLanguage == "All Items" || viewerPageVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == viewerPageVM.SelectedCodeLanguage)
            ).ToList();
        }
        viewerPageVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(results);
    }

    public static void LoadPathImplementation(ViewerPageVM _viewerPageVM)
    {
        var localOrDriverVM = new LocalOrDriverVM();
        LocalOrDriveWindow localOrDriveWindow = new LocalOrDriveWindow { DataContext = localOrDriverVM };
        localOrDriverVM.CloseAction = localOrDriveWindow.Close; // Pass the Close action to the ViewModel
        localOrDriveWindow.ShowDialog();
        HelperDrive.ClearFolder();

        if (localOrDriverVM.ButtonClicked == "Local")
        {
            _viewerPageVM.CurrentData.DriveOrLocal = false;
            var folderBrowser = new VistaFolderBrowserDialog();
            if (folderBrowser.ShowDialog() == true)
            {
                string selectedPath = folderBrowser.SelectedPath;

                ViewerPageHelper.LoadItemsAndUpdatePath(_viewerPageVM, selectedPath);
            }
        }
        else if (localOrDriverVM.ButtonClicked == "Drive")
        {
            _viewerPageVM.CurrentData.DriveOrLocal = true;
            _viewerPageVM = HelperDrive.LoadFilesFromGoogleDrive(_viewerPageVM);
            var firstParent = _viewerPageVM.CurrentPath.Split('\\')[0];
            UpdateTextBox(_viewerPageVM, firstParent);
            ListElementsInCurrentPath(_viewerPageVM);
            _viewerPageVM.CurrentData.SpacePath = new string(_viewerPageVM.CurrentPath);
            _viewerPageVM.CurrentData.BackStack.Add(_viewerPageVM.CurrentPath);
            ViewerPageHelper.LanguageAndCodeForElementsDrive(_viewerPageVM, _viewerPageVM.SettingsDatas);
        }
    }

    public static void ReloadDriveFiles(ViewerPageVM _viewerPageVM)
    {
        HelperDrive.ClearFolder();

        _viewerPageVM.CurrentData.DriveOrLocal = true;
        _viewerPageVM = HelperDrive.LoadFilesFromGoogleDrive(_viewerPageVM);
        var firstParent = _viewerPageVM.CurrentPath.Split('\\')[0];
        UpdateTextBox(_viewerPageVM, firstParent);
        ListElementsInCurrentPath(_viewerPageVM);
        _viewerPageVM.CurrentData.SpacePath = new string(_viewerPageVM.CurrentPath);
        _viewerPageVM.CurrentData.BackStack.Add(_viewerPageVM.CurrentPath);
        ViewerPageHelper.LanguageAndCodeForElementsDrive(_viewerPageVM, _viewerPageVM.SettingsDatas);

    }
    public static void LoadItemsAndUpdatePath(ViewerPageVM _viewerPageVM, string selectedPath)
    {
        _viewerPageVM.CurrentData.AllItems.Clear();

        try
        {
            LoadItems(_viewerPageVM, selectedPath);
            UpdateTextBox(_viewerPageVM, selectedPath);
            ListElementsInCurrentPath(_viewerPageVM);
            _viewerPageVM.CurrentData.SpacePath = new string(_viewerPageVM.CurrentPath);
            _viewerPageVM.CurrentData.BackStack.Add(_viewerPageVM.CurrentPath);
            LanguageAndCodeForElements(_viewerPageVM, _viewerPageVM.SettingsDatas);
        }
        catch (UnauthorizedAccessException)
        {
            _viewerPageVM.CurrentData.AllItems.Clear();
            System.Windows.MessageBox.Show($"Access to the path '{selectedPath}' is denied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            _viewerPageVM.CurrentData.AllItems.Clear();
            System.Windows.MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    public static void ListElementsInCurrentPath(ViewerPageVM _viewerPageVM, string actualPath = "")
    {
        if (actualPath == "")
        {
            _viewerPageVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(_viewerPageVM.CurrentData.AllItems.Where(item => item.Path == _viewerPageVM.CurrentPath).ToList());
        }
    }

    private static void UpdateTextBox(ViewerPageVM _viewerPageVM, string path)
    {
        _viewerPageVM.CurrentPath = path;
        _viewerPageVM.CurrentPathDisplayed = path;
    }

    public static void LoadItems(ViewerPageVM _viewerPageVM, string path)     //path - cel de unde trebuie sa ia tot contentul
    {
        string[] files = Directory.GetFiles(path);
        string[] folders = Directory.GetDirectories(path);

        foreach (var file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            string extension = Path.GetExtension(file);

            //make extension lower case
            extension = extension.ToLower();

            switch (extension)
            {
                //audio
                case ".m4a":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                    break;
                case ".mp3":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                    break;
                case ".mpga":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                    break;
                case ".wav":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                    break;
                case ".mpeg":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                    break;

                //video
                case ".mp4":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                    break;
                case ".avi":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                    break;
                case ".mov":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                    break;
                case ".flv":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                    break;
                case ".wmv":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                    break;
                case ".webm":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                    break;
                case ".mpg":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                    break;
                case ".3gp":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                    break;

                //orher files
                case ".txt":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), extension));
                    break;
                case ".pdf":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FilePdfBox", new SolidColorBrush(Colors.Red), extension));
                    break;
                case ".png":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FilePngBox", new SolidColorBrush(Colors.Fuchsia), extension));
                    break;
                case ".jpg":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "ImageJpgBox", new SolidColorBrush(Colors.Red), extension));
                    break;
                case ".gif":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileGifBox", new SolidColorBrush(Colors.Green), extension));
                    break;
                case ".zip":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FolderZip", new SolidColorBrush(Colors.DarkMagenta), extension));
                    break;
                case ".xls":
                case ".xlsx":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileExcel", new SolidColorBrush(Colors.DarkGreen), extension));
                    break;
                case ".ppt":
                case ".pptx":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FilePowerpoint", new SolidColorBrush(Colors.DarkOrange), extension));
                    break;
                case ".exe":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "Application", new SolidColorBrush(Colors.DarkBlue), extension));
                    break;
                case ".doc":
                case ".docx":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FileWord", new SolidColorBrush(Colors.DarkBlue), extension));
                    break;
                case ".rar":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "FolderZip", new SolidColorBrush(Colors.MediumPurple), extension));
                    break;
                case ".jpeg":
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "ImageJpegBox", new SolidColorBrush(Colors.DarkRed), extension));
                    break;
                default:
                    _viewerPageVM.CurrentData.AllItems.Add(new Element(path, fileName, "File", new SolidColorBrush(Colors.LightSlateGray), extension));
                    break;
            }
        }

        foreach (var folder in folders)
        {
            string folderName = Path.GetFileName(folder);
            _viewerPageVM.CurrentData.AllItems.Add(new Element(path, folderName, "Folder", new SolidColorBrush(Colors.DodgerBlue), "Folder"));

            LoadItems(_viewerPageVM, folder);
        }
    }

    public static string DetectLanguage(string filePath, string otherArgument)
    {
        if(otherArgument=="t")
        {
            //keep only the first 8000 characters from filePath
            filePath = filePath.Length > 8000 ? filePath.Substring(0, 8000) : filePath;
        }

        string scriptPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        scriptPath = Path.Combine(scriptPath, "ScriptsAnsBatches");
        scriptPath = Path.Combine(scriptPath, "run_script.bat");
        string base64FilePath = Convert.ToBase64String(Encoding.UTF8.GetBytes(filePath));
        string parameter = "\"" + base64FilePath + "\" " + otherArgument;

        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = scriptPath,
            Arguments = parameter,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        Process process = new Process() { StartInfo = startInfo };
        process.Start();

        string result = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        return result;
    }




    static void MostFrequentCppKeyword(string code)
    {
        string[] keywords = new string[] { "size_t", "std", "vector", "main", "<endl", "iostream", "string", "include", "auto", "double", "int", "struct", "break", "else", "long", "switch", "case", "enum", "typedef", "char", "extern", "return", "union", "const", "float", "short", "unsigned", "continue", "for", "signed", "void", "default", "goto", "sizeof", "volatile", "do", "if", "static", "while", "new", "delete", "try", "catch", "bool", "false", "true", "namespace", "using", "class", "public", "protected", "private", "friend", "virtual", "inline", "dynamic_cast", "static_cast", "reinterpret_cast", "const_cast", "template", "mutable", "operator", "typeid", "typename", "explicit", "this", "nullptr", "asm", "bitand", "bitor", "not", "not_eq", "or_eq", "xor", "xor_eq", "and_eq", "and", "alignas", "alignof", "constexpr", "decltype", "noexcept", "nullptr", "static_assert", "thread_local", "cout", "cin" };
        StringBuilder message = new StringBuilder();

        foreach (string keyword in keywords)
        {
            int count = Regex.Matches(code, keyword).Count;
            message.AppendLine($"{keyword}: {count}");
        }

        // Write the results to a text file
        File.WriteAllText("C:\\Users\\Kiwy\\Desktop\\FilesOrganizer\\FilesOrganizer\\Views\\TextFile.txt", message.ToString());
    }
    static void MostFrequentCSharpKeywords(string code)
    {
        string[] keywords = new string[] { "System", "ToString", "Console", "WriteLine", "Write", "interface", "internal", "unchecked", "using", "throw", "operator", "from", "sbyte", "short", "set", "const", "static", "continue", "orderby", "as", "namespace", "ascending", "let", "goto", "protected", "finally", "params", "enum", "false", "unsafe", "virtual", "out", "value", "is", "add", "public", "break", "char", "double", "int", "delegate", "sealed", "override", "ref", "new", "do", "void", "fixed", "sizeof", "dynamic", "byte", "case", "private", "get", "while", "descending", "for", "true", "try", "select", "bool", "remove", "switch", "abstract", "event", "global", "foreach", "float", "var", "uint", "implicit", "ulong", "explicit", "decimal", "volatile", "object", "null", "lock", "join", "yield", "where", "catch", "async", "checked", "class", "long", "typeof", "struct", "stackalloc", "extern", "group", "if", "partial", "return", "into", "alias", "readonly", "else", "default", "base", "ushort", "string", "await" };
        StringBuilder message = new StringBuilder();

        foreach (string keyword in keywords)
        {
            if (keyword == "for" || keyword == "out")
            {
                int count = Regex.Matches(code, $@"\b{keyword}").Count;
                message.AppendLine($"{keyword}: {count}");
            }
            else if (keyword == "as" || keyword == "is")
            {
                int count = Regex.Matches(code, $@"\b{keyword}\b").Count;
                message.AppendLine($"{keyword}: {count}");
            }
            else
            {
                int count = Regex.Matches(code, keyword).Count;
                message.AppendLine($"{keyword}: {count}");
            }
        }

        // Write the results to a text file
        File.WriteAllText("C:\\Users\\Kiwy\\Desktop\\FilesOrganizer\\FilesOrganizer\\Views\\TextFile.txt", message.ToString());
    }
    static void MostFrequentPythonKeywords(string code)
    {
        string[] keywords = new string[] { "assert", "while", "or", "def", "cos", "raise", "data", "for", "is", "try", "from", "del", "class", "and", "in", "type", "pass", "array", "break", "not", "close", "write", "continue", "int", "acos", "import", "if", "print", "exp", "floor", "elif", "except", "lambda", "oxphys", "return", "finally", "range", "exec", "global", "numeric", "fabs", "float", "atan", "asin", "open", "zeros", "sqrt", "else" };
        StringBuilder message = new StringBuilder();

        foreach (string keyword in keywords)
        {
            if (keyword == "in")
            {
                int count = Regex.Matches(code, $@"\b{keyword}\b").Count;
                message.AppendLine($"{keyword}: {count}");
            }
            else
            {
                int count = Regex.Matches(code, keyword).Count;
                message.AppendLine($"{keyword}: {count}");
            }
        }

        // Write the results to a text file
        File.WriteAllText("C:\\Users\\Kiwy\\Desktop\\FilesOrganizer\\FilesOrganizer\\Views\\TextFile.txt", message.ToString());
    }
    static void MostFrequentTypeScriptKeywords(string code)
    {
        string[] keywords = new string[] { "console", "log", "alert", "window", "subscribe", "event", "break", "as", "any", "case", "catch", "class", "const", "continue", "debugger", "default", "delete", "do", "else", "enum", "export", "extends", "false", "finally", "for", "from", "function", "if", "implements", "import", "in", "instanceof", "interface", "let", "new", "null", "package", "private", "protected", "public", "return", "super", "switch", "this", "throw", "true", "try", "typeof", "var", "void", "while", "with", "yield" };
        StringBuilder message = new StringBuilder();

        foreach (string keyword in keywords)
        {
            if (keyword == "in" || keyword == "as")
            {
                int count = Regex.Matches(code, $@"\b{keyword}\b").Count;
                message.AppendLine($"{keyword}: {count}");
            }
            else
            {
                int count = Regex.Matches(code, keyword).Count;
                message.AppendLine($"{keyword}: {count}");
            }
        }

        // Write the results to a text file
        File.WriteAllText("C:\\Users\\Kiwy\\Desktop\\FilesOrganizer\\FilesOrganizer\\Views\\TextFile.txt", message.ToString());
    }
    static void MostFrequentJavaKeywords(string code)
    {
        string[] keywords = new string[] { "import", "public class", "public static void" };
        StringBuilder message = new StringBuilder();

        foreach (string keyword in keywords)
        {
            int count = Regex.Matches(code, keyword).Count;
            message.AppendLine($"{keyword}: {count}");
        }

        File.WriteAllText("C:\\Users\\Kiwy\\Desktop\\FilesOrganizer\\FilesOrganizer\\Views\\TextFile.txt", message.ToString());
    }
    static void MostFrequentHTMLwords(string code)
    {
        string[] keywords = @"<html|<head|<title|<body|<h1|<h2|<h3|<h4|<h5|<h6|<p|<br|<hr|<div|<span|<ul|<ol|<li|<dl|<dt|<dd|<a|href|<img|src|<table|<tr|<td|<th|<form|<input|<button|<select|<option|<textarea|<label|<fieldset|<legend|<script|<style|<link|<meta|<header|<footer|<nav|<section|<article|<aside|<figure|<figcaption|<main|<datalist|<output|<progress|<meter|<details|<summary|<command|<canvas|<audio|<video|<source|<track|<embed|<param|<object|<area|<map|<base|<bdo|<bdi|<ruby|<rt|<rp|<data|<time|<mark|<wbr|<ins|<del|<cite|<dfn|<abbr|<address|<em|<strong|<small|<s|<cite|<q|<dfn|<abbr|<time|<code|<var|<samp|<kbd|<sub|<sup|<i|<b|<u|<strike|<big|<font|<basefont|<br|<wbr|<nobr|<tt|<blink|<marquee|/>".Split('|');
        StringBuilder message = new StringBuilder();

        foreach (string keyword in keywords)
        {
            int count = Regex.Matches(code, keyword).Count;
            message.AppendLine($"{keyword}: {count}");
        }

        File.WriteAllText("C:\\Users\\Kiwy\\Desktop\\FilesOrganizer\\FilesOrganizer\\Views\\TextFile.txt", message.ToString());
    }




    #region code
    static int CountPythonKeywords(string code)
    {
        return Regex.Matches(code, @"assert|while|.*{}.*|.*join.*|.*split.*|.*append.*|.*.py|.*lambda.*|.*sorted.*|\bor\b|def|cos|.*add.*|raise|data|for|.*items|try.*|kwargs.*|key|value|from|del|class|and|\bin\b|type|pass|array|break|not|timeit|close|write|continue|int.*|acos|import|if|print.*|input.*|exp|floor|.*list.*|elif|except|oxphys|return|finally|range|.*ceil.*|.*pow.*|.*max.*|round|.*abs.*|.*math.*|exec|global|.*sqrt.*|numeric|fabs|float|atan|asin|open|zeros|sqrt|else").Count;
    }
    static int CountJavaKeywords(string code)
    {
        return Regex.Matches(code, @"JLabel|mouseEntered|mouseReleased|mouseExited|ImageIcon|public|class|Main|static|System.|void|main|.*String.*|.*System.*|System|.*out.*|.*println.*|println|int|double|boolean|char|Scanner|new|.*in|JOptionPane.*|.*showMessageDialog|.*showInputDialog|Integer.*|Double.*|Random|import|java|if.*|else|.*>=.*|.*<=.*|switch|case|break.*|&&|for|\[\]|\[\d+\]|Character|ArrayList.*|.*add.*|.*set.*|.*remove.*|return|Stopwatch|Toolbox|throws|interruptedException|extends|Calendar.*|implements|FileOutputStream|ObjectOutputStream").Count;
    }
    static int CountHtmlKeywords(string code)
    {
        return Regex.Matches(code, @"\b(html|head|title|body|h1|h2|h3|h4|h5|h6|p|br|hr|div|span|ul|ol|li|dl|dt|dd|a|href|img|src|table|tr|td|th|form|input|button|select|option|textarea|label|fieldset|legend|script|style|link|meta|header|footer|nav|section|article|aside|figure|figcaption|main|datalist|output|progress|meter|details|summary|command|canvas|audio|video|source|track|embed|param|object|area|map|base|bdo|bdi|ruby|rt|rp|data|time|mark|wbr|ins|del|cite|dfn|abbr|address|em|strong|small|s|cite|q|dfn|abbr|time|code|var|samp|kbd|sub|sup|i|b|u|strike|big|font|basefont|br|wbr|nobr|tt|blink|marquee)\b").Count;
    }
    static int CountCssKeywords(string code)
    {
        return Regex.Matches(code, @"color|background-color|important|border|margin|padding|display|font-size|font-family|width|height|cursor|position|top|bottom|left|right|align-items|justify-content|flex-direction|flex-wrap|flex|grid-template-columns|grid-template-rows|grid-column|grid-row|grid-area|gap|justify-items|align-content|place-items|place-content|auto|normal|stretch|center|start|end|flex-start|flex-end|self-start|self-end|space-between|space-around|space-evenly|safe|unsafe|baseline|first baseline|last baseline|space-between|space-around|space-evenly|row|row-reverse|column|column-reverse|nowrap|wrap|wrap-reverse|flow|flow-root|table|inline-table|table-row-group|table-header-group|table-footer-group|table-row|table-cell|table-column-group|table-column|table-caption|ruby-base|ruby-text|ruby-base-container|ruby-text-container|inline-block|inline-list-item|inline-flex|inline-grid|run-in|contents|none|absolute|relative|sticky|fixed").Count;
    }
    static int CountCSharpKeywords(string code)
    {
        return Regex.Matches(code, @"using\b|System\b|HttpGet\b|HttpPost\b|Task\b|Math\b|Min\b|Sqrt\b|Main\b|Readline\b|ToString\b|Console|WriteLine|Write\b|interface\b|internal\b|unchecked\b|throw\b|operator\b|from\b|sbyte\b|short\b|set\b|const\b|static\b|continue\b|orderby\b|namespace\b|ascending\b|let\b|goto\b|protected\b|finally\b|params\b|enum\b|false\b|unsafe\b|virtual\b|value\b|add\b|public\b|break\b|char\b|double\b|int\b|delegate\b|sealed\b|override\b|ref\b|new\b|do\b|void\b|fixed\b|sizeof\b|dynamic\b|byte\b|case\b|private\b|get\b|while\b|descending\b|for\b|true\b|try\b|select\b|bool\b|remove\b|switch\b|abstract\b|event\b|global\b|foreach\b|float\b|var\b|uint\b|implicit\b|ulong\b|explicit\b|decimal\b|volatile\b|object\b|null\b|lock\b|join\b|yield\b|where\b|string\b|catch\b|async\b|checked\b|class\b|long\b|typeof\b|struct\b|stackalloc\b|extern\b|group\b|if\b|partial\b|return\b|into\b|readonly\b|else\b|default\b|base\b|ushort\b|await\b").Count;
    }
    static int CountCppKeywords(string code)
    {
        return Regex.Matches(code, @"\bstd\b|\bcpp\b|\bgetline\b|\bvector\b|\bmain\b|\bmax\b|\bmin\b|\bsqrt\b|\bsrand\b|\brand\b|\btime\b|\bthis\b|\b->\b|<endl|\biostream\b|\bstring\b|\binclude\b|.*::.*|\bauto\b|\bdouble\b|\bint\b|\bstruct\b|\bbreak\b|\belse\b|\blong\b|\bswitch\b|\bcase\b|\benum\b|\bthrow\b|\bcatch\b|\btry\b|\bpow\b|\babs\b|\bround\b|\bfloor\b|\bdynamic_cast\b|\bstatic_cast\b|\bconst_cast\b|\boverride\b|\bfinal\b|\bdelete\b|\bnew\b|\btypedef\b|\bchar\b|\bextern\b|\breturn\b|\bunion\b|\bconst\b|\bpair\b|\bfloat\b|\bshort\b|\bunsigned\b|==|\bcontinue\b|\bfor\b|\bsigned\b|\bvoid\b|\bdefault\b|\bgoto\b|\bsizeof\b|\[\]|\[\d+\]|size_t|\bceil\b|\bvolatile\b|&&|\bdo\b|\bif\b|\bstatic\b|\bwhile\b|\bnew\b|\bdelete\b|\btry\b|\bcatch\b|\bbool\b|\bfalse\b|\btrue\b|\bnamespace\b|\busing\b|\bclass\b|\bpublic\b|\bprotected\b|\bprivate\b|\bfriend\b|\bvirtual\b|\binline\b|\bdynamic_cast\b|\bstatic_cast\b|\breinterpret_cast\b|\bconst_cast\b|\btemplate\b|\bmutable\b|\boperator\b|\btypeid\b|\btypename\b|\bexplicit\b|\bthis\b|\bnullptr\b|\basm\b|\bbitand\b|\bbitor\b|\bnot\b|\bnot_eq\b|\bor_eq\b|\bxor\b|\bxor_eq\b|\band_eq\b|\band\b|\balignas\b|\balignof\b|\bconstexpr\b|\bdecltype\b|\bnoexcept\b|\bnullptr\b|\bstatic_assert\b|\bthread_local\b|\bcout\b|\bcin\b|>>|<<").Count;
    }
    static int CountCKeywords(string code)
    {
        //return Regex.Matches(code, @"char|time|fgets|<std|FILE|remove|\[\]|\[\d+\]|srand|fopen|fprintf|fclose|enum|while|const|strncpy|strncat|strlwr|strupt|strcat|strcpy|struct|strset|strnset|strrev|strcmp|strncmp|strcmpi|strnicmp|byte|break|>=|<=|case|double|bool|switch|default|toupper|void|short|unsigned|true|long|%f|%lf|array|printf|typedef|fgets|float|stdin|ctype|time|strlen|while|scanf.*|include|stdio|stdbool|stdlib|for|else|int|return|.*%d.*|.*%c.*|.*%p.*|.*%s.*|do|if|main|^|==|!=|&&|sizeof").Count;
        return Regex.Matches(code, @"\bchar\b|\btime\b|\bfgets\b|<std\b|\bFILE\b|\bremove\b|\[\]\b|\[\d+\]\b|\bsrand\b|\bfopen\b|\bfprintf\b|printf|.*printf.*|\bfclose\b|\benum\b|\bwhile\b|\bconst\b|\bstrncpy\b|\bstrncat\b|\bstrlwr\b|\bstrupr\b|\bstrcat\b|\bstrcpy\b|\bstruct\b|\bstrset\b|\bstrnset\b|\bstrrev\b|\bstrcmp\b|\bstrncmp\b|\bstrcmpi\b|\bstrnicmp\b|\bbyte\b|\bbreak\b|>=|<=|\bcase\b|\bdouble\b|\bbool\b|\bswitch\b|\bdefault\b|\btoupper\b|\bvoid\b|\bshort\b|\bunsigned\b|\btrue\b|\blong\b|%f|%lf|\barray\b|\bprintf\b|\btypedef\b|\bfgets\b|\bfloat\b|\bstdin\b|\bctype\b|\btime\b|\bstrlen\b|\bwhile\b|\bscanf.*|\binclude\b|\bstdio\b|\bstdbool\b|\bstdlib\b|\bfor\b|\belse\b|\bint\b|\breturn\b|.*%d.*|.*%c.*|.*%p.*|.*%s.*|\bdo\b|\bif\b|\bmain\b|^|==|!=|&&|\bsizeof\b").Count;

    }
    static int CountRubyKeywords(string code)
    {
        return Regex.Matches(code, @"puts|print|true|false|.*downcase.*|.*strip.*|.*lenght.*|.*include?.*|\[\d+\]|.*index.*|.*upercase.*|.*abs.*|.*round.*|.*ceil.*|.*floor.*|.*Math|.*sqrt.*|.*log.*|.*chomp.*|.*gets.*|.*to_f.*|.*Array.*|.*Reverse.*|.*sort.*|.*=>.*|def|end|return|or|and|elseif|>=|<=|!=|case|when|while|in|File.*|.*open.*|do|.*readline.*|.*readlines.*|.*write.*|begin|rescue|class|.*new.*|include").Count;
    }

    #region Regex word counting

    //C#
    public static Dictionary<string, int> CountEachCSharpKeyword(string code)
    {
        // Define a list of C# keywords
        var keywords = new List<string> { "using", "System", "HttpGet", "HttpPost", "Task", "Math", "Min", "Main", "Readline", "ToString", "Console", "WriteLine", "Write", "interface", "internal", "unchecked", "throw", "operator", "from", "sbyte", "short", "set", "const", "static", "continue", "orderby", "namespace", "ascending", "let", "goto", "protected", "finally", "params", "enum", "false", "unsafe", "virtual", "value", "add", "public", "break", "char", "double", "int", "delegate", "sealed", "override", "ref", "new", "do", "void", "fixed", "sizeof", "dynamic", "byte", "case", "private", "get", "while", "descending", "for", "true", "try", "select", "bool", "remove", "switch", "abstract", "event", "global", "foreach", "float", "var", "uint", "implicit", "ulong", "explicit", "decimal", "volatile", "object", "null", "lock", "join", "yield", "where", "string", "catch", "async", "checked", "class", "long", "typeof", "struct", "stackalloc", "extern", "group", "if", "partial", "return", "into", "alias", "readonly", "else", "default", "base", "ushort", "await" };

        // Initialize a new dictionary to store the counts
        var keywordCounts = new Dictionary<string, int>();

        // Split the code into words
        var words = code.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Iterate over each word in the code
        foreach (var word in words)
        {
            // If the word is a C# keyword, increment its count in the dictionary
            if (keywords.Contains(word))
            {
                if (keywordCounts.ContainsKey(word))
                {
                    keywordCounts[word]++;
                }
                else
                {
                    keywordCounts[word] = 1;
                }
            }
        }

        // Return the dictionary of keyword counts
        return keywordCounts;
    }



    //C++
    public static Dictionary<string, int> CountEachCppKeyword(string code)
    {
        var keywords = new string[] { "std", "getline", "vector", "main", "this", "endl", "iostream", "string", "include", "auto", "double", "int", "struct", "break", "else", "long", "switch", "case", "enum", "throw", "catch", "try", "dynamic_cast", "static_cast", "const_cast", "override", "final", "delete", "new", "typedef", "char", "extern", "return", "union", "const", "pair", "float", "short", "unsigned", "continue", "for", "signed", "void", "default", "goto", "sizeof", "size_t", "volatile", "do", "if", "static", "while", "bool", "false", "true", "namespace", "using", "class", "public", "protected", "private", "friend", "virtual", "inline", "reinterpret_cast", "template", "mutable", "operator", "typeid", "typename", "explicit", "nullptr", "asm", "bitand", "bitor", "not", "not_eq", "or_eq", "xor", "xor_eq", "and_eq", "and", "alignas", "alignof", "constexpr", "decltype", "noexcept", "static_assert", "thread_local", "cout", "cin" };
        var keywordCounts = new Dictionary<string, int>();

        foreach (var keyword in keywords)
        {
            keywordCounts[keyword] = Regex.Matches(code, @"\b" + keyword + @"\b").Count;
        }

        return keywordCounts;
    }


    #endregion

    public static string RemoveComments(string code)
    {
        var lines = code.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        for (int i = 0; i < lines.Length; i++)
        {
            int commentIndex = lines[i].IndexOf("//");
            if (commentIndex != -1)
            {
                lines[i] = lines[i].Substring(0, commentIndex);
            }
        }
        return string.Join(Environment.NewLine, lines);
    }
    static void ConvertSvgToPng(string svgFilePath, string pngFilePath)
    {
        pngFilePath = Path.Combine(pngFilePath, "Image.png");
        var svgDocument = SvgDocument.Open(svgFilePath);
        var bitmap = svgDocument.Draw();

        var newBitmap = new Bitmap(bitmap.Width, bitmap.Height);

        using (var g = Graphics.FromImage(newBitmap))
        {
            g.Clear(System.Drawing.Color.White);
            g.DrawImageUnscaled(bitmap, 0, 0);
        }

        newBitmap.Save(pngFilePath, System.Drawing.Imaging.ImageFormat.Png);
    }
    static void ConvertAvifToPng(string avifFilePath, string pngFilePath)
    {
        pngFilePath = Path.Combine(pngFilePath, "Image.png");

        using (var image = new MagickImage(avifFilePath))
        {
            image.Write(pngFilePath, MagickFormat.Png);
        }
    }
    public static string TextFromPic(string imagePath)
    {
        string sentence = "";
        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        solutionDirectory = Path.Combine(solutionDirectory, "IntermediateSavings");
        bool ok = false;
        if (imagePath.Contains(".svg"))
        {
            ok = true;
            ConvertSvgToPng(imagePath, solutionDirectory);
            imagePath = Path.Combine(solutionDirectory, "Image.png");
        }
        if (imagePath.Contains(".avif"))
        {
            ok = true;
            ConvertAvifToPng(imagePath, solutionDirectory);
            imagePath = Path.Combine(solutionDirectory, "Image.png");
        }
        try
        {
            using (var engine = new TesseractEngine(@"F:\Descarcari\tesseract\tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(imagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        sentence = page.GetText();
                    }
                }
            }
            if (ok)
            {
                File.Delete(imagePath);
            }
        }
        catch (Exception ex)
        {
            return "";
        }
        return sentence;
    }
    #endregion

    //de sters elem (doar pentru debug)
    public static string AnalyzeCodeFromString(string sentence, Element elem)
    {
        //sentence = RemoveComments(sentence);

        if (string.IsNullOrWhiteSpace(sentence))
        {
            return "None";
        }

        int csharpCount = CountCSharpKeywords(sentence);
        int pythonCount = CountPythonKeywords(sentence);
        int javaCount = CountJavaKeywords(sentence);
        int htmlCount = CountHtmlKeywords(sentence);
        int cssCount = CountCssKeywords(sentence);
        int cppCount = CountCppKeywords(sentence);
        int cCount = CountCKeywords(sentence);
        int totalCount = csharpCount + pythonCount + javaCount + htmlCount + cssCount + cppCount + cCount;

        // Calculate the total number of words in the sentence
        int totalWords = sentence.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
        // Calculate the minimum number of keywords to classify a language
        int minKeywordCount = (int)(totalWords * 0.10); // 10% of total words


        if (totalCount == 0 || totalCount < minKeywordCount)
        {
            return "None";
        }

        double csharpProb = (double)csharpCount / totalCount;
        double pythonProb = (double)pythonCount / totalCount;
        double javaProb = (double)javaCount / totalCount;
        double htmlProb = (double)htmlCount / totalCount;
        double cssProb = (double)cssCount / totalCount;
        double cppProb = (double)cppCount / totalCount;
        double cProb = (double)cCount / totalCount;

        if (csharpCount >= minKeywordCount && csharpProb > pythonProb && csharpProb > javaProb && csharpProb > htmlProb && csharpProb > cssProb && csharpProb > cppProb && csharpProb > cProb)
        {
            return "C#";
        }
        else if (pythonCount >= minKeywordCount && pythonProb > csharpProb && pythonProb > javaProb && pythonProb > htmlProb && pythonProb > cssProb && pythonProb > cppProb && pythonProb > cProb)
        {
            return "Python";
        }
        else if (javaCount >= minKeywordCount && javaProb > csharpProb && javaProb > pythonProb && javaProb > htmlProb && javaProb > cssProb && javaProb > cppProb && javaProb > cProb)
        {
            return "Java";
        }
        else if (htmlCount >= minKeywordCount && htmlProb > csharpProb && htmlProb > pythonProb && htmlProb > javaProb && htmlProb > cssProb && htmlProb > cppProb && htmlProb > cProb)
        {
            return "HTML";
        }
        else if (cssCount >= minKeywordCount && cssProb > csharpProb && cssProb > pythonProb && cssProb > javaProb && cssProb > htmlProb && cssProb > cppProb && cssProb > cProb)
        {
            return "CSS";
        }
        else if (cppCount >= minKeywordCount && cppProb > csharpProb && cppProb > pythonProb && cppProb > javaProb && cppProb > htmlProb && cppProb > cssProb && cppProb > cProb)
        {
            return "C++";
        }
        else if (cCount >= minKeywordCount && cProb > csharpProb && cProb > pythonProb && cProb > javaProb && cProb > htmlProb && cProb > cssProb && cProb > cppProb)
        {
            return "C";
        }

        return "None";
    }
    public static void DetectCodeLanguagePics(ViewerPageVM viewerPageVM)
    {
        ObservableCollection<string> nonScriptExtensions = new ObservableCollection<string>
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".svg"
        };

        StringBuilder results = new StringBuilder();

        for (int i = 0; i < viewerPageVM.CurrentData.AllItems.Count; i++)
        {
            Element item = viewerPageVM.CurrentData.AllItems[i];

            if (nonScriptExtensions.Contains(item.Extension))
            {
                var text = TextFromPic(item.Path + "\\\\" + item.Name);
                item.CodeLanguage = AnalyzeCodeFromString(text, item);
            }

            viewerPageVM.CurrentData.AllItems[i] = item;

            results.AppendLine(item.Name + " " + item.CodeLanguage);
        }

        System.Windows.MessageBox.Show(results.ToString());
    }
    private static string CodeFromExtension(string extension)
    {
        string code;

        switch (extension)
        {
            case ".cs":
                code = "C#";
                break;
            case ".py":
                code = "Python";
                break;
            case ".java":
                code = "Java";
                break;
            case ".html":
                code = "HTML";
                break;
            case ".cpp":
                code = "C++";
                break;
            case ".c":
                code = "C";
                break;
            default:
                code = "None";
                break;
        }

        return code;
    }
    public static string CodeFromVideo(string path)
    {
        var inputFile = new MediaFile { Filename = path };
        List<string> languages = new List<string>();

        using (var engine = new Engine())
        {
            engine.GetMetadata(inputFile);
            string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
            solutionDirectory = Path.Combine(solutionDirectory, "IntermediateSavings");
            solutionDirectory = Path.Combine(solutionDirectory, "Image.jpg");

            for (int k = 1; k < 5; k++)
            {
                var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(inputFile.Metadata.Duration.TotalSeconds * k / 5) };
                var outputFile = new MediaFile { Filename = solutionDirectory };
                engine.GetThumbnail(inputFile, outputFile, options);
                var text = TextFromPic(outputFile.Filename);
                languages.Add(AnalyzeCodeFromString(text, null));
                File.Delete(outputFile.Filename);
            }
        }

        // Find the most common language in the list
        var mostCommonLanguage = languages
            .GroupBy(l => l)
            .OrderByDescending(g => g.Count())
            .First()
            .Key;

        return mostCommonLanguage;
    }

    public static void LanguageAndCodeForElementsDrive(ViewerPageVM viewerPageVM, Models.Settings settings)
    {
        //fisiere din care pot lua text
        var plainTextFileExtensions = new List<string> { ".rtf", ".txt", ".json", ".odt", ".doc", ".docx", ".xls", ".xlsx", ".html", ".htm", ".pdf", ".xml", ".ppt", ".pptx" };

        //fisiere pe care pot aplica tesseractul
        var imageExtensions = new List<string> { ".avif", ".gif", ".heif", ".jpeg", ".jpg", ".png", ".raw", ".svg", ".tiff", ".heic" };

        //python script extensions
        var videoExtensions = new List<string> { ".mp4", ".avi", ".mov", ".mkv", ".flv", ".wmv", ".webm", ".mpeg", ".mpg", ".3gp" };
        var audioExtensions = new List<string> { ".mp3", ".wav" };

        for (int i = 0; i < viewerPageVM.CurrentData.AllItems.Count; i++)
        {
            Element item = viewerPageVM.CurrentData.AllItems[i];
            string extension = Path.GetExtension(item.Name);
            string text = "", language;
            switch (extension)
            {
                case var ext when plainTextFileExtensions.Contains(ext):
                    if ((bool)settings.TextFilesLanguage || (bool)settings.TextFilesCode)
                    {
                        if (viewerPageVM.CurrentData.DriveOrLocal)
                        {
                            text = GetFileContent(viewerPageVM.CurrentData.DriveOrLocal, item);
                        }

                        if ((bool)settings.TextFilesLanguage)
                        {
                            //limba
                            language = DetectLanguage(text, "t");
                            item.Language = LanguageAssociation(language);
                        }
                        if ((bool)settings.TextFilesCode)
                        {
                            //cod
                            item.CodeLanguage = AnalyzeCodeFromString(text, item);
                        }
                    }
                    break;

                case var ext when imageExtensions.Contains(ext):
                    {
                        var path = "";

                        if ((bool)settings.ImagesLanguage || (bool)settings.ImagesCode)
                        {
                            //daca nu exista fisierul, il descarc
                            string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                            solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
                            solutionDirectory = Path.Combine(solutionDirectory, item.Path);
                            if (item.Status == FileStatus.Undownloaded)
                            {
                                HelperDrive.DownloadFile(item.Id, solutionDirectory);
                                item.Status = FileStatus.Downloaded;
                            }

                            string extensionlocal = Path.GetExtension(item.Name);

                            if (string.IsNullOrEmpty(extensionlocal))
                            {
                                path = Path.Combine(solutionDirectory, item.Name + item.Extension);
                            }
                            else
                            {
                                path = Path.Combine(solutionDirectory, item.Name);
                            }

                            //limba
                            text = TextFromPic(path);
                            if ((bool)settings.ImagesLanguage)
                            {
                                language = DetectLanguage(text, "t");
                                item.Language = LanguageAssociation(language);
                            }
                            if ((bool)settings.ImagesCode)
                            {
                                //cod
                                item.CodeLanguage = AnalyzeCodeFromString(text, item);
                            }
                        }
                        break;
                    }

                case var ext when videoExtensions.Contains(ext):
                    {
                        var path = "";

                        if ((bool)settings.VideosLanguage || (bool)settings.VideosCode)
                        {
                            //daca nu exista fisierul, il descarc
                            string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                            solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
                            solutionDirectory = Path.Combine(solutionDirectory, item.Path);
                            if (item.Status == FileStatus.Undownloaded)
                            {
                                HelperDrive.DownloadFile(item.Id, solutionDirectory);
                                item.Status = FileStatus.Downloaded;
                            }

                            string extensionlocal = Path.GetExtension(item.Name);

                            if (string.IsNullOrEmpty(extensionlocal))
                            {
                                path = Path.Combine(solutionDirectory, item.Name + item.Extension);
                            }
                            else
                            {
                                path = Path.Combine(solutionDirectory, item.Name);
                            }
                        }
                        if ((bool)settings.VideosLanguage)
                        {
                            //limba
                            language = DetectLanguage(path, "a");
                            item.Language = LanguageAssociation(GetLastWord(language));
                        }
                        if ((bool)settings.VideosCode)
                        {
                            //cod
                            item.CodeLanguage = CodeFromVideo(path);
                        }
                        break;
                    }
                case var ext when audioExtensions.Contains(ext):
                    {
                        var path = "";
                        if ((bool)settings.VideosLanguage)
                        {
                            //daca nu exista fisierul, il descarc
                            string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                            solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
                            solutionDirectory = Path.Combine(solutionDirectory, item.Path);
                            if (item.Status == FileStatus.Undownloaded)
                            {
                                HelperDrive.DownloadFile(item.Id, solutionDirectory);
                                item.Status = FileStatus.Downloaded;
                            }

                            string extensionlocal = Path.GetExtension(item.Name);

                            if (string.IsNullOrEmpty(extensionlocal))
                            {
                                path = Path.Combine(solutionDirectory, item.Name + item.Extension);
                            }
                            else
                            {
                                path = Path.Combine(solutionDirectory, item.Name);
                            }

                            //limba
                            language = DetectLanguage(path, "a");
                            language = language.Trim();
                            item.Language = LanguageAssociation(language);
                        }
                        break;
                    }
                default:
                    item.Language = "None";
                    item.CodeLanguage = CodeFromExtension(extension);
                    break;
            }
        }
    }

    public static void LanguageAndCodeForElements(ViewerPageVM viewerPageVM, Models.Settings settings)
    {
        //fisiere din care pot lua text
        var plainTextFileExtensions = new List<string> { ".rtf", ".txt", ".json", ".odt", ".doc", ".docx", ".xls", ".xlsx", ".html", ".htm", ".pdf", ".xml", ".ppt", ".pptx" };

        //fisiere pe care pot aplica tesseractul
        var imageExtensions = new List<string> { ".avif", ".gif", ".heif", ".jpeg", ".jpg", ".png", ".raw", ".svg", ".tiff", ".heic" };

        //python script extensions
        var videoExtensions = new List<string> { ".mp4", ".avi", ".mov", ".mkv", ".flv", ".wmv", ".webm", ".mpeg", ".mpg", ".3gp" };
        var audioExtensions = new List<string> { ".mp3", ".wav" };

        for (int i = 0; i < viewerPageVM.CurrentData.AllItems.Count; i++)
        {
            Element item = viewerPageVM.CurrentData.AllItems[i];
            string extension = item.Extension;
            string text, language;
            switch (extension)
            {
                case var ext when plainTextFileExtensions.Contains(ext):
                    if ((bool)settings.TextFilesLanguage || (bool)settings.TextFilesCode)
                    {
                        text = GetFileContent(viewerPageVM.CurrentData.DriveOrLocal, item);
                        if ((bool)settings.TextFilesLanguage)
                        {
                            //limba
                            language = DetectLanguage(text, "t");
                            item.Language = LanguageAssociation(language);
                        }
                        if ((bool)settings.TextFilesCode)
                        {
                            //cod
                            item.CodeLanguage = AnalyzeCodeFromString(text, item);
                        }
                    }
                    break;

                case var ext when imageExtensions.Contains(ext):
                    if ((bool)settings.ImagesLanguage || (bool)settings.ImagesCode)
                    {
                        //limba
                        text = TextFromPic(item.Path + "\\\\" + item.Name + item.Extension);
                        if ((bool)settings.ImagesLanguage)
                        {
                            language = DetectLanguage(text, "t");
                            item.Language = LanguageAssociation(language);
                        }
                        if ((bool)settings.ImagesCode)
                        {
                            //cod
                            item.CodeLanguage = AnalyzeCodeFromString(text, item);
                        }
                    }
                    break;

                case var ext when videoExtensions.Contains(ext):
                    if ((bool)settings.VideosLanguage)
                    {
                        //limba
                        language = DetectLanguage(item.Path + "\\\\" + item.Name + item.Extension, "a");
                        item.Language = LanguageAssociation(GetLastWord(language));
                    }
                    if ((bool)settings.VideosCode)
                    {
                        //cod
                        item.CodeLanguage = CodeFromVideo(item.Path + "\\\\" + item.Name + item.Extension);
                    }
                    break;

                case var ext when audioExtensions.Contains(ext):
                    if ((bool)settings.VideosLanguage)
                    {
                        //limba
                        language = DetectLanguage(item.Path + "\\\\" + item.Name + item.Extension, "a");
                        language = language.Trim();
                        item.Language = LanguageAssociation(language);
                    }
                    break;
                default:
                    item.Language = "None";
                    item.CodeLanguage = CodeFromExtension(extension);
                    break;
            }
        }
    }
    public static string GetLastWord(string str)
    {
        if (str == null || str == "")
        {
            return "None";
        }
        var lines = str.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var lastLine = lines[lines.Length - 1];
        var words = lastLine.Split(' ');
        return words[words.Length - 1];
    }
    public static string LanguageAssociation(string language)
    {
        language = language.Trim(); // Remove leading and trailing whitespace

        string association = "";
        switch (language)
        {
            case "en":
                association = "English";
                break;
            case "fr":
                association = "French";
                break;
            case "es":
                association = "Spanish";
                break;
            case "de":
                association = "German";
                break;
            case "it":
                association = "Italian";
                break;
            case "pt":
                association = "Portuguese";
                break;
            case "nl":
                association = "Dutch";
                break;
            case "pl":
                association = "Polish";
                break;
            case "ru":
                association = "Russian";
                break;
            case "ja":
                association = "Japanese";
                break;
            case "ko":
                association = "Korean";
                break;
            case "ar":
                association = "Arabic";
                break;
            case "he":
                association = "Hebrew";
                break;
            case "tr":
                association = "Turkish";
                break;
            case "el":
                association = "Greek";
                break;
            case "hi":
                association = "Hindi";
                break;
            case "th":
                association = "Thai";
                break;
            case "vi":
                association = "Vietnamese";
                break;
            case "sv":
                association = "Swedish";
                break;
            case "cs":
                association = "Czech";
                break;
            case "da":
                association = "Danish";
                break;
            case "fi":
                association = "Finnish";
                break;
            case "hu":
                association = "Hungarian";
                break;
            case "no":
                association = "Norwegian";
                break;
            case "ro":
                association = "Romanian";
                break;
            case "sk":
                association = "Slovak";
                break;
            case "uk":
                association = "Ukrainian";
                break;
            case "bg":
                association = "Bulgarian";
                break;
            case "ca":
                association = "Catalan";
                break;
            case "hr":
                association = "Croatian";
                break;
            case "id":
                association = "Indonesian";
                break;
            case "af":
                association = "Afrikaans";
                break;
            case "bn":
                association = "Bengali";
                break;
            case "cy":
                association = "Welsh";
                break;
            case "et":
                association = "Estonian";
                break;
            case "fa":
                association = "Persian";
                break;
            case "gu":
                association = "Gujarati";
                break;
            case "kn":
                association = "Kannada";
                break;
            case "lt":
                association = "Lithuanian";
                break;
            case "lv":
                association = "Latvian";
                break;
            case "mk":
                association = "Macedonian";
                break;
            case "ml":
                association = "Malayalam";
                break;
            case "mr":
                association = "Marathi";
                break;
            case "ne":
                association = "Nepali";
                break;
            case "pa":
                association = "Punjabi";
                break;
            case "sl":
                association = "Slovenian";
                break;
            case "so":
                association = "Somali";
                break;
            case "sq":
                association = "Albanian";
                break;
            case "sw":
                association = "Swahili";
                break;
            case "ta":
                association = "Tamil";
                break;
            case "te":
                association = "Telugu";
                break;
            case "tl":
                association = "Tagalog";
                break;
            case "ur":
                association = "Urdu";
                break;
            case "zh-cn":
                association = "Chinese Simplified";
                break;
            case "zh-tw":
                association = "Chinese Traditional";
                break;
        }
        if (association == "")
        {
            association = "None";
        }
        return association;
    }
    public static void SaveData(ViewerPageVM viewerPageVM)
    {
        try
        {
            //check the paths
            var saves = ViewerPageHelper.LoadSavesFromTextFile();

            foreach (var save in saves)
            {
                if (save == viewerPageVM.CurrentData.SpacePath.Split('\\').Last())
                {
                    MessageBox.Show("You already have a space with this name.");
                }
            }

            string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
            string saveFolderPath = Path.Combine(solutionDirectory, "Saves");
            Directory.CreateDirectory(saveFolderPath);

            if (viewerPageVM.CurrentData.SpacePath == null)
            {
                throw new Exception("Cannot save an empty space.");
            }

            string savingName = viewerPageVM.CurrentData.SpacePath.Split('\\').Last();
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(savingName);
            string fileName = $"{fileNameWithoutExtension}.json";

            string filePath = Path.Combine(saveFolderPath, fileName);

            try
            {
                string jsonData = JsonConvert.SerializeObject(viewerPageVM.CurrentData, Formatting.Indented);

                File.WriteAllText(filePath, jsonData);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string savesFilePath = Path.Combine(saveFolderPath, "saves.txt");

            try
            {
                if (!File.Exists(savesFilePath))
                {
                    File.Create(savesFilePath).Close();
                }

                string existingSave = File.ReadLines(savesFilePath).FirstOrDefault(line => line.Trim() == fileNameWithoutExtension);

                if (existingSave == null)
                {
                    File.AppendAllText(savesFilePath, fileNameWithoutExtension + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error updating 'saves' file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static ObservableCollection<string> LoadSavesFromTextFile()
    {
        ObservableCollection<string> saves = new ObservableCollection<string>();
        try
        {
            string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
            string saveFolderPath = Path.Combine(solutionDirectory, "Saves");
            string savesFilePath = Path.Combine(saveFolderPath, "saves.txt");

            if (File.Exists(savesFilePath))
            {
                // Read all lines from the "saves.txt" file and add them to the ObservableCollection
                string[] savedNames = File.ReadAllLines(savesFilePath);
                foreach (string savedName in savedNames)
                {
                    saves.Add(savedName);
                }
            }
            return saves;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error loading saves from file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return saves;
        }
    }
    public static TransmittedData TakeChanges(TransmittedData copyFrom, TransmittedData copyOn)
    {
        copyOn.Categories = copyFrom.Categories;
        foreach (Element item in copyFrom.AllItems)
        {
            if (copyOn.AllItems.Any(x => x.Name == item.Name && x.Path == item.Path))
            {
                Element element = copyOn.AllItems.FirstOrDefault(x => x.Name == item.Name && x.Path == item.Path && x.Extension == item.Extension);
                element.Category = item.Category;
                element.Priority = item.Priority;
            }
        }
        return copyOn;
    }
    public static bool AreObjectsEqual(TransmittedData obj1, TransmittedData obj2)
    {
        if (obj1 == null || obj2 == null)
            return false;

        // Check equality based on relevant properties of TransmittedData
        return obj1.SpacePath == obj2.SpacePath &&
               AreObservableCollectionsEqual(obj1.AllItems, obj2.AllItems);
    }
    private static bool AreObservableCollectionsEqual<T>(ObservableCollection<T> col1, ObservableCollection<T> col2)
    {
        if (col1 == null && col2 == null)
            return true;

        if (col1 == null || col2 == null)
            return false;

        if (col1.Count != col2.Count)
            return false;

        for (int i = 0; i < col1.Count; i++)
        {
            // Use custom equality checks for specific types
            if (!AreEqual(col1[i], col2[i]))
            {
                return false;
            }
        }

        return true;
    }
    private static bool AreEqual(object obj1, object obj2)
    {
        if (obj1 is Element elem1 && obj2 is Element elem2)
        {
            return elem1.Path == elem2.Path &&
                   elem1.Name == elem2.Name &&
                   elem1.Icon == elem2.Icon &&
                   elem1.Extension == elem2.Extension;
        }
        return true;
    }
    public static string GetFileContent(bool driveOrLocal, Element file)
    {
        string fileContent = "";
        string extension = file.Extension;
        var path = "";
        string ext = Path.GetExtension(file.Name);

        if (driveOrLocal)
        {
            string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
            solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
            solutionDirectory = Path.Combine(solutionDirectory, file.Path);
            if (file.Status == FileStatus.Undownloaded)
            {
                HelperDrive.DownloadFile(file.Id, solutionDirectory);
                file.Status = FileStatus.Downloaded;
            }

            //if (string.IsNullOrEmpty(ext))
            //{
            //    path = Path.Combine(solutionDirectory, file.Id + file.Extension);
            //}
            //else
            //{
            //    path = Path.Combine(solutionDirectory, file.Id);
            //}
            path = Path.Combine(solutionDirectory, file.Id + file.Extension);

        }
        else
        {
            path = file.Path + "\\" + file.Name + file.Extension;
        }

        switch (extension)
        {
            case ".rtf":
                using (var rtBox = new RichTextBox())
                {
                    rtBox.LoadFile(path);
                    fileContent = rtBox.Text;
                }
                break;
            case ".txt":
            case ".json":
                fileContent = File.ReadAllText(path);
                break;
            case ".odt":
                {
                    TextDocument document = new TextDocument();
                    document.Load(path);

                    StringBuilder sb = new StringBuilder();
                    foreach (var item in document.Content)
                    {
                        if (item is AODL.Document.Content.Text.Paragraph paragraph)
                        {
                            foreach (var textItem in paragraph.TextContent)
                            {
                                if (textItem is SimpleText simpleText)
                                {
                                    sb.AppendLine(simpleText.Text);
                                }
                            }
                        }
                    }

                    fileContent = sb.ToString();
                    break;
                }
            case ".doc":
            case ".docx":
                using (var document = DocX.Load(path))
                {
                    fileContent = document.Text;
                }
                break;
            case ".xls":
            case ".xlsx":
                {
                    var sb = new StringBuilder();

                    if (extension == ".xls")
                    {
                        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            var hssfwb = new NPOI.HSSF.UserModel.HSSFWorkbook(stream);

                            var sheet = hssfwb.GetSheetAt(0);
                            for (int row = 0; row <= sheet.LastRowNum; row++)
                            {
                                if (sheet.GetRow(row) != null)
                                {
                                    for (int col = 0; col < sheet.GetRow(row).LastCellNum; col++)
                                    {
                                        sb.Append(sheet.GetRow(row).GetCell(col)?.ToString() + " ");
                                    }
                                }
                            }
                        }
                    }
                    else if (extension == ".xlsx")
                    {
                        using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(path)))
                        {
                            foreach (var worksheet in package.Workbook.Worksheets)
                            {
                                for (int row = worksheet.Dimension.Start.Row; row <= worksheet.Dimension.End.Row; row++)
                                {
                                    for (int col = worksheet.Dimension.Start.Column; col <= worksheet.Dimension.End.Column; col++)
                                    {
                                        sb.Append(worksheet.Cells[row, col].Value?.ToString() + " ");
                                    }
                                }
                            }
                        }
                    }

                    fileContent = sb.ToString();
                    break;
                }
            case ".html":
            case ".htm":
                using (var reader = new StreamReader(path))
                {
                    fileContent = reader.ReadToEnd();
                }
                break;
            case ".pdf":
                fileContent = string.Empty;
                using (var reader = new iTextSharp.text.pdf.PdfReader(path))
                {
                    for (int page = 1; page <= reader.NumberOfPages; page++)
                    {
                        fileContent += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page);
                    }
                }
                break;
            case ".xml":
                var doc = XDocument.Load(path);
                fileContent = doc.ToString();
                break;
            case ".ppt":
            case ".pptx":
                using (PresentationDocument presentationDocument = PresentationDocument.Open(path, false))
                {
                    var texts = presentationDocument.PresentationPart.SlideParts
                        .Where(slidePart => slidePart.Slide != null)
                        .SelectMany(slidePart => slidePart.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
                        .SelectMany(para => para.Descendants<DocumentFormat.OpenXml.Drawing.Run>())
                        .SelectMany(run => run.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                        .Select(text => text.Text);

                    fileContent = string.Join(" ", texts);
                }
                break;
            default:
                fileContent = "";
                break;
        }

        //fileContent = fileContent.Substring(0, Math.Min(fileContent.Length, 8000));
        return fileContent;
    }
    public static int FileMatchesCriteria(bool driveOrLocal, Element file, string keyword, bool byContent, bool byName)
    {
        string fileContent = "";
        string extension = Path.GetExtension(file.Extension);

        string normalizedKeyword = new string(keyword.Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray()).ToLower();

        int matches = 0;

        if (byName)
        {
            string normalizedFileName = file.Name.Replace(" ", "").Replace(".", "").ToLower();
            string normalizedKeywordForName = keyword.Replace(" ", "").Replace(".", "").ToLower();

            if (normalizedFileName.Contains(normalizedKeywordForName))
            {
                matches++;
            }
        }

        if (byContent)
        {
            if (driveOrLocal)
            {
                fileContent = GetFileContent(driveOrLocal, file);
            }
            else
            {
                if (extension == ".txt")
                {
                    fileContent = File.ReadAllText(file.Path + "\\\\" + file.Name + file.Extension);
                }
                else if (extension == ".odt")
                {
                    TextDocument document = new TextDocument();
                    document.Load(file.Path + "\\\\" + file.Name + file.Extension);

                    StringBuilder sb = new StringBuilder();
                    foreach (var item in document.Content)
                    {
                        if (item is AODL.Document.Content.Text.Paragraph paragraph)
                        {
                            foreach (var textItem in paragraph.TextContent)
                            {
                                if (textItem is SimpleText simpleText)
                                {
                                    sb.AppendLine(simpleText.Text);
                                }
                            }
                        }
                    }

                    fileContent = sb.ToString();
                }
                else if (extension == ".doc" || extension == ".docx")
                {
                    using (var document = DocX.Load(file.Path + "\\\\" + file.Name + file.Extension))
                    {
                        fileContent = document.Text;
                    }
                }
                else if (extension == ".xls" || extension == ".xlsx")
                {
                    var sb = new StringBuilder();

                    if (extension == ".xls")
                    {
                        using (var stream = new FileStream(file.Path + "\\\\" + file.Name + file.Extension, FileMode.Open, FileAccess.Read))
                        {
                            var hssfwb = new NPOI.HSSF.UserModel.HSSFWorkbook(stream);

                            var sheet = hssfwb.GetSheetAt(0);
                            for (int row = 0; row <= sheet.LastRowNum; row++)
                            {
                                if (sheet.GetRow(row) != null)
                                {
                                    for (int col = 0; col < sheet.GetRow(row).LastCellNum; col++)
                                    {
                                        sb.Append(sheet.GetRow(row).GetCell(col)?.ToString() + " ");
                                    }
                                }
                            }
                        }
                    }
                    else if (extension == ".xlsx")
                    {
                        using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(file.Path + "\\\\" + file.Name + file.Extension)))
                        {
                            foreach (var worksheet in package.Workbook.Worksheets)
                            {
                                for (int row = worksheet.Dimension.Start.Row; row <= worksheet.Dimension.End.Row; row++)
                                {
                                    for (int col = worksheet.Dimension.Start.Column; col <= worksheet.Dimension.End.Column; col++)
                                    {
                                        sb.Append(worksheet.Cells[row, col].Value?.ToString() + " ");
                                    }
                                }
                            }
                        }
                    }

                    fileContent = sb.ToString();
                }
                else if (extension == ".html" || extension == ".htm")
                {
                    using (var reader = new StreamReader(file.Path + "\\\\" + file.Name + file.Extension))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
                else if (extension == ".pdf")
                {
                    fileContent = string.Empty;
                    using (var reader = new iTextSharp.text.pdf.PdfReader(file.Path + "\\\\" + file.Name + file.Extension))
                    {
                        for (int page = 1; page <= reader.NumberOfPages; page++)
                        {
                            fileContent += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page);
                        }
                    }
                }
                else if (extension == ".json")
                {
                    fileContent = File.ReadAllText(file.Path + "\\\\" + file.Name + file.Extension);
                }
                else if (extension == ".xml")
                {
                    var doc = XDocument.Load(file.Path + "\\\\" + file.Name + file.Extension);
                    fileContent = doc.ToString();
                }
                else if (extension == ".ppt" || extension == ".pptx")
                {
                    using (PresentationDocument presentationDocument = PresentationDocument.Open(file.Path + "\\\\" + file.Name + file.Extension, false))
                    {
                        var texts = presentationDocument.PresentationPart.SlideParts
                            .Where(slidePart => slidePart.Slide != null)
                            .SelectMany(slidePart => slidePart.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
                            .SelectMany(para => para.Descendants<DocumentFormat.OpenXml.Drawing.Run>())
                            .SelectMany(run => run.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                            .Select(text => text.Text);

                        fileContent = string.Join(" ", texts);
                    }
                }
                else if (extension == ".rtf")
                {
                    using (var rtBox = new RichTextBox())
                    {
                        rtBox.LoadFile(file.Path + "\\\\" + file.Name + file.Extension);
                        fileContent = rtBox.Text;
                    }
                }
                else
                {
                    fileContent = "";
                }
            }

            string normalizedFileContent = new string(fileContent.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray()).ToLower();

            var contentMatches = Regex.Matches(normalizedFileContent, Regex.Escape(normalizedKeyword));
            matches += contentMatches.Count;
        }
        return matches;
    }
    public static Models.Settings TakeInfosForSettings(ViewerPageVM viewerPageVM)
    {
        List<bool> bools = new List<bool>();
        int similarityThreshold = 0;
        int minValueSSIM = 0;
        int minValueArea = 0;
        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        solutionDirectory = Path.Combine(solutionDirectory, "Saves");
        solutionDirectory = Path.Combine(solutionDirectory, "Settings.txt");

        if (File.Exists(solutionDirectory) && new FileInfo(solutionDirectory).Length > 0)
        {
            try
            {
                using (StreamReader reader = new StreamReader(solutionDirectory))
                {
                    string line;
                    int i = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            if (i != 8)
                                bools.Add(bool.Parse(line));
                            else
                            {
                                similarityThreshold = int.Parse(line);
                                minValueSSIM = int.Parse(reader.ReadLine());
                                minValueArea = int.Parse(reader.ReadLine());
                            }
                            i++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        if (bools.Count != 8)
        {
            return new Models.Settings();
        }
        else
        {
            return new Models.Settings(bools, similarityThreshold, minValueSSIM, minValueArea);
        }
    }
    public static void RecheckCode(SettingsVM settingsVM)
    {
        //fisiere din care pot lua text
        //var plainTextFileExtensions = new List<string> { ".rtf", ".txt", ".json", ".odt", ".doc", ".docx", ".xls", ".xlsx", ".html", ".htm", ".pdf", ".xml", ".ppt", ".pptx" };
        var plainTextFileExtensions = new List<string> { ".rtf", ".txt", ".odt", ".doc", ".docx", ".pdf" };

        //fisiere pe care pot aplica tesseractul
        var imageExtensions = new List<string> { ".avif", ".gif", ".heif", ".jpeg", ".jpg", ".png", ".raw", ".svg", ".tiff", ".heic" };

        //python script extensions
        var videoExtensions = new List<string> { ".mp4", ".avi", ".mov", ".mkv", ".flv", ".wmv", ".webm", ".mpeg", ".mpg", ".3gp" };

        for (int i = 0; i < settingsVM.Datas.AllItems.Count; i++)
        {
            Element item = settingsVM.Datas.AllItems[i];
            string extension = item.Extension;
            string text, language;
            switch (extension)
            {
                case var ext when plainTextFileExtensions.Contains(ext):
                    if ((bool)settingsVM.SettingsDatas.TextFilesCode)
                    {
                        if (settingsVM.Datas.DriveOrLocal)
                        {
                            text = GetFileContent(settingsVM.Datas.DriveOrLocal, item);
                        }
                        else
                        {
                            text = GetFileContent(settingsVM.Datas.DriveOrLocal, item);
                        }
                        item.CodeLanguage = AnalyzeCodeFromString(text, item);
                    }
                    break;

                case var ext when imageExtensions.Contains(ext):
                    if ((bool)settingsVM.SettingsDatas.ImagesCode)
                    {
                        var path = "";
                        if (settingsVM.Datas.DriveOrLocal)
                        {
                            //daca nu exista fisierul, il descarc
                            string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                            solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
                            solutionDirectory = Path.Combine(solutionDirectory, item.Path);
                            if (item.Status == FileStatus.Undownloaded)
                            {
                                HelperDrive.DownloadFile(item.Id, solutionDirectory);
                                item.Status = FileStatus.Downloaded;
                            }

                            string extensionlocal = Path.GetExtension(item.Name);

                            if (string.IsNullOrEmpty(extensionlocal))
                            {
                                path = Path.Combine(solutionDirectory, item.Name + item.Extension);
                            }
                            else
                            {
                                path = Path.Combine(solutionDirectory, item.Name);
                            }
                        }
                        else
                        {
                            path = item.Path + "\\\\" + item.Name + item.Extension;
                        }
                        text = TextFromPic(path);
                        item.CodeLanguage = AnalyzeCodeFromString(text, item);
                    }
                    break;

                case var ext when videoExtensions.Contains(ext):
                    if ((bool)settingsVM.SettingsDatas.VideosCode)
                    {
                        var path = "";
                        if (settingsVM.Datas.DriveOrLocal)
                        {
                            //daca nu exista fisierul, il descarc
                            string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                            solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
                            solutionDirectory = Path.Combine(solutionDirectory, item.Path);
                            if (item.Status == FileStatus.Undownloaded)
                            {
                                HelperDrive.DownloadFile(item.Id, solutionDirectory);
                                item.Status = FileStatus.Downloaded;
                            }

                            string extensionlocal = Path.GetExtension(item.Name);

                            if (string.IsNullOrEmpty(extensionlocal))
                            {
                                path = Path.Combine(solutionDirectory, item.Name + item.Extension);
                            }
                            else
                            {
                                path = Path.Combine(solutionDirectory, item.Name);
                            }
                        }
                        else
                        {
                            path = item.Path + "\\\\" + item.Name + item.Extension;
                        }

                        //cod
                        item.CodeLanguage = CodeFromVideo(path);
                    }
                    break;

                default:
                    item.CodeLanguage = CodeFromExtension(extension);
                    break;
            }
        }
    }

    private static void DeleteImplementation(string fileName)
    {
        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        string saveFolderPath = Path.Combine(solutionDirectory, "Saves");

        string filePath = Path.Combine(saveFolderPath, $"{fileName}.json");
        if (!File.Exists(filePath))
        {
            throw new Exception($"File '{fileName}.json' does not exist.");
        }

        File.Delete(filePath);

        string savesFilePath = Path.Combine(saveFolderPath, "saves.txt");
        if (!File.Exists(savesFilePath))
        {
            throw new Exception("'saves.txt' does not exist.");
        }

        var lines = File.ReadAllLines(savesFilePath).ToList();
        lines.Remove(fileName);
        File.WriteAllLines(savesFilePath, lines);

        //_savesVM.Saves = ViewerPageHelper.LoadSavesFromTextFile();
    }

    public static void CheckWorkingSpaces(SettingsVM settingsVM)
    {
        //check the paths
        var saves = ViewerPageHelper.LoadSavesFromTextFile();

        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        for (int i = 0; i < saves.Count; i++)
        {
            string filePath = Path.Combine(solutionDirectory, "Saves", saves[i]);
            filePath = filePath + ".json";

            string jsonData = File.ReadAllText(filePath);
            TransmittedData loadedData = null;
            loadedData = JsonConvert.DeserializeObject<TransmittedData>(jsonData);
            if (!Directory.Exists(loadedData.SpacePath) && !loadedData.DriveOrLocal)
            {
                DeleteImplementation(saves[i]);
            }
        }
    }
    public static void RecheckLanguage(SettingsVM settingsVM)
    {
        //fisiere din care pot lua text
        var plainTextFileExtensions = new List<string> { ".rtf", ".txt", ".json", ".odt", ".doc", ".docx", ".xls", ".xlsx", ".html", ".htm", ".pdf", ".xml", ".ppt", ".pptx" };

        //fisiere pe care pot aplica tesseractul
        var imageExtensions = new List<string> { ".avif", ".gif", ".heif", ".jpeg", ".jpg", ".png", ".raw", ".svg", ".tiff", ".heic" };

        //python script extensions
        var videoExtensions = new List<string> { ".mp4", ".avi", ".mov", ".mkv", ".flv", ".wmv", ".webm", ".mpeg", ".mpg", ".3gp" };
        var audioExtensions = new List<string> { ".mp3", ".wav" };

        for (int i = 0; i < settingsVM.Datas.AllItems.Count; i++)
        {
            Element item = settingsVM.Datas.AllItems[i];
            string extension = item.Extension;
            string text, language;
            switch (extension)
            {
                case var ext when plainTextFileExtensions.Contains(ext):
                    if ((bool)settingsVM.SettingsDatas.TextFilesLanguage)
                    {
                        if (settingsVM.Datas.DriveOrLocal)
                        {
                            text = GetFileContent(settingsVM.Datas.DriveOrLocal, item);
                        }
                        else
                        {
                            text = GetFileContent(settingsVM.Datas.DriveOrLocal, item);
                        }
                        language = DetectLanguage(text, "t");
                        item.Language = LanguageAssociation(language);
                    }
                    break;

                case var ext when imageExtensions.Contains(ext):
                    if ((bool)settingsVM.SettingsDatas.ImagesLanguage)
                    {
                        var path = "";
                        if (settingsVM.Datas.DriveOrLocal)
                        {
                            //daca nu exista fisierul, il descarc
                            string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                            solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
                            solutionDirectory = Path.Combine(solutionDirectory, item.Path);
                            if (item.Status == FileStatus.Undownloaded)
                            {
                                HelperDrive.DownloadFile(item.Id, solutionDirectory);
                                item.Status = FileStatus.Downloaded;
                            }

                            string extensionlocal = Path.GetExtension(item.Name);

                            if (string.IsNullOrEmpty(extensionlocal))
                            {
                                path = Path.Combine(solutionDirectory, item.Name + item.Extension);
                            }
                            else
                            {
                                path = Path.Combine(solutionDirectory, item.Name);
                            }
                        }
                        else
                        {
                            path = item.Path + "\\\\" + item.Name + item.Extension;
                        }
                        text = TextFromPic(path);
                        language = DetectLanguage(text, "t");
                        item.Language = LanguageAssociation(language);
                    }
                    break;

                case var ext when videoExtensions.Contains(ext):
                    if ((bool)settingsVM.SettingsDatas.VideosLanguage)
                    {
                        var path = "";
                        if (settingsVM.Datas.DriveOrLocal)
                        {
                            //daca nu exista fisierul, il descarc
                            string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                            solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
                            solutionDirectory = Path.Combine(solutionDirectory, item.Path);
                            if (item.Status == FileStatus.Undownloaded)
                            {
                                HelperDrive.DownloadFile(item.Id, solutionDirectory);
                                item.Status = FileStatus.Downloaded;
                            }

                            string extensionlocal = Path.GetExtension(item.Name);

                            if (string.IsNullOrEmpty(extensionlocal))
                            {
                                path = Path.Combine(solutionDirectory, item.Name + item.Extension);
                            }
                            else
                            {
                                path = Path.Combine(solutionDirectory, item.Name);
                            }
                        }
                        else
                        {
                            path = item.Path + "\\\\" + item.Name + item.Extension;
                        }

                        //limba
                        language = DetectLanguage(path, "a");
                        item.Language = GetLastWord(language);
                        item.Language = LanguageAssociation(item.Language);
                    }
                    break;

                case var ext when audioExtensions.Contains(ext):
                    if ((bool)settingsVM.SettingsDatas.AudiosLanguage)
                    {
                        var path = "";
                        if (settingsVM.Datas.DriveOrLocal)
                        {
                            //daca nu exista fisierul, il descarc
                            string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
                            solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
                            solutionDirectory = Path.Combine(solutionDirectory, item.Path);
                            if (item.Status == FileStatus.Undownloaded)
                            {
                                HelperDrive.DownloadFile(item.Id, solutionDirectory);
                                item.Status = FileStatus.Downloaded;
                            }

                            string extensionlocal = Path.GetExtension(item.Name);

                            if (string.IsNullOrEmpty(extensionlocal))
                            {
                                path = Path.Combine(solutionDirectory, item.Name + item.Extension);
                            }
                            else
                            {
                                path = Path.Combine(solutionDirectory, item.Name);
                            }
                        }
                        else
                        {
                            path = item.Path + "\\\\" + item.Name + item.Extension;
                        }

                        //limba
                        language = DetectLanguage(path, "a");
                        language = language.Trim();
                        item.Language = language;
                        item.Language = LanguageAssociation(item.Language);
                    }
                    break;
                default:
                    item.Language = "None";
                    break;
            }
        }

        for (int i = 0; i < settingsVM.Datas.CurrentListBoxSource.Count; i++)
        {
            for (int j = 0; j < settingsVM.Datas.AllItems.Count; j++)
            {
                if (settingsVM.Datas.CurrentListBoxSource[i].Name == settingsVM.Datas.AllItems[j].Name &&
                    settingsVM.Datas.CurrentListBoxSource[i].Extension == settingsVM.Datas.AllItems[j].Extension)
                {
                    settingsVM.Datas.CurrentListBoxSource[i].Language = settingsVM.Datas.AllItems[j].Language;
                }
            }
        }
        settingsVM.Datas.OnPropertyChanged(nameof(settingsVM.Datas.CurrentListBoxSource));
        settingsVM.MainViewModel.CurrentData = settingsVM.Datas;
        settingsVM.MainViewModel.CurrentData.OnPropertyChanged(nameof(settingsVM.MainViewModel.CurrentData.CurrentListBoxSource));
    }
    public static void AddFileToList(CreateFolderVM createFolderVM)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            string selectedFileName = openFileDialog.FileName;

            for (int i = 0; i < createFolderVM.CurrentData.AllItems.Count; i++)
            {
                if (selectedFileName == createFolderVM.CurrentData.AllItems[i].Path + "\\" + createFolderVM.CurrentData.AllItems[i].Name + createFolderVM.CurrentData.AllItems[i].Extension)
                {
                    System.Windows.MessageBox.Show("The file is already in the list.");
                    return;
                }
            }
            LoadOneElement(createFolderVM, selectedFileName);
        }
    }

    public static void AddFileToListDrive(CreateFolderVM createFolderVM)
    {
        var fileExplorerDriveVM = new FileExplorerDriveVM(createFolderVM.CurrentData.SpacePath, createFolderVM.CurrentData, createFolderVM.PozInList, "File", createFolderVM.AllItems);
        var fileExplorerDriveWindow = new FileExplorerDriveWindow { DataContext = fileExplorerDriveVM };
        fileExplorerDriveWindow.ShowDialog();

        var temp = fileExplorerDriveVM.SelectedItem;

        if (temp == null)
        {
            return;
        }
        for (int i = 0; i < createFolderVM.CurrentData.AllItems.Count; i++)
        {
            if (temp.Path + "\\" + temp.Name + temp.Extension == createFolderVM.CurrentData.AllItems[i].Path + "\\" + createFolderVM.CurrentData.AllItems[i].Name + createFolderVM.CurrentData.AllItems[i].Extension)
            {
                System.Windows.MessageBox.Show("The file is already in the list.");
                return;
            }
        }
        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
        solutionDirectory = Path.Combine(solutionDirectory, temp.Path);
        if (temp.Status == FileStatus.Undownloaded)
        {
            HelperDrive.DownloadFile(temp.Id, solutionDirectory);
            temp.Status = FileStatus.Downloaded;
        }


        temp.Priority = createFolderVM.SelectedPriority;
        temp.Category.Add(createFolderVM.SelectedCategory);
        temp.Language = createFolderVM.SelectedLanguage;
        temp.CodeLanguage = createFolderVM.SelectedCodeLanguage;

        createFolderVM.CurrentData.CurrentListBoxSource.Add(temp);
        createFolderVM.CurrentData.AllItems.Add(temp);
    }

    private static void LoadOneElement(CreateFolderVM createFolderVM, string path)
    {
        string fileName = Path.GetFileName(path);
        fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
        string extension = Path.GetExtension(path);
        path = Path.GetDirectoryName(path);
        Element element;

        extension = extension.ToLower();

        switch (extension)
        {
            //audio
            case ".m4a":
                element = new Element(path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension);
                break;
            case ".mp3":
                element = new Element(path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension);
                break;
            case ".mpga":
                element = new Element(path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension);
                break;
            case ".wav":
                element = new Element(path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension);
                break;
            case ".mpeg":
                element = new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension);
                break;

            //video
            case ".mp4":
                element = new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension);
                break;
            case ".avi":
                element = new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension);
                break;
            case ".mov":
                element = new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension);
                break;
            case ".flv":
                element = new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension);
                break;
            case ".wmv":
                element = new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension);
                break;
            case ".webm":
                element = new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension);
                break;
            case ".mpg":
                element = new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension);
                break;
            case ".3gp":
                element = new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension);
                break;

            //orher files
            case ".txt":
                element = new Element(path, fileName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), extension);
                break;
            case ".pdf":
                element = new Element(path, fileName, "FilePdfBox", new SolidColorBrush(Colors.Red), extension);
                break;
            case ".png":
                element = new Element(path, fileName, "FilePngBox", new SolidColorBrush(Colors.Fuchsia), extension);
                break;
            case ".jpg":
                element = new Element(path, fileName, "ImageJpgBox", new SolidColorBrush(Colors.Red), extension);
                break;
            case ".gif":
                element = new Element(path, fileName, "FileGifBox", new SolidColorBrush(Colors.Green), extension);
                break;
            case ".zip":
                element = new Element(path, fileName, "FolderZip", new SolidColorBrush(Colors.DarkMagenta), extension);
                break;
            case ".xls":
            case ".xlsx":
                element = new Element(path, fileName, "FileExcel", new SolidColorBrush(Colors.DarkGreen), extension);
                break;
            case ".ppt":
            case ".pptx":
                element = new Element(path, fileName, "FilePowerpoint", new SolidColorBrush(Colors.DarkOrange), extension);
                break;
            case ".exe":
                element = new Element(path, fileName, "Application", new SolidColorBrush(Colors.DarkBlue), extension);
                break;
            case ".doc":
            case ".docx":
                element = new Element(path, fileName, "FileWord", new SolidColorBrush(Colors.DarkBlue), extension);
                break;
            case ".rar":
                element = new Element(path, fileName, "FolderZip", new SolidColorBrush(Colors.MediumPurple), extension);
                break;
            case ".jpeg":
                element = new Element(path, fileName, "ImageJpegBox", new SolidColorBrush(Colors.DarkRed), extension);
                break;
            default:
                element = new Element(path, fileName, "File", new SolidColorBrush(Colors.LightSlateGray), extension);
                break;
        }

        element.Priority = createFolderVM.SelectedPriority;
        element.Category.Add(createFolderVM.SelectedCategory);
        element.Language = createFolderVM.SelectedLanguage;
        element.CodeLanguage = createFolderVM.SelectedCodeLanguage;
        createFolderVM.CurrentData.AllItems.Add(element);
        createFolderVM.CurrentData.CurrentListBoxSource.Add(element);
    }
    private static void LoadItems(CreateFolderVM createFolderVM, string path, string truePath)
    {
        string[] files = Directory.GetFiles(truePath);
        string[] folders = Directory.GetDirectories(truePath);

        foreach (var file in files)
        {
            string fileName = Path.GetFileName(file);
            string extension = Path.GetExtension(file);
            Element element;

            extension = extension.ToLower();
            switch (extension)
            {
                //audio
                case ".m4a":
                case ".mp3":
                case ".mpga":
                case ".wav":
                case ".mpeg":
                    element = new Element(path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension);
                    break;

                //video
                case ".mp4":
                case ".avi":
                case ".mov":
                case ".flv":
                case ".wmv":
                case ".webm":
                case ".mpg":
                case ".3gp":
                    element = new Element(path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension);
                    break;

                //other files
                case ".txt":
                    element = new Element(path, fileName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), extension);
                    break;
                case ".pdf":
                    element = new Element(path, fileName, "FilePdfBox", new SolidColorBrush(Colors.Red), extension);
                    break;
                case ".png":
                    element = new Element(path, fileName, "FilePngBox", new SolidColorBrush(Colors.Fuchsia), extension);
                    break;
                case ".jpg":
                    element = new Element(path, fileName, "ImageJpgBox", new SolidColorBrush(Colors.Red), extension);
                    break;
                case ".gif":
                    element = new Element(path, fileName, "FileGifBox", new SolidColorBrush(Colors.Green), extension);
                    break;
                case ".zip":
                    element = new Element(path, fileName, "FolderZip", new SolidColorBrush(Colors.DarkMagenta), extension);
                    break;
                case ".xls":
                case ".xlsx":
                    element = new Element(path, fileName, "FileExcel", new SolidColorBrush(Colors.DarkGreen), extension);
                    break;
                case ".ppt":
                case ".pptx":
                    element = new Element(path, fileName, "FilePowerpoint", new SolidColorBrush(Colors.DarkOrange), extension);
                    break;
                case ".exe":
                    element = new Element(path, fileName, "DarkBlue", new SolidColorBrush(Colors.Black), extension);
                    break;
                case ".doc":
                case ".docx":
                    element = new Element(path, fileName, "FileWord", new SolidColorBrush(Colors.DarkBlue), extension);
                    break;
                case ".rar":
                    element = new Element(path, fileName, "FolderZip", new SolidColorBrush(Colors.MediumPurple), extension);
                    break;
                case ".jpeg":
                    element = new Element(path, fileName, "ImageJpegBox", new SolidColorBrush(Colors.DarkRed), extension);
                    break;
                default:
                    element = new Element(path, fileName, "File", new SolidColorBrush(Colors.LightSlateGray), extension);
                    break;
            }

            element.Priority = createFolderVM.SelectedPriority;
            element.Category.Clear();
            element.Category.Add(createFolderVM.SelectedCategory);
            element.Language = createFolderVM.SelectedLanguage;
            element.CodeLanguage = createFolderVM.SelectedCodeLanguage;

            if (!(bool)createFolderVM.FilterType)
            {
                if (!createFolderVM.CurrentData.AllItems.Any(item => item.Path == element.Path && item.Name == element.Name &&
                    (createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0 || createFolderVM.SearchApplied == false) &&
                    (createFolderVM.SelectedPriority == null || createFolderVM.SelectedPriority == "All Items" || createFolderVM.SelectedPriority != "All Items" && item.Priority == createFolderVM.SelectedPriority) &&
                    (createFolderVM.SelectedCategory == null || createFolderVM.SelectedCategory.CategoryName == "All Items" || createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.SolidColorBrushColor.ToString() == createFolderVM.SelectedCategory.SolidColorBrushColor.ToString() && c.CategoryName == createFolderVM.SelectedCategory.CategoryName)) &&
                    (createFolderVM.SelectedLanguage == null || createFolderVM.SelectedLanguage == "All Items" || createFolderVM.SelectedLanguage != "All Items" && item.Language == createFolderVM.SelectedLanguage) &&
                    (createFolderVM.SelectedCodeLanguage == null || createFolderVM.SelectedCodeLanguage == "All Items" || createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == createFolderVM.SelectedCodeLanguage)
                )) //cu reuniune 
                {
                    createFolderVM.CurrentData.AllItems.Add(element);
                }
            }
            else
            {
                if (!createFolderVM.CurrentData.AllItems.Any(item => item.Path == element.Path && item.Name == element.Name &&
                    ((createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0 || createFolderVM.SearchApplied == false) &&
                    (createFolderVM.SelectedPriority == null || createFolderVM.SelectedPriority == "All Items" || createFolderVM.SelectedPriority != "All Items" && item.Priority == createFolderVM.SelectedPriority) ||
                    createFolderVM.SelectedCategory == null || createFolderVM.SelectedCategory.CategoryName == "All Items" || createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.SolidColorBrushColor.ToString() == createFolderVM.SelectedCategory.SolidColorBrushColor.ToString() && c.CategoryName == createFolderVM.SelectedCategory.CategoryName) ||
                    createFolderVM.SelectedLanguage == null || createFolderVM.SelectedLanguage == "All Items" || createFolderVM.SelectedLanguage != "All Items" && item.Language == createFolderVM.SelectedLanguage ||
                    createFolderVM.SelectedCodeLanguage == null || createFolderVM.SelectedCodeLanguage == "All Items" || createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == createFolderVM.SelectedCodeLanguage
                    )))
                {
                    createFolderVM.CurrentData.AllItems.Add(element);
                }
            }
        }

        foreach (var folder in folders)
        {
            string folderName = Path.GetFileName(folder);
            truePath += "\\" + folderName;
            var element = new Element(path, folderName, "Folder", new SolidColorBrush(Colors.DodgerBlue), "Folder");
            path += "\\" + folderName;

            element.Priority = createFolderVM.SelectedPriority;
            element.Category.Clear();
            element.Category.Add(createFolderVM.SelectedCategory);
            element.Language = createFolderVM.SelectedLanguage;
            element.CodeLanguage = createFolderVM.SelectedCodeLanguage;

            if (!(bool)createFolderVM.FilterType)
            {
                if (!createFolderVM.CurrentData.AllItems.Any(item => item.Path == element.Path && item.Name == element.Name &&
                    (createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0 || createFolderVM.SearchApplied == false) &&
                    (createFolderVM.SelectedPriority == null || createFolderVM.SelectedPriority == "All Items" || createFolderVM.SelectedPriority != "All Items" && item.Priority == createFolderVM.SelectedPriority) &&
                    (createFolderVM.SelectedCategory == null || createFolderVM.SelectedCategory.CategoryName == "All Items" || createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.SolidColorBrushColor.ToString() == createFolderVM.SelectedCategory.SolidColorBrushColor.ToString() && c.CategoryName == createFolderVM.SelectedCategory.CategoryName)) &&
                    (createFolderVM.SelectedLanguage == null || createFolderVM.SelectedLanguage == "All Items" || createFolderVM.SelectedLanguage != "All Items" && item.Language == createFolderVM.SelectedLanguage) &&
                    (createFolderVM.SelectedCodeLanguage == null || createFolderVM.SelectedCodeLanguage == "All Items" || createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == createFolderVM.SelectedCodeLanguage)
                )) //cu reuniune 
                {
                    createFolderVM.CurrentData.AllItems.Add(element);
                }
            }
            else
            {
                if (!createFolderVM.CurrentData.AllItems.Any(item => item.Path == element.Path && item.Name == element.Name &&
                    ((createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0 || createFolderVM.SearchApplied == false) &&
                    (createFolderVM.SelectedPriority == null || createFolderVM.SelectedPriority == "All Items" || createFolderVM.SelectedPriority != "All Items" && item.Priority == createFolderVM.SelectedPriority) ||
                    createFolderVM.SelectedCategory == null || createFolderVM.SelectedCategory.CategoryName == "All Items" || createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.SolidColorBrushColor.ToString() == createFolderVM.SelectedCategory.SolidColorBrushColor.ToString() && c.CategoryName == createFolderVM.SelectedCategory.CategoryName) ||
                    createFolderVM.SelectedLanguage == null || createFolderVM.SelectedLanguage == "All Items" || createFolderVM.SelectedLanguage != "All Items" && item.Language == createFolderVM.SelectedLanguage ||
                    createFolderVM.SelectedCodeLanguage == null || createFolderVM.SelectedCodeLanguage == "All Items" || createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == createFolderVM.SelectedCodeLanguage
                    )))
                {
                    createFolderVM.CurrentData.AllItems.Add(element);
                }
            }

            LoadItems(createFolderVM, folder, truePath);
        }
    }
    public static ObservableCollection<Element> ReturnAllItemsToCreateFolder(CreateFolderVM createFolderVM)
    {
        var list = createFolderVM.CurrentData.AllItems;
        List<Element> results;

        if (!(bool)createFolderVM.FilterType) //cu reuniune 
        {
            results = list.Where(item =>
                (createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0 || createFolderVM.SearchApplied == false) &&
                (createFolderVM.SelectedPriority == null || createFolderVM.SelectedPriority == "All Items" || createFolderVM.SelectedPriority != "All Items" && item.Priority == createFolderVM.SelectedPriority) &&
                (createFolderVM.SelectedCategory == null || createFolderVM.SelectedCategory.CategoryName == "All Items" || createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.SolidColorBrushColor.ToString() == createFolderVM.SelectedCategory.SolidColorBrushColor.ToString() && c.CategoryName == createFolderVM.SelectedCategory.CategoryName)) &&
                (createFolderVM.SelectedLanguage == null || createFolderVM.SelectedLanguage == "All Items" || createFolderVM.SelectedLanguage != "All Items" && item.Language == createFolderVM.SelectedLanguage) &&
                (createFolderVM.SelectedCodeLanguage == null || createFolderVM.SelectedCodeLanguage == "All Items" || createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == createFolderVM.SelectedCodeLanguage)
            ).ToList();
        }
        else
        {
            results = list.Where(item =>
                (createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0 || createFolderVM.SearchApplied == false) &&
                (createFolderVM.SelectedPriority == null || createFolderVM.SelectedPriority == "All Items" || createFolderVM.SelectedPriority != "All Items" && item.Priority == createFolderVM.SelectedPriority) ||
                createFolderVM.SelectedCategory == null || createFolderVM.SelectedCategory.CategoryName == "All Items" || createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.SolidColorBrushColor.ToString() == createFolderVM.SelectedCategory.SolidColorBrushColor.ToString() && c.CategoryName == createFolderVM.SelectedCategory.CategoryName) ||
                createFolderVM.SelectedLanguage == null || createFolderVM.SelectedLanguage == "All Items" || createFolderVM.SelectedLanguage != "All Items" && item.Language == createFolderVM.SelectedLanguage ||
                createFolderVM.SelectedCodeLanguage == null || createFolderVM.SelectedCodeLanguage == "All Items" || createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == createFolderVM.SelectedCodeLanguage
            ).ToList();
        }
        return new ObservableCollection<Element>(results);
    }
    public static ObservableCollection<Element> SortElementsByFolderDepth(ObservableCollection<Element> elements)
    {
        return new ObservableCollection<Element>(elements.OrderBy(e => e.Path.Count(f => f == Path.DirectorySeparatorChar)));
    }
    public static string FindLongestCommonPath(ObservableCollection<Element> elements)
    {
        if (!elements.Any())
        {
            return string.Empty;
        }

        var paths = elements.Select(e => e.Path).ToArray();
        var commonPath = paths[0];

        for (int i = 1; i < paths.Length; i++)
        {
            while (!paths[i].StartsWith(commonPath, StringComparison.OrdinalIgnoreCase))
            {
                commonPath = Path.GetDirectoryName(commonPath);
                if (string.IsNullOrEmpty(commonPath))
                {
                    // Handle the case when the common path becomes null or empty
                    return string.Empty;
                }
            }
        }

        return commonPath;
    }

    public static void CopyElementsToNewFolder(ObservableCollection<Element> elements, string newFolderPath, string commonPath, CreateFolderVM createFolderVM, string fictivePath)
    {
        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
        foreach (var element in elements)
        {
            var solDir = solutionDirectory;
            string elementPath = "";
            string elementName = "";
            //if element path dont exist
            string driveLetterPattern = @"^[A-Z]:\\"; // Matches drive letter followed by colon and backslash
            if (Directory.Exists(element.Path) && Regex.IsMatch(element.Path, driveLetterPattern))
            {
                elementPath = element.Path;
                elementName = element.Name + element.Extension;
            }
            else
            {
                if (element.Extension != "Folder")
                {
                    solDir = Path.Combine(solutionDirectory, element.Path);
                    if (element.Status == FileStatus.Undownloaded)
                    {
                        HelperDrive.DownloadFile(element.Id, solDir);
                        element.Status = FileStatus.Downloaded;
                    }
                }
                string extensionlocal = Path.GetExtension(element.Name);
                elementPath = solDir;

                elementName = element.Id + element.Extension;
            }


            // s-au stabilit elementPath si elementName pentru cazurile local/drive






            string shortPath = "";
            if (commonPath != "")
            {
                shortPath = element.Path.Replace(commonPath, string.Empty);
            }
            string newPath = newFolderPath + shortPath;
            string newFictifePath = fictivePath + shortPath;

            //s-a cautat calea comuna de unde sa inceapa copierea






            if (Directory.Exists(newPath))      //adaug la pathul bun
            {
                if (element.Extension == "Folder")
                {
                    Directory.CreateDirectory(newPath + "\\" + element.Name);
                    Element temp = new Element(newFictifePath, elementName, "Folder", new SolidColorBrush(Colors.DodgerBlue), "Folder");
                    temp.Status = FileStatus.ConversionResult;
                    createFolderVM.CurrentData.AllItems.Add(temp);
                    //element.Path = newPath;
                }
                else
                {
                    string newFileName = element.Name;
                    string ext = Path.GetExtension(element.Name);
                    var cleanName = "";
                    if (!string.IsNullOrEmpty(ext))
                    {
                        cleanName = Path.GetFileNameWithoutExtension(element.Name);
                    }
                    else
                    {
                        cleanName = element.Name;
                    }
                    string destinationPath = Path.Combine(newPath, newFileName);
                    if (elements.Count(elements => elements.Name == element.Name) > 1)
                    {
                        // If a file with the same name exists or a duplicate has been found before, append the parent folder's name to the new file's name
                        string parentFolderName = new DirectoryInfo(elementPath).Parent.Name;
                        newFileName = $"{element.Name} ({parentFolderName})";
                    }
                    File.Copy(Path.Combine(elementPath, elementName), Path.Combine(newPath, newFileName + element.Extension));
                    Element temp = new Element(newFictifePath, newFileName, element.Icon, element.Color, element.Extension);
                    temp.Status = FileStatus.ConversionResult;
                    createFolderVM.CurrentData.AllItems.Add(temp);
                    //element.Path = newPath;
                }
            }
            else                    //adaug in radacina
            {
                if (element.Extension == "Folder")
                {
                    Directory.CreateDirectory(newFolderPath + "\\" + elementName);
                    Element temp = new Element(newFictifePath, elementName, "Folder", new SolidColorBrush(Colors.DodgerBlue), "Folder");
                    temp.Status = FileStatus.ConversionResult;
                    createFolderVM.CurrentData.AllItems.Add(temp);
                    //element.Path = newFolderPath;
                }
                else
                {
                    string newFileName = element.Name;
                    string destinationPath = Path.Combine(newFolderPath, newFileName);
                    if (elements.Count(elements => elements.Name == element.Name) > 1)
                    {
                        // If a file with the same name exists or a duplicate has been found before, append the parent folder's name to the new file's name
                        string parentFolderName = new DirectoryInfo(elementPath).Parent.Name;
                        newFileName = $"{element.Name} ({parentFolderName})";
                    }

                    string extensionlocal = Path.GetExtension(elementName);

                    if (string.IsNullOrEmpty(extensionlocal))
                    {
                        elementName = element.Name + element.Extension;
                    }
                    else
                    {
                        elementName = element.Name;
                    }

                    try
                    {
                        File.Copy(Path.Combine(elementPath, elementName + element.Extension), Path.Combine(newFolderPath, newFileName + element.Extension));
                    }
                    catch
                    {
                        File.Copy(Path.Combine(elementPath, element.Id + element.Extension), Path.Combine(newFolderPath, newFileName + element.Extension));
                    }
                    //take the path after DriveDownloads//
                    //C:\Users\Kiwy\Desktop\FilesOrganizer\FilesOrganizer\DriveDownloads\Contul meu Drive\n -> Contul meu Drive\n
                    string marker = "DriveDownloads";
                    int index = newFolderPath.IndexOf(marker);
                    string p = newFolderPath.Substring(index + marker.Length + 1);

                    if(element.Id==null)
                    {
                        //get the last letters after last // from newFolderPath
                        int lastIndex = newFolderPath.LastIndexOf('\\');
                        string lastPart = newFolderPath.Substring(lastIndex + 1);

                        var elPath = element.Path;
                        int last = elPath.LastIndexOf('\\');
                        string newElPath = elPath.Substring(0, last);

                        p = newElPath + "\\"+lastPart;
                    }

                    Element temp = new Element(p, newFileName, element.Icon, element.Color, element.Extension);
                    temp.Status = FileStatus.ConversionResult;
                    createFolderVM.CurrentData.AllItems.Add(temp);
                    //element.Path = newFolderPath;
                }
            }
        }
    }





    public static void CreateNewFolder(CreateFolderVM createFolderVM, string folderName, string path)
    {
        string newFolderPath = "";
        string driveLetterPattern = @"^[A-Z]:\\"; // Matches drive letter followed by colon and backslash
        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
        if (Directory.Exists(path) && Regex.IsMatch(path, driveLetterPattern))
        {
            newFolderPath = Path.Combine(path, folderName);
        }
        else
        {
            newFolderPath = Path.Combine(solutionDirectory + "\\" + path, folderName);
        }

        string fictivePath = Path.Combine(path, folderName);
        Directory.CreateDirectory(newFolderPath);

        // Element temp = new Element(path, folderName, "Folder", new SolidColorBrush(Colors.DodgerBlue), "Folder");
        // temp.Status = FileStatus.ConversionResult;
        //// createFolderVM.CurrentData.AllItems.Add(temp);

        ObservableCollection<Element> elements = createFolderVM.CurrentData.AllItems;
        elements = SortElementsByFolderDepth(elements);

        string commonPath = FindLongestCommonPath(elements);
        CopyElementsToNewFolder(elements, newFolderPath, commonPath, createFolderVM, fictivePath);
    }
}