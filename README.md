- [Checkout payment gateway](#checkout-payment-gateway)
  - [Build](#build)
    - [Sample](#sample)
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
![image](https://user-images.githubusercontent.com/196512/147661511-f31701f0-8c03-4810-a380-cfdd9076ae46.png)

## Build 

The solution is developed in **.NET 6**/ **Visual Studio 2022**

- **To build and run**
  - Ensure that you have the .NET 6 runtime and SDK on your machine
  - **Clone the repository** from 
    - https://github.com/iliasashaikh/cko.git
  - The solution includes Sql database files that can be attached to an instance of LocalDB. The location of the db files may need updating in the config.
  - Open the file `(repoRoot)\Cko.PaymentGateway\Cko.PaymentGateway.Api\appsettings.json`
  - Update the setting for connection string for the database `CkoDb` by replacing `c:\\dev\\cko\\` to the local repo folder.
  - Similarly, update the setting for the integration testing db at `(repoRoot)\Cko.PaymentGateway\Tests\Cko.PaymentGateway.IntegrationTests2\appsettings.json`
  - **To run the Payment Gateway API** 
    - open console and `cd` the to cloned directory
    - Now type `cd Cko.PaymentGateway`
    - ` dotnet run --project .\Cko.PaymentGateway.Api\Cko.PaymentGateway.Api.csproj`
    - Open http://localhost:5678/index.html in your browser to see the API documentation
  - **To run the Banking simulator**
    - open another console and `cd` to the cloned directory
    - Now type `cd Cko.PaymentGateway`
    - `dotnet run --project .\MyBank.Api\MyBank.Api.csproj`

### Sample

Run both the API and Bank simulator

**Create a payment**

- Open the swagger page by browsing to http://localhost:5678/index.html 
- To create a new `Payment` Post this `PaymentRequest` to `/api/Payments`
```  
 {
     "customerName": "John",
     "customerAddress": "Doe",
     "customerReference": "00000000-0000-0000-0000-000000000000",
     "cardNumber": "5555555555554444",
     "cvv": "1234",
     "cardExpiry": "2022-12-28T13:53:46.844Z",
     "bankIdentifierCode": "hsbc",
     "saveCustomerDetails": true,
     "itemDetails": "something bought",
     "amount": 10,
     "ccy": "usd",
     "merchantId": 1,
     "merchantName": "amazon"
 }
```    
![image](https://user-images.githubusercontent.com/196512/147667790-61bc3551-0c83-4d03-a3f4-6b017682118e.png)

This should 
- create a new Payment and Customer in the `Cko` database
- make a call to the Bank simulator 
- process payment and return a response with a Guid `PaymentReference` and int `PaymentId`
- Make a note of the `PaymentId` returned 

![image](https://user-images.githubusercontent.com/196512/147667887-d8991b53-ff7a-484d-a533-984d80237113.png)


**Return payment details**
- Payment details can be retrieved from /api/{PaymentId}
![image](https://user-images.githubusercontent.com/196512/147667925-8e3042af-0f78-48c9-a137-78464a21681e.png)

Response
![image](https://user-images.githubusercontent.com/196512/147667970-3c276271-9011-46dc-8a90-e34db1b92f49.png)


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
Unit testing uses NUnit and use NSubstitute for mocking all Db and external API calls
- test/Cko.PaymentGateway.UnitTests

#### **Integrations Tests**
Integration tests make use of the `WebApplicationFactory` to create an in-memory web server. The integration tests are in the project
- test\Cko.PaymentGateway.IntegrationTests2
- Tests use a copy of the database on the `db\Test` folder
- All tests clear down the database before running and populate data as necessary

## Key External Libraries
- **Refit** - http client factory & used for Bank Sdk
- **Dapper** - fast data access
- **Serilog** - logging framework
- **FluentValidation** - for validation
- **NUnit** - testing
- **Swashbuckle** - Swagger API documentation
- **AutoMapper**
- **NSubstitute** - Mocking framework to mock dependencies for unit tests
- **Scrutor** - to scan and register with DI container

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
- All dataaccess is via Repositories


## Areas for improvement
- **Database** - Since the use case is quite simple I could have used a non-relational database possibly cloud hosted, e.g. MongoDb Atlas, Cosmos Db etc.
- **Authentication/ Authorisation** - Ideally I would have liked to implement authentication & authorisation, either storing credentials in the  database and use ASP.NET Core Identity. Not sure how appropriate it would be to use external providers like Google, Microsoft etc.
- **HTTPS** - The APIs are hosted over Http. It would be fairly simple to set the APIs up for https, but it would have added to the complexity of running the code.
- To improve resilience in production code, I would ideally incorporate the library Polly.NET.
- **Container/ Docker** - I could have packaged the entire solution with full sql server in a Docker container.

