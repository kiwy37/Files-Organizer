using FilesOrganizer.Helpers;
using FilesOrganizer.Models;
using FilesOrganizer.ViewModels.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FilesOrganizerTests;

[TestClass]
public class UnitarTestins
{
    string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;

    [TestMethod]
    public void TestFilteringItems1()
    {
        var viewerPageVM = new ViewerPageVM();
        var path = "C:\\Users\\Kiwy\\Desktop\\poze\\cinci sute";

        ViewerPageHelper.LoadItemsAndUpdatePath(viewerPageVM, path);

        for (int i = 0; i < viewerPageVM.CurrentData.AllItems.Count; i++)
        {
            if (i % 2 == 0)
            {
                viewerPageVM.CurrentData.AllItems[i].Priority = "High";
                viewerPageVM.CurrentData.AllItems[i].CodeLanguage = "C#";
            }
            else
            {
                viewerPageVM.CurrentData.AllItems[i].Priority = "Low";
                viewerPageVM.CurrentData.AllItems[i].CodeLanguage = "Python";
            }
        }

        viewerPageVM.SelectedPriority = "High";
        viewerPageVM.SelectedCodeLanguage = "C#";

        ViewerPageHelper.FilterItems(viewerPageVM);

        Assert.IsTrue(viewerPageVM.CurrentData.AllItems.Count / 2 == viewerPageVM.CurrentData.CurrentListBoxSource.Count);
    }


    [TestMethod]
    public void TestFilteringItems2()
    {
        var viewerPageVM = new ViewerPageVM();
        var path = "C:\\Users\\Kiwy\\Desktop\\poze\\cinci sute";

        ViewerPageHelper.LoadItemsAndUpdatePath(viewerPageVM, path);

        for (int i = 0; i < viewerPageVM.CurrentData.AllItems.Count; i++)
        {
            if (i % 2 == 0)
            {
                viewerPageVM.CurrentData.AllItems[i].Priority = "High";
                viewerPageVM.CurrentData.AllItems[i].CodeLanguage = "C#";
            }
            else
            {
                viewerPageVM.CurrentData.AllItems[i].Priority = "Low";
                viewerPageVM.CurrentData.AllItems[i].CodeLanguage = "Python";
            }
        }

        viewerPageVM.SelectedPriority = "Low";
        viewerPageVM.SelectedCodeLanguage = "Python";

        ViewerPageHelper.FilterItems(viewerPageVM);

        Assert.IsTrue(viewerPageVM.CurrentData.AllItems.Count / 2 == viewerPageVM.CurrentData.CurrentListBoxSource.Count);
    }

    [TestMethod]
    public void TestFilteringItems3()
    {
        var viewerPageVM = new ViewerPageVM();
        var path = "C:\\Users\\Kiwy\\Desktop\\poze\\cinci sute";

        ViewerPageHelper.LoadItemsAndUpdatePath(viewerPageVM, path);

        for (int i = 0; i < viewerPageVM.CurrentData.AllItems.Count; i++)
        {
            if (i % 2 == 0)
            {
                viewerPageVM.CurrentData.AllItems[i].Priority = "High";
                viewerPageVM.CurrentData.AllItems[i].CodeLanguage = "C#";
                viewerPageVM.CurrentData.AllItems[i].Language = "English";
            }
        }

        viewerPageVM.SelectedPriority = "High";
        viewerPageVM.SelectedCodeLanguage = "C#";

        ViewerPageHelper.FilterItems(viewerPageVM);

        Assert.IsTrue(viewerPageVM.CurrentData.AllItems.Count / 2 == viewerPageVM.CurrentData.CurrentListBoxSource.Count);
    }


    [TestMethod]
    public void SaveData_ValidData_SavesCorrectly()
    {
        var viewerPageVM = new ViewerPageVM();
        var TestDirectory = "C:\\Users\\Kiwy\\Desktop\\poze\\cinci sute";

        ViewerPageHelper.LoadItemsAndUpdatePath(viewerPageVM, TestDirectory);

        // Act
        ViewerPageHelper.SaveData(viewerPageVM);

        // Assert
        string expectedFileName = "cinci sute.json";
        string expectedFilePath = Path.Combine(solutionDirectory, "Saves", expectedFileName);
        Assert.IsTrue(File.Exists(expectedFilePath), "The expected JSON file was not created.");

        // Verify the content of the saved file
        string savedContent = File.ReadAllText(expectedFilePath);
        var savedData = JsonConvert.DeserializeObject<FilesOrganizer.Models.TransmittedData>(savedContent);
        Assert.IsNotNull(savedData, "Failed to deserialize the saved JSON content.");
        Assert.AreEqual(viewerPageVM.CurrentData.SpacePath, savedData.SpacePath, "The saved data does not match the original data.");

        // Verify the saves.txt file
        string savesFilePath = Path.Combine(solutionDirectory, "Saves", "saves.txt");
        Assert.IsTrue(File.Exists(savesFilePath), "'saves.txt' file was not created.");

        var allSaves = File.ReadAllLines(savesFilePath).Select(line => line.Trim()).ToList();
        string expectedSaveEntry = Path.GetFileNameWithoutExtension(expectedFileName);
        Assert.IsTrue(allSaves.Contains(expectedSaveEntry), "The 'saves.txt' file does not contain the expected content.");
    }
}

