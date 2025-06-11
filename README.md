# Kommo CRM Integration API

This project is a .NET web API designed to integrate with Kommo CRM. It allows for managing leads and orders, and synchronizing data between a local database and Kommo CRM.

## API Endpoints

This section details the available API endpoints.

### Orders API (`/api/Orders`)

Handles operations related to orders.

*   **POST `/api/Orders`**: Creates a new order.
    *   **Request Body**: `OrderCreationDto`
        ```json
        {
          "phoneNumber": "string",
          "userName": "string",
          "amount": 0
        }
        ```
    *   **Response**: `OrderViewModel`
        ```json
        {
          "id": 0,
          "phoneNumber": "string",
          "userName": "string",
          "amount": 0,
          "leadId": 0,
          "status": "string"
        }
        ```
*   **PUT `/api/Orders/{id}`**: Updates an existing order.
    *   **URL Parameters**:
        *   `id` (long): The ID of the order to update.
    *   **Request Body**: `OrderModificationDto`
        ```json
        {
          "phoneNumber": "string",
          "userName": "string",
          "amount": 0
        }
        ```
    *   **Response**: `OrderViewModel`
*   **GET `/api/Orders`**: Retrieves all orders.
    *   **Response**: `List<OrderViewModel>`
*   **GET `/api/Orders/pipelines`**: Retrieves pipeline statuses from Kommo CRM.
    *   **Response**: List of Kommo pipeline statuses.

### Webhook API (`/api/Webhook/kommo`)

Handles incoming webhooks from Kommo CRM.

*   **POST `/api/Webhook/kommo/leads`**: Receives lead updates from Kommo CRM.
    *   **Request ContentType**: `application/x-www-form-urlencoded`
    *   **Request Body**: Form data containing lead information (e.g., `leads[add][0][id]`, `leads[update][0][name]`, etc.). The endpoint handles `add`, `update`, and `status` changes for leads.
    *   **Response**: `200 OK` if successful.

    *Note: This endpoint is designed to be called by Kommo CRM's webhook system. It parses the form data to update the local database based on changes in Kommo CRM.*

## KommoClient Functionality

The `KommoClient.cs` class is responsible for all direct communication with the Kommo CRM API.

Key features include:

*   **Authentication**: Handles Bearer token authentication for API requests.
*   **Lead Management**:
    *   `AddLeadAsync(string name, decimal price, long? contactId = null, ...)`: Creates a new lead in Kommo CRM.
    *   `UpdateLeadAsync(long leadId, string? name = null, decimal? price = null, ...)`: Updates an existing lead in Kommo CRM.
*   **Pipeline Information**:
    *   `GetStatusAsync(...)`: Retrieves the list of pipeline statuses from Kommo CRM, focusing on the main pipeline.

The client uses `HttpClient` for making asynchronous HTTP requests and `System.Text.Json` for serializing and deserializing JSON payloads. It requires the Kommo subdomain and a long-lived access token for initialization.

## Project Setup and Configuration

Follow these steps to set up and run the project:

1.  **Clone the repository:**
    ```bash
    git clone <repository-url>
    cd <repository-directory>
    ```
2.  **Configure Kommo CRM API access:**
    *   Open the `Kommo Client/appsettings.json` file (or `appsettings.Development.json` for development environment).
    *   Update the `KommoOptions` section with your Kommo CRM subdomain and long-lived access token:
        ```json
        {
          "Logging": {
            // ...
          },
          "AllowedHosts": "*",
          "KommoOptions": {
            "UserName": "your-kommo-subdomain", // e.g., yourcompany
            "LongLivedToken": "your-kommo-long-lived-access-token"
          },
          "ConnectionStrings": {
            "DefaultConnection": "Data Source=marketplace.db" // Or your preferred DB connection string
          }
        }
        ```
    *   **Important**: Ensure your Kommo long-lived token is correctly generated and has the necessary permissions.
3.  **Database Setup:**
    *   This project uses Entity Framework Core. The default configuration uses SQLite with the database file `marketplace.db`.
    *   To apply migrations and create the database (if it doesn't exist):
        ```bash
        cd "Kommo Client"
        dotnet ef database update
        cd ..
        ```
    *   If you prefer a different database provider, update the `ConnectionStrings:DefaultConnection` in `appsettings.json` and potentially install the corresponding EF Core provider package.
4.  **Build and Run the application:**
    *   From the root directory:
        ```bash
        dotnet build
        ```
    *   To run the application (typically from the `Kommo Client` directory):
        ```bash
        cd "Kommo Client"
        dotnet run
        ```
    *   The API will typically be available at `https://localhost:<port>` or `http://localhost:<port>`. Check the console output for the exact URLs.
    *   Swagger UI will be available at `/swagger` in development mode for easy API testing.

### Dependencies

*   .NET SDK (version specified in global.json or project file, likely .NET 6.0 or newer)
*   Entity Framework Core tools (if you need to manage migrations):
    ```bash
    dotnet tool install --global dotnet-ef
    ```

## Contributing

Contributions are welcome! If you'd like to contribute to this project, please follow these general steps:

1.  **Fork the repository.**
2.  **Create a new branch** for your feature or bug fix:
    ```bash
    git checkout -b feature/your-feature-name
    ```
3.  **Make your changes.**
    *   Ensure your code follows the existing coding style.
    *   Add or update unit tests as appropriate.
4.  **Commit your changes** with a clear and descriptive commit message:
    ```bash
    git commit -m "feat: Add new feature X"
    ```
    *(Consider using [Conventional Commits](https://www.conventionalcommits.org/) for commit messages.)*
5.  **Push your branch** to your forked repository:
    ```bash
    git push origin feature/your-feature-name
    ```
6.  **Create a Pull Request (PR)** against the main repository's `main` (or `master`) branch.
    *   Provide a clear description of the changes in your PR.
    *   Link any relevant issues.

If you find any bugs or have suggestions for improvements, please open an issue in the repository.
