namespace trempApplication.Properties.Models
{
    public class Ride
    {
        public Guid id { get; set; }
        public Passenger Driver { get; set; }
        public Car Car { get; set; }
        public int Capacity { get; set; }
        public List<Passenger> Passengers { get; set; }
        public bool Active { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public List<string> Stations { get; set; }
        public List<string> PickUpTimes { get; set; }
    }
}
