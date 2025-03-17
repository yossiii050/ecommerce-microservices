





---

# E-Commerce Microservices Platform


This repository contains the source code for an e-commerce platform built using a microservices architecture in .NET. The system is designed as a collection of independent services that work together to provide a robust, scalable, and maintainable solution.

---

## Project Overview

The platform is composed of multiple microservices, each responsible for a specific domain of the e-commerce workflow. By splitting the application into focused services, the project aims to improve modularity, facilitate independent deployments, and simplify maintenance.

---

## Microservices Breakdown

- **Product Microservice**  
  Handles product management including details, categorization, and inventory tracking.

- **.NET Identity Microservice**  
  Manages user authentication and authorization using .NET Identity.

- **Coupon Microservice**  
  Processes coupon issuance and discount logic.

- **Shopping Cart Microservice**  
  Manages user shopping carts and the addition or removal of products.

- **Order Microservice**  
  Oversees order creation, tracking, and status updates.

- **Email Microservice**  
  Responsible for sending notifications and email communications.

- **Payment Microservice**  
  Processes payments and handles payment confirmations.

- **Ocelot Gateway Project**  
  Serves as an API gateway, routing requests to the appropriate microservices and managing cross-cutting concerns such as security and rate limiting.

- **MVC Web Application**  
  Provides the front-end interface for users to interact with the e-commerce system.

---

## Technologies & Tools

- **.NET 8:** Core framework for building the microservices.
- **Entity Framework Core:** ORM used for data access and database operations.
- **SQL Server:** Relational database for storing persistent data.
- **Azure Service Bus:** Facilitates asynchronous communication between microservices.
- **Ocelot API Gateway:** Manages request routing, load balancing, and security.
- **.NET Identity:** Provides comprehensive user authentication and authorization solutions.
- **Clean Architecture:** Ensures a separation of concerns and a maintainable codebase.
[![My Skills](https://skillicons.dev/icons?i=dotnet,js,html,css,mysql,azure,fastapi,docker)](https://skillicons.dev)
<p align="left"> <a href="https://www.w3schools.com/cs/" target="_blank" rel="noreferrer"> <img src="https://raw.githubusercontent.com/devicons/devicon/master/icons/csharp/csharp-original.svg" alt="csharp" width="40" height="40"/> </a> <a href="https://www.w3schools.com/css/" target="_blank" rel="noreferrer"> <img src="https://raw.githubusercontent.com/devicons/devicon/master/icons/css3/css3-original-wordmark.svg" alt="css3" width="40" height="40"/> </a> <a href="https://www.docker.com/" target="_blank" rel="noreferrer"> <img src="https://raw.githubusercontent.com/devicons/devicon/master/icons/docker/docker-original-wordmark.svg" alt="docker" width="40" height="40"/> </a> <a href="https://dotnet.microsoft.com/" target="_blank" rel="noreferrer"> <img src="https://raw.githubusercontent.com/devicons/devicon/master/icons/dot-net/dot-net-original-wordmark.svg" alt="dotnet" width="40" height="40"/> </a> <a href="https://git-scm.com/" target="_blank" rel="noreferrer"> <img src="https://www.vectorlogo.zone/logos/git-scm/git-scm-icon.svg" alt="git" width="40" height="40"/> </a> <a href="https://www.w3.org/html/" target="_blank" rel="noreferrer"> <img src="https://raw.githubusercontent.com/devicons/devicon/master/icons/html5/html5-original-wordmark.svg" alt="html5" width="40" height="40"/> </a> <a href="https://developer.mozilla.org/en-US/docs/Web/JavaScript" target="_blank" rel="noreferrer"> <img src="https://raw.githubusercontent.com/devicons/devicon/master/icons/javascript/javascript-original.svg" alt="javascript" width="40" height="40"/> </a> <a href="https://www.microsoft.com/en-us/sql-server" target="_blank" rel="noreferrer"> <img src="https://www.svgrepo.com/show/303229/microsoft-sql-server-logo.svg" alt="mssql" width="40" height="40"/> </a> <a href="https://www.mysql.com/" target="_blank" rel="noreferrer"> <img src="https://raw.githubusercontent.com/devicons/devicon/master/icons/mysql/mysql-original-wordmark.svg" alt="mysql" width="40" height="40"/> </a> <a href="https://nodejs.org" target="_blank" rel="noreferrer"> <img src="https://raw.githubusercontent.com/devicons/devicon/master/icons/nodejs/nodejs-original-wordmark.svg" alt="nodejs" width="40" height="40"/> </a> </p>
---

## Installation & Setup

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- [Azure Service Bus](https://azure.microsoft.com/en-us/services/service-bus/)
-  IDE [Visual Studio](https://visualstudio.microsoft.com/) 

### Steps to Run the Application

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/yossiii050/ecommerce-microservices.git
   ```

2. **Navigate to the Project Directory:**

   ```bash
   cd ecommerce-microservices
   ```

3. **Configure the Services:**

   Each microservice has its own configuration file. Ensure that the database connection strings, Azure Service Bus settings, and any other required parameters are properly configured.

4. **Build and Run:**

   For each microservice, navigate to its directory and run:

   ```bash
   dotnet build
   dotnet run
   ```

   You can also use Docker to containerize and run the entire solution if Docker configurations are provided.

---

## Developer Notes

- **Architecture:** The project follows a clean architecture approach, emphasizing separation of concerns and dependency injection to ensure that each component is testable and maintainable.
- **Communication:** Services communicate asynchronously via Azure Service Bus for decoupled integration, and synchronously through the Ocelot API Gateway.
- **Code Quality:** Emphasis is placed on writing clean, well-documented code. Unit tests and integration tests are included to verify the behavior of critical components.
- **Extensibility:** The modular design allows for easy addition of new services or features without impacting the existing system.

---

## Contributing

Contributions are welcome. If you have suggestions or improvements, please open an issue or submit a pull request.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Contact

For any questions or further details, feel free to reach out:

- **Email:** yossiii050@gmail.com
- **LinkedIn:** [Your LinkedIn Profile](https://www.linkedin.com/in/yossi-yosupov3186/)

---

This README is intended to serve as both documentation for future development and a reference for understanding the architecture of the system. Enjoy exploring and extending the platform!
