
### Server-Side README

# Student Share Drive Application - Server Side (פרוייקט שנתי- אפליקציית טרמפים לסטודנטים)

#### Project Overview
This repository contains the server-side code for a comprehensive student share drive application. Built with ASP.NET Core (C#) and integrated with MongoDB, the server includes advanced features such as a recommendation system for matching drivers and passengers using NLP and AI algorithms. The server also supports real-time notifications and integration with third-party APIs.

## Features
- **ASP.NET Core (C#)**: Robust framework for server-side application.
- **MongoDB Integration**: Scalable database for user data and file storage.
- **Natural Language Processing (NLP)**: Enhanced search functionality and personalized recommendations.
- **Recommendation System**: AI-driven algorithms to match drivers with passengers.
- **Real-Time Notifications**: Instant updates and alerts for users.
- **Third-Party API Integration**: Connectivity with external services.

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
