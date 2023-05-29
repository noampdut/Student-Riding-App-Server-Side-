using MongoDB.Driver;
using System.Linq.Expressions;
using trempApplication.Properties.Interfaces;
using trempApplication.Properties.Models;

namespace trempApplication.Properties.Services
{
    public class RideService : IRide
    {
        private IMongoCollection<Ride> ridesCollection;
        public RideService(IConfiguration configuration)
        {
            var mongoClient = new MongoClient(configuration.GetConnectionString("PassengerConnection"));
            var mongoDB = mongoClient.GetDatabase("DB");
            ridesCollection = mongoDB.GetCollection<Ride>("Rides");
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
        /*
        public static bool IsCurrentTimeLessThanOrEqualToDate(Date driverDate)
        {
            // Convert input Date object to DateTime object
            DateTime dateTime = new DateTime(Convert.ToInt32(driverDate.Year), Convert.ToInt32(driverDate.Month), Convert.ToInt32(driverDate.Day),
                Convert.ToInt32(driverDate.Hour), Convert.ToInt32(driverDate.Minute), 0);

            // Get the current date and time
            DateTime now = DateTime.Now;

            // Compare the current time with the input date
            if (now <= dateTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CompareCapacity(int capacity, int wayPointsCount)
        {
            if (capacity > wayPointsCount)
            {
                return true;
            }
            return false;
        }
        */

       // private Expression<Func<Ride, bool>> IsCurrentTimeLessThanOrEqualToDate(Date date, Date uDate)
        //{
          //  DateTime currentDate = new DateTime(Convert.ToInt32(date.Year), Convert.ToInt32(date.Month), Convert.ToInt32(date.Day), Convert.ToInt32(date.Hour), Convert.ToInt32(date.Minute), 0, DateTimeKind.Utc);
            //DateTime targetDate = new DateTime(Convert.ToInt32(uDate.Year), Convert.ToInt32(uDate.Month), Convert.ToInt32(uDate.Day), Convert.ToInt32(uDate.Hour), Convert.ToInt32(uDate.Minute), 0, DateTimeKind.Utc);

            //return u => u.Date <= currentDate;
        //}

        private Expression<Func<Ride, bool>> CheckTimeDifference2(int minutes, Date date1, Date date2)
        {
            DateTime currentDate = new DateTime(Convert.ToInt32(date1.Year), Convert.ToInt32(date1.Month), Convert.ToInt32(date1.Day), Convert.ToInt32(date1.Hour), Convert.ToInt32(date1.Minute), 0, DateTimeKind.Utc);
            DateTime targetDate = new DateTime(Convert.ToInt32(date2.Year), Convert.ToInt32(date2.Month), Convert.ToInt32(date2.Day), Convert.ToInt32(date2.Hour), Convert.ToInt32(date2.Minute), 0, DateTimeKind.Utc);

            TimeSpan timeDifference = currentDate - targetDate;
            return u => timeDifference.TotalMinutes <= minutes;
        }

        private Expression<Func<Ride, bool>> CompareCapacity(int capacity, int stationCount)
        {
            return u => u.Capacity > stationCount;
        }
        public async Task<(bool IsSuccess, List<Ride> Rides, string ErrorMessage)> GetPotentialRides(Date uDate, bool ToUniversity)
        {
             try
             {

                 var initialFilter = Builders<Ride>.Filter.Eq(u => u.ToUniversity, ToUniversity);

                 var rides = await ridesCollection.Find(initialFilter).ToListAsync();

                 var filteredRides = new List<Ride>();
                 foreach (var ride in rides)
                 {

                    int totalWaypoints = ride.Stations.Count;
                    int maxWaypointsThreshold = ride.Capacity;

                    // checking if number of stops is higher or equal to the capacity
                    if (totalWaypoints >= maxWaypointsThreshold) {
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
    }
    }
