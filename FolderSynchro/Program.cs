using FolderSynchro;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: FolderSync <source_path> <replica_path> <interval_seconds> <log_file_path>");
            return;
        }

        string sourcePath = args[0];
        string replicaPath = args[1];
        if (!int.TryParse(args[2], out int intervalSeconds) || intervalSeconds <= 0)
        {
            Console.WriteLine("Error: interval_seconds must be a positive integer.");
            return;
        }
        string logPath = args[3];

        string configPath = "syncconfig.json";
        SyncConfig config = SyncConfig.Load(configPath);

        Logger logger = new Logger(logPath);
        FolderSynchronizer syncer = new FolderSynchronizer(sourcePath, replicaPath, logger, config);

        logger.Log("Starting folder synchronization...");
        if (config.ExcludeFiles.Count > 0 || config.ExcludeDirectories.Count > 0)
        {
            logger.Log("Exclusions loaded from config:");
            if (config.ExcludeFiles.Count > 0)
                logger.Log("  Excluded files: " + string.Join(", ", config.ExcludeFiles));
            if (config.ExcludeDirectories.Count > 0)
                logger.Log("  Excluded directories: " + string.Join(", ", config.ExcludeDirectories));
        }

        while (true)
        {
            try
            {
                await syncer.SynchronizeAsync();
                logger.Log("Synchronization completed.");
            }
            catch (Exception ex)
            {
                logger.Log($"Error during synchronization: {ex.Message}");
            }

            await Task.Delay(intervalSeconds * 1000);
        }
    }
}
