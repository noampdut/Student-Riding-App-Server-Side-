namespace trempApplication.Properties.Models
{

    /// <summary>
    ///  this class maintains the optional route with the new frined as optional waypoint 
    ///  the ride id - the ride without the new friend- if we approve this ride we will update the ride 
    ///  and more information for the client side
    /// </summary>
    public class SuggestedRide
    {
        public Guid RideId { get; set; }
        // public OptionalRoute OptionalRoute { get; set; }

        public double Distance { get; set; } // total dist
        public double Duration { get; set; } // total time
        public List<string> Instructions { get; set; } // list of step-by-step instructions

        //public List<string> Waypoints { get; set; }

      //  public Dictionary<string, string> PickUpTimes { get; set; }

        public List<PickUpPoint> pickUpPoints { get; set; }

        public Passenger Driver { get; set; }
        /// <summary>
        /// in order to show detaild to the new friend 
        /// </summary>
        public string PickUpTime { get; set; }
        public string PickUpPoint { get; set; }

        public double Relevance { get; set; }
        public int Capacity { get; set; }
    }
}
