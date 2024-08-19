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
using System.Xml.Linq;
using System.Collections.ObjectModel;
using FilesOrganizer.Views;
namespace FilesOrganizer.Helpers;

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
        string[] Scopes = { DriveService.Scope.Drive };
        string ApplicationName = "Drive API .NET Quickstart";

        UserCredential credential;

        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;

        string credentialsPath = Path.Combine(solutionDirectory, "Credentials", "client_secret.json");
        using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
        {
            string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
        }

        Service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    public static void ClearFolder()
    {
        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
        if (Directory.Exists(solutionDirectory))
        {
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
    }
    public static void RenameFile(string fileId, string newName)
    {
        var file = new Google.Apis.Drive.v3.Data.File() { Name = newName };
        var request = Service.Files.Update(file, fileId);
        request.Fields = "id, name";
        var updatedFile = request.Execute();

        if (updatedFile != null)
        {
            Console.WriteLine($"File ID: {updatedFile.Id}, New Name: {updatedFile.Name}");
        }
        else
        {
            Console.WriteLine("Failed to rename file or file does not exist.");
        }
    }


    public static ViewerPageVM PushChanges(ViewerPageVM viewerPageVM)
    {
        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");
        var path = "";
        var pathUpload = "";
        var goodPath = "";

        viewerPageVM.CurrentData.AllItems = new ObservableCollection<Element>(viewerPageVM.CurrentData.AllItems.OrderBy(e => e.Path.Count(f => f == '\\')));

        foreach (var element in viewerPageVM.CurrentData.AllItems)
        {
            string extension = Path.GetExtension(element.Name);
            if (string.IsNullOrEmpty(extension))
            {
                pathUpload = Path.Combine(solutionDirectory, element.Path + "\\" + element.Name + element.Extension);
                goodPath = Path.Combine(solutionDirectory, element.Path + "\\" + element.Name);
            }
            else
            {
                pathUpload = Path.Combine(solutionDirectory, element.Path + "\\" + element.Name);
                goodPath = Path.Combine(solutionDirectory, element.Path + "\\" + element.Name);
            }

            pathUpload = Path.Combine(solutionDirectory, element.Path + "\\" + element.Name + element.Extension);

            path = Path.Combine(solutionDirectory, element.Path + "\\" + element.Id + element.Extension);

            if (element.Status == FileStatus.Downloaded)
            {
                UpdateFile(Service, element.Id, path);
                element.Status = FileStatus.Undownloaded;
            }
            if (element.Status == FileStatus.ConversionResult)
            {
                if (element.Extension == "Folder")
                {
                    var parentName = element.Path.Split('\\').LastOrDefault();
                    var parent = viewerPageVM.CurrentData.AllItems.FirstOrDefault(x => x.Name == parentName);
                    string id = "";
                    if (parent == null)
                    {
                        id = CreateFolder(element.Name, null);
                    }
                    else
                    {
                        id = CreateFolder(element.Name, parent.Id);
                    }
                    element.Id = id;
                    element.Status = FileStatus.Undownloaded;
                }
                else
                {
                    //vreau sa iau stringul de fisier tata din element.path
                    //vreau sa gasesc elementul cu numele acela 
                    var parentName = element.Path.Split('\\').LastOrDefault();
                    var parent = viewerPageVM.CurrentData.AllItems.FirstOrDefault(x => x.Name == parentName);
                    var ex = Path.GetExtension(element.Name);
                    var id = "";
                    if (!string.IsNullOrEmpty(extension))
                    {
                        if(parent == null)
                        {
                            id = UploadFile(pathUpload, element.Name, null);
                        }
                        else
                        {
                            id = UploadFile(pathUpload, element.Name, parent.Id);
                        }
                        //id = UploadFile(pathUpload, element.Name, parent.Id);
                    }
                    else
                    {
                        if (parent == null)
                        {
                            id = UploadFile(pathUpload, element.Name + element.Extension, null);
                        }
                        else
                        {
                            id = UploadFile(pathUpload, element.Name + element.Extension, parent.Id);
                        }
                         //id = UploadFile(pathUpload, element.Name + element.Extension, parent.Id);
                        RenameFile(id, element.Name);
                    }
                    element.Id = id;
                    element.Status = FileStatus.Undownloaded;
                }
            }
        }
        return viewerPageVM;
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
            string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
        }

        Service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
        FilesResource.ListRequest listRequest = Service.Files.List();
        listRequest.Fields = "nextPageToken, files(id, name, mimeType, parents, modifiedTime, size)";
        listRequest.Q = "'me' in owners and trashed = false";

        IList<Google.Apis.Drive.v3.Data.File> files = new List<Google.Apis.Drive.v3.Data.File>();
        do
        {
            var result = listRequest.Execute();
            files = files.Concat(result.Files).ToList();
            listRequest.PageToken = result.NextPageToken;
        } while (listRequest.PageToken != null);

        if (files != null && files.Count > 0)
        {
            foreach (var file in files)
            {
                //MessageBox.Show("\nFile Name: " + file.Name);
                var fileName = file.Name;

                // Fetch and print the full path of the file
                string path = GetFullPath(file);
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
                }

                long fileSize = file.Size.HasValue ? file.Size.Value : 0;
                DateTime? lastModified = file.ModifiedTime;
                string lastModifiedStr = lastModified.HasValue ? lastModified.Value.ToLocalTime().ToString("dd MMMM yyyy, HH:mm") : "Unknown";

                Element element = new Element(file.Id, path, fileName, "", new SolidColorBrush(), extension);

                element.Size = fileSize;
                element.LastAccessed = lastModifiedStr;

                switch (extension)
                {
                    case "Folder":
                        element.Icon = "Folder";
                        element.Color = new SolidColorBrush(Colors.DodgerBlue);
                        break;

                    //audio
                    case ".m4a":
                    case ".mp3":
                    case ".mpga":
                    case ".wav":
                    case ".mpeg":
                        element.Icon = "FileMusicOutline";
                        element.Color = new SolidColorBrush(Colors.RoyalBlue);
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
                        element.Icon = "FileVideoOutline";
                        element.Color = new SolidColorBrush(Colors.RoyalBlue);
                        break;

                    //other files
                    case ".txt":
                        element.Icon = "TextBox";
                        element.Color = new SolidColorBrush(Colors.DeepSkyBlue);
                        break;
                    case ".pdf":
                        element.Icon = "FilePdfBox";
                        element.Color = new SolidColorBrush(Colors.Red);
                        break;
                    case ".png":
                        element.Icon = "FilePngBox";
                        element.Color = new SolidColorBrush(Colors.Fuchsia);
                        break;
                    case ".jpg":
                        element.Icon = "ImageJpgBox";
                        element.Color = new SolidColorBrush(Colors.Red);
                        break;
                    case ".gif":
                        element.Icon = "FileGifBox";
                        element.Color = new SolidColorBrush(Colors.Green);
                        break;
                    case ".zip":
                    case ".rar":
                        element.Icon = "FolderZip";
                        element.Color = new SolidColorBrush(Colors.DarkMagenta);
                        break;
                    case ".xls":
                    case ".xlsx":
                        element.Icon = "FileExcel";
                        element.Color = new SolidColorBrush(Colors.DarkGreen);
                        break;
                    case ".ppt":
                    case ".pptx":
                        element.Icon = "FilePowerpoint";
                        element.Color = new SolidColorBrush(Colors.DarkOrange);
                        break;
                    case ".exe":
                        element.Icon = "Application";
                        element.Color = new SolidColorBrush(Colors.DarkBlue);
                        break;
                    case ".doc":
                    case ".docx":
                        element.Icon = "FileWord";
                        element.Color = new SolidColorBrush(Colors.DarkBlue);
                        break;
                    case ".jpeg":
                        element.Icon = "ImageJpegBox";
                        element.Color = new SolidColorBrush(Colors.DarkRed);
                        break;
                    default:
                        element.Icon = "File";
                        element.Color = new SolidColorBrush(Colors.LightSlateGray);
                        break;
                }

                viewerPageVM.CurrentData.AllItems.Add(element);

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
            { "application/msword", ".doc" },
            { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx" },
            { "application/vnd.ms-excel", ".xls" },
            { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx" },
            { "application/vnd.ms-powerpoint", ".ppt" },
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
    private static void BuildFileMap()
    {
        var request = Service.Files.List();
        request.Fields = "files(id, name, parents)";
        var files = request.Execute().Files;
        foreach (var file in files)
        {
            fileMap[file.Id] = file;
        }
    }
    private static string GetFullPath(Google.Apis.Drive.v3.Data.File file)
    {
        if (file.Parents == null || !file.Parents.Any())
        {
            return file.Name;
        }

        if (!fileMap.ContainsKey(file.Parents[0]))
        {
            // Fetch the file details from the API
            var request = Service.Files.Get(file.Parents[0]);
            request.Fields = "id, name, parents";
            var fileDetails = request.Execute();
            fileMap[file.Parents[0]] = fileDetails;
        }

        var parent = fileMap[file.Parents[0]];
        return Path.Combine(GetFullPath(parent), file.Name);
    }
    public static void DownloadFile(string fileId, string saveDirectory)
    {
        Directory.CreateDirectory(saveDirectory);
        var request = Service.Files.Get(fileId);
        var stream = new MemoryStream();

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
                var exportRequest = Service.Files.Export(fileId, exportMimeType);
                exportRequest.DownloadWithStatus(stream);

                // Get the extension for the exportMimeType
                string extension = GetExtension(exportMimeType);

                // Append the extension to the filename
                //savePath = Path.Combine(saveDirectory, file.Name + extension);                
                savePath = Path.Combine(saveDirectory, file.Id + extension);
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
            request.MediaDownloader.ProgressChanged += (progress) =>
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
                savePath = Path.Combine(saveDirectory, file.Id + extension);
            }
            else
            {
                savePath = Path.Combine(saveDirectory, file.Id);
            }

            savePath = Path.Combine(saveDirectory, fileId + extension);
        }



        SaveStream(stream, savePath);
    }
    private static void SaveStream(MemoryStream stream, string saveTo)
    {
        using (FileStream file = new FileStream(saveTo, FileMode.Create, FileAccess.Write))
        {
            stream.WriteTo(file);
        }
    }

    public static string CreateFolder(string folderName, string parentFolderId)
    {
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = folderName,
            MimeType = "application/vnd.google-apps.folder",
        };

        if (parentFolderId != null)
        {
            fileMetadata.Parents = new List<string> { parentFolderId };
        }

        var request = Service.Files.Create(fileMetadata);
        request.Fields = "id";

        var file = request.Execute();

        return file.Id;
    }

    public static string UploadFile(string uploadFilePath, string uploadFileName, string parentFolderId)
    {
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = uploadFileName,
            Parents = new List<string>
    {
        parentFolderId
    }
        };

        FilesResource.CreateMediaUpload request;

        using (var stream = new FileStream(uploadFilePath, FileMode.Open))
        {
            request = Service.Files.Create(fileMetadata, stream, "application/octet-stream");
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

        try
        {
            using (var stream = new FileStream(newFilePath, FileMode.Open))
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
        }
        catch
        {
            MessageBox.Show("You need to close files before pushing changes to Drive.");
        }
    }

    public static void DeleteFile(string fileId)
    {
        try
        {
            Service.Files.Delete(fileId).Execute();
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}