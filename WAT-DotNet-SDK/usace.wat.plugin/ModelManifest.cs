namespace usace.wat.plugin
{
    public class ModelManifest
    {
        public string manifest_id { get; set; }
        public Plugin plugin { get; set; }
        public ModelIdentifier model_identifier { get; set; }
        public FileData[] inputs { get; set; }
        public FileData[] outputs { get; set; }
    }
}
