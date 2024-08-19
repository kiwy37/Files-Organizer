using FilesOrganizer.Core;
using FilesOrganizer.Models;
using FilesOrganizer.ViewModels.Commands;
using NPOI.POIFS.FileSystem;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FilesOrganizer.Helpers;

public class ConvertItemsHelper
{
    Converter converter = new Converter();
    public ConvertItemsHelper()
    {
    }

    private void CheckIfIsSearch(ViewerPageVM viewerPageVM)
    {
        viewerPageVM.SearchApplied = true;
        var list = new ObservableCollection<Element>();

        list = viewerPageVM.CurrentData.AllItems;

        viewerPageVM.CurrentData.CurrentListBoxSource = new ObservableCollection<Element>(
            list
                .Select(path =>
                {
                    int appearances = ViewerPageHelper.FileMatchesCriteria(viewerPageVM.CurrentData.DriveOrLocal, path, viewerPageVM.SearchingWord, viewerPageVM.IsByContentChecked, viewerPageVM.IsByNameChecked);
                    path.Appearance = appearances > 1 ? $"{appearances} appearances" : $"{appearances} appearance";
                    return path;
                })
                .Where(path => int.Parse(path.Appearance.Split(' ')[0]) > 0)
                .OrderByDescending(path => int.Parse(path.Appearance.Split(' ')[0]))
                .ToList()
        );
    }

