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
public class PerformanceTests
{
    [TestMethod]
    public void TestLoadItemsAndUpdatePathPerformance()
    {
        var viewerPageVM = new ViewerPageVM();
        var testCases = new List<string>
        {
            "C:\\Users\\Kiwy\\Desktop\\poze\\cinci sute",
            //"C:\\Users\\Kiwy\\Desktop\\poze\\O mie",
            //"C:\\Users\\Kiwy\\Desktop\\poze\\o mie cincisute",
            //"C:\\Users\\Kiwy\\Desktop\\poze\\doua mii",
            //"C:\\Users\\Kiwy\\Desktop\\poze\\doua mii cincisute",
            //"C:\\Users\\Kiwy\\Desktop\\poze\\trei mii"
        };

        foreach (var selectedPath in testCases)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            ViewerPageHelper.LoadItemsAndUpdatePath(viewerPageVM, selectedPath);

            stopwatch.Stop();

            // Display the elapsed time in a console
            Console.WriteLine($"Time for path '{selectedPath}': {stopwatch.ElapsedMilliseconds} ms");
        }
    }


    [TestMethod]
    public void TestSaveDataPerformance()
    {
        var testCases = new List<string>
    {
        "C:\\Users\\Kiwy\\Desktop\\poze\\cinci sute",
        "C:\\Users\\Kiwy\\Desktop\\poze\\O mie",
        "C:\\Users\\Kiwy\\Desktop\\poze\\o mie cincisute",
        "C:\\Users\\Kiwy\\Desktop\\poze\\doua mii",
        "C:\\Users\\Kiwy\\Desktop\\poze\\doua mii cincisute",
        "C:\\Users\\Kiwy\\Desktop\\poze\\trei mii"
    };

        foreach (var selectedPath in testCases)
        {
            var viewerPageVM = new ViewerPageVM();
            ViewerPageHelper.LoadItemsAndUpdatePath(viewerPageVM, selectedPath);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            ViewerPageHelper.SaveData(viewerPageVM);

            stopwatch.Stop();

            // Display the elapsed time in a console
            Console.WriteLine($"Time for path '{selectedPath}': {stopwatch.ElapsedMilliseconds} ms");

            // Assert
            //Assert.IsTrue(stopwatch.ElapsedMilliseconds < 2000);
        }
    }

}
