using FilesOrganizer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOrganizerTests;

[TestClass]
public class SSIMPhotosGroupingTest
{
    string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;

    [TestMethod]
    public void TestCalculateSSIM_SameImages()
    {
        solutionDirectory = Path.Combine(solutionDirectory, "SSIMPhotos");
        string filePath1 = solutionDirectory + @"\floare1.jpg";
        string filePath2 = solutionDirectory + @"\floare1.jpg";
        double expectedSSIM = 1.0; 

        double actualSSIM = HelperSimilarFiles.CalculateSSIM(filePath1, filePath2);

        Assert.AreEqual(expectedSSIM, actualSSIM);
    }

    [TestMethod]
    public void TestCalculateSSIM_DifferentImages()
    {
        solutionDirectory = Path.Combine(solutionDirectory, "SSIMPhotos");
        string filePath1 = solutionDirectory + @"\floare.jpg";
        string filePath2 = solutionDirectory + @"\school.jpg";
        double expectedSSIM = 0.0; // Completely different images should have SSIM 0.0

        double actualSSIM = HelperSimilarFiles.CalculateSSIM(filePath1, filePath2);

        Assert.AreEqual(expectedSSIM, actualSSIM);
    }


    [TestMethod]
    public void TestCalculateSSIM_SimilarImages()
    {
        // Arrange
        solutionDirectory = Path.Combine(solutionDirectory, "SSIMPhotos");
        string filePath1 = solutionDirectory + @"\floare.jpg";
        string filePath2 = solutionDirectory + @"\floare1.jpg";
        double similarityThreshold = 0.2;

        // Act
        double actualSSIM = HelperSimilarFiles.CalculateSSIM(filePath1, filePath2);

        // Assert
        Assert.IsTrue(actualSSIM >= similarityThreshold);
    }

    [TestMethod]
    public void TestCalculateSSIM_DifferentSizeImages()
    {
        // Arrange
        solutionDirectory = Path.Combine(solutionDirectory, "SSIMPhotos");
        string filePath1 = solutionDirectory + @"\floare1.jpg";
        string filePath2 = solutionDirectory + @"\school1.jpg";

        // Act
        double actualSSIM = HelperSimilarFiles.CalculateSSIM(filePath1, filePath2);

        Assert.IsTrue(actualSSIM >= 0.0 && actualSSIM <= 1.0);
    }
}
