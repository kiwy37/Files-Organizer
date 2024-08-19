using FilesOrganizer.Helpers;
using Org.BouncyCastle.Crypto.Modes.Gcm;

namespace FilesOrganizerTests;

[TestClass]
public class CroppedPhotosGroupingTest
{
    string solutionDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;

    [TestMethod]
    public void TestCompareImageCutInHalf()
    {
        solutionDirectory = Path.Combine(solutionDirectory, "CroppedPhotos");

        string filePath1 = solutionDirectory + @"\floare1.jpg";
        string filePath2 = solutionDirectory + @"\floare11.jpg";
        double expectedSimilarity = 0.5; // This is a hypothetical expected value. Replace with actual expected value.

        double actualSimilarity = HelperSimilarFiles.CalculateCroppedArea(filePath1, filePath2);

        Assert.AreEqual(expectedSimilarity, actualSimilarity);
    }
    [TestMethod]
    public void TestCompareImageCutInThird()
    {
        solutionDirectory = Path.Combine(solutionDirectory, "CroppedPhotos");

        string filePath1 = solutionDirectory + @"\cartoon1.jpg";
        string filePath2 = solutionDirectory + @"\cartoon11.jpg";
        double expectedSimilarity = 0.333; 

        double actualSimilarity = HelperSimilarFiles.CalculateCroppedArea(filePath1, filePath2);
        actualSimilarity = Math.Round(actualSimilarity, 3);

        Assert.AreEqual(expectedSimilarity, actualSimilarity);
    }
    [TestMethod]
    public void TestCompareImageFilesSameImages()
    {
        // Arrange
        solutionDirectory = Path.Combine(solutionDirectory, "CroppedPhotos");
        string filePath1 = solutionDirectory + @"\floare1.jpg";
        string filePath2 = solutionDirectory + @"\floare1.jpg";
        double expectedSimilarity = 1.0; 

        double actualSimilarity = HelperSimilarFiles.CalculateCroppedArea(filePath1, filePath2);

        Assert.AreEqual(expectedSimilarity, actualSimilarity);
    }

    [TestMethod]
    public void TestCompareImageFilesDifferentImages()
    {
        solutionDirectory = Path.Combine(solutionDirectory, "CroppedPhotos");
        string filePath1 = solutionDirectory + @"\floare1.jpg";
        string filePath2 = solutionDirectory + @"\desktop1.jpg";
        double expectedSimilarity = 0.0;

        double actualSimilarity = HelperSimilarFiles.CalculateCroppedArea(filePath1, filePath2);

        Assert.AreEqual(expectedSimilarity, actualSimilarity);
    }
}