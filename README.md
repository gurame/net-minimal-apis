# net-minimal-apis

This repository contains a .NET Minimal API project. The project is structured to demonstrate the use of minimal APIs in .NET, focusing on simplicity and performance.

## Project Structure

- **Program.cs**: The main entry point for the application.
- **appsettings.json** and **appsettings.Development.json**: Configuration files for the application.
- **Endpoints/**: Contains endpoint definitions and extensions.
- **Services/**: Contains service implementations.
- **Validators/**: Contains validation logic.
- **Models/**: Contains data models.
- **Data/**: Contains data access logic.

## Features

- Minimal API setup with .NET
- Swagger integration for API documentation
- Dependency Injection for services
- Fluent Validation for request validation
- Asynchronous database initialization

## Getting Started

1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/net-minimal-apis.git
    ```
2. Navigate to the project directory:
    ```sh
    cd net-minimal-apis/src/Library.Api
    ```
3. Restore the dependencies:
    ```sh
    dotnet restore
    ```
4. Run the application:
    ```sh
    dotnet run
    ```

## Configuration

Modify the `appsettings.json` or `appsettings.Development.json` files to configure the application settings, such as the database connection string.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any changes.

## License

This project is licensed under the MIT License.