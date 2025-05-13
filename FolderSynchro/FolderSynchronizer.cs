using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using FolderSynchro;

public class FolderSynchronizer
{
    private readonly string _source;
    private readonly string _replica;
    private readonly Logger _logger;
    private readonly SyncConfig _config; 
    public FolderSynchronizer(string source, string replica, Logger logger, SyncConfig config)     {
        _source = source;
        _replica = replica;
        _logger = logger;
        _config = config;
    }

    public async Task SynchronizeAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        int copiedFiles = 0;
        int deletedFiles = 0;

        try
        {
            copiedFiles = await CopyNewAndModifiedFilesAsync();
            deletedFiles = await DeleteRemovedFilesAsync();
            stopwatch.Stop();
            _logger.Log($"Synchronization completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds. Files copied/updated: {copiedFiles}, files deleted: {deletedFiles}.");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.Log($"Error during synchronization after {stopwatch.Elapsed.TotalSeconds:F2} seconds: {ex}");
        }
    }

    private async Task<int> CopyNewAndModifiedFilesAsync()
    {
        int copiedFiles = 0;
        foreach (var sourceFilePath in Directory.GetFiles(_source, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(_source, sourceFilePath);

                        if (IsExcluded(relativePath, false)) continue;

            string replicaFilePath = Path.Combine(_replica, relativePath);
            string? replicaDir = Path.GetDirectoryName(replicaFilePath);

                        if (replicaDir != null && IsExcluded(Path.GetRelativePath(_replica, replicaDir), true)) continue;

            if (!Directory.Exists(replicaDir))
                Directory.CreateDirectory(replicaDir!);

            if (!File.Exists(replicaFilePath) || await GetMD5Async(sourceFilePath) != await GetMD5Async(replicaFilePath))
            {
                await CopyFileAsync(sourceFilePath, replicaFilePath);
                copiedFiles++;
                _logger.Log($"Copied/Updated: {relativePath}");
            }
        }
        return copiedFiles;
    }

    private async Task<int> DeleteRemovedFilesAsync()
    {
        int deletedFiles = 0;
        await Task.Run(() =>
        {
            foreach (var replicaFilePath in Directory.GetFiles(_replica, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(_replica, replicaFilePath);

                                if (IsExcluded(relativePath, false)) continue;

                string sourceFilePath = Path.Combine(_source, relativePath);

                if (!File.Exists(sourceFilePath))
                {
                    File.Delete(replicaFilePath);
                    deletedFiles++;
                    _logger.Log($"Deleted: {relativePath}");
                }
            }
        });
        return deletedFiles;
    }

        private bool IsExcluded(string relativePath, bool isDirectory)
    {
        if (isDirectory)
        {
                        return _config.ExcludeDirectories.Any(pattern =>
                relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Contains(pattern));
        }
        else
        {
                        return _config.ExcludeFiles.Any(pattern =>
                Regex.IsMatch(Path.GetFileName(relativePath), WildcardToRegex(pattern), RegexOptions.IgnoreCase));
        }
    }

        private static string WildcardToRegex(string pattern)
    {
        return "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
    }

    private static async Task<string> GetMD5Async(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await md5.ComputeHashAsync(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    private static async Task CopyFileAsync(string sourcePath, string destinationPath)
    {
        const int bufferSize = 81920;
        using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true))
        using (var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, useAsync: true))
        {
            await sourceStream.CopyToAsync(destinationStream);
        }
    }
}
