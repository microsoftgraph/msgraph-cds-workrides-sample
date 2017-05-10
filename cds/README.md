# Overview 
The [Microsoft Common Data Service](http://aka.ms/commondataservice) (CDS) is a focal point for a business's data. Professional developers can write applications that interact with that data in CDS using the CDS SDK. And power users can take advantage of PowerApps, Flow, and PowerBI to create apps, design workflows, and perform deep analytics over that CDS data without writing any code.

Key documentation:
- [Common Data Service](http://aka.ms/commondataservice)

# Why would a developer want to use both Microsoft Graph and CDS?

Microsoft Graph gives you access to rich data from Microsoft services. Once you bring that data to CDS, you can combine it with other business data that your business depends on. Then you can build rich applications and workflows with easy to use building blocks… whether you know how to write code or not.

# Data Model
The data model used by the sample is three entities:
- Fabrikam Drivers (FabrikamDrivers_)
- Fabrikam Employee (FabrikamEmployee_)
- Fabrikam Ride Share (FabrikamRideShare_)

The definition for the entities is contained in a package file: 
FabrikamCo-CDS-Resources.pkg

The package file can be imported into an environment using the Admin Center in the PowerApps portal:
- Open [PowerApps portal](http://web.powerapps.com)
- Click on *Settings* > *Admin center*
- In the Admin center, click on the desired environment
- Click on *Import resources* and supply the package file

Once the data model has been loaded, then data is most easily reviewed and edited using the [Excel Add-in](https://powerapps.microsoft.com/en-us/tutorials/data-platform-interactive-excel/). 

Key documentation:
- [Understand entities in the Common Data Service](https://powerapps.microsoft.com/en-us/tutorials/data-platform-intro/)
- [Open entity data in Excel](https://powerapps.microsoft.com/en-us/tutorials/data-platform-interactive-excel/)

# AAD required permissions for Common Data Service apps
To access to the Common Data Service APIs from the Xamarin mobile apps, the AAD application needs permissions added for *PowerApps Runtime Service* and *Windows Azure Service Management API*.

The required permissions for the WorkRides app to work with the CDS APIs are:
* PowerApps Runtime Service
  * Add all delegated permissions
* Windows Azure Service Management API
  * Add all delegated permissions

Detailed information about application registration for an app that will interact with CDS can be found in [Get started with the Common Data Service SDK - Application registration](https://docs.microsoft.com/en-us/common-data-service/entity-reference/cds-sdk-get-started#application-registration-1)

# From the Xamarin App to Common Data Service
The Xamarin app connects to the customer's Common Data Service environment to access data about employees and store driver and ride details data.  From the Xamarin app, we used the CDS SDK to interact with CDS. 

Documentation for the CDS SDK can be found at [Building apps with the Common Data Service](https://docs.microsoft.com/en-us/common-data-service/entity-reference/cds-sdk-home-page). Unfortunately the CDS SDK is currently not PCL compatible so it can't be called directly from a Xamarin app, so the CDS SDK code should be wrapped by a web API or a series of Azure Functions. In the future, we'll be working on a way to access CDS data from a Xamarin app in a single step.  
In the sample, a web API has been built. The interaction with CDS from the Xamarin App is therefore:
*Xamarin App* > *Web API* > *CDS SDK* > *CDS*

The Web API can be seen in the *CarPool.WebApp* project. An example of the SDK code in the web API can be seen in the *CarPool.WebApp.Controllers.EmployeeController* class.

The code that calls the Web API can be seen in the *CarPool.Clients.Core* project. Specifically the *CarPool.Clients.Core.Services.Data.CDSDataProvider* class contains all the Web API interaction code.

Key documentation:
- [Get started with the Common Data Service SDK](https://docs.microsoft.com/en-us/common-data-service/entity-reference/cds-sdk-get-started)
- [Get started with the Common Data Service SDK via Azure Functions](https://docs.microsoft.com/en-us/common-data-service/entity-reference/cds-sdk-azure-functions-get-started)

# From PowerApps to Common Data Service
[PowerApps](https://powerapps.microsoft.com/) has deep support for leveraging CDS and the [PowerApps portal](http://web.powerapps.com) is used to manage CDS environments. PowerApps apps use the CDS connector to leverage CDS entities as data sources. 

Key documentation:
- [Generate an app by using a Common Data Service database](https://powerapps.microsoft.com/en-us/tutorials/data-platform-create-app/)
- [Create an app from scratch using a Common Data Service database](https://powerapps.microsoft.com/en-us/tutorials/data-platform-create-app-scratch/)



