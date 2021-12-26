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



## Design Principles

Although this is a fairly simple project and solution I have tried to follow some clean-code design principles. It might appear to be overly complex in some aspects and possibly not as fully featured in others, but those were choices based on the desire to demonstrate good coding practices but being conscious of time as well.

## Database
- I have used a Sql server database 
- The database is included in the repository and consists of
  - Cko.PaymentGateway\Db\cko.mdf
  - Cko.PaymentGateway\Db\cko.ldf

### DataModel

Entities




## Projects

### Gateway API
- api/Cko.PaymentGateway.Api
- domain/Cko.PaymentGateway.Entities
- domain/Cko.PaymentGateway.Models
- domain/Cko.PaymentGateway.Repository
- services/Cko.PaymentGateway.Services


### Bank Simulator
- MyBank.API

## Key Libraries
- Polly - resiliency framework
- Refit - http client factory & used for Bank Sdk
- Dapper - fast data access
- Serilog - logging framework
- FluentValidation
- NUnit - testing
- Swashbuckle - Swagger API documentation
- AutoMapper


## Data flow
- Merchant calls the Checkout API at localhost/api/processPayment with a `PaymentRequest` as Json in the Request body
- The controller calls a service `Payment Processor` to process the request
- If the customer exists in the database, the service uses the stored card details if those aren't provided, else uses the one stored in the database
- If the card is valid, the Bank Api is called using a `Refit` wrapper


## Areas for improvement
- **Database** - Since the use case is quite simple I could have used a non-relational database possibly cloud hosted, e.g. MongoDb Atlas, Cosmos Db etc.
- **Authentication/ Authorisation** - Ideally I would have liked to implement authentication & authorisation, either storing credentials in the  database and use ASP.NET Core Identity. Not sure how appropriate it would be to use external providers like Google, Microsoft etc.
- **HTTPS** - The APIs are hosted over Http. It would be fairly simple to set the APIs up for https, but it would have added to the complexity of running the code.

