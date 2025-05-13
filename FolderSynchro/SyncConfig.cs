using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FolderSynchro
{
    public class SyncConfig
    {
        public List<string> ExcludeFiles { get; set; } = new();
        public List<string> ExcludeDirectories { get; set; } = new();

        public static SyncConfig Load(string path)
        {
            if (!File.Exists(path))
                return new SyncConfig();

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<SyncConfig>(json) ?? new SyncConfig();
        }
    }
}