    public void ConvertItems(List<string> selectedItems, string extension, ViewerPageVM viewerPageVM, string initPath, string endPath, string driveInitPath, string driveEndPath)
    {
        foreach (var selectedItem in selectedItems)
        {
            switch (extension)
            {
                case "DOC":
                    {
                        if (selectedItem.ToUpper() == "DOCX")
                        {
                            var conv = viewerPageVM.ConversionName + ".docx";
                            converter.ConvertDocToDocx(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".docx");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".docx");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "PDF")
                        {
                            var conv = viewerPageVM.ConversionName + ".pdf";
                            converter.ConvertDocToPdf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "TXT")
                        {
                            var conv = viewerPageVM.ConversionName + ".txt";
                            converter.ConvertDocToTxt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), ".txt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), ".txt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "ODT")
                        {
                            var conv = viewerPageVM.ConversionName + ".odt";
                            converter.ConvertDocToOdt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".odt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".odt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "RTF")
                        {
                            var conv = viewerPageVM.ConversionName + ".rtf";
                            converter.ConvertDocToRtf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".rtf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".rtf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        break;
                    }
                case "DOCX":
                    {
                        if (selectedItem.ToUpper() == "DOC")
                        {
                            var conv = viewerPageVM.ConversionName + ".doc";
                            converter.ConvertDocxToDoc(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".doc");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".doc");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "PDF")
                        {
                            var conv = viewerPageVM.ConversionName + ".pdf";
                            converter.ConvertDocxToPdf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "TXT")
                        {
                            var conv = viewerPageVM.ConversionName + ".txt";
                            converter.ConvertDocxToTxt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), ".txt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), ".txt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "ODT")
                        {
                            var conv = viewerPageVM.ConversionName + ".odt";
                            converter.ConvertDocxToOdt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".odt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".odt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "RTF")
                        {
                            var conv = viewerPageVM.ConversionName + ".rtf";
                            converter.ConvertDocxToRtf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".rtf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".rtf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        break;
                    }
                case "PDF":
                    {
                        if (selectedItem.ToUpper() == "DOCX")
                        {
                            var conv = viewerPageVM.ConversionName + ".docx";
                            converter.ConvertPdfToDocx(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, conv, "File", new SolidColorBrush(Colors.Gainsboro), ".docx");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, conv, "File", new SolidColorBrush(Colors.Gainsboro), ".docx");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        //if (selectedItem.ToUpper() == "TXT")
                        //{
                        //    var conv = viewerPageVM.ConversionName + ".txt";
                        //    var initPath = viewerPageVM.SelectedItem.Path + "\\"+viewerPageVM.SelectedItem.Name;
                        //    var endPath = viewerPageVM.SavingConversionPath+"\\"+viewerPageVM.SelectedItem.Name;
                        //    converter.ConvertPdfToTxt(initPath, endPath, conv);
                        //}
                        break;
                    }
                case "TXT":
                    {
                        if (selectedItem.ToUpper() == "PDF")
                        {
                            var conv = viewerPageVM.ConversionName + ".pdf";
                            converter.ConvertTxtToPdf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "DOCX")
                        {
                            var conv = viewerPageVM.ConversionName + ".docx";
                            converter.ConvertTxtToDocx(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "ODT")
                        {
                            var conv = viewerPageVM.ConversionName + ".odt";
                            converter.ConvertTxtToOdt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".odt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".odt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "RTF")
                        {
                            var conv = viewerPageVM.ConversionName + ".rtf";
                            converter.ConvertTxtToRtf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".rtf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".rtf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "DOC")
                        {
                            var conv = viewerPageVM.ConversionName + ".doc";
                            converter.ConvertTxtToDoc(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        break;
                    }
                case "ODT":
                    {
                        if (selectedItem.ToUpper() == "PDF")
                        {
                            var conv = viewerPageVM.ConversionName + ".pdf";
                            converter.ConvertOdtToPdf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "DOCX")
                        {
                            var conv = viewerPageVM.ConversionName + ".docx";
                            converter.ConvertOdtToDocx(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "TXT")
                        {
                            var conv = viewerPageVM.ConversionName + ".txt";
                            converter.ConvertOdtToTxt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".txt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".txt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "DOC")
                        {
                            var conv = viewerPageVM.ConversionName + ".doc";
                            converter.ConvertOdtToDoc(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "RTF")
                        {
                            var conv = viewerPageVM.ConversionName + ".rtf";
                            converter.ConvertOdtToRtf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".rtf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".rtf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        break;
                    }
                case "RTF":
                    {
                        if (selectedItem.ToUpper() == "TXT")
                        {
                            var conv = viewerPageVM.ConversionName + ".txt";
                            converter.ConvertRtfToTxt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".txt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".txt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "PDF")
                        {
                            var conv = viewerPageVM.ConversionName + ".pdf";
                            converter.ConvertRtfToPdf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".pdf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".pdf");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "ODT")
                        {
                            var conv = viewerPageVM.ConversionName + ".odt";
                            converter.ConvertRtfToOdt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".odt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".odt");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "DOC")
                        {
                            var conv = viewerPageVM.ConversionName + ".doc";
                            converter.ConvertRtfToDoc(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "DOCX")
                        {
                            var conv = viewerPageVM.ConversionName + ".docx";
                            converter.ConvertRtfToDocx(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        break;
                    }
                case "JPG":
                    {
                        if (selectedItem.ToUpper() == "GIF")
                        {
                            var conv = viewerPageVM.ConversionName + ".gif";
                            converter.ConvertJpgToGif(initPath, endPath, conv);

                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FileGifBox", new SolidColorBrush(Colors.Fuchsia), ".gif");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FileGifBox", new SolidColorBrush(Colors.Fuchsia), ".gif");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        if (selectedItem.ToUpper() == "PNG")
                        {
                            var conv = viewerPageVM.ConversionName + ".png";
                            converter.ConvertJpgToPng(initPath, endPath, conv);

                            if (viewerPageVM.CurrentData.DriveOrLocal)
                            {
                                var temp = new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePngBox", new SolidColorBrush(Colors.Fuchsia), ".png");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            else
                            {
                                var temp = new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FilePngBox", new SolidColorBrush(Colors.Fuchsia), ".png");
                                temp.Appearance = "0";
                                viewerPageVM.CurrentData.AllItems.Add(temp);
                            }
                            CheckIfIsSearch(viewerPageVM);
                            ViewerPageHelper.FilterItems(viewerPageVM);
                        }
                        break;
                    }
            }
        }
    }
}
