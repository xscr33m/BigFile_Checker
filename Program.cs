using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "xscr33m's BigFile Checker for GitHub-Repos v0.1";

        string coloredTextStart = $"================================================= \n" +
                                  $"||                                            || \n" +
                                  $"|| xscr33m's BigFile Checker for GitHub-Repos || \n" +
                                  $"||                                            || \n" +
                                  $"=================================================";
        string coloredTextExit = $"< Press any key to exit the program >";

        WriteColoredConsoleText(coloredTextStart);

        WriteColoredConsoleText($"{DateTime.Now}: [INFO] < Initializing.... > \n", ConsoleColor.White);

        if (args.Length == 0)
        {
            WriteColoredConsoleText($"{DateTime.Now}: [ERROR] Please drag the folder path to the executable file. \n", ConsoleColor.Red);

            WriteColoredConsoleText(coloredTextExit);
            Console.ReadKey();
            return;
        }

        string folderPath = args[0];

        if (!Directory.Exists(folderPath))
        {
            WriteColoredConsoleText($"{DateTime.Now}: [ERROR] The specified folder path does not exist. \n", ConsoleColor.Red);

            WriteColoredConsoleText(coloredTextExit);
            Console.ReadKey();
            return;
        }

        string gitIgnoreFilePath = Path.Combine(folderPath, ".gitignore");
        string logFilePath = Path.Combine(folderPath, "xscr33ms_BigFile_Checker_Log.txt");

        WriteColoredConsoleText($"{DateTime.Now}: [INFO] Scanning files larger than 100 MB in the following folder: \n{folderPath}.... \n", ConsoleColor.White);

        List<string> largeFiles = GetLargeFiles(folderPath, 100 * 1024 * 1024);
        List<string> mediumFiles = GetLargeFiles(folderPath, 50 * 1024 * 1024);

        if (largeFiles.Count == 0 || mediumFiles.Count == 0)
        {
            WriteColoredConsoleText($"################################################", ConsoleColor.Red);
            WriteColoredConsoleText($"Files found larger than 100 MB → COUNT:", ConsoleColor.White);
            WriteColoredConsoleText($" == {largeFiles.Count} == \n", ConsoleColor.Red);
            WriteColoredConsoleText($"Files found larger than 50 MB → COUNT:", ConsoleColor.White);
            WriteColoredConsoleText($" == {mediumFiles.Count} == \n", ConsoleColor.Red);

            WriteColoredConsoleText($"[INFO] No files bigger than 100 or 50 MB were found! \n" +
                                    $"No .gitignore were touched or created! \n" +
                                    $"If you want to ignore files from your repo, create a .gitignore manually!", ConsoleColor.Green);
            WriteColoredConsoleText($"################################################ \n", ConsoleColor.Red);

            WriteColoredConsoleText(coloredTextExit);
            Console.ReadKey();
            return;
        }
        else
        {
            WriteColoredConsoleText($"################################################", ConsoleColor.Red);
            WriteColoredConsoleText($"Files found larger than 100 MB → COUNT:", ConsoleColor.White);
            WriteColoredConsoleText($" == {largeFiles.Count} == \n", ConsoleColor.Red);
            WriteColoredConsoleText($"Files found larger than 50 MB → COUNT:", ConsoleColor.White);
            WriteColoredConsoleText($" == {mediumFiles.Count} == \n", ConsoleColor.Red);

            WriteColoredConsoleText($"[IMPORTANT] Found files gonna be added to your existing .gitignore, regardless if already existing. \n" +
                                    $"I highly suggest to only run the code once or update your template-gitignore with it. \n" +
                                    $"Make sure to validate your .gitignore file created here:", ConsoleColor.DarkYellow);
            WriteColoredConsoleText($"{gitIgnoreFilePath} \n", ConsoleColor.White);
            WriteColoredConsoleText($"[INFO] You find all files >50 MB that have been found in the logfile created here:", ConsoleColor.Green);
            WriteColoredConsoleText($"{logFilePath}", ConsoleColor.White);
            WriteColoredConsoleText($"################################################ \n", ConsoleColor.Red);

            UpdateGitIgnoreFile(gitIgnoreFilePath, largeFiles);
            CreateLogFile(logFilePath, largeFiles, mediumFiles);

            WriteColoredConsoleText($"[INFO] < Finishing.... > \n", ConsoleColor.White);

            WriteColoredConsoleText(coloredTextExit);
            Console.ReadKey();
        }

    }

    static List<string> GetLargeFiles(string folderPath, long sizeLimit)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

        return directoryInfo.GetFiles("*.*", SearchOption.AllDirectories)
            .Where(file => file.Length > sizeLimit)
            .Select(file => file.FullName)
            .ToList();
    }

    static void UpdateGitIgnoreFile(string gitIgnoreFilePath, List<string> files)
    {
        WriteColoredConsoleText($"{DateTime.Now}: [INFO] Updating .gitignore file....", ConsoleColor.White);

        StringBuilder gitIgnoreContent = new StringBuilder();

        if (File.Exists(gitIgnoreFilePath))
        {
            gitIgnoreContent.Append(File.ReadAllText(gitIgnoreFilePath));
            gitIgnoreContent.AppendLine();
        }

        gitIgnoreContent.AppendLine("# xscr33m's BigFile Checker for GitHub-Repos");
        gitIgnoreContent.AppendLine("# Found files bigger than 100 MB:");

        foreach (string file in files)
        {
            string relativePath = GetRelativePath(gitIgnoreFilePath, file);
            gitIgnoreContent.AppendLine(relativePath);
        }

        gitIgnoreContent.AppendLine($"{Environment.NewLine}# xscr33m's BigFile Checker Log-File:");
        gitIgnoreContent.AppendLine("xscr33ms_BigFile_Checker_Log.txt");

        // Save the file in UTF-8 format with BOM
        File.WriteAllText(gitIgnoreFilePath, gitIgnoreContent.ToString(), Encoding.UTF8);

        WriteColoredConsoleText($"{DateTime.Now}: [INFO] .gitignore file successfully updated. \n", ConsoleColor.Green);
    }

    /*************************************
     *    .gitignore examples:           *
     * FOLDER_Name  =  specific folder   *
     * File.txt     =  specific filename *
     ************************************/
    static string GetRelativePath(string basePath, string fullPath)
    {
        Uri baseUri = new Uri(basePath);
        Uri fullUri = new Uri(fullPath);

        return Uri.UnescapeDataString("/" + baseUri.MakeRelativeUri(fullUri).ToString().Replace('\\', Path.DirectorySeparatorChar));
    }

    static void CreateLogFile(string logFilePath, List<string> files, List<string> files1)
    {
        WriteColoredConsoleText($"{DateTime.Now}: [INFO] Creating log file...", ConsoleColor.White);

        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            if (logFilePath != null)
            {
                writer.WriteLine("================================================");
                writer.WriteLine("||                                            ||");
                writer.WriteLine("|| xscr33m's BigFile Checker for GitHub-Repos ||");
                writer.WriteLine("||                                            ||");
                writer.WriteLine("================================================");

                writer.WriteLine($"{DateTime.Now}: [INFO] Found files >100 MB:", ConsoleColor.White);
                foreach (string file in files)
                {
                    writer.WriteLine($"{file} - {GetSizeString(new FileInfo(file).Length)}", ConsoleColor.White);
                }

                writer.WriteLine($"\n{DateTime.Now}: [INFO] Found files >50 MB:", ConsoleColor.White);
                foreach (string file in files1)
                {
                    writer.WriteLine($"{file} - {GetSizeString(new FileInfo(file).Length)}", ConsoleColor.White);
                }

                writer.WriteLine();
            }
        }

        WriteColoredConsoleText($"{DateTime.Now}: [INFO] Log file successfully created. \n", ConsoleColor.Green);
    }

    static string GetSizeString(long sizeInBytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = sizeInBytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    private static void WriteColoredConsoleText(string coloredText, ConsoleColor? color = null)
    {
        if (color.HasValue)
        {
            Console.ForegroundColor = color.Value;
            foreach (char character in coloredText)
            {
                Console.Write(character);
            }

            Console.ResetColor();
            Console.WriteLine();
        }
        else
        {
            Random random = new Random();
            for (int i = 0; i < coloredText.Length; i++)
            {
                ConsoleColor randomColor = (ConsoleColor)random.Next(3, 7);

                Console.ForegroundColor = randomColor;

                Console.Write(coloredText[i]);

                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }
}
