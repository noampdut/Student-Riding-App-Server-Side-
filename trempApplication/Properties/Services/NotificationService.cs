using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using MongoDB.Driver;
using trempApplication.Properties.Interfaces;
using trempApplication.Properties.Models;

namespace trempApplication.Properties.Services
{
    public class NotificationService : INotificationService
    {
        public NotificationService(IConfiguration configuration)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("firebase_private_key.json")
            });
        }

        public async Task<string> sendNotification(string registrationToken, string title, string body)
        {
            var message = new Message()
            {
                /*Data = new Dictionary<string, string>()
                {
                    {"myData", "1337" },
                },*/
                Token = registrationToken,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                }
            };

            return await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }
}
