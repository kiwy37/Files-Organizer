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
using FilesOrganizer.Commands;

namespace FilesOrganizer.ViewModels.Commands;

public class Helper
{
    public static void OpenFile(string filePath)
    {
        try
        {
            if (System.IO.File.Exists(filePath))
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                };

                System.Diagnostics.Process.Start(startInfo);
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
                ((item.Path == viewerPageVM.CurrentPath) || item.Path.Contains(viewerPageVM.CurrentPath)) && ((viewerPageVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || viewerPageVM.SearchApplied == false) &&
                ((viewerPageVM.SelectedPriority == null || viewerPageVM.SelectedPriority == "All Items") || (viewerPageVM.SelectedPriority != "All Items" && item.Priority == viewerPageVM.SelectedPriority)) &&
                ((viewerPageVM.SelectedCategory == null || viewerPageVM.SelectedCategory.CategoryName == "All Items") || (viewerPageVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.Col.ToString() == viewerPageVM.SelectedCategory.Col.ToString() && c.CategoryName == viewerPageVM.SelectedCategory.CategoryName))) &&
                ((viewerPageVM.SelectedLanguage == null || viewerPageVM.SelectedLanguage == "All Items") || (viewerPageVM.SelectedLanguage != "All Items" && item.Language == viewerPageVM.SelectedLanguage)) &&
                ((viewerPageVM.SelectedCodeLanguage == null || viewerPageVM.SelectedCodeLanguage == "All Items") || (viewerPageVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == viewerPageVM.SelectedCodeLanguage))
            ).ToList();
        }
        else
        {
            results = list.Where(item =>
                ((item.Path == viewerPageVM.CurrentPath) || item.Path.Contains(viewerPageVM.CurrentPath)) && ((viewerPageVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || viewerPageVM.SearchApplied == false) &&
                (((viewerPageVM.SelectedPriority == null || viewerPageVM.SelectedPriority == "All Items") || (viewerPageVM.SelectedPriority != "All Items" && item.Priority == viewerPageVM.SelectedPriority)) ||
                ((viewerPageVM.SelectedCategory == null || viewerPageVM.SelectedCategory.CategoryName == "All Items") || (viewerPageVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.Col.ToString() == viewerPageVM.SelectedCategory.Col.ToString() && c.CategoryName == viewerPageVM.SelectedCategory.CategoryName))) ||
                ((viewerPageVM.SelectedLanguage == null || viewerPageVM.SelectedLanguage == "All Items") || (viewerPageVM.SelectedLanguage != "All Items" && item.Language == viewerPageVM.SelectedLanguage)) ||
                ((viewerPageVM.SelectedCodeLanguage == null || viewerPageVM.SelectedCodeLanguage == "All Items") || (viewerPageVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == viewerPageVM.SelectedCodeLanguage)))
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
                bdoi = viewerPageVM.SelectedCategory.Col.ToString();
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
                (item.Path.Contains(actualPath)) && ((viewerPageVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || viewerPageVM.SearchApplied == false) &&
                ((viewerPageVM.SelectedPriority == null || viewerPageVM.SelectedPriority == "All Items") || (viewerPageVM.SelectedPriority != "All Items" && item.Priority == viewerPageVM.SelectedPriority)) &&
                ((viewerPageVM.SelectedCategory == null || viewerPageVM.SelectedCategory.CategoryName == "All Items") || (viewerPageVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.Col.ToString() == viewerPageVM.SelectedCategory.Col.ToString() && c.CategoryName == viewerPageVM.SelectedCategory.CategoryName))) &&
                ((viewerPageVM.SelectedLanguage == null || viewerPageVM.SelectedLanguage == "All Items") || (viewerPageVM.SelectedLanguage != "All Items" && item.Language == viewerPageVM.SelectedLanguage)) &&
                ((viewerPageVM.SelectedCodeLanguage == null || viewerPageVM.SelectedCodeLanguage == "All Items") || (viewerPageVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == viewerPageVM.SelectedCodeLanguage))
            ).ToList();
        }
        else
        {
            results = list.Where(item =>
                (item.Path.Contains(actualPath)) && ((viewerPageVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || viewerPageVM.SearchApplied == false) &&
                (((viewerPageVM.SelectedPriority == null || viewerPageVM.SelectedPriority == "All Items") || (viewerPageVM.SelectedPriority != "All Items" && item.Priority == viewerPageVM.SelectedPriority)) ||
                ((viewerPageVM.SelectedCategory == null || viewerPageVM.SelectedCategory.CategoryName == "All Items") || (viewerPageVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.Col.ToString() == viewerPageVM.SelectedCategory.Col.ToString() && c.CategoryName == viewerPageVM.SelectedCategory.CategoryName))) ||
                ((viewerPageVM.SelectedLanguage == null || viewerPageVM.SelectedLanguage == "All Items") || (viewerPageVM.SelectedLanguage != "All Items" && item.Language == viewerPageVM.SelectedLanguage)) ||
                ((viewerPageVM.SelectedCodeLanguage == null || viewerPageVM.SelectedCodeLanguage == "All Items") || (viewerPageVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == viewerPageVM.SelectedCodeLanguage)))
            ).ToList();
        }
        viewerPageVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(results);
    }
    public static string DetectLanguage(string filePath, string otherArgument)
    {
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
        return Regex.Matches(code, @"assert|while|\bor\b|def|cos|raise|data|for|try|from|del|class|and|\bin\b|type|pass|array|break|not|close|write|continue|int|acos|import|if|print|exp|floor|elif|except|lambda|oxphys|return|finally|range|exec|global|numeric|fabs|float|atan|asin|open|zeros|sqrt|else").Count;
    }
    static int CountJavaKeywords(string code)
    {
        return Regex.Matches(code, @"import|public class|public static void").Count;
    }
    static int CountHtmlKeywords(string code)
    {
        return Regex.Matches(code, @"<html|<head|<title|<body|<h1|<h2|<h3|<h4|<h5|<h6|<p|<br|<hr|<div|<span|<ul|<ol|<li|<dl|<dt|<dd|<a|href|<img|src|<table|<tr|<td|<th|<form|<input|<button|<select|<option|<textarea|<label|<fieldset|<legend|<script|<style|<link|<meta|<header|<footer|<nav|<section|<article|<aside|<figure|<figcaption|<main|<datalist|<output|<progress|<meter|<details|<summary|<command|<canvas|<audio|<video|<source|<track|<embed|<param|<object|<area|<map|<base|<bdo|<bdi|<ruby|<rt|<rp|<data|<time|<mark|<wbr|<ins|<del|<cite|<dfn|<abbr|<address|<em|<strong|<small|<s|<cite|<q|<dfn|<abbr|<time|<code|<var|<samp|<kbd|<sub|<sup|<i|<b|<u|<strike|<big|<font|<basefont|<br|<wbr|<nobr|<tt|<blink|<marquee|/>").Count;
    }
    static int CountCssKeywords(string code)
    {
        return Regex.Matches(code, @"color|background-color|important|border|margin|padding|display|font-size|font-family|width|height|cursor|position|top|bottom|left|right|align-items|justify-content|flex-direction|flex-wrap|flex|grid-template-columns|grid-template-rows|grid-column|grid-row|grid-area|gap|justify-items|align-content|place-items|place-content|auto|normal|stretch|center|start|end|flex-start|flex-end|self-start|self-end|space-between|space-around|space-evenly|safe|unsafe|baseline|first baseline|last baseline|space-between|space-around|space-evenly|row|row-reverse|column|column-reverse|nowrap|wrap|wrap-reverse|flow|flow-root|table|inline-table|table-row-group|table-header-group|table-footer-group|table-row|table-cell|table-column-group|table-column|table-caption|ruby-base|ruby-text|ruby-base-container|ruby-text-container|inline-block|inline-list-item|inline-flex|inline-grid|run-in|contents|none|absolute|relative|sticky|fixed").Count;
    }
    static int CountCSharpKeywords(string code)
    {
        return Regex.Matches(code, @"System|HttpGet|HttpPost|Task|Math|]|Min|Sqrt|ToString|Console|WriteLine|Write|interface|internal|unchecked|using|throw|operator|from|sbyte|short|set|const|static|continue|orderby|\bas\b|namespace|ascending|let|goto|protected|finally|params|enum|false|unsafe|virtual|\bout|value|\bis\b|add|public|break|char|double|int|delegate|sealed|override|ref|new|do|void|fixed|sizeof|dynamic|byte|case|private|get|while|descending|\bfor|true|try|select|bool|remove|switch|abstract|event|global|foreach|float|var|uint|implicit|ulong|explicit|decimal|volatile|object|null|lock|join|yield|where|catch|async|checked|class|long|typeof|struct|stackalloc|extern|group|if|partial|return|into|alias|readonly|else|default|base|ushort|await").Count;
    }
    static int CountCppKeywords(string code)
    {
        return Regex.Matches(code, @"std|vector|main|<endl|iostream|string|include|auto|double|int|struct|break|else|long|switch|case|enum|typedef|char|extern|return|union|const|float|short|unsigned|continue|for|signed|void|default|goto|sizeof|size_t|volatile|do|if|static|while|new|delete|try|catch|bool|false|true|namespace|using|class|public|protected|private|friend|virtual|inline|dynamic_cast|static_cast|reinterpret_cast|const_cast|template|mutable|operator|typeid|typename|explicit|this|nullptr|asm|bitand|bitor|not|not_eq|or_eq|xor|xor_eq|and_eq|and|alignas|alignof|constexpr|decltype|noexcept|nullptr|static_assert|thread_local|cout|cin|>>|<<").Count;
    }
    static int CountCKeywords(string code)
    {
        return Regex.Matches(code, @"char|printf|fgets|stdin|strlen|while|scanf|#include|<stdio|for|int|return|%d|%c|%p|do|if|main()|^|sizeof").Count;
    }
    static int CountTypeScriptKeywords(string code)
    {
        return Regex.Matches(code, @"console|log|alert|window|subscribe|event|break|===|\bas\b|any|case|catch|class|const|continue|debugger|default|delete|do|else|enum|export|extends|false|finally|for|from|function|if|implements|import|\bin\b|instanceof|interface|let|new|null|package|private|protected|public|return|super|switch|this|throw|true|try|typeof|var|void|while|with|yield").Count;
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
    static string TextFromPic(string imagePath)
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
    static string AnalyzeCodeFromString(string sentence, Element elem)
    {
        //string sentence = TextFromPic(imagePath);

        if(elem.Name.Contains("floare1"))
        {

        }

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
        int typeScriptCount = CountTypeScriptKeywords(sentence);
        int cCount = CountCKeywords(sentence);
        int totalCount = csharpCount + pythonCount + javaCount + htmlCount + cssCount + cppCount + typeScriptCount +cCount;

        if (elem.Name.Contains("C9"))
            MostFrequentCppKeyword(sentence);

        if (totalCount == 0)
        {
            return "None";
        }

        double csharpProb = (double)csharpCount / totalCount;
        double pythonProb = (double)pythonCount / totalCount;
        double javaProb = (double)javaCount / totalCount;
        double htmlProb = (double)htmlCount / totalCount;
        double cssProb = (double)cssCount / totalCount;
        double cppProb = (double)cppCount / totalCount;
        double typeScriptProb = (double)typeScriptCount / totalCount;
        double cProb = (double)cCount / totalCount; 

        if (csharpProb > pythonProb && csharpProb > javaProb && csharpProb > htmlProb && csharpProb > cssProb && csharpProb > cppProb && csharpProb > typeScriptProb && csharpProb > cProb)
        {
            return "C#";
        }
        else if (pythonProb > csharpProb && pythonProb > javaProb && pythonProb > htmlProb && pythonProb > cssProb && pythonProb > cppProb && pythonProb > typeScriptProb && pythonProb > cProb)
        {
            return "Python";
        }
        else if (javaProb > csharpProb && javaProb > pythonProb && javaProb > htmlProb && javaProb > cssProb && javaProb > cppProb && javaProb > typeScriptProb && javaProb > cProb)
        {
            return "Java";
        }
        else if (htmlProb > csharpProb && htmlProb > pythonProb && htmlProb > javaProb && htmlProb > cssProb && htmlProb > cppProb && htmlProb > typeScriptProb && htmlProb > cProb)
        {
            return "HTML";
        }
        else if (cssProb > csharpProb && cssProb > pythonProb && cssProb > javaProb && cssProb > htmlProb && cssProb > cppProb && cssProb > typeScriptProb && cssProb > cProb)
        {
            return "CSS";
        }
        else if (cppProb > csharpProb && cppProb > pythonProb && cppProb > javaProb && cppProb > htmlProb && cppProb > cssProb && cppProb > typeScriptProb && cppProb > cProb)
        {
            return "C++";
        }
        else if (typeScriptProb > csharpProb && typeScriptProb > pythonProb && typeScriptProb > javaProb && typeScriptProb > htmlProb && typeScriptProb > cssProb && typeScriptProb > cppProb && typeScriptProb > cProb)
        {
            return "TypeScript";
        }
        else if (cProb > csharpProb && cProb > pythonProb && cProb > javaProb && cProb > htmlProb && cProb > cssProb && cProb > cppProb && cProb > typeScriptProb)
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
                item.CodeLanguage = AnalyzeCodeFromString(text,item);
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
            case ".css":
                code = "CSS";
                break;
            case ".cpp":
                code = "C++";
                break;
            case ".ts":
                code = "TypeScript";
                break;
            default:
                code = "None";
                break;
        }

        return code;
    }
    public static string LanguageFromVideo(Element item)
    {
        var inputFile = new MediaFile { Filename = item.Path + "\\\\" + item.Name };
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
                languages.Add(DetectLanguage(text, "t"));
                File.Delete(outputFile.Filename);
            }
        }

        var mostFrequentLanguage = languages
            .GroupBy(i => i)
            .OrderByDescending(grp => grp.Count())
            .Select(grp => grp.Key).First();
        mostFrequentLanguage = LanguageAssociation(mostFrequentLanguage);
        return mostFrequentLanguage;
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
            string extension = Path.GetExtension(item.Name);
            string text, language;
            switch (extension)
            {
                case var ext when plainTextFileExtensions.Contains(ext):
                    if ((bool)settings.TextFilesLanguage || (bool)settings.TextFilesCode)
                    {
                        text = GetFileContent(item);
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
                        text = TextFromPic(item.Path + "\\\\" + item.Name);
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
                        language = DetectLanguage(item.Path + "\\\\" + item.Name, "a");
                        item.Language = GetLastWord(language);
                    }
                    if ((bool)settings.VideosCode)
                    {
                        //cod
                        item.CodeLanguage = LanguageFromVideo(item);
                    }
                    break;

                case var ext when audioExtensions.Contains(ext):
                    if ((bool)settings.VideosLanguage)
                    {
                        //limba
                        language = DetectLanguage(item.Path + "\\\\" + item.Name, "a");
                        language = language.Trim();
                        item.Language = language;
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
        if(association == "")
        {
            association = "None";
        }
        return association;
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
    public static string GetFileContent(Element file)
    {
        string fileContent = "";
        string extension = Path.GetExtension(file.Name).ToLower();

        switch (extension)
        {
            case ".rtf":
                using (var rtBox = new System.Windows.Forms.RichTextBox())
                {
                    rtBox.LoadFile(file.Path + "\\" + file.Name);
                    fileContent = rtBox.Text;
                }
                break;
            case ".txt":
            case ".json":
                fileContent = File.ReadAllText(file.Path + "\\" + file.Name);
                break;
            case ".odt":
                {
                    TextDocument document = new TextDocument();
                    document.Load(file.Path + "\\" + file.Name);

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
                using (var document = DocX.Load(file.Path + "\\" + file.Name))
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
                        using (var stream = new FileStream(file.Path + "\\" + file.Name, FileMode.Open, FileAccess.Read))
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
                        using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(file.Path + "\\" + file.Name)))
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
                using (var reader = new StreamReader(file.Path + "\\" + file.Name))
                {
                    fileContent = reader.ReadToEnd();
                }
                break;
            case ".pdf":
                fileContent = string.Empty;
                using (var reader = new iTextSharp.text.pdf.PdfReader(file.Path + "\\" + file.Name))
                {
                    for (int page = 1; page <= reader.NumberOfPages; page++)
                    {
                        fileContent += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page);
                    }
                }
                break;
            case ".xml":
                var doc = XDocument.Load(file.Path + "\\" + file.Name);
                fileContent = doc.ToString();
                break;
            case ".ppt":
            case ".pptx":
                using (PresentationDocument presentationDocument = PresentationDocument.Open(file.Path + "\\" + file.Name, false))
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

        fileContent = fileContent.Substring(0, Math.Min(fileContent.Length, 8000));
        return fileContent;
    }
    public static int FileMatchesCriteria(bool driveOrLocal, Element file, string keyword, bool byContent, bool byName)
    {
        string fileContent="";
        string extension = Path.GetExtension(file.Name);

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
            if (extension == ".txt")
            {
                if (driveOrLocal)
                {
                    fileContent=HelperDrive.GetTxtFileContent(file.Id);
                }
                else
                {
                    fileContent = File.ReadAllText(file.Path + "\\\\" + file.Name);
                }
            }
            else if (extension == ".odt")
            {
                if (driveOrLocal)
                {
                }
                else
                {
                    TextDocument document = new TextDocument();
                    document.Load(file.Path + "\\\\" + file.Name);

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
            }
            else if (extension == ".doc" || extension == ".docx")
            {
                if (driveOrLocal)
                {
                }
                else
                {
                    using (var document = DocX.Load(file.Path + "\\\\" + file.Name))
                    {
                        fileContent = document.Text;
                    }
                }
            }
            else if (extension == ".xls" || extension == ".xlsx")
            {

                var sb = new StringBuilder();
                if (driveOrLocal)
                {
                    if (extension == ".xls")
                    {
                    }
                    else if (extension == ".xlsx")
                    {
                    }
                }
                else
                {
                    if (extension == ".xls")
                    {
                        using (var stream = new FileStream(file.Path + "\\\\" + file.Name, FileMode.Open, FileAccess.Read))
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
                        using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(file.Path + "\\\\" + file.Name)))
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
            }
            else if (extension == ".html" || extension == ".htm")
            {
                if (driveOrLocal)
                {
                }
                else
                {
                    using (var reader = new StreamReader(file.Path + "\\\\" + file.Name))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }
            else if (extension == ".pdf")
            {
                if (driveOrLocal)
                {
                }
                else
                {
                    fileContent = string.Empty;
                    using (var reader = new iTextSharp.text.pdf.PdfReader(file.Path + "\\\\" + file.Name))
                    {
                        for (int page = 1; page <= reader.NumberOfPages; page++)
                        {
                            fileContent += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page);
                        }
                    }
                }
            }
            else if (extension == ".json")
            {
                if (driveOrLocal)
                {
                }
                else
                {
                    fileContent = File.ReadAllText(file.Path + "\\\\" + file.Name);
                }
            }
            else if (extension == ".xml")
            {
                if (driveOrLocal)
                {
                }
                else
                {
                    var doc = XDocument.Load(file.Path + "\\\\" + file.Name);
                    fileContent = doc.ToString();
                }
            }
            else if (extension == ".ppt" || extension == ".pptx")
            {
                if (driveOrLocal)
                {
                }
                else
                {
                    using (PresentationDocument presentationDocument = PresentationDocument.Open(file.Path + "\\\\" + file.Name, false))
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
            }
            else if (extension == ".rtf")
            {
                if (driveOrLocal)
                {
                }
                else
                {
                    using (var rtBox = new System.Windows.Forms.RichTextBox())
                    {
                        rtBox.LoadFile(file.Path + "\\\\" + file.Name);
                        fileContent = rtBox.Text;
                    }
                }
            }
            else
            {
                fileContent = "";
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
        int similarityThreshold=0;
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
                            if(i!=8)
                                bools.Add(bool.Parse(line));
                            else
                                similarityThreshold = int.Parse(line);
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
            return new Models.Settings(bools, similarityThreshold);
        }
    }
    public static void RecheckCode(SettingsVM settingsVM)
    {
        //fisiere din care pot lua text
        var plainTextFileExtensions = new List<string> { ".rtf", ".txt", ".json", ".odt", ".doc", ".docx", ".xls", ".xlsx", ".html", ".htm", ".pdf", ".xml", ".ppt", ".pptx" };

        //fisiere pe care pot aplica tesseractul
        var imageExtensions = new List<string> { ".avif", ".gif", ".heif", ".jpeg", ".jpg", ".png", ".raw", ".svg", ".tiff" };

        //python script extensions
        var videoExtensions = new List<string> { ".mp4", ".avi", ".mov", ".mkv", ".flv", ".wmv", ".webm", ".mpeg", ".mpg", ".3gp" };

        for (int i = 0; i < settingsVM.Datas.AllItems.Count; i++)
        {
            Element item = settingsVM.Datas.AllItems[i];
            string extension = Path.GetExtension(item.Name);
            string text, language;
            switch (extension)
            {
                case var ext when plainTextFileExtensions.Contains(ext):
                    if ((bool)settingsVM.SettingsDatas.TextFilesCode)
                    {
                        text = GetFileContent(item);
                        item.CodeLanguage = AnalyzeCodeFromString(text, item);
                    }
                    break;

                case var ext when imageExtensions.Contains(ext):
                    if ((bool)settingsVM.SettingsDatas.ImagesCode)
                    {
                        text = TextFromPic(item.Path + "\\\\" + item.Name);
                        item.CodeLanguage = AnalyzeCodeFromString(text, item);
                    }
                    break;

                case var ext when videoExtensions.Contains(ext):
                    if ((bool)settingsVM.SettingsDatas.VideosCode)
                    {
                        //cod
                        item.CodeLanguage = LanguageFromVideo(item);
                    }
                    break;

                default:
                    item.CodeLanguage = CodeFromExtension(extension);
                    break;
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
            string extension = Path.GetExtension(item.Name);
            string text, language;
            switch (extension)
            {
                case var ext when plainTextFileExtensions.Contains(ext):
                    if ((bool)settingsVM.SettingsDatas.TextFilesLanguage)
                    {
                        text = GetFileContent(item);
                        language = DetectLanguage(text, "t");
                        item.Language = LanguageAssociation(language);
                    }
                    break;

                case var ext when imageExtensions.Contains(ext):
                    if ((bool)settingsVM.SettingsDatas.ImagesLanguage)
                    {
                        text = TextFromPic(item.Path + "\\\\" + item.Name);
                        language = DetectLanguage(text, "t");
                        item.Language = LanguageAssociation(language);
                    }
                    break;

                case var ext when videoExtensions.Contains(ext):
                    if ((bool)settingsVM.SettingsDatas.VideosLanguage)
                    {
                        //limba
                        language = DetectLanguage(item.Path + "\\\\" + item.Name, "a");
                        item.Language = GetLastWord(language);
                        item.Language = LanguageAssociation(item.Language);
                    }
                    break;

                case var ext when audioExtensions.Contains(ext):
                    if ((bool)settingsVM.SettingsDatas.VideosLanguage)
                    {
                        //limba
                        language = DetectLanguage(item.Path + "\\\\" + item.Name, "a");
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
            LoadOneElement(createFolderVM, selectedFileName);
        }
    }
    public static void AddFolderToList(CreateFolderVM createFolderVM)
    {
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
            string selectedFolderName = folderBrowserDialog.SelectedPath;
            var path = selectedFolderName;
            string fileName = System.IO.Path.GetFileName(path);

            //if (createFolderVM.BackStack.ElementAt(createFolderVM.PozInList).StartsWith("Filter results"))      //daca ce vad e filtrare
            //{
            //    path = createFolderVM.InitPath;
            //    path = System.IO.Path.GetDirectoryName(path);
            //}
            //else
            //{
            //    path = createFolderVM.BackStack.ElementAt(createFolderVM.PozInList);
            //}

            if (createFolderVM.CurrentData.CurrentListBoxSource.Any(elem => String.Equals(elem.Path, System.IO.Path.GetDirectoryName(selectedFolderName))))
            {
                System.Windows.Forms.MessageBox.Show("This folder is already in the list.");
                return;
            }

            Element element = new Element(path, fileName, "Folder", new SolidColorBrush(Colors.DodgerBlue), "Folder");

            element.Priority = createFolderVM.SelectedPriority;

            element.Category.Clear();
            element.Category.Add(createFolderVM.SelectedCategory);

            element.Language = createFolderVM.SelectedLanguage;
            element.CodeLanguage = createFolderVM.SelectedCodeLanguage;

            createFolderVM.CurrentData.CurrentListBoxSource.Add(element);
            createFolderVM.CurrentData.AllItems.Add(element);
            
            LoadItems(createFolderVM, path, path);
        }
    }
    private static void LoadOneElement(CreateFolderVM createFolderVM, string path)
    {
        string fileName = System.IO.Path.GetFileName(path);
        string extension = System.IO.Path.GetExtension(path);
        path = System.IO.Path.GetDirectoryName(path);
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
        createFolderVM.CurrentData.CurrentListBoxSource.Add(element);
        createFolderVM.CurrentData.AllItems.Add(element);
    }
    private static void LoadItems(CreateFolderVM createFolderVM, string path, string truePath)
    {
        string[] files = System.IO.Directory.GetFiles(truePath);
        string[] folders = System.IO.Directory.GetDirectories(truePath);

        foreach (var file in files)
        {
            string fileName = System.IO.Path.GetFileName(file);
            string extension = System.IO.Path.GetExtension(file);
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
                    (((createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || createFolderVM.SearchApplied == false) &&
                    ((createFolderVM.SelectedPriority == null || createFolderVM.SelectedPriority == "All Items") || (createFolderVM.SelectedPriority != "All Items" && item.Priority == createFolderVM.SelectedPriority)) &&
                    ((createFolderVM.SelectedCategory == null || createFolderVM.SelectedCategory.CategoryName == "All Items") || (createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.Col.ToString() == createFolderVM.SelectedCategory.Col.ToString() && c.CategoryName == createFolderVM.SelectedCategory.CategoryName))) &&
                    ((createFolderVM.SelectedLanguage == null || createFolderVM.SelectedLanguage == "All Items") || (createFolderVM.SelectedLanguage != "All Items" && item.Language == createFolderVM.SelectedLanguage)) &&
                    ((createFolderVM.SelectedCodeLanguage == null || createFolderVM.SelectedCodeLanguage == "All Items") || (createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == createFolderVM.SelectedCodeLanguage))
                ))) //cu reuniune 
                {
                    createFolderVM.CurrentData.AllItems.Add(element);
                }
            }
            else
            {
                if (!createFolderVM.CurrentData.AllItems.Any(item => item.Path == element.Path && item.Name == element.Name &&
                    (((createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || createFolderVM.SearchApplied == false) &&
                    ((createFolderVM.SelectedPriority == null || createFolderVM.SelectedPriority == "All Items") || (createFolderVM.SelectedPriority != "All Items" && item.Priority == createFolderVM.SelectedPriority)) ||
                    ((createFolderVM.SelectedCategory == null || createFolderVM.SelectedCategory.CategoryName == "All Items") || (createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.Col.ToString() == createFolderVM.SelectedCategory.Col.ToString() && c.CategoryName == createFolderVM.SelectedCategory.CategoryName))) ||
                    ((createFolderVM.SelectedLanguage == null || createFolderVM.SelectedLanguage == "All Items") || (createFolderVM.SelectedLanguage != "All Items" && item.Language == createFolderVM.SelectedLanguage)) ||
                    ((createFolderVM.SelectedCodeLanguage == null || createFolderVM.SelectedCodeLanguage == "All Items") || (createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == createFolderVM.SelectedCodeLanguage))
                    )))
                {
                    createFolderVM.CurrentData.AllItems.Add(element);
                }
            }
        }

        foreach (var folder in folders)
        {
            string folderName = System.IO.Path.GetFileName(folder);
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
                    (((createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || createFolderVM.SearchApplied == false) &&
                    ((createFolderVM.SelectedPriority == null || createFolderVM.SelectedPriority == "All Items") || (createFolderVM.SelectedPriority != "All Items" && item.Priority == createFolderVM.SelectedPriority)) &&
                    ((createFolderVM.SelectedCategory == null || createFolderVM.SelectedCategory.CategoryName == "All Items") || (createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.Col.ToString() == createFolderVM.SelectedCategory.Col.ToString() && c.CategoryName == createFolderVM.SelectedCategory.CategoryName))) &&
                    ((createFolderVM.SelectedLanguage == null || createFolderVM.SelectedLanguage == "All Items") || (createFolderVM.SelectedLanguage != "All Items" && item.Language == createFolderVM.SelectedLanguage)) &&
                    ((createFolderVM.SelectedCodeLanguage == null || createFolderVM.SelectedCodeLanguage == "All Items") || (createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == createFolderVM.SelectedCodeLanguage))
                ))) //cu reuniune 
                {
                    createFolderVM.CurrentData.AllItems.Add(element);
                }
            }
            else
            {
                if (!createFolderVM.CurrentData.AllItems.Any(item => item.Path == element.Path && item.Name == element.Name &&
                    (((createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || createFolderVM.SearchApplied == false) &&
                    ((createFolderVM.SelectedPriority == null || createFolderVM.SelectedPriority == "All Items") || (createFolderVM.SelectedPriority != "All Items" && item.Priority == createFolderVM.SelectedPriority)) ||
                    ((createFolderVM.SelectedCategory == null || createFolderVM.SelectedCategory.CategoryName == "All Items") || (createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.Col.ToString() == createFolderVM.SelectedCategory.Col.ToString() && c.CategoryName == createFolderVM.SelectedCategory.CategoryName))) ||
                    ((createFolderVM.SelectedLanguage == null || createFolderVM.SelectedLanguage == "All Items") || (createFolderVM.SelectedLanguage != "All Items" && item.Language == createFolderVM.SelectedLanguage)) ||
                    ((createFolderVM.SelectedCodeLanguage == null || createFolderVM.SelectedCodeLanguage == "All Items") || (createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == createFolderVM.SelectedCodeLanguage))
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
                (((createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || createFolderVM.SearchApplied == false) &&
                ((createFolderVM.SelectedPriority == null || createFolderVM.SelectedPriority == "All Items") || (createFolderVM.SelectedPriority != "All Items" && item.Priority == createFolderVM.SelectedPriority)) &&
                ((createFolderVM.SelectedCategory == null || createFolderVM.SelectedCategory.CategoryName == "All Items") || (createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.Col.ToString() == createFolderVM.SelectedCategory.Col.ToString() && c.CategoryName == createFolderVM.SelectedCategory.CategoryName))) &&
                ((createFolderVM.SelectedLanguage == null || createFolderVM.SelectedLanguage == "All Items") || (createFolderVM.SelectedLanguage != "All Items" && item.Language == createFolderVM.SelectedLanguage)) &&
                ((createFolderVM.SelectedCodeLanguage == null || createFolderVM.SelectedCodeLanguage == "All Items") || (createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == createFolderVM.SelectedCodeLanguage))
            )).ToList();
        }
        else
        {
            results = list.Where(item =>
                (((createFolderVM.SearchApplied == true && int.Parse(item.Appearance.Split(' ')[0]) > 0) || createFolderVM.SearchApplied == false) &&
                ((createFolderVM.SelectedPriority == null || createFolderVM.SelectedPriority == "All Items") || (createFolderVM.SelectedPriority != "All Items" && item.Priority == createFolderVM.SelectedPriority)) ||
                ((createFolderVM.SelectedCategory == null || createFolderVM.SelectedCategory.CategoryName == "All Items") || (createFolderVM.SelectedCategory.CategoryName != "All Items" && item.Category.Any(c => c.Col.ToString() == createFolderVM.SelectedCategory.Col.ToString() && c.CategoryName == createFolderVM.SelectedCategory.CategoryName))) ||
                ((createFolderVM.SelectedLanguage == null || createFolderVM.SelectedLanguage == "All Items") || (createFolderVM.SelectedLanguage != "All Items" && item.Language == createFolderVM.SelectedLanguage)) ||
                ((createFolderVM.SelectedCodeLanguage == null || createFolderVM.SelectedCodeLanguage == "All Items") || (createFolderVM.SelectedCodeLanguage != "All Items" && item.CodeLanguage == createFolderVM.SelectedCodeLanguage))
            )).ToList();
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
            }
        }

        return commonPath;
    }
    public static void CopyElementsToNewFolder(ObservableCollection<Element> elements, string newFolderPath, string commonPath)
    {
        foreach (var element in elements)
        {
            string shortPath = element.Path.Replace(commonPath, string.Empty);
            string newPath = newFolderPath + shortPath;

            if (element.Name.Contains("c_doi"))
            {
            }

            if (Directory.Exists(newPath))      //adaug la pathul bun
            {
                if (element.Extension == "Folder")
                {
                    Directory.CreateDirectory(newPath + "\\" + element.Name);
                }
                else
                {
                    File.Copy(Path.Combine(element.Path, element.Name), Path.Combine(newPath, element.Name));
                }
            }
            else                    //adaug in radacina
            {
                if (element.Extension == "Folder")
                {
                    Directory.CreateDirectory(newFolderPath+"\\"+element.Name);
                }
                else
                {
                    File.Copy(Path.Combine(element.Path, element.Name), Path.Combine(newFolderPath, element.Name));
                }
            }
        }
    }
    public static void CreateNewFolder(CreateFolderVM createFolderVM, string folderName, string path)
    {
        //nu pune C2
        string newFolderPath = Path.Combine(path, folderName);
        Directory.CreateDirectory(newFolderPath);

        ObservableCollection<Element> elements = ReturnAllItemsToCreateFolder(createFolderVM);
        elements = SortElementsByFolderDepth(elements);

        string commonPath = FindLongestCommonPath(elements);
        CopyElementsToNewFolder(elements, newFolderPath, commonPath);
    }
}