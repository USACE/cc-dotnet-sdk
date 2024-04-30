using System.Text.Json.Serialization;

namespace Usace.CC.Plugin
{
    /// <summary>
    /// Generic action -- like a process for a plugin to execute
    /// used for declaritively describing processes to include and their order
    /// 
    /// examples: manipulate input files, run a simulation, manipulate output files
    /// </summary>
    public class Action
    {
        public string Name { get; set; }
        public string Type { get; set; }
        [JsonPropertyName("desc")]
        public string Description { get; set; }
        [JsonPropertyName("params")]
        public Dictionary<string, string> Parameters { get; set; }
    }
}