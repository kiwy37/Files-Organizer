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

namespace FilesOrganizer.ViewModels.Commands;

public class HelperSimilarFiles
{
    //delete lists with one elements from the list
    public static ObservableCollection<ObservableCollection<Element>> RemoveSingleElementGroups(ObservableCollection<ObservableCollection<Element>> groups)
    {
        return new ObservableCollection<ObservableCollection<Element>>(groups.Where(group => group.Count > 1));
    }

    public static ObservableCollection<ObservableCollection<Element>> HierarchicalClustering(ObservableCollection<ObservableCollection<Element>> clusters, double threshold, bool text)
    {
        while (true)
        {
            double minDistance = double.MaxValue;
            int cluster1 = 0, cluster2 = 0;

            // Find the two closest clusters
            for (int i = 0; i < clusters.Count; i++)
            {
                for (int j = i + 1; j < clusters.Count; j++)
                {
                    double distance = 1 - AreSimilar(clusters[i][0].Path + "\\" + clusters[i][0].Name, clusters[j][0].Path + "\\" + clusters[j][0].Name);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        cluster1 = i;
                        cluster2 = j;
                    }
                }
            }

            // If the closest distance is greater than the threshold, stop
            if (minDistance > threshold)
            {
                break;
            }

            // Merge the two closest clusters
            foreach (var item in clusters[cluster2])
            {
                clusters[cluster1].Add(item);
            }

            clusters.RemoveAt(cluster2);
        }

        clusters = RemoveSingleElementGroups(clusters);
        return new ObservableCollection<ObservableCollection<Element>>(clusters);
    }

    private static double AreSimilar(string filePath1, string filePath2)
    {
        string extension1 = System.IO.Path.GetExtension(filePath1);
        string extension2 = System.IO.Path.GetExtension(filePath2);
        List<string> extensions = new List<string> { ".txt", ".rtf", ".json", ".odt", ".doc", ".docx", ".xls", ".xlsx", ".html", ".htm", ".pdf", ".xml", ".ppt", ".pptx" };

        if (extension1 != extension2)
        {
            return 0;
        }

        switch (extension1)
        {
            case var ext when extensions.Contains(ext):
                return CompareTextFiles(filePath1, filePath2);
            case ".jpg":
            case ".png":
                double cropMatch = CompareImageFiles(filePath1, filePath2);
                if (cropMatch == 1.0)
                {
                    return 1.0;
                }
                // If not, calculate the SSIM
                else
                {
                    return CalculateSSIM(filePath1, filePath2);
                }
            default:
                return 0;
        }
    }

    #region Pictures
    private static double CalculateSSIM(string filePath1, string filePath2)
    {
        try
        {
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

    //verifica daca e cropp din imagine
    //private static double CompareImageFiles(string filePath1, string filePath2)
    //{
    //    try
    //    {
    //        // Load the images
    //        using (Image<Bgr, byte> image1 = new Image<Bgr, byte>(filePath1))
    //        using (Image<Bgr, byte> image2 = new Image<Bgr, byte>(filePath2))
    //        {
    //            // Check image dimensions
    //            if (image1.Width == 0 || image1.Height == 0 || image2.Width == 0 || image2.Height == 0)
    //            {
    //                throw new ArgumentException("Invalid image dimensions.");
    //            }

    //            // Determine which image is smaller (the template) and which is larger (the source)
    //            Image<Bgr, byte> sourceImage, templateImage;
    //            if (image1.Width * image1.Height < image2.Width * image2.Height)
    //            {
    //                sourceImage = image2;
    //                templateImage = image1;
    //            }
    //            else
    //            {
    //                sourceImage = image1;
    //                templateImage = image2;
    //            }

    //            // Perform template matching
    //            using (Image<Gray, float> result = sourceImage.MatchTemplate(templateImage, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed))
    //            {
    //                double[] minValues, maxValues;
    //                Point[] minLocations, maxLocations;
    //                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

    //                if (Math.Abs(maxValues[0] - 1) < 0.01)
    //                {
    //                    return 1.0;
    //                }
    //                else
    //                {
    //                    return 0.0;
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        return 0.0;
    //    }
    //}

    private static double CompareImageFiles(string filePath1, string filePath2)
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

    private static double CompareTextFiles(string filePath1, string filePath2)
    {
        string text1 = GetFileContent(filePath1);
        string text2 = GetFileContent(filePath2);

        var diffBuilder = new InlineDiffBuilder(new Differ());
        var diff = diffBuilder.BuildDiffModel(text1, text2);

        int changes = diff.Lines.Count(x => x.Type != ChangeType.Unchanged);
        double totalLines = diff.Lines.Count;

        double similarity = 1.0 - (double)changes / totalLines;

        return similarity;
    }

    public static string GetFileContent(string path)
    {
        string fileContent = "";
        string extension = Path.GetExtension(path).ToLower();

        switch (extension)
        {
            case ".rtf":
                using (var rtBox = new System.Windows.Forms.RichTextBox())
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

