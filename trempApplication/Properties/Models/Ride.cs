namespace trempApplication.Properties.Models
{
    public class Ride
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public Guid CarId { get; set; }
        public int Capacity { get; set; }
        public string Source { get; set; } 
        public string Dest { get; set; }
        public Date Date { get; set; } 
        public List<Guid> PassengerIds { get; set; }
        public List<string> Stations { get; set; }
        public List<string> PickUpTimes { get; set; }
    }
}