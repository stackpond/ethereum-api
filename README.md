[![Build Status](https://travis-ci.com/stackpond/ethereum-api.svg?branch=main)](https://travis-ci.com/stackpond/ethereum-api)

# Ethereum Explorer API

This API internally consumes Infura API (refer https://infura.io/docs/ethereum) to get Ethereum blockchain transactions.

Try out the API deployed in Heroku at https://stackpond-ethereum-api.herokuapp.com/swagger.

My other project Ethereum Explorer (refer https://github.com/stackpond/ethereum-explorer) consumes this API to display Ethereum transactions.

## Development setup

|Software|Download Link|
|---|---|
|.NET Core 3.1|https://dotnet.microsoft.com/download|
|Microsoft Visual Studio Professional 2019|https://visualstudio.microsoft.com/downloads/|


**Commands below can be executed in the command line mode if Visual Studio IDE is not used**

### Build

Run `dotnet build` to build.

### Running

|OS|Command|
|---|---|
|Windows|`dotnet run --project .\EthereumApi.Web\EthereumApi.Web.csproj`|
|Non-windows|`dotnet run --project ./EthereumApi.Web/EthereumApi.Web.csproj`|

### Test

Run `dotnet test` to test.

## Important notes

The transaction results are paged for better user experience.

Infura API has rate limit imposed (refer https://infura.io/pricing) which may also slow down the processing.

**The search by address is especially slow because Infura API does not provide any API for this use. Hence every transactions of every block needs to be processed and it can take a very long time before the results are fetched. I have few ideas to speed this up by creating an indexer service separate from core API which can then be consumed for faster processing. This is TBD.**
