using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Linq;

namespace FilesOrganizer.Commands;

public class Converter
{
    string saveFolderPath = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName, "ConversionHelper");
    public Converter()
    {

    }
    #region For Docx
    public void ConvertDocxToDoc(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to doc --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".docx", ".doc") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertDocxToRtf(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to rtf --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".docx", ".rtf") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertDocxToOdt(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to odt --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".docx", ".odt") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertDocxToPdf(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to pdf --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }
        //"soffice --headless --convert-to pdf --outdir \"C:\\Users\\Kiwy\\Desktop\\licenta 2.0\\FilesOrganizer\\FilesOrganizer\\ConversionHelper\" --writer --norestore \"C:\\Users\\Kiwy\\Desktop\\converter_tests\\documente\\doc1.docx\""
        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".docx", ".pdf") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertDocxToTxt(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to txt:Text --outdir \"" + saveFolderPath + "\" --writer --norestore \""+ inputFilePath +"\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".docx", ".txt") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    #endregion
    #region For Doc
    public void ConvertDocToDocx(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to docx --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".doc", ".docx") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertDocToRtf(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to rtf --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".doc", ".rtf") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertDocToOdt(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to odt --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".doc", ".odt") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertDocToPdf(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to pdf --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }
        //"soffice --headless --convert-to pdf --outdir \"C:\\Users\\Kiwy\\Desktop\\licenta 2.0\\FilesOrganizer\\FilesOrganizer\\ConversionHelper\" --writer --norestore \"C:\\Users\\Kiwy\\Desktop\\converter_tests\\documente\\doc1.docx\""
        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".doc", ".pdf") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertDocToTxt(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to txt:Text --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".doc", ".txt") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    #endregion
    #region For Pdf
    public void ConvertPdfToDocx(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                //soffice --headless --infilter="writer_pdf_import" --convert-to docx --outdir "C:\Users\Kiwy\Desktop\folder_test" "C:\Users\Kiwy\Desktop\folder_test\Q.pdf"
                var command = $"soffice --headless --infilter=\"writer_pdf_import\" --convert-to docx --outdir \"" + saveFolderPath +"\" \""+inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".pdf", ".docx") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(Directory.GetParent(saveDirectory).FullName, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }

    //public void ConvertPdfToTxt(string inputFilePath, string saveDirectory, string savingName)
    //{
    //    var startInfo = new ProcessStartInfo
    //    {
    //        FileName = "cmd.exe",
    //        RedirectStandardInput = true,
    //        RedirectStandardOutput = true,
    //        UseShellExecute = false,
    //        CreateNoWindow = true
    //    };

    //    var process = new Process { StartInfo = startInfo };
    //    process.Start();
    //    using (var sw = process.StandardInput)
    //    {
    //        if (sw.BaseStream.CanWrite)
    //        {
    //            var command = $"soffice --headless --convert-to txt:Text --outdir \"{saveFolderPath}\" \"{inputFilePath}\"";
    //            sw.WriteLine(command);
    //        }
    //    }

    //    process.WaitForExit();

    //    var renameProcess = new Process { StartInfo = startInfo };
    //    renameProcess.Start();
    //    using (var sw = renameProcess.StandardInput)
    //    {
    //        if (sw.BaseStream.CanWrite)
    //        {
    //            var renameCommand = $"rename \"{saveFolderPath}\\{System.IO.Path.GetFileName(inputFilePath).Replace(".pdf", ".txt")}\" \"{savingName}\"";
    //            sw.WriteLine(renameCommand);
    //        }
    //    }

    //    renameProcess.WaitForExit();

    //    File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(Directory.GetParent(saveDirectory).FullName, savingName));
    //    File.Delete(Path.Combine(saveFolderPath, savingName));
    //}
    #endregion
    #region For Txt
    public void ConvertTxtToPdf(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                //             soffice --headless --convert-to pdf --outdir "C:\Users\Kiwy\Desktop\converter_tests\documente\f" --writer --norestore "C:\Users\Kiwy\Desktop\converter_tests\documente\f\aa.txt"
                var command = "soffice --headless --convert-to pdf --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }
        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".txt", ".pdf") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertTxtToDocx(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to docx --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }
        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".txt", ".docx") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertTxtToOdt(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to odt --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".txt", ".odt") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertTxtToRtf(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to rtf --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".txt", ".rtf") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertTxtToDoc(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to doc --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".txt", ".doc") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    #endregion
    #region For Odt
    public void ConvertOdtToDocx(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to docx --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".odt", ".docx") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertOdtToPdf(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to pdf --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".odt", ".pdf") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertOdtToTxt(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to txt:Text --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".odt", ".txt") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertOdtToRtf(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to rtf --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".odt", ".rtf") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertOdtToDoc(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to doc --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".odt", ".doc") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    #endregion
    #region for Rtf
    public void ConvertRtfToTxt(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to txt:Text --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".rtf", ".txt") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertRtfToPdf(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to pdf --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".rtf", ".pdf") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertRtfToOdt(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to odt --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".rtf", ".odt") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertRtfToDoc(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to doc --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".rtf", ".doc") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    public void ConvertRtfToDocx(string inputFilePath, string saveDirectory, string savingName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = startInfo };
        process.Start();
        using (var sw = process.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var command = "soffice --headless --convert-to docx --outdir \"" + saveFolderPath + "\" --writer --norestore \"" + inputFilePath + "\"";
                sw.WriteLine(command);
            }
        }

        process.WaitForExit();

        var renameProcess = new Process { StartInfo = startInfo };
        renameProcess.Start();
        using (var sw = renameProcess.StandardInput)
        {
            if (sw.BaseStream.CanWrite)
            {
                var renameCommand = "rename \"" + saveFolderPath + "\\" + System.IO.Path.GetFileName(inputFilePath).Replace(".rtf", ".docx") + "\" \"" + savingName + "\"";
                sw.WriteLine(renameCommand);
            }
        }

        renameProcess.WaitForExit();

        File.Move(Path.Combine(saveFolderPath, savingName), Path.Combine(saveDirectory, savingName));
        File.Delete(Path.Combine(saveFolderPath, savingName));
    }
    #endregion
    #region For Photos
    public void ConvertJpgToGif(string sourcePath, string targetDirectory, string conv)
    {
        // Load the image.
        using (Image image = Image.FromFile(sourcePath))
        {
            // Create the target path with the provided file name.
            string directoryName = Path.GetDirectoryName(targetDirectory);

            string targetPath = Path.Combine(directoryName, conv);

            // Save the image in GIF format.
            image.Save(targetPath, System.Drawing.Imaging.ImageFormat.Gif);
        }
    }
    #endregion
}
