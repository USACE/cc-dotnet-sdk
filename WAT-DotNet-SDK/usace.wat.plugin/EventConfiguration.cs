namespace usace.wat.plugin
{
    public class EventConfiguration
    {
        private SeedSet[] seeds;
        public int event_number { get; set; }
        public int realization_number { get; set; }

        public SeedSet GetSeedSet(string identifier)
        {
            foreach(var seedSet in seeds)
            {
                if(seedSet.identifier == identifier)
                {
                    return seedSet;
                }
            }
            return null;
        }
    }
}
