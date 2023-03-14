namespace trempApplication.Properties.Models
{
    public class Passenger
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Faculty { get; set; }
        public List<Guid> CarIds { get; set; }
        public List<string> Addresses { get; set; }
    }
}