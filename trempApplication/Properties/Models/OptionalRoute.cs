namespace trempApplication.Properties.Models
{
    public class OptionalRoute
    {
        public double Distance { get; set; }
        public double Duration { get; set; }
        public List<string> Instructions { get; set; } // list of step-by-step instructions
        public double RelevanceScore { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public List<string> Waypoints { get; set; }
    }
}
