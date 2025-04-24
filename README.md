
# 🚗 Smart Carpool App – Server Side

**Final Year Project – Smart Ride-Sharing Platform for Students**

This repository hosts the backend for a full-stack smart carpool application, designed for university students. Built with ASP.NET Core and MongoDB, it includes an intelligent recommendation engine powered by NLP and machine learning, with full support for real-time updates and ride history tracking.

---

## 📁 Related Repositories

- 📱 [Android App (Client)](https://github.com/noampdut/Studnet-Riding-App)

---

## 🔍 Features

- ✅ **ASP.NET Core Web API (C#)** – Secure, fast, and scalable.
- 🗂️ **MongoDB** – NoSQL database for all ride, user, and history data.
- 🧠 **AI-Powered Recommendation Engine** – Matches drivers and passengers based on:
  - Location, time, and preferences
  - Google Maps API for route optimization
  - NLP model (BERT) for semantic matching of user bios
- 🧾 **User Profiles & Smart Matching** – Each user writes a few words about themselves, and the system finds compatible rides using semantic similarity.
- 📡 **Real-Time Notifications** via Firebase:
  - New passenger joined
  - Ride update or cancellation
  - Confirmation alerts
- 🔄 **Two-Way Notification Support** – Drivers and passengers both receive timely updates for every relevant event.
- 🕓 **Ride History** – Tracks all actions: requests, confirmations, cancellations, and reviews.
- ⭐ **Review System** – Feedback after each ride improves future recommendations.

---

## 🛠️ Prerequisites

Before running the server, install:

- [.NET 6 SDK or newer](https://dotnet.microsoft.com/download)
- [MongoDB](https://www.mongodb.com/atlas/database)
- [Firebase Project](https://firebase.google.com/)
- [Google Cloud API Key](https://console.cloud.google.com/)

---

🔐 Authentication Note
While the live version is currently unavailable due to external service costs (MongoDB, Firebase, and Google APIs), a demo version was showcased using pre-created test users.
🧪 To see the login and full user experience in action, check out the demo video on YouTube.
Note: Credentials for demo users can easily be configured in Firebase for testing or presentation purposes.


## ⚙️ Setup Instructions

### 1️⃣ Clone the Repository

```bash
git clone https://github.com/noampdut/Share-Drive-Server-Side.git
cd Share-Drive-Server-Side


## Prerequisites
Before running the server, ensure you have the following installed:
- [.NET Core SDK](https://dotnet.microsoft.com/download) (version compatible with the project)
- [MongoDB](https://www.mongodb.com/try/download/community) (installed and running)

## Setup Instructions

### Step 1: Clone the Repository
```bash
git clone https://github.com/noampdut/Share-Drive-Server-Side.git
cd student-share-drive-server
```

### Step 2: Configure MongoDB
Ensure MongoDB is running and accessible. Update the connection string in `appsettings.json` to match your MongoDB instance.

### Step 3: Restore Dependencies
```bash
dotnet restore
```

### Step 4: Run the Server
```bash
dotnet run
```

The server will start and listen on the default port. You can configure the port and other settings in the `appsettings.json` file.

## Additional Notes
- Ensure that the server and MongoDB are both running before attempting to use the client applications.
- For more detailed API documentation, refer to the [API Documentation](docs/API.md).
- The project has received numerous compliments for its innovative features and performance.
