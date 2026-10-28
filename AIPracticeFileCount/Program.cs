string? inputPath = args.Length > 0
    ? args[0]
    : ReadDirectoryPath();

if (string.IsNullOrWhiteSpace(inputPath))
{
    Console.WriteLine("A directory path is required.");
    return;
}

string directoryPath;

try
{
    directoryPath = Path.GetFullPath(inputPath.Trim());
}
catch (ArgumentException)
{
    Console.WriteLine($"The path is invalid: {inputPath}");
    return;
}

if (!Directory.Exists(directoryPath))
{
    Console.WriteLine($"The directory does not exist: {directoryPath}");
    return;
}

var filesByFolder = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
var inaccessibleFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
int totalFiles = 0;
int totalFolders = 0;

ScanDirectory(directoryPath);

Console.WriteLine();
Console.WriteLine($"Total files: {totalFiles}");
Console.WriteLine($"Total folders: {totalFolders}");
Console.WriteLine();
Console.WriteLine("Files by folder:");

foreach ((string folder, List<string> files) in filesByFolder.OrderBy(entry => entry.Key))
{
    Console.WriteLine($"{folder}: {files.Count} file(s)");
}

if (inaccessibleFolders.Count > 0)
{
    Console.WriteLine();
    Console.WriteLine("Folders that could not be accessed:");

    foreach (string folder in inaccessibleFolders.OrderBy(folder => folder))
    {
        Console.WriteLine(folder);
    }
}

string? ReadDirectoryPath()
{
    Console.Write("Enter a directory path: ");
    return Console.ReadLine();
}

void ScanDirectory(string currentDirectory)
{
    filesByFolder.TryAdd(currentDirectory, []);

    try
    {
        foreach (string file in Directory.EnumerateFiles(currentDirectory))
        {
            filesByFolder[currentDirectory].Add(file);
            totalFiles++;
        }
    }
    catch (UnauthorizedAccessException)
    {
        inaccessibleFolders.Add(currentDirectory);
    }
    catch (IOException)
    {
        inaccessibleFolders.Add(currentDirectory);
    }

    try
    {
        foreach (string childDirectory in Directory.EnumerateDirectories(currentDirectory))
        {
            totalFolders++;
            ScanDirectory(childDirectory);
        }
    }
    catch (UnauthorizedAccessException)
    {
        inaccessibleFolders.Add(currentDirectory);
    }
    catch (IOException)
    {
        inaccessibleFolders.Add(currentDirectory);
    }
}
