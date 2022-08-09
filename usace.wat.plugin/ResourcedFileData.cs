namespace usace.wat.plugin
{
    public class ResourcedFileData
    {
        public string Id { get; set; }
        public string Filename { get; set; }
        public ResourceInfo Resource_info { get; set; }
        public ResourcedInternalPathData[] Internal_paths { get; set; }
    }
}
