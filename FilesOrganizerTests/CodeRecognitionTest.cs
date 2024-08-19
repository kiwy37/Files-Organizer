using FilesOrganizer.Helpers;
using FilesOrganizer.Models;
using FilesOrganizer.ViewModels.Commands;
using NPOI.SS.Formula.Functions;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace FilesOrganizerTests;

[TestClass]
public class CodeRecognitionTest
{
    string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;

    //from pictures
    [TestMethod]
    public void TestCImages()
    {
        try
        {
            solutionDirectory += @"\CodePics\C";
            Dictionary<string, int> languageFrequency = new Dictionary<string, int>();

            for (int i = 1; i <= 30; i++)
            {
                string path = solutionDirectory;
                Element element = new Element(path, "C" + i.ToString(), "File", new SolidColorBrush(Colors.RoyalBlue), ".png");
                var text = ViewerPageHelper.TextFromPic(element.Path + "\\C" + i.ToString() + ".png");
                var language = ViewerPageHelper.AnalyzeCodeFromString(text, element);

                if (languageFrequency.ContainsKey(language))
                {
                    languageFrequency[language]++;
                }
                else
                {
                    languageFrequency[language] = 1;
                }
            }

            foreach (var kvp in languageFrequency)
            {
                Console.WriteLine("Language: " + kvp.Key + ", Frequency: " + kvp.Value);
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
    [TestMethod]
    public void TestCImagesWithPercentage()
    {
        try
        {
            solutionDirectory = @"C:\Users\Kiwy\Desktop\code pics\C";
            Dictionary<string, int> languageFrequency = new Dictionary<string, int>();
            int totalFiles = 0;
            StringBuilder result = new StringBuilder();

            foreach (var file in Directory.GetFiles(solutionDirectory, "*.*", SearchOption.AllDirectories))
            {
                totalFiles++;
                Element element = new Element(solutionDirectory, Path.GetFileName(file), "File", new SolidColorBrush(Colors.RoyalBlue), Path.GetExtension(file));

                var text = ViewerPageHelper.TextFromPic(element.Path + "\\" + element.Name);
                var language = ViewerPageHelper.AnalyzeCodeFromString(text, element);

                if (languageFrequency.ContainsKey(language))
                {
                    languageFrequency[language]++;
                }
                else
                {
                    languageFrequency[language] = 1;
                }

                result.AppendLine("File: " + element.Name + ", Language: " + language);
            }

            if (languageFrequency.ContainsKey("C"))
            {
                double percentage = (double)languageFrequency["C"] / totalFiles * 100;
                result.AppendLine("Percentage of files matching with C: " + percentage + "%");
            }
            else
            {
                result.AppendLine("No files matching with C found.");
            }

            File.WriteAllText(@"C:\Users\Kiwy\Desktop\code pics\outputC.txt", result.ToString());
        }
        catch (Exception ex)
        {
            // Handle exceptions
            File.WriteAllText(@"C:\Users\Kiwy\Desktop\code pics\error.txt", "An error occurred: " + ex.Message);
        }
    }
    [TestMethod]
    public void TestCSharpImagesWithPercentage()
    {
        try
        {
            solutionDirectory = @"C:\Users\Kiwy\Desktop\code pics\C#";
            Dictionary<string, int> languageFrequency = new Dictionary<string, int>();
            int totalFiles = 0;
            StringBuilder result = new StringBuilder();

            foreach (var file in Directory.GetFiles(solutionDirectory, "*.*", SearchOption.AllDirectories))
            {
                totalFiles++;
                Element element = new Element(solutionDirectory, Path.GetFileName(file), "File", new SolidColorBrush(Colors.RoyalBlue), Path.GetExtension(file));
                var text = ViewerPageHelper.TextFromPic(element.Path + "\\" + element.Name);
                var language = ViewerPageHelper.AnalyzeCodeFromString(text, element);

                if (languageFrequency.ContainsKey(language))
                {
                    languageFrequency[language]++;
                }
                else
                {
                    languageFrequency[language] = 1;
                }

                result.AppendLine("File: " + element.Name + ", Language: " + language);
            }

            if (languageFrequency.ContainsKey("C#"))
            {
                double percentage = (double)languageFrequency["C#"] / totalFiles * 100;
                result.AppendLine("Percentage of files matching with C#: " + percentage + "%");
            }
            else
            {
                result.AppendLine("No files matching with C# found.");
            }

            File.WriteAllText(@"C:\Users\Kiwy\Desktop\code pics\outputC#.txt", result.ToString());
        }
        catch (Exception ex)
        {
            // Handle exceptions
            File.WriteAllText(@"C:\Users\Kiwy\Desktop\code pics\error.txt", "An error occurred: " + ex.Message);
        }
    }

    [TestMethod]
    public void TestCppImagesWithPercentage()
    {
        try
        {
            solutionDirectory = @"C:\Users\Kiwy\Desktop\code pics\C++";
            Dictionary<string, int> languageFrequency = new Dictionary<string, int>();
            int totalFiles = 0;
            StringBuilder result = new StringBuilder();

            foreach (var file in Directory.GetFiles(solutionDirectory, "*.*", SearchOption.AllDirectories))
            {
                totalFiles++;
                Element element = new Element(solutionDirectory, Path.GetFileName(file), "File", new SolidColorBrush(Colors.RoyalBlue), Path.GetExtension(file));
                var text = ViewerPageHelper.TextFromPic(element.Path + "\\" + element.Name);
                var language = ViewerPageHelper.AnalyzeCodeFromString(text, element);

                if (languageFrequency.ContainsKey(language))
                {
                    languageFrequency[language]++;
                }
                else
                {
                    languageFrequency[language] = 1;
                }

                result.AppendLine("File: " + element.Name + ", Language: " + language);
            }

            if (languageFrequency.ContainsKey("C++"))
            {
                double percentage = (double)languageFrequency["C++"] / totalFiles * 100;
                result.AppendLine("Percentage of files matching with C++: " + percentage + "%");
            }
            else
            {
                result.AppendLine("No files matching with C++ found.");
            }

            File.WriteAllText(@"C:\Users\Kiwy\Desktop\code pics\outputC++.txt", result.ToString());
        }
        catch (Exception ex)
        {
            // Handle exceptions
            File.WriteAllText(@"C:\Users\Kiwy\Desktop\code pics\error.txt", "An error occurred: " + ex.Message);
        }
    }


    [TestMethod]
    public void TestJavaImagesWithPercentage()
    {
        try
        {
            solutionDirectory = @"C:\Users\Kiwy\Desktop\code pics\Java";
            Dictionary<string, int> languageFrequency = new Dictionary<string, int>();
            int totalFiles = 0;
            StringBuilder result = new StringBuilder();

            foreach (var file in Directory.GetFiles(solutionDirectory, "*.*", SearchOption.AllDirectories))
            {
                totalFiles++;
                Element element = new Element(solutionDirectory, Path.GetFileName(file), "File", new SolidColorBrush(Colors.RoyalBlue), Path.GetExtension(file));
                var text = ViewerPageHelper.TextFromPic(element.Path + "\\" + element.Name);
                var language = ViewerPageHelper.AnalyzeCodeFromString(text, element);

                if (languageFrequency.ContainsKey(language))
                {
                    languageFrequency[language]++;
                }
                else
                {
                    languageFrequency[language] = 1;
                }

                result.AppendLine("File: " + element.Name + ", Language: " + language);
            }

            if (languageFrequency.ContainsKey("Java"))
            {
                double percentage = (double)languageFrequency["Java"] / totalFiles * 100;
                result.AppendLine("Percentage of files matching with Java: " + percentage + "%");
            }
            else
            {
                result.AppendLine("No files matching with Java found.");
            }

            File.WriteAllText(@"C:\Users\Kiwy\Desktop\code pics\outputJava.txt", result.ToString());
        }
        catch (Exception ex)
        {
            // Handle exceptions
            File.WriteAllText(@"C:\Users\Kiwy\Desktop\code pics\error.txt", "An error occurred: " + ex.Message);
        }
    }

    [TestMethod]
    public void TestPythonImagesWithPercentage()
    {
        try
        {
            solutionDirectory = @"C:\Users\Kiwy\Desktop\code pics\Python";
            Dictionary<string, int> languageFrequency = new Dictionary<string, int>();
            int totalFiles = 0;
            StringBuilder result = new StringBuilder();

            foreach (var file in Directory.GetFiles(solutionDirectory, "*.*", SearchOption.AllDirectories))
            {
                totalFiles++;
                Element element = new Element(solutionDirectory, Path.GetFileName(file), "File", new SolidColorBrush(Colors.RoyalBlue), Path.GetExtension(file));
                var text = ViewerPageHelper.TextFromPic(element.Path + "\\" + element.Name);
                var language = ViewerPageHelper.AnalyzeCodeFromString(text, element);

                if (languageFrequency.ContainsKey(language))
                {
                    languageFrequency[language]++;
                }
                else
                {
                    languageFrequency[language] = 1;
                }

                result.AppendLine("File: " + element.Name + ", Language: " + language);
            }

            if (languageFrequency.ContainsKey("Python"))
            {
                double percentage = (double)languageFrequency["Python"] / totalFiles * 100;
                result.AppendLine("Percentage of files matching with Python: " + percentage + "%");
            }
            else
            {
                result.AppendLine("No files matching with Python found.");
            }

            File.WriteAllText(@"C:\Users\Kiwy\Desktop\code pics\outputPython.txt", result.ToString());
        }
        catch (Exception ex)
        {
            // Handle exceptions
            File.WriteAllText(@"C:\Users\Kiwy\Desktop\code pics\error.txt", "An error occurred: " + ex.Message);
        }
    }

    [TestMethod]
    public void TestHTMLImagesWithPercentage()
    {
        try
        {
            solutionDirectory = @"C:\Users\Kiwy\Desktop\code pics\HTML";
            Dictionary<string, int> languageFrequency = new Dictionary<string, int>();
            int totalFiles = 0;
            StringBuilder result = new StringBuilder();

            foreach (var file in Directory.GetFiles(solutionDirectory, "*.*", SearchOption.AllDirectories))
            {
                totalFiles++;
                Element element = new Element(solutionDirectory, Path.GetFileName(file), "File", new SolidColorBrush(Colors.RoyalBlue), Path.GetExtension(file));
                var text = ViewerPageHelper.TextFromPic(element.Path + "\\" + element.Name);
                var language = ViewerPageHelper.AnalyzeCodeFromString(text, element);

                if (languageFrequency.ContainsKey(language))
                {
                    languageFrequency[language]++;
                }
                else
                {
                    languageFrequency[language] = 1;
                }

                result.AppendLine("File: " + element.Name + ", Language: " + language);
            }

            if (languageFrequency.ContainsKey("HTML"))
            {
                double percentage = (double)languageFrequency["HTML"] / totalFiles * 100;
                result.AppendLine("Percentage of files matching with HTML: " + percentage + "%");
            }
            else
            {
                result.AppendLine("No files matching with HTML found.");
            }

            File.WriteAllText(@"C:\Users\Kiwy\Desktop\code pics\outputHTML.txt", result.ToString());
        }
        catch (Exception ex)
        {
            // Handle exceptions
            File.WriteAllText(@"C:\Users\Kiwy\Desktop\code pics\error.txt", "An error occurred: " + ex.Message);
        }
    }
}
