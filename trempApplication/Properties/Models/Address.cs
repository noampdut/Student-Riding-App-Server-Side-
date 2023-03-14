namespace trempApplication.Properties.Models
{
    public class Address
    {
        public Guid Id { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string BuildingNumber { get; set; }
        public string PostalCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
