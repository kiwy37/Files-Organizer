using FilesOrganizer.Models;
using FilesOrganizer.ViewModels.Commands;
using NPOI.POIFS.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FilesOrganizer.Commands;

public class ConvertItemsHelper
{
    Converter converter = new Converter();
    public ConvertItemsHelper()
    {
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
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".docx"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".docx"));
                            }


                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".docx"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".docx"));
                            }
                        }
                        if (selectedItem.ToUpper() == "PDF")
                        {
                            var conv = viewerPageVM.ConversionName + ".pdf";
                            converter.ConvertDocToPdf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(endPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                        }
                        if (selectedItem.ToUpper() == "TXT")
                        {
                            var conv = viewerPageVM.ConversionName + ".txt";
                            converter.ConvertDocToTxt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), ".txt"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), ".txt"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), ".txt"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), ".txt"));
                            }
                        }
                        if (selectedItem.ToUpper() == "ODT")
                        {
                            var conv = viewerPageVM.ConversionName + ".odt";
                            converter.ConvertDocToOdt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".odt"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".odt"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".odt"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".odt"));
                            }
                        }
                        if (selectedItem.ToUpper() == "RTF")
                        {
                            var conv = viewerPageVM.ConversionName + ".rtf";
                            converter.ConvertDocToRtf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".rtf"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".rtf"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".rtf"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".rtf"));
                            }
                        }
                        break;
                    }
                case "DOCX":
                    {
                        if (selectedItem.ToUpper() == "DOC")
                        {
                            var conv = viewerPageVM.ConversionName + ".doc";
                            converter.ConvertDocxToDoc(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".doc"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".doc"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".doc"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".doc"));
                            }
                        }
                        if (selectedItem.ToUpper() == "PDF")
                        {
                            var conv = viewerPageVM.ConversionName + ".pdf";
                            converter.ConvertDocxToPdf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                        }
                        if (selectedItem.ToUpper() == "TXT")
                        {
                            var conv = viewerPageVM.ConversionName + ".txt";
                            converter.ConvertDocxToTxt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), ".txt"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), ".txt"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), ".txt"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), ".txt"));
                            }
                        }
                        if (selectedItem.ToUpper() == "ODT")
                        {
                            var conv = viewerPageVM.ConversionName + ".odt";
                            converter.ConvertDocxToOdt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".odt"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".odt"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".odt"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".odt"));
                            }
                        }
                        if (selectedItem.ToUpper() == "RTF")
                        {
                            var conv = viewerPageVM.ConversionName + ".rtf";
                            converter.ConvertDocxToRtf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".rtf"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".rtf"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".rtf"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "MicrosoftWord", new SolidColorBrush(Colors.DarkBlue), ".rtf"));
                            }
                        }
                        break;
                    }
                case "PDF":
                    {
                        if (selectedItem.ToUpper() == "DOCX")
                        {
                            var conv = viewerPageVM.ConversionName + ".docx";
                            converter.ConvertPdfToDocx(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, conv, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, conv, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, conv, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, conv, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
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
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                            if(viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                        }
                        if (selectedItem.ToUpper() == "DOCX")
                        {
                            var conv = viewerPageVM.ConversionName + ".docx";
                            converter.ConvertTxtToDocx(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
                        }
                        if (selectedItem.ToUpper() == "ODT")
                        {
                            var conv = viewerPageVM.ConversionName + ".odt";
                            converter.ConvertTxtToOdt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".odt"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".odt"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".odt"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".odt"));
                            }
                        }
                        if (selectedItem.ToUpper() == "RTF")
                        {
                            var conv = viewerPageVM.ConversionName + ".rtf";
                            converter.ConvertTxtToRtf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".rtf"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".rtf"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".rtf"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".rtf"));
                            }
                        }
                        if (selectedItem.ToUpper() == "DOC")
                        {
                            var conv = viewerPageVM.ConversionName + ".doc";
                            converter.ConvertTxtToDoc(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc"));
                            }
                        }
                        break;
                    }
                case "ODT":
                    {
                        if (selectedItem.ToUpper() == "PDF")
                        {
                            var conv = viewerPageVM.ConversionName + ".pdf";
                            converter.ConvertOdtToPdf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".pdf"));
                            }
                        }
                        if (selectedItem.ToUpper() == "DOCX")
                        {
                            var conv = viewerPageVM.ConversionName + ".docx";
                            converter.ConvertOdtToDocx(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
                        }
                        if (selectedItem.ToUpper() == "TXT")
                        {
                            var conv = viewerPageVM.ConversionName + ".txt";
                            converter.ConvertOdtToTxt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".txt"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".txt"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".txt"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".txt"));
                            }
                        }
                        if (selectedItem.ToUpper() == "DOC")
                        {
                            var conv = viewerPageVM.ConversionName + ".doc";
                            converter.ConvertOdtToDoc(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc"));
                            }
                        }
                        if (selectedItem.ToUpper() == "RTF")
                        {
                            var conv = viewerPageVM.ConversionName + ".rtf";
                            converter.ConvertOdtToRtf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".rtf"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".rtf"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".rtf"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".rtf"));
                            }
                        }
                        break;
                    }
                case "RTF":
                    {
                        if (selectedItem.ToUpper() == "TXT")
                        {
                            var conv = viewerPageVM.ConversionName + ".txt";
                            converter.ConvertRtfToTxt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".txt"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".txt"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".txt"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "FilePdfBox", new SolidColorBrush(Colors.Red), ".txt"));
                            }
                        }
                        if (selectedItem.ToUpper() == "PDF")
                        {
                            var conv = viewerPageVM.ConversionName + ".pdf";
                            converter.ConvertRtfToPdf(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".pdf"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".pdf"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".pdf"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".pdf"));
                            }
                        }
                        if (selectedItem.ToUpper() == "ODT")
                        {
                            var conv = viewerPageVM.ConversionName + ".odt";
                            converter.ConvertRtfToOdt(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".odt"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".odt"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".odt"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".odt"));
                            }
                        }
                        if (selectedItem.ToUpper() == "DOC")
                        {
                            var conv = viewerPageVM.ConversionName + ".doc";
                            converter.ConvertRtfToDoc(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".doc"));
                            }
                        }
                        if (selectedItem.ToUpper() == "DOCX")
                        {
                            var conv = viewerPageVM.ConversionName + ".docx";
                            converter.ConvertRtfToDocx(initPath, endPath, conv);
                            if (viewerPageVM.CurrentData.DriveOdLocal)
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
                            else
                            {
                                viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
                            if (viewerPageVM.CurrentPath == endPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, viewerPageVM.ConversionName, "File", new SolidColorBrush(Colors.Gainsboro), ".docx"));
                            }
                        }
                        break;
                    }
                case "JPG":
                    {
                        if (selectedItem.ToUpper() == "GIF")
                        {
                            var conv = viewerPageVM.ConversionName + ".gif";
                            converter.ConvertJpgToGif(initPath, endPath, conv);
                            viewerPageVM.CurrentData.AllItems.Add(new Element(FileStatus.ConversionResult, endPath, conv, "FilePngBox", new SolidColorBrush(Colors.Fuchsia), ".png"));
                            if (viewerPageVM.CurrentPath == viewerPageVM.SelectedItem.Path)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, endPath, conv, "FilePngBox", new SolidColorBrush(Colors.Fuchsia), ".png"));
                            }
                            if (viewerPageVM.CurrentPath == driveEndPath)
                            {
                                viewerPageVM.CurrentData.CurrentListBoxSource.Add(new Element(FileStatus.ConversionResult, driveEndPath, conv, "FilePngBox", new SolidColorBrush(Colors.Fuchsia), ".png"));
                            }
                        }
                        break;
                    }
            }
        }
    }
}
