﻿# Checkout payment gateway

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
  - To run the Banking simulator
    - open another console and `cd` to the cloned directory
    - Now type `cd Cko.PaymentGateway`
    - `dotnet run .\MyBank.Api\MyBank.Api.csproj`





## Design Principles

Although this is a fairly simple project and solution I have tried to follow some design principles. It might appear to be overly complex in some aspects and possibly not as fully featured in others, but those were choices based on the desire to demonstrate good coding practices with some 

- The idea is to have a loosely coupled 'clean' architecture
- 

