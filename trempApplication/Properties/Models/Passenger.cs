namespace trempApplication.Properties.Models
{
    public class Passenger
    {
        public Guid Id { get; set; }
        public string IdNumber { get; set; }
        public string UserName { get; set; }
        public string Faculty { get; set; }
        public string PhoneNumber { get; set; }
        public List<Guid> CarIds { get; set; }
        public string Bio { get; set; }

        public string Token { get; set; }
    }
}