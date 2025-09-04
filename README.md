# Video Game Library Application
## Project Description
This project is a web-based Video Game Library application, demonstrating the evolution from a monolithic architecture to a microservices-based system. The application allows users to manage their personal video game collection and wishlist. It is built primarily with C#, complemented by HTML, CSS, and JavaScript for the front-end components.

The primary objective of this project is to showcase an understanding of key software engineering concepts, including:
* **Monolithic Architecture**: The initial, single-tiered application that handles all functionality.
* **Microservices Architecture**: The refactoring of the application into smaller, independent services, each with a single, dedicated responsibility.
* **RESTful APIs**: The use of a standard, stateless protocol for communication between the front-end and the back-end microservices.
* **JWT Authentication**: A secure, token-based system for user authentication and authorization across the services.

## Architecture
The application is composed of three main components:
1. VideoGameLibrary (Front-end):
* This is the user-facing part of the application.
* It contains the HTML, CSS, and JavaScript for the user interface.
* It communicates with the back-end microservices to perform all data operations.
2. UserAuthentication (Microservice):
* **Responsibility**: Manages all user-related authentication and authorization.
* **Endpoints**: Handles user registration (``/api/Auth/register``) and login (``/api/Auth/login``), issuing a JSON Web Token (JWT) upon successful authentication.
* **Database**: Contains only user login credentials. It has no knowledge of user-specific video game or wishlist data.
3. UserData (Microservice):
* **Responsibility**: Manages all user-specific video game and wishlist data.
* **Endpoints**: Provides CRUD (Create, Read, Update, Delete) operations for video games (``/api/users/videogames``) and wishlists (``/api/users/wishlist``).
* **Authorization**: Validates the JWT from the ``UserAuthentication`` for every request to ensure that a user can only access their own data.

## How to Run the Application
This guide assumes you have the .NET 8 SDK and a SQL Server instance installed.

### Step 1: Configure Your Databases
You will need to set up two separate databases for the microservices.
* For ``UserAuthentication``:
  * Create a database (e.g., ``UserAuthenticationDB``).
  * Update the ``ConnectionStrings:DefaultConnection`` in ``UserAuthentication/appsettings.json`` to point to your new database.
* For ``UserData``:
  * Create a database (e.g., ``UserDataDB``).
  * Update the ``ConnectionStrings:DefaultConnection`` in ``UserData/appsettings.json`` to point to your new database.

### Step 2: Apply Database Migrations
For each microservice, open a terminal in its respective project folder (``UserAuthentication`` and ``UserData``) and run the following commands to apply the database migrations. This will create the necessary tables in your databases.
```
dotnet ef database update
```
### Step 3: Configure JWT Settings
In both the ``UserAuthentication`` and ``UserData`` projects, open the ``appsettings.json`` file and add the following section. Ensure the values are identical in both files. The Key must be a long, complex, and secure string.
```
"Jwt": {
    "Key": "your_super_secret_and_long_key_for_both_services",
    "Issuer": "YourAuthServiceIssuer",
    "Audience": "YourAudience"
}
```
### Step 4: Run the Microservices
Navigate to each of the microservice project folders in a separate terminal and run them.
```
# In the UserAuthentication folder
dotnet run

# In the UserData folder
dotnet run
```
Both services will run on different ports (e.g., 5001 and 5002). The URLs will be displayed in the terminal.

### Step 5: Run the Monolithic Application
Once the two microservices are running, you can run the monolithic front-end application.
```
# In the VideoGameLibrary folder
dotnet run
```
Your front-end application will start and you can access it in your web browser. You can now register and log in, and the application will use the microservices to handle your data.
