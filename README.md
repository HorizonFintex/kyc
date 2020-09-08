# KYC API Sample

This sample demonstrates how to make different requests to our [KYC platform](https://www.kycware.com).

  - Request to upload a KYC pack
  - Request to list all KYC packs stored in Azure cold storage
  - Request to download a KYC pack from Azure cold storage
  - Request to approve a KYC pack from Azure cold storage
  - Request to remove a KYC pack from Azure cold storage

# How to run this sample project

## Pre-requisites

  - Install .NET Core for Windows by following the instructions at dot.net/core, which will include Visual Studio 2019.
  - Install the following two packages from NuGet:
  -- Microsoft.Extensions.Configuration
  -- Microsoft.Extensions.Hosting
  - A login account for [KYCWare platform](https://www.kycware.com)
  - KYCWare Platform developer guide, this is provided by Horizon Fintex, in the guide you can find values such as
  -- Client Id
  -- Client Secret
  -- Scope
  -- Grant Type
  -- Redirect Url
  -- KYC API Authorization Endpoint
  -- Service provider code

## Step 1: Clone or download this repository
From your command line or shell:
```
git clone https://github.com/HorizonFintex/KYCSample sample
cd sample
````

## Step 2: Request an authorization code
Go to 
```
https://www.kycware.com/api
```
You will be prompted to login with the account details provided by Horizon Fintex.
Upon login successfully, in the next screen you will see a button that you can click on to request the authorization code. By clicking the button you will be redirected to another login page (very similar to the first login page), after logged in, the authorization code is displayed in the screen. 
## Step 3: Request refresh token
At the end of previous step. In the screen there is one more option named "Request refresh token".
Follow the steps, you will be redirected to another screem that displays your refresh token.
Note that the refresh token is only valid for 14 days.

Please contact Horizon Fintex if you want to adjust the living period for the token.
## Step 4: Update appsettings.json under sample project
Now that you have acquired the refresh token, you can update appsettings.json file under sample project with this token and the values provided in the developer guide. Once done, click Save.

## Step 5: Run the sample project
Now you can run the project.

# Development 
In 

License
----

MIT


**KYCWare!**

