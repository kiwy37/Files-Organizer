using FilesOrganizer.Helpers;
using FilesOrganizer.Models;
using FilesOrganizer.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FilesOrganizerTests;

[TestClass]
public class ExceptionsTests
{
    [TestMethod]
    public void TestCalculateSSIM_ReturnsZero_WhenImageFilesDoNotExist()
    {
        // Arrange
        var helper = new HelperSimilarFiles();
        var filePath1 = @"path\to\nonexistent_image1.png";
        var filePath2 = @"path\to\nonexistent_image2.png";

        // Act
        var result = HelperSimilarFiles.CalculateSSIM(filePath1, filePath2);

        // Assert
        Assert.AreEqual(0.0, result);
    }

    [TestMethod]
    public void TestCalculateSSIM_ReturnsZero_WhenImagesHaveDifferentSizes()
    {
        // Arrange
        var helper = new HelperSimilarFiles();
        var filePath1 = @"path\to\image1.png"; // This image should have a different size than image2
        var filePath2 = @"path\to\image2.png"; // This image should have a different size than image1

        // Act
        var result = HelperSimilarFiles.CalculateSSIM(filePath1, filePath2);

        // Assert
        Assert.AreEqual(0.0, result);
    }

    [TestMethod]
    public void TestCalculateCroppedArea_ThrowsException_WhenImagesDoNotExist()
    {
        // Arrange
        var filePath1 = @"path\to\nonexistent_image1.png";
        var filePath2 = @"path\to\nonexistent_image2.png";

        // Act
        var result = HelperSimilarFiles.CalculateCroppedArea(filePath1, filePath2);

        Assert.AreEqual(0.0, result);
    }

    [TestMethod]
    public void TestGetFileContent_ReturnsEmptyString_WhenFileDoesNotExist()
    {
        // Arrange
        var filePath = @"path\to\nonexistent_file.txt";

        // Act
        var result = HelperSimilarFiles.GetFileContent(filePath);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void TestOpenFile_DoesNotThrowException_WhenFileDoesNotExist()
    {
        // Arrange
        var filePath = @"path\to\nonexistent_file.txt";

        // Act and Assert
        ViewerPageHelper.OpenFile(filePath);
    }

    [TestMethod]
    public void TestConvertItems_DoesNotThrowException_WhenItemsDoNotExist()
    {
        // Arrange
        var helper = new ConvertItemsHelper();
        var selectedItems = new List<string> { @"path\to\nonexistent_item1", @"path\to\nonexistent_item2" };
        var extension = ".txt";
        var viewerPageVM = new ViewerPageVM();
        var initPath = @"path\to\initPath";
        var endPath = @"path\to\endPath";
        var driveInitPath = @"path\to\driveInitPath";
        var driveEndPath = @"path\to\driveEndPath";

        // Act and Assert
        helper.ConvertItems(selectedItems, extension, viewerPageVM, initPath, endPath, driveInitPath, driveEndPath);
    }

    [TestMethod]
    public void TestClearFolder_RemovesAllFilesAndDirectories()
    {
        // Arrange
        var helperDrive = new HelperDrive();
        string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
        solutionDirectory = Path.Combine(solutionDirectory, "DriveDownloads");

        // Create some test files and directories in the test directory
        Directory.CreateDirectory(solutionDirectory);
        File.Create(Path.Combine(solutionDirectory, "testFile1.txt")).Dispose();
        File.Create(Path.Combine(solutionDirectory, "testFile2.txt")).Dispose();
        Directory.CreateDirectory(Path.Combine(solutionDirectory, "testDirectory"));

        // Act
        HelperDrive.ClearFolder();

        // Assert
        Assert.IsFalse(Directory.EnumerateFileSystemEntries(solutionDirectory).Any(), "Expected the directory to be empty.");
    }

    [TestMethod]
    public void TestLoadFilesFromGoogleDrive_Succeeds()
    {
        var helperDrive = new HelperDrive();
        var viewerPageVM = new ViewerPageVM();

        try
        {
            HelperDrive.LoadFilesFromGoogleDrive(viewerPageVM);
            Assert.IsTrue(true);
        }
        catch (Exception)
        {
            Assert.Fail("Couldn't connect to Google Drive.");
        }
    }

    [TestMethod]
    public void TestGetFileContent_ReturnsEmptyString_WhenFileExtensionIsIncorrect()
    {
        // Arrange
        var filePath = @"path\to\file_with_incorrect_extension.xyz";

        // Act
        var result = HelperSimilarFiles.GetFileContent(filePath);

        // Assert
        Assert.AreEqual(string.Empty, result, "Expected an empty string for a file with an incorrect extension.");
    }

    [TestMethod]
    public void TestDetectLanguage_ReturnsUnknown_WhenFileDoesNotExistOrUnsupportedExtension()
    {
        // Arrange
        var filePath = @"path\to\nonexistent_or_unsupported_file.xyz";

        // Act
        var result = ViewerPageHelper.DetectLanguage(filePath, string.Empty);

        // Assert
        Assert.AreEqual(string.Empty, result, "Expected an empty string for a file with an incorrect extension.");
    }
}
