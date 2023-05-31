namespace trempApplication.Properties.Models
{
    public class OptionalRoute
    {
        public double Distance { get; set; }
        public double Duration { get; set; }
        public List<string> Instructions { get; set; } // list of step-by-step instructions
        
        public List<string> Waypoints { get; set; }

        public Dictionary<string, string> PickUpTimes { get; set; }
        //public string PickUpCurrent { get; set; }

        public List<GoogleApi.Entities.Maps.Directions.Response.Leg> Legs  { get; set; }
    }
}
