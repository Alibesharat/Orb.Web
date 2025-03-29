# Orb - Sciola Media App

## Overview
Orb is a modern web application designed for the Sciola media platform. This application serves as a landing page for the Orb app, featuring a blog section and an intuitive admin dashboard. Built with ASP.NET Core and SQLite, Orb aims to provide a lightweight and efficient user experience.

## Project Structure
The project is organized into several key directories and files:

- **Orb.Web.csproj**: Project file containing dependencies and build settings.
- **Program.cs**: Entry point for the application, configuring the web host and middleware.
- **appsettings.json**: Configuration settings for the application.
- **appsettings.Development.json**: Development-specific configuration settings.
- **Properties/launchSettings.json**: Launch settings for different environments.
- **wwwroot/**: Contains static files such as CSS, JavaScript, and libraries.
  - **css/**: Custom and Bootstrap CSS files.
  - **js/**: Custom and Bootstrap JavaScript files.
  - **lib/**: Third-party libraries like jQuery.
- **Areas/Admin/**: Contains controllers and views for the admin area.
  - **Controllers/**: Manages admin functionalities.
  - **Views/**: Contains views related to the admin dashboard, blog, and user management.
- **Controllers/**: Public-facing controllers for home and blog functionalities.
- **Views/**: Public-facing views for home and blog sections.
- **Models/**: Contains data models for blog posts and users.
- **Data/**: Database context and migrations for SQLite.
- **Services/**: Business logic for blog and user management.
- **README.md**: Documentation for the project.

## Requirements
1. **ASP.NET Core**: The application will be built using ASP.NET Core for a robust and scalable web framework.
2. **SQLite**: A lightweight database solution for storing application data.
3. **Admin Dashboard**: A modern and user-friendly dashboard for managing blog posts and users.
4. **Blog Section**: A dedicated section for displaying and managing blog posts.
5. **Responsive Design**: The application will utilize Bootstrap for a responsive and modern UI.
6. **Lightweight and Low Dependency**: The project will focus on minimal dependencies to ensure performance and maintainability.

## Setup Instructions
1. Clone the repository:
   ```
   git clone <repository-url>
   ```
2. Navigate to the project directory:
   ```
   cd Orb.Web
   ```
3. Restore the project dependencies:
   ```
   dotnet restore
   ```
4. Run the application:
   ```
   dotnet run
   ```
5. Access the application in your web browser at `http://localhost:5000`.

## Contributing
Contributions are welcome! Please submit a pull request or open an issue for any enhancements or bug fixes.

## License
This project is licensed under the MIT License. See the LICENSE file for details.