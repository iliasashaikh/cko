﻿# Checkout payment gateway

## Build 

- The solution is developed in .NET 6/ Visual Studio 2022
- To build and run
  - Ensure that you have the .NET 6 runtime and SDK
  -  on your machine
  - **Clone the repository** from 
    - https://github.com/iliasashaikh/cko.git 
  - **To run the Payment Gateway API** 
    - open console and `cd` the to cloned directory
    - Now type `cd Cko.PaymentGateway`
    - ` dotnet run --project .\Cko.PaymentGateway.Api\Cko.PaymentGateway.Api.csproj`
  - To run the Banking simulator
    - open another console and `cd` to the cloned directory
    - Now type `cd Cko.PaymentGateway`
    - `dotnet run .\MyBank.Api\MyBank.Api.csproj`


## Design Principles

Although this is a fairly simple project and solution I have tried to follow some design principles. It might appear to be overly complex in some aspects and possibly not as fully featured in others, but those were choices based on the desire to demonstrate good coding practices but being conscious of time as well.

The key projects/ components in the solution are

## Database

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

## Areas for improvement
- **Database** - Since the use case is quite simple I could have used a non-relational database possibly cloud hosted, e.g. MongoDb Atlas, Cosmos Db etc.
- **Authentication/ Authorisation** - Ideally I would have liked to implement authentication & authorisation, either storing credentials in the  database and use ASP.NET Core Identity. Not sure how appropriate it would be to use external providers like Google, Microsoft etc.
- **HTTPS** - The APIs are hosted over Http. It would be fairly simple to set the APIs up for https, but it would have added to the complexity of running the code.

