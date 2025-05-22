using System.Text;
using System.Text.Json;

namespace HeaderFileGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Configuration config = JsonSerializer.Deserialize<Configuration>(File.ReadAllText("Configuration.json"));

            string solutionName = Path.GetFileName(config.Solution);
            foreach (string project in config.Projects) 
            {
                string directory = $"{config.Solution}\\{project}\\{config.SourceFolder}";
                string outputPath = $"{config.Solution}\\{project}\\{config.OutputFolder}\\{solutionName}\\{project}.hpp";

                if (!Directory.Exists(directory)) 
                {
                    Console.WriteLine($"{project}: directory does not exist");
                    continue;
                }

                if (!File.Exists(outputPath))
                {
                    Console.WriteLine($"{project}: file does not exist");
                    continue;
                }

                WriteHeaderFile(directory, outputPath, project);
            }

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        private static void WriteHeaderFile(string directory, string outputPath, string projectName) 
        {
            string absoluteOutputPath = Path.GetFullPath(outputPath);
            string absoluteDirectoryPath = Path.GetFullPath(directory);

            try
            {
                var headerFiles = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)
                    .Where(file => file.EndsWith(".h") || file.EndsWith(".hpp"))
                    .Select(file => Path.GetFullPath(file))
                    .Where(file => file != absoluteOutputPath)
                    .OrderBy(file => file);

                StringBuilder content = new();
                content.AppendLine("#pragma once");
                content.AppendLine();

                foreach (var file in headerFiles)
                {
                    string? outputDir = Path.GetDirectoryName(absoluteOutputPath);
                    if (outputDir is null) continue;

                    string? relPath = Path.GetRelativePath(outputDir, file);
                    if (relPath is null) continue;

                    relPath = relPath.Replace('\\', '/');
                    content.AppendLine($"#include \"{relPath}\"");
                }

                File.WriteAllText(outputPath, content.ToString());

                Console.WriteLine($"{projectName}: Successfully generated {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{projectName}: error: {ex.Message}");
            }
        }
    }
}
