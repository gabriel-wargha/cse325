using Newtonsoft.Json;
using System.IO.Enumeration;
using System.Text;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

GenerateSalesSummaryReport(salesFiles, salesTotalDir);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);

        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

void GenerateSalesSummaryReport(IEnumerable<string> salesFiles, string outputDirectory)
{
    var report = new StringBuilder();

    report.AppendLine("Sales Summary");
    report.AppendLine("--------------------------");
    var Total = CalculateSalesTotal(salesFiles);
    report.AppendLine($" Total Sales: {Total:C}");
    report.AppendLine("Details: ");
    foreach (var file in salesFiles)
    {
        var FileName = Path.GetFileName(file);
        if (FileName == "sales.json")
        {
            var fileText = File.ReadAllText(file);
            SalesData? data = JsonConvert.DeserializeObject<SalesData>(fileText);


            report.AppendLine($"{FileName}: {data?.Total:C}");
        }

    }



    File.WriteAllText(Path.Combine(outputDirectory, "salesSummary.txt"), report.ToString());
}

record SalesData(double Total);