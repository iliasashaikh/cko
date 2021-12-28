- [Checkout payment gateway](#checkout-payment-gateway)
  - [Build](#build)
  - [Database](#database)
    - [DataModel](#datamodel)
    - [Data Access](#data-access)
  - [Projects](#projects)
    - [Gateway API](#gateway-api)
    - [Bank Simulator](#bank-simulator)
    - [Testing](#testing)
      - [**Unit Tests**](#unit-tests)
      - [**Integrations Tests**](#integrations-tests)
  - [Key External Libraries](#key-external-libraries)
  - [Design decisions](#design-decisions)
  - [Data flow](#data-flow)
  - [Areas for improvement](#areas-for-improvement)

# Checkout payment gateway

## Build 

- The solution is developed in .NET 6/ Visual Studio 2022
- To build and run
  - Ensure that you have the .NET 6 runtime and SDK on your machine
  - **Clone the repository** from 
    - https://github.com/iliasashaikh/cko.git 
  - **To run the Payment Gateway API** 
    - open console and `cd` the to cloned directory
    - Now type `cd Cko.PaymentGateway`
    - ` dotnet run --project .\Cko.PaymentGateway.Api\Cko.PaymentGateway.Api.csproj`
    - Open http://localhost:5678/swagger/index.html in your browser to see the API documentation
  - To run the Banking simulator
    - open another console and `cd` to the cloned directory
    - Now type `cd Cko.PaymentGateway`
    - `dotnet run .\MyBank.Api\MyBank.Api.csproj`


## Database
- I have used a Sql server single file database. This choice was basically to make it easier to ship the database in this demo app and is included in the repository and consists of
  - Cko.PaymentGateway\Db\cko.mdf
  - Cko.PaymentGateway\Db\cko.ldf
> Although the database doesn't need a full fledged database, it is necessary to have a Sql runtime for LocalDb. This can be installed from [SqlLocalDb](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?redirectedfrom=MSDN&view=sql-server-ver15)

### DataModel

**Entities**

- Payment 
  - Stores all information for a payment request
  - Stores the current state on the payment and is used to retrieve Payment details at a later time
- Merchant
  - Represents a merchant, the payment request ***must*** include an existing MerchantID
- Bank
  - Used to store the API for a bank
- Payment Card
  - Stores customer card details
- Customer
  - Stores customer details

### Data Access
- All dataaccess is via 'Repositories' using the `Repository` pattern.
- Uses `Dapper` as a micro-orm
  
> Why not Entity Framework?
> 
> Dapper is more performant, and as someone who is fairly comfortable with writing sql, I usually prefer Dapper to EF.



## Projects

### Gateway API
- api/Cko.PaymentGateway.Api
- domain/Cko.PaymentGateway.Entities
- domain/Cko.PaymentGateway.Models
- domain/Cko.PaymentGateway.Repository
- services/Cko.PaymentGateway.Services


### Bank Simulator
This is a very simple .NET 6 API built using the new `Minimal API` syntax, with hard coded responses.
The project is included in the solution.
- MyBank.API

### Testing

#### **Unit Tests**
Unit testing uses NUnit.
- test/Cko.PaymentGateway.UnitTests

#### **Integrations Tests**
Integration tests make use of the `WebApplicationFactory` to create an in-memory web server. The integration tests are in the project
- test/Cko.PaymentGateway.IntegrationTests

## Key External Libraries
- Refit - http client factory & used for Bank Sdk
- Dapper - fast data access
- Serilog - logging framework
- FluentValidation
- NUnit - testing
- Swashbuckle - Swagger API documentation
- AutoMapper
- NSubstitute - Mocking framework to mock dependencies for unit tests
- Scrutor - to scan and register with DI container

## Design decisions

Although this is a fairly simple project and solution I have tried to follow some clean-code design principles. It might appear to be overly complex in some aspects and possibly not as fully featured in others, but those were choices based on the desire to demonstrate good coding practices but being conscious of time as well.

- The `Controller` is very slim, and all it does is call into other services that implement the logic.
- All entities and model class properties are non-nullable
- Internal methods are tested using the `InternalsVisibleTo` assembly attribute.
- The Data Model is deliberately simple, e.g. All Names and Addresses are a single field/ column and not split into constituent parts as would be in a production system.
  

## Data flow
- Merchant calls the Checkout API at localhost/api/processPayment with a `PaymentRequest` as Json in the Request body
- The controller calls a service `Payment Processor` to process the request
- If the customer exists in the database, the service uses the stored card details if those aren't provided, else uses the one stored in the database
- If the card is valid, the Bank Api is called using a `Refit` wrapper

**Assumptions**
- It is expected that the Merchant has been setup in the system previously and the request includes a valid `MerchantId` that is present in the Checkout Db.
- The customer details can be looked up from the database if a GUID for a customer is passed in, else the payment request should contain all customer details necessary.
- The merchant can ask for the customer details to be saved, in which case they are saved and the response includes a generated customer reference.
- All dataaccess uses the 


## Areas for improvement
- **Database** - Since the use case is quite simple I could have used a non-relational database possibly cloud hosted, e.g. MongoDb Atlas, Cosmos Db etc.
- **Authentication/ Authorisation** - Ideally I would have liked to implement authentication & authorisation, either storing credentials in the  database and use ASP.NET Core Identity. Not sure how appropriate it would be to use external providers like Google, Microsoft etc.
- **HTTPS** - The APIs are hosted over Http. It would be fairly simple to set the APIs up for https, but it would have added to the complexity of running the code.
- To improve resilience in production code, I would ideally incorporate the library Polly.NET.

