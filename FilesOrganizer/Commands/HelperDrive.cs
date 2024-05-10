using FilesOrganizer.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using FilesOrganizer.Models;
using Google.Apis.Download;
using Google.Apis.Upload;
using AODL.Document.Content.Text;
using AODL.Document.TextDocuments;
using DocumentFormat.OpenXml.Packaging;
using iTextSharp.text.pdf;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Text;
using Xceed.Words.NET;
using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;
namespace FilesOrganizer.Commands;

public class HelperDrive
{
    private static DriveService _service;

    public static DriveService Service
    {
        get { return _service; }
        set { _service = value; }
    }

    public HelperDrive()
    {
    }

    public static void ClearFolder()
    {
        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");

        DirectoryInfo di = new DirectoryInfo(solutionDirectory);

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }

    public static void PushChanges(ViewerPageVM viewerPageVM)
    {
        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
        var path = "";
        foreach (var element in viewerPageVM.CurrentData.AllItems)
        {
            string extension = Path.GetExtension(element.Name);
            if (string.IsNullOrEmpty(extension))
            {
                path = Path.Combine(solutionDirectory, element.Path+"\\"+element.Name + element.Extension);
            }
            else
            {
                path = Path.Combine(solutionDirectory, element.Path + "\\" + element.Name);
            }

            if (element.Status == FileStatus.Downloaded)
            {
                UpdateFile(viewerPageVM.Service, element.Id, path);
                element.Status = FileStatus.Undownloaded;
            }
            if(element.Status == FileStatus.ConversionResult)
            {
                //vreau sa iau stringul de fisier tata din element.path
                //vreau sa gasesc elementul cu numele acela 
                var parentName = element.Path.Split('\\').LastOrDefault();
                var parent = viewerPageVM.CurrentData.AllItems.FirstOrDefault(x => x.Name == parentName);
                var id = UploadFile(viewerPageVM.Service, path, parent.Id);
                element.Id = id;
                element.Status = FileStatus.Undownloaded;
            }
        }
    }


