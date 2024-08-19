using DiffPlex.DiffBuilder.Model;
using DiffPlex.DiffBuilder;
using DiffPlex;
using FilesOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AODL.Document.Content.Text;
using AODL.Document.TextDocuments;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using System.Xml.Linq;
using Xceed.Words.NET;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;
using MathNet.Numerics.IntegralTransforms;
using System.Numerics;
using NPOI.HSSF.Record;
using NPOI.SS.Formula.Functions;
using NAudio.Wave;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Accord.Math.Distances;
using System.Diagnostics;

namespace FilesOrganizer.Helpers;

public class HelperSimilarFiles
{
    public static ObservableCollection<ObservableCollection<Element>> ClusterDocuments(bool driveOrLocal, float clustersDistance, ObservableCollection<Element> files)
    {
        string scriptPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        scriptPath = Path.Combine(scriptPath, "ScriptsAnsBatches");
        scriptPath = Path.Combine(scriptPath, "run_script2.bat");

        StringBuilder filesArgument = new StringBuilder();
        filesArgument.Append("[");
        foreach (var file in files)
        {
            var name = file.Name + file.Extension;
            string fileName = Convert.ToBase64String(Encoding.UTF8.GetBytes(name.Replace("\"", "\\\"")));
            var path = "";

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

                path = Path.Combine(solutionDirectory, file.Id + file.Extension);

            }
            else
            {
                path = file.Path + "\\" + file.Name + file.Extension;
            }

            string fileContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(path.Replace("\"", "\\\"")));

            string ext = Path.GetExtension(file.Name);


