namespace usace.wat.plugin
{
    public class ModelPayload
    {
        public string payload_id { get; set; }
        public Model model { get; set; }
        public int event_index { get; set; }
        public ResourcedFileData[] inputs { get; set; }
        public ResourcedFileData[] outputs { get; set; }
    }
}
