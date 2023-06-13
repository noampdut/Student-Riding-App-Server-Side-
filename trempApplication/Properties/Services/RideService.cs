using MongoDB.Driver;
using System.Linq.Expressions;
using trempApplication.Properties.Interfaces;
using trempApplication.Properties.Models;

namespace trempApplication.Properties.Services
{
    public class RideService : IRide
    {
        private IMongoCollection<Ride> ridesCollection;
       // private IMongoCollection<Passenger> passengersCollection;
        private IPassenger _passengerService;

        public RideService(IConfiguration configuration, IPassenger passengerService)
        {
            var mongoClient = new MongoClient(configuration.GetConnectionString("PassengerConnection"));
            var mongoDB = mongoClient.GetDatabase("DB");
            ridesCollection = mongoDB.GetCollection<Ride>("Rides");
           // passengersCollection = mongoDB.GetCollection<Passenger>("Passengers");
            _passengerService = passengerService;

        }

        public async Task<(bool IsSuccess, string ErrorMessage)> AddRide(Ride ride)
        {
            try
            {
                if (ride == null)
                {
                    return (false, "ride is null object");
                }

                await ridesCollection.InsertOneAsync(ride);
                return (true, ride.Id.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding ride to database", ex);
            }
        }


        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteRide(Guid id)
        {
            try
            {
                var filter = Builders<Ride>.Filter.Eq(u => u.Id, id);
                var ride = await ridesCollection.DeleteOneAsync(filter);
                if (ride == null)
                {
                    return (false, "No ride was found to be deleted");
                }
                return (true, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting ride from database", ex);
            }
        }

        public async Task<(bool IsSuccess, List<Ride> Ride, string ErrorMessage)> GetAllRides()
        {
            try
            {
                var rides = await ridesCollection.Find(u => true).ToListAsync();
                if (rides != null)
                {
                    return (true, rides, null);
                }
                return (false, null, "No rides found");
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all rides from database", ex);
            }
        }

        public async Task<(bool IsSuccess, Ride Ride, string ErrorMessage)> GetRideById(Guid id)
        {
            try
            {
                var filter = Builders<Ride>.Filter.Eq(u => u.Id, id);
                var ride = await ridesCollection.Find(filter).FirstOrDefaultAsync();
                if (ride != null)
                {
                    return (true, ride, null);
                }
                return (false, null, "No ride found");
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting ride by id from database", ex);
            }
        }


        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateRide(Ride ride, Guid id)
        {
            try
            {
                if (ride == null)
                {
                    return (false, "The ride object is null.");
                }

                var filter = Builders<Ride>.Filter.Eq(u => u.Id, id);
                await ridesCollection.ReplaceOneAsync(filter, ride);
                return (true, null);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating ride in database", ex);
            }
        }

        public async Task<(bool IsSuccess, List<Ride> Rides, string ErrorMessage)> GetPotentialRides(Date uDate, bool ToUniversity, Guid client_id)
        {
             try
             {

                 var initialFilter = Builders<Ride>.Filter.Eq(u => u.ToUniversity, ToUniversity);

                 var rides = await ridesCollection.Find(initialFilter).ToListAsync();

                 var filteredRides = new List<Ride>();
                 foreach (var ride in rides)
                 {
                    // Do not offer to a client a drive he offered to others
                    if(ride.Driver.Id == client_id)
                    {
                        continue;
                    }

                    // checking if number of stops is higher or equal to the capacity
                    if (ride.Capacity <= 0) {
                        continue;
                    }

                    // if time differnce is not ok 
                    if(CheckTimeDifference(60,uDate, ride.Date) == false) { continue; }

                   filteredRides.Add(ride);
                     
                 }
            

                 if (rides != null && rides.Any())
                 {
                     return (true, filteredRides, null);
                 }

                 return (false, null, "No rides found");
             }
             catch (Exception ex)
             {
                 throw new Exception("Error getting potential rides from database", ex);
             }
            
        }

        private bool CheckTimeDifference(int timeDifference, Date date1, Date date2)
        {

            // Convert Date objects to DateTime objects
            DateTime dateTime1 = new DateTime(Convert.ToInt32(date1.Year), Convert.ToInt32(date1.Month), Convert.ToInt32(date1.Day),
                Convert.ToInt32(date1.Hour), Convert.ToInt32(date1.Minute), 0);

            DateTime dateTime2 = new DateTime(Convert.ToInt32(date2.Year), Convert.ToInt32(date2.Month), Convert.ToInt32(date2.Day),
                Convert.ToInt32(date2.Hour), Convert.ToInt32(date2.Minute), 0);

            // Calculate time difference
            TimeSpan difference = dateTime2 - dateTime1;

            // Check if time difference is within an hour
            if ((difference.TotalMinutes <= timeDifference) && (difference.TotalMinutes >= -timeDifference))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private bool CheckIfDateIsInThePast(Date date1)
        {
            // Convert Date object to DateTime objects
            DateTime dateTime1 = new DateTime(Convert.ToInt32(date1.Year), Convert.ToInt32(date1.Month), Convert.ToInt32(date1.Day),
                Convert.ToInt32(date1.Hour), Convert.ToInt32(date1.Minute), 0);

            DateTime currentDate = DateTime.Now;

            // Compare the provided date with the current date and time
            if (dateTime1 < currentDate)
            {
                // The provided date is in the past
                return true; 
            }
            else
            {
                // The provided date is in the future or the current moment
                return false; 
            }
        }


        public async Task<(bool IsSuccess, List<Ride> Rides, string ErrorMessage)> GetRidesByPassenger(string id)
        {
            List<Ride> FilteredRides = new List<Ride>();
            var passenger = await _passengerService.GetPassengerByIdNumber(id);
            Guid passengerGuid = passenger.Passenger.Id;
            try
            {
                var rides = GetAllRides().Result.Ride;
                foreach (var ride in rides)
                {
                    
                    if (ride.Driver.Id == passengerGuid)
                    {
                        FilteredRides.Add(ride);
                        continue;
                    }
                    foreach (var point in ride.pickUpPoints)
                    {
                        if(point.PassengerId == id)
                        {
                            FilteredRides.Add(ride);
                            break;
                        }
                    }
                }

                if (rides != null)
                {
                    return (true, FilteredRides, null);
                }

                return (false, null, "No rides found");
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting rides" , ex);
            }
        }

        public async Task<(bool IsSuccess, List<Ride> Rides, string ErrorMessage)> GetActiveOrHistoryRides(string id, bool getActive)
        {
            try
            {
                var rides = GetRidesByPassenger(id).Result.Rides;
                var ActiveRides = new List<Ride>();
                var HistoryRides = new List<Ride>();
                foreach (var ride in rides)
                {
                    // the ride is active (not in history)
                    if (CheckIfDateIsInThePast(ride.Date) == false)
                    {
                        ActiveRides.Add(ride);
                    }
                    else
                    {
                        HistoryRides.Add(ride);
                    }
                }

                if (getActive && ActiveRides != null)
                {
                    return (true, ActiveRides, null);
                }
                else if (!getActive && HistoryRides != null)
                {
                    return (true, HistoryRides, null);
                }

                return (false, null, "No rides found");
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting rides from database", ex);
            }
        }
    }
}