            filesArgument.Append($"(\"{fileName}\", \"{fileContent}\"),");
        }
        filesArgument.Remove(filesArgument.Length - 1, 1); // Remove the trailing comma
        filesArgument.Append("]");

        var distanceEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(clustersDistance.ToString()));
        var filesEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(filesArgument.ToString()));

        string arguments = $"{distanceEncoded} \"{filesEncoded}\"";


        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = scriptPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        Process process = new Process() { StartInfo = startInfo };
        process.Start();

        string result = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        var res = InterpretResult(result, files);

        return res;
    }

    public static ObservableCollection<ObservableCollection<Element>> ClusterPhotosSSIM(float eps, int min_samples, ObservableCollection<Element> files)
    {
        var clusters = new ObservableCollection<ObservableCollection<Element>>();
        var visited = new HashSet<Element>();
        var noise = new ObservableCollection<Element>();

        foreach (var file in files)
        {
            if (visited.Contains(file)) continue;

            visited.Add(file);

            var neighbors = GetNeighbors(file, files, eps);

            if (neighbors.Count < min_samples)
            {
                noise.Add(file);
            }
            else
            {
                var cluster = new ObservableCollection<Element> { file };
                clusters.Add(cluster);

                ExpandCluster(file, neighbors, cluster, visited, files, eps, min_samples);
            }
        }

        return clusters;
    }

    public static ObservableCollection<ObservableCollection<Element>> ClusterPhotosCropped(float eps, ObservableCollection<Element> files)
    {
        var clusters = new ObservableCollection<ObservableCollection<Element>>();
        var visited = new HashSet<Element>();

        foreach (var file in files)
        {
            if (visited.Contains(file)) continue;

            var cluster = new ObservableCollection<Element> { file };
            visited.Add(file);

            foreach (var potentialCropped in files)
            {
                if (file == potentialCropped || visited.Contains(potentialCropped)) continue;

                var similarity = CalculateCroppedArea(file.Path + "\\" + file.Name + file.Extension, potentialCropped.Path + "\\" + potentialCropped.Name + potentialCropped.Extension);

                // If the similarity is high enough, consider the image as a cropped version of the original image
                if (similarity > eps)
                {
                    cluster.Add(potentialCropped);
                    visited.Add(potentialCropped);
                }
            }

            // Only add the cluster to the list if it contains more than one element
            if (cluster.Count > 1)
            {
                clusters.Add(cluster);
            }
        }

        return clusters;
    }



    private static void ExpandCluster(Element file, List<Element> neighbors, ObservableCollection<Element> cluster, HashSet<Element> visited, ObservableCollection<Element> files, float eps, int min_samples)
    {
        var i = 0;
        while (i < neighbors.Count)
        {
            var neighbor = neighbors[i];

            if (!visited.Contains(neighbor))
            {
                visited.Add(neighbor);

                var neighborNeighbors = GetNeighbors(neighbor, files, eps);

                if (neighborNeighbors.Count >= min_samples)
                {
                    neighbors.AddRange(neighborNeighbors);
                }
            }

            if (!cluster.Contains(neighbor))
            {
                cluster.Add(neighbor);
            }

            i++;
        }
    }

    private static List<Element> GetNeighbors(Element file, ObservableCollection<Element> files, float similarityThreshold)
    {
        var neighbors = new List<Element>();

        foreach (var potentialNeighbor in files)
        {
            if (file == potentialNeighbor) continue;

            var similarity = CalculateSSIM(file.Path + "\\" + file.Name + file.Extension, potentialNeighbor.Path + "\\" + potentialNeighbor.Name + potentialNeighbor.Extension);

            if (similarity >= similarityThreshold)
            {
                neighbors.Add(potentialNeighbor);
            }
        }

        return neighbors;
    }



    private static ObservableCollection<ObservableCollection<Element>> InterpretResult(string result, ObservableCollection<Element> files)
    {
        var lines = result.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var fileGroups = new Dictionary<string, ObservableCollection<Element>>();

        foreach (var line in lines)
        {
            var parts = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            var fileName = parts[0].Trim();
            var groupNumber = parts[1].Trim();

            if (!fileGroups.ContainsKey(groupNumber))
            {
                fileGroups[groupNumber] = new ObservableCollection<Element>();
            }

            var file = files.FirstOrDefault(f => f.Name + f.Extension == fileName);
            if (file != null)
            {
                fileGroups[groupNumber].Add(file);
            }
        }
        //remove all groups with only one element
        fileGroups = fileGroups.Where(x => x.Value.Count > 1).ToDictionary(x => x.Key, x => x.Value);

        return new ObservableCollection<ObservableCollection<Element>>(fileGroups.Values);
    }

    #region Pictures
    public static double CalculateSSIM(string filePath1, string filePath2)
    {
        try
        {
            if (!File.Exists(filePath1) || !File.Exists(filePath2))
            {
                throw new FileNotFoundException("One or both of the image files could not be found.");
            }

            using (Image<Bgr, byte> img1 = new Image<Bgr, byte>(filePath1))
            using (Image<Bgr, byte> img2 = new Image<Bgr, byte>(filePath2))
            {
                if (img1.Size != img2.Size)
                {
                    throw new ArgumentException("Images must have the same dimensions.");
                }

                float ssim = 0;
                for (int channel = 0; channel < 3; channel++)
                {
                    Image<Gray, byte> img1Channel = img1.Split()[channel];
                    Image<Gray, byte> img2Channel = img2.Split()[channel];

                    Image<Gray, float> img1ChannelFloat = img1Channel.Convert<Gray, float>();
                    Image<Gray, float> img2ChannelFloat = img2Channel.Convert<Gray, float>();

                    Image<Gray, float> img1Sq = img1ChannelFloat.Pow(2);
                    Image<Gray, float> img2Sq = img2ChannelFloat.Pow(2);
                    Image<Gray, float> img1Img2 = img1ChannelFloat.Mul(img2ChannelFloat);

                    double mu1 = img1ChannelFloat.GetAverage().Intensity;
                    double mu2 = img2ChannelFloat.GetAverage().Intensity;

                    double mu1Sq = mu1 * mu1;
                    double mu2Sq = mu2 * mu2;
                    double mu1Mu2 = mu1 * mu2;

                    double sigma1Sq = img1Sq.GetAverage().Intensity - mu1Sq;
                    double sigma2Sq = img2Sq.GetAverage().Intensity - mu2Sq;
                    double sigma12 = img1Img2.GetAverage().Intensity - mu1Mu2;

                    float ssimMap = (float)((2 * mu1Mu2 + 1.0) * (2 * sigma12 + 1.0) / ((mu1Sq + mu2Sq + 1.0) * (sigma1Sq + sigma2Sq + 1.0)));
                    ssim += ssimMap;
                }

                return ssim / 3;
            }
        }
        catch (Exception ex)
        {
            return 0.0;
        }
    }

    public static double CalculateCroppedArea(string filePath1, string filePath2)
    {
        try
        {
            // Load the images
            using (Image<Bgr, byte> image1 = new Image<Bgr, byte>(filePath1))
            using (Image<Bgr, byte> image2 = new Image<Bgr, byte>(filePath2))
            {
                // Check image dimensions
                if (image1.Width == 0 || image1.Height == 0 || image2.Width == 0 || image2.Height == 0)
                {
                    throw new ArgumentException("Invalid image dimensions.");
                }

                // Determine which image is smaller (the template) and which is larger (the source)
                Image<Bgr, byte> sourceImage, templateImage;
                double sourceArea, templateArea;
                if (image1.Width * image1.Height < image2.Width * image2.Height)
                {
                    sourceImage = image2;
                    templateImage = image1;
                    sourceArea = image2.Width * image2.Height;
                    templateArea = image1.Width * image1.Height;
                }
                else
                {
                    sourceImage = image1;
                    templateImage = image2;
                    sourceArea = image1.Width * image1.Height;
                    templateArea = image2.Width * image2.Height;
                }

                // Perform template matching
                using (Image<Gray, float> result = sourceImage.MatchTemplate(templateImage, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed))
                {
                    double[] minValues, maxValues;
                    Point[] minLocations, maxLocations;
                    result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                    if (Math.Abs(maxValues[0] - 1) < 0.01)
                    {
                        // Return the percentage of the original image that remains after cropping
                        return templateArea / sourceArea;
                    }
                    else
                    {
                        return 0.0;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return 0.0;
        }
    }

    #endregion
    #region For text files

    private static double CompareTextFiles(bool driveOrLocal, string filePath1, string filePath2, string id1, string id2)
    {
        string text1 = "";
        string text2 = "";
        if (driveOrLocal)
        {
            var extension1 = Path.GetExtension(filePath1).ToLower();
            var extension2 = Path.GetExtension(filePath2).ToLower();





            text1 = GetFileContent(filePath1);
            text2 = GetFileContent(filePath2);
        }
        else
        {
            text1 = GetFileContent(filePath1);
            text2 = GetFileContent(filePath2);
        }

        var diffBuilder = new InlineDiffBuilder(new Differ());
        var diff = diffBuilder.BuildDiffModel(text1, text2);

        int changes = diff.Lines.Count(x => x.Type != ChangeType.Unchanged);
        double totalLines = diff.Lines.Count;

        double similarity = 1.0 - changes / totalLines;

        return similarity;
    }

    public static string GetFileContent(string path)
    {
        string fileContent = "";
        string extension = Path.GetExtension(path).ToLower();
        if (!File.Exists(path))
        {
            return "";
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
                        if (item is Paragraph paragraph)
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

        return fileContent;
    }
    #endregion

    public static void ShowSimilarElements(ObservableCollection<ObservableCollection<Element>> groups)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < groups.Count; i++)
        {
            sb.AppendLine($"Group {i + 1}:");

            foreach (var item in groups[i])
            {
                sb.AppendLine(item.Path + "\\" + item.Name);
            }

            sb.AppendLine();
        }

        MessageBox.Show(sb.ToString(), "Similar Files");
    }
}

