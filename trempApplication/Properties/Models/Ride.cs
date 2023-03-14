namespace trempApplication.Properties.Models
{
    public class Ride
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public Guid CarId { get; set; }
        public int Capacity { get; set; }
        public List<Guid> PassengerIds { get; set; }
        public bool Active { get; set; }
        public Guid SourceId { get; set; } // GUID for Address
        public Guid DestinationId { get; set; }
        public List<Guid> StationIds { get; set; }
        public List<string> PickUpTimes { get; set; }
    }
}