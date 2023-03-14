namespace trempApplication.Properties.Models
{
    public class Passenger
    {
        public Guid id { get; set; }
        public string UserName { get; set; }
        public string Faculty { get; set; }
        public List<Car> Cars { get; set; }
        public List<string> Addresses { get; set; }

    }
}