    public static ViewerPageVM LoadFilesFromGoogleDrive(ViewerPageVM viewerPageVM)
    {
        viewerPageVM.CurrentData.AllItems.Clear();

        string[] Scopes = { DriveService.Scope.Drive };
        string ApplicationName = "Drive API .NET Quickstart";

        UserCredential credential;

        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        string credentialsPath = Path.Combine(solutionDirectory, "Credentials", "client_secret.json");
        using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
        {
            string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            //credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
        }

        viewerPageVM.Service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
        Service = viewerPageVM.Service;
        FilesResource.ListRequest listRequest = viewerPageVM.Service.Files.List();
        listRequest.Fields = "nextPageToken, files(id, name, mimeType, parents)";
        listRequest.Q = "'me' in owners and trashed = false";

        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;
        if (files != null && files.Count > 0)
        {
            foreach (var file in files)
            {
                //MessageBox.Show("\nFile Name: " + file.Name);
                var fileName = file.Name;

                // Fetch and print the full path of the file
                string path = GetFullPath(viewerPageVM.Service, file);
                //MessageBox.Show("Full Path: " + path);
                //get paretnt
                path = path.Substring(0, path.LastIndexOf("\\"));
                viewerPageVM.CurrentPath = path;
                viewerPageVM.CurrentPathDisplayed = path;

                // Try to get the extension from the filename (unreliable)
                int lastDotIndex = file.Name.LastIndexOf('.');
                string extensionFromName = lastDotIndex >= 0 ? file.Name.Substring(lastDotIndex) : "";

                // Prefer using mimeType for reliable extension (primary method)
                string mimeType = file.MimeType;
                string extension = GetExtension(mimeType);

                // If extension is not found from mimeType, use filename extension (secondary)
                if (string.IsNullOrEmpty(extension))
                {
                    extension = extensionFromName;
                    //MessageBox.Show("File Extension:" + extension);
                }
                else
                {
                    //MessageBox.Show("File Extension:" + extension);
                }


                switch (extension)
                {
                    case "Folder":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "Folder", new SolidColorBrush(Colors.DodgerBlue), extension));
                        break;

                    //audio
                    case ".m4a":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".mp3":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".mpga":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".wav":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileMusicOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".mpeg":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;

                    //video
                    case ".mp4":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".avi":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".mov":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".flv":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".wmv":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".webm":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".mpg":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;
                    case ".3gp":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileVideoOutline", new SolidColorBrush(Colors.RoyalBlue), extension));
                        break;

                    //orher files
                    case ".txt":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "TextBox", new SolidColorBrush(Colors.DeepSkyBlue), extension));
                        break;
                    case ".pdf":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FilePdfBox", new SolidColorBrush(Colors.Red), extension));
                        break;
                    case ".png":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FilePngBox", new SolidColorBrush(Colors.Fuchsia), extension));
                        break;
                    case ".jpg":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "ImageJpgBox", new SolidColorBrush(Colors.Red), extension));
                        break;
                    case ".gif":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileGifBox", new SolidColorBrush(Colors.Green), extension));
                        break;
                    case ".zip":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FolderZip", new SolidColorBrush(Colors.DarkMagenta), extension));
                        break;
                    case ".xls":
                    case ".xlsx":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileExcel", new SolidColorBrush(Colors.DarkGreen), extension));
                        break;
                    case ".ppt":
                    case ".pptx":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FilePowerpoint", new SolidColorBrush(Colors.DarkOrange), extension));
                        break;
                    case ".exe":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "Application", new SolidColorBrush(Colors.DarkBlue), extension));
                        break;
                    case ".doc":
                    case ".docx":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FileWord", new SolidColorBrush(Colors.DarkBlue), extension));
                        break;
                    case ".rar":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "FolderZip", new SolidColorBrush(Colors.MediumPurple), extension));
                        break;
                    case ".jpeg":
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "ImageJpegBox", new SolidColorBrush(Colors.DarkRed), extension));
                        break;
                    default:
                        viewerPageVM.CurrentData.AllItems.Add(new Element(file.Id, path, fileName, "File", new SolidColorBrush(Colors.LightSlateGray), extension));
                        break;
                }


            }
        }
        else
        {
            MessageBox.Show("No files found.");
        }
        return viewerPageVM;
    }
    private static string GetExtension(string mimeType)
    {
        var mimeTypes = new Dictionary<string, string>
        {
            { "application/vnd.google-apps.document", ".docx" },
            { "application/vnd.google-apps.spreadsheet", ".xlsx" },
            { "application/vnd.google-apps.presentation", ".pptx" },
            { "application/vnd.google-apps.folder", "Folder" },
            { "video/mp4", ".mp4" },
            { "video/quicktime", ".mov" },
            { "image/jpeg", ".jpg" },
            { "image/png", ".png" },
            { "image/gif", ".gif" },
            { "text/plain", ".txt" },
            { "text/html", ".html" },
            { "application/json", ".json" },
            { "application/pdf", ".pdf" },
            { "application/msword", ".doc" },  // Removed duplicate
            { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx" },
            { "application/vnd.ms-excel", ".xls" },  // Removed duplicate
            { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx" },
            { "application/vnd.ms-powerpoint", ".ppt" },  // Removed duplicate
            { "application/vnd.openxmlformats-officedocument.presentationml.presentation", ".pptx" },
            { "application/zip", ".zip" },
            { "application/x-rar-compressed", ".rar" },
            { "audio/mpeg", ".mp3" },
            { "audio/x-wav", ".wav" },
            { "application/vnd.oasis.opendocument.text", ".odt" },
            { "application/rtf", ".rtf" },
        };

        return mimeTypes.TryGetValue(mimeType, out var extension) ? extension : "";
    }
    private static Dictionary<string, Google.Apis.Drive.v3.Data.File> fileMap = new Dictionary<string, Google.Apis.Drive.v3.Data.File>();
    private static void BuildFileMap(DriveService service)
    {
        var request = service.Files.List();
        request.Fields = "files(id, name, parents)";
        var files = request.Execute().Files;
        foreach (var file in files)
        {
            fileMap[file.Id] = file;
        }
    }
    private static string GetFullPath(DriveService service, Google.Apis.Drive.v3.Data.File file)
    {
        if (file.Parents == null || !file.Parents.Any())
        {
            return file.Name;
        }

        if (!fileMap.ContainsKey(file.Parents[0]))
        {
            // Fetch the file details from the API
            var request = service.Files.Get(file.Parents[0]);
            request.Fields = "id, name, parents";
            var fileDetails = request.Execute();
            fileMap[file.Parents[0]] = fileDetails;
        }

        var parent = fileMap[file.Parents[0]];
        return Path.Combine(GetFullPath(service, parent), file.Name);
    }
    public static void DownloadFile(DriveService service, string fileId, string saveDirectory)
    {
        Directory.CreateDirectory(saveDirectory);
        var request = service.Files.Get(fileId);
        var stream = new System.IO.MemoryStream();

        // Get the file to check its mimeType
        var file = request.Execute();

        // Check if the file is a Google Doc, Sheet, Slide, or Form
        var exportMimeTypes = new Dictionary<string, string>
        {
            { "application/vnd.google-apps.document", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { "application/vnd.google-apps.spreadsheet", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { "application/vnd.google-apps.presentation", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            // Add more mappings as needed
        };

        string savePath;
        if (file.MimeType.Contains("application/vnd.google-apps"))
        {
            if (exportMimeTypes.TryGetValue(file.MimeType, out var exportMimeType))
            {
                var exportRequest = service.Files.Export(fileId, exportMimeType);
                exportRequest.DownloadWithStatus(stream);

                // Get the extension for the exportMimeType
                string extension = GetExtension(exportMimeType);

                // Append the extension to the filename
                savePath = Path.Combine(saveDirectory, file.Name + extension);
            }
            else
            {
                Console.WriteLine($"Unsupported Google Apps MIME type: {file.MimeType}");
                return;
            }
        }
        else
        {
            // Download other file types
            request.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            break;
                        }
                    case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };
            request.Download(stream);

            string extension = Path.GetExtension(file.Name);

            if (string.IsNullOrEmpty(extension))
            {
                if (string.IsNullOrEmpty(extension))
                {
                    extension = GetExtension(file.MimeType);
                }
                savePath = Path.Combine(saveDirectory, file.Name + extension);
            }
            else
            {
                savePath = Path.Combine(saveDirectory, file.Name);
            }
        }

        SaveStream(stream, savePath);
    }
    private static void SaveStream(System.IO.MemoryStream stream, string saveTo)
    {
        using (System.IO.FileStream file = new System.IO.FileStream(saveTo, System.IO.FileMode.Create, System.IO.FileAccess.Write))
        {
            stream.WriteTo(file);
        }
    }

    public static string UploadFile(DriveService service, string uploadFilePath, string parentFolderId)
    {
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = Path.GetFileName(uploadFilePath),
            Parents = new List<string>
        {
            parentFolderId
        }
        };

        FilesResource.CreateMediaUpload request;

        using (var stream = new System.IO.FileStream(uploadFilePath, System.IO.FileMode.Open))
        {
            request = service.Files.Create(fileMetadata, stream, "application/octet-stream");
            request.Fields = "id";
            var progress = request.Upload();

            if (progress.Status == UploadStatus.Completed)
            {
                Console.WriteLine("Upload completed successfully.");
            }
            else if (progress.Status == UploadStatus.Failed)
            {
                Console.WriteLine("Upload failed. Error: " + progress.Exception);
            }
        }

        var file = request.ResponseBody;
        if (file != null)
        {
            Console.WriteLine("Uploaded file ID: " + file.Id);
            return file.Id;
        }
        else
        {
            Console.WriteLine("Response body is null");
            return "";
        }
    }

    public static void UpdateFile(DriveService service, string fileId, string newFilePath)
    {
        Google.Apis.Drive.v3.Data.File fileMetadata = new Google.Apis.Drive.v3.Data.File();

        FilesResource.UpdateMediaUpload request;

        using (var stream = new System.IO.FileStream(newFilePath, System.IO.FileMode.Open))
        {
            request = service.Files.Update(fileMetadata, fileId, stream, "application/octet-stream");
            var progress = request.Upload();

            if (progress.Status == UploadStatus.Completed)
            {
                Console.WriteLine("Upload completed successfully.");
            }
            else if (progress.Status == UploadStatus.Failed)
            {
                Console.WriteLine("Upload failed. Error: " + progress.Exception);
            }
        }

        var file = request.ResponseBody;
        if (file != null)
        {
            Console.WriteLine("File ID: " + file.Id);
        }
        else
        {
            Console.WriteLine("Response body is null");
        }
    }

    public static string GetTxtFileContent(string fileId)
    {
        var request = Service.Files.Get(fileId);
        request.Alt = FilesResource.GetRequest.AltEnum.Media;
        using (var stream = new MemoryStream())
        {
            request.DownloadWithStatus(stream);
            stream.Position = 0; // Reset the stream position to the beginning
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }

    public static string GetRtfFileContent(DriveService service, string fileId)
    {
        var request = service.Files.Get(fileId);
        request.Alt = FilesResource.GetRequest.AltEnum.Media;
        using (var stream = new MemoryStream())
        {
            request.DownloadWithStatus(stream);
            stream.Position = 0; // Reset the stream position to the beginning
            using (var reader = new StreamReader(stream))
            {
                string content = reader.ReadToEnd();
                return ExtractTextFromRtf(content);
            }
        }
    }
    private static string ExtractTextFromRtf(string rtf)
    {
        using (var richTextBox = new System.Windows.Forms.RichTextBox())
        {
            richTextBox.Rtf = rtf;
            return richTextBox.Text;
        }
    }
    public static string GetOdtFileContent(DriveService service, string fileId)
    {
        var request = service.Files.Get(fileId);
        request.Alt = FilesResource.GetRequest.AltEnum.Media;
        using (var stream = new MemoryStream())
        {
            request.DownloadWithStatus(stream);
            stream.Position = 0; // Reset the stream position to the beginning

            // Save the stream to a temporary file with .odt extension
            var tempFilePath = Path.GetTempFileName();
            var odtFilePath = Path.ChangeExtension(tempFilePath, ".odt");
            File.Move(tempFilePath, odtFilePath);

            using (var fileStream = File.OpenWrite(odtFilePath))
            {
                stream.CopyTo(fileStream);
            }

            // Load the ODT file using AODL
            TextDocument document = new TextDocument();
            document.Load(odtFilePath);

            // Extract the text content
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
            return sb.ToString();
        }
    }

    public static string GetDocxFileContent(DriveService service, string fileId)
    {
        var request = service.Files.Get(fileId);
        request.Alt = FilesResource.GetRequest.AltEnum.Media;

        // Create a temporary file with .docx extension
        var tempFilePath = Path.GetTempFileName();
        var docxFilePath = Path.ChangeExtension(tempFilePath, ".docx");
        File.Move(tempFilePath, docxFilePath);

        // Download the file
        using (var fileStream = File.Create(docxFilePath))
        {
            request.Download(fileStream);
        }

        // Load the .docx file and get its content
        string fileContent;
        using (var document = DocX.Load(docxFilePath))
        {
            fileContent = document.Text;
        }

        return fileContent;
    }

    public static string GetDocFileContent(DriveService service, string fileId)
    {
        var request = service.Files.Get(fileId);
        request.Alt = FilesResource.GetRequest.AltEnum.Media;

        // Create a temporary file with .doc extension
        var tempFilePath = Path.GetTempFileName();
        var docFilePath = Path.ChangeExtension(tempFilePath, ".doc");
        File.Move(tempFilePath, docFilePath);

        // Download the file
        using (var fileStream = File.Create(docFilePath))
        {
            request.Download(fileStream);
        }

        // Load the .doc file and get its content
        Application word = new Application();
        Document doc = word.Documents.Open(docFilePath);
        string fileContent = doc.Content.Text;

        // Close the document and quit Word application
        doc.Close();
        word.Quit();

        return fileContent;
    }

    public static string GetPdfFileContent(DriveService service, string fileId)
    {
        var request = service.Files.Get(fileId);
        request.Alt = FilesResource.GetRequest.AltEnum.Media;

        // Create a temporary file with .pdf extension
        var tempFilePath = Path.GetTempFileName();
        var pdfFilePath = Path.ChangeExtension(tempFilePath, ".pdf");
        File.Move(tempFilePath, pdfFilePath);

        // Download the file
        using (var fileStream = File.Create(pdfFilePath))
        {
            request.Download(fileStream);
        }

        // Load the .pdf file and get its content
        string fileContent = string.Empty;
        using (var reader = new PdfReader(pdfFilePath))
        {
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                fileContent += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page);
            }
        }

        return fileContent;
    }
    public static string GetJsonFileContent(DriveService service, string fileId)
    {
        var request = service.Files.Get(fileId);
        request.Alt = FilesResource.GetRequest.AltEnum.Media;

        // Create a temporary file with .json extension
        var tempFilePath = Path.GetTempFileName();
        var jsonFilePath = Path.ChangeExtension(tempFilePath, ".json");
        File.Move(tempFilePath, jsonFilePath);

        // Download the file
        using (var fileStream = File.Create(jsonFilePath))
        {
            request.Download(fileStream);
        }

        // Load the .json file and get its content
        string fileContent;
        using (var reader = new StreamReader(jsonFilePath))
        {
            fileContent = reader.ReadToEnd();
        }

        return fileContent;
    }

    public static string GetXlsFileContent(DriveService service, string fileId)
    {
        var request = service.Files.Get(fileId);
        request.Alt = FilesResource.GetRequest.AltEnum.Media;

        // Create a temporary file with .xls extension
        var tempFilePath = Path.GetTempFileName();
        var xlsFilePath = Path.ChangeExtension(tempFilePath, ".xls");
        File.Move(tempFilePath, xlsFilePath);

        // Download the file
        using (var fileStream = File.Create(xlsFilePath))
        {
            request.Download(fileStream);
        }

        // Load the .xls file and get its content
        StringBuilder sb = new StringBuilder();
        using (var stream = new FileStream(xlsFilePath, FileMode.Open, FileAccess.Read))
        {
            var hssfwb = new HSSFWorkbook(stream);

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

        return sb.ToString();
    }

    public static string GetXlsxFileContent(DriveService service, string fileId)
    {
        var request = service.Files.Get(fileId);
        request.Alt = FilesResource.GetRequest.AltEnum.Media;

        // Create a temporary file with .xlsx extension
        var tempFilePath = Path.GetTempFileName();
        var xlsxFilePath = Path.ChangeExtension(tempFilePath, ".xlsx");
        File.Move(tempFilePath, xlsxFilePath);

        // Download the file
        using (var fileStream = File.Create(xlsxFilePath))
        {
            request.Download(fileStream);
        }

        // Load the .xlsx file and get its content
        StringBuilder sb = new StringBuilder();
        using (var stream = new FileStream(xlsxFilePath, FileMode.Open, FileAccess.Read))
        {
            var xssfwb = new XSSFWorkbook(stream);

            var sheet = xssfwb.GetSheetAt(0);
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

        return sb.ToString();
    }

    public static string GetHtmlFileContent(DriveService service, string fileId)
    {
        var request = service.Files.Get(fileId);
        request.Alt = FilesResource.GetRequest.AltEnum.Media;

        // Create a temporary file with .html extension
        var tempFilePath = Path.GetTempFileName();
        var htmlFilePath = Path.ChangeExtension(tempFilePath, ".html");
        File.Move(tempFilePath, htmlFilePath);

        // Download the file
        using (var fileStream = File.Create(htmlFilePath))
        {
            request.Download(fileStream);
        }

        // Load the .html file and get its content
        string fileContent;
        using (var reader = new StreamReader(htmlFilePath))
        {
            fileContent = reader.ReadToEnd();
        }

        return fileContent;
    }

    public static string GetHtmFileContent(DriveService service, string fileId)
    {
        var request = service.Files.Get(fileId);
        request.Alt = FilesResource.GetRequest.AltEnum.Media;

        // Create a temporary file with .htm extension
        var tempFilePath = Path.GetTempFileName();
        var htmFilePath = Path.ChangeExtension(tempFilePath, ".htm");
        File.Move(tempFilePath, htmFilePath);

        // Download the file
        using (var fileStream = File.Create(htmFilePath))
        {
            request.Download(fileStream);
        }

        // Load the .htm file and get its content
        string fileContent;
        using (var reader = new StreamReader(htmFilePath))
        {
            fileContent = reader.ReadToEnd();
        }

        return fileContent;
    }

    public static string GetPptxFileContent(DriveService service, string fileId)
    {
        var request = service.Files.Get(fileId);
        request.Alt = FilesResource.GetRequest.AltEnum.Media;

        // Create a temporary file with .pptx extension
        var tempFilePath = Path.GetTempFileName();
        var pptxFilePath = Path.ChangeExtension(tempFilePath, ".pptx");
        File.Move(tempFilePath, pptxFilePath);

        // Download the file
        using (var fileStream = File.Create(pptxFilePath))
        {
            request.Download(fileStream);
        }

        // Load the .pptx file and get its content
        StringBuilder sb = new StringBuilder();
        using (PresentationDocument doc = PresentationDocument.Open(pptxFilePath, false))
        {
            PresentationPart presentationPart = doc.PresentationPart;
            if (presentationPart != null)
            {
                foreach (SlidePart slidePart in presentationPart.SlideParts)
                {
                    if (slidePart.Slide != null)
                    {
                        foreach (var text in slidePart.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                        {
                            sb.AppendLine(text.Text);
                        }
                    }
                }
            }
        }

        return sb.ToString();
    }

}
