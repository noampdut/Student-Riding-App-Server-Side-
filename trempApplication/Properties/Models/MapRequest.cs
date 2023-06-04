namespace trempApplication.Properties.Models
{
    public class MapRequest
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public List<string> Waypoints { get; set; }
        public Date Date { get; set; }
        public bool ToUniversity { get; set; }



    }
}
