using FilesOrganizer.Helpers;
using FilesOrganizer.Models;
using FilesOrganizer.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FilesOrganizerTests;

[TestClass]
public class IntegrationTests
{
    string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;

    [TestMethod]
    public void TestTextExtractionFromPhoto()
    {
        // Arrange
        string imagePath = Path.Combine(solutionDirectory, "CodePics\\imageWithText.jpg");
        string expectedText = " \r\n\r\n \r\n\r\nIt was the best of\r\ntimes, it was the worst\r\nof times, it was the age\r\nof wisdom, it was the\r\nage of foolishness...\r\n";

        // Remove all spaces and empty lines from expectedText
        string normalizedExpectedText = new string(expectedText.Where(c => !char.IsWhiteSpace(c)).ToArray());

        // Act
        string extractedText = ViewerPageHelper.TextFromPic(imagePath);

        // Remove all spaces and empty lines from extractedText
        string normalizedExtractedText = new string(extractedText.Where(c => !char.IsWhiteSpace(c)).ToArray());

        // Assert
        Assert.AreEqual(normalizedExpectedText, normalizedExtractedText, "The extracted text does not match the expected text after normalization.");
    }

    [TestMethod]
    public void TestDetectLanguage()
    {
        var testCases = new (string, string, string)[]
        {
            ("This is a language check test.", "t", "English"),
            ("Esta es una prueba de verificación de idioma.", "t", "Spanish"),
            ("Il s'agit d'un test de vérification linguistique.", "t", "French"),
        };

        CollectionAssert.AreEqual(
            testCases.Select(t => t.Item3).ToArray(),
            testCases.Select(t => ViewerPageHelper.LanguageAssociation(ViewerPageHelper.DetectLanguage(t.Item1, t.Item2))).ToArray()
        );
    }

    [TestMethod]
    public void TestFileUploadToGoogleDrive()
    {
        // Arrange
        var hd = new HelperDrive();
        string customFilePath = Path.Combine(solutionDirectory, "SSIMPhotos\\floare1.jpg");
        string tempFileName = "Uploaded Image Test.jpg";
        var parentFolderId = "1KS8hiuIUr7Bp3j7U5EH9HQDKELZw5A1c";

        // Act
        string uploadedFileId = HelperDrive.UploadFile(customFilePath, tempFileName, parentFolderId);

        try
        {
            // Additional verification: Check if the file exists on Google Drive
            var file = HelperDrive.Service.Files.Get(uploadedFileId).Execute();
            Assert.IsNotNull(file, "The file does not exist on Google Drive.");
            Assert.AreEqual(tempFileName, file.Name, "The uploaded file name does not match.");

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(uploadedFileId), "File upload failed or returned file ID is null/empty.");
        }
        finally
        {
            // Cleanup: Delete the uploaded file after the test
            if (!string.IsNullOrEmpty(uploadedFileId))
            {
                HelperDrive.DeleteFile(uploadedFileId);
            }
        }
    }


    [TestMethod]
    public void TestFileDownloadFromGoogleDrive()
    {
        // Arrange
        var hd = new HelperDrive();
        string fileId = "1y8d-fLs0UaFGG44gEhF6wVHoyaDt8XLK"; // Specify the file ID to download
        string downloadDirectory = Path.Combine(solutionDirectory, "DownloadedFiles");
        string expectedDownloadPath = Path.Combine(downloadDirectory, "Cerinte Proiect IS.pdf"); // Adjust based on the file you're downloading

        // Ensure the download directory exists
        if (!Directory.Exists(downloadDirectory))
        {
            Directory.CreateDirectory(downloadDirectory);
        }

        // Act
        HelperDrive.DownloadFile(fileId, downloadDirectory);

        // Assert
        Assert.IsTrue(File.Exists(expectedDownloadPath), "The file was not downloaded to the expected location.");

        // Additional verification: Check the file size, content, etc.
        // For example, to check the file size is not zero (file is not empty):
        var fileInfo = new FileInfo(expectedDownloadPath);
        Assert.IsTrue(fileInfo.Length > 0, "The downloaded file is empty.");

        // Cleanup: Optionally delete the downloaded file after the test
        File.Delete(expectedDownloadPath);
    }

}
