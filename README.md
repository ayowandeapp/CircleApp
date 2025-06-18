# Social Media App - ASP.NET Core MVC

This project is a full-featured **Social Media Application** built with **ASP.NET Core MVC**, **C#**, **JavaScript**, and **Entity Framework Core**. It mimics core features of modern social platforms like posts, likes, comments, stories, real-time notifications, user profiles, and more.

---

## 📌 Key Features Implemented

Below is the roadmap of implemented functionalities:

### 🏠 Timeline & Posts
- **Designing the Home Page** with a timeline layout
- **Creating and Managing Posts** (CRUD operations)
- **Liking, Commenting, Favoriting Posts**
- **Managing Post Visibility** (Public/Private)
- **Reporting and Removing Posts**
- **Displaying Post Details and Engagement**

### 📖 Stories & Hashtags
- **Adding Instagram-like Stories**
- **Handling Trending Topics using Hashtags**

### 🔧 Architecture & Components
- **Service-Oriented Architecture Transition**
- **Loading Dynamic Data via ViewComponent**

### 👤 User Profiles
- **User Profile Display and Settings**
- **Managing Favorite Posts**
- **Profile Customization**

### 🔐 Authentication & Authorization
- **User Authentication & Role-Based Authorization**
- **OAuth Login via Google and GitHub**

### 🧑‍🤝‍🧑 Social Interactions
- **Friendship System**: Send, Accept Requests & Suggestions
- **Real-Time Post Interactions** (AJAX without page refresh)

### 🔔 Notifications & Admin
- **Real-Time Notifications with SignalR**
- **Admin Dashboard** for Managing Reported Posts

---

## 💾 Technologies Used

- **Frontend**: HTML, CSS, JavaScript, Razor Views
- **Backend**: ASP.NET Core MVC (C#)
- **Database**: Entity Framework Core + SQL Server
- **Authentication**: ASP.NET Identity, OAuth (Google, GitHub)
- **Real-Time**: SignalR
- **Architecture**: MVC + Service Layer

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK or later](https://dotnet.microsoft.com/download)
- [SQL Server or SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) with ASP.NET & Web development workload

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/social-media-app.git
   cd social-media-app

2. **Restore dependencies**
   ```bash
   dotnet restore

3. **Update the appsettings.json with your local DB connection string**
   
4. **Apply migrations**
   ```bash
   dotnet ef database update

5. **Run the project**
   ```bash
   dotnet run
