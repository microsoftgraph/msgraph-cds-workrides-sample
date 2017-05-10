# Overview
In [PowerApps](https://powerapps.microsoft.com/), you can manage organizational data by running an app that you created or that someone else, such as the HR manager in our scenario, created and shared with you. Apps run on mobile devices such as phones, or you can run them in a browser by opening Dynamics 365. You can create an infinite variety of apps – all without learning a programming language such as C#.

PowerApps is used in this sample to facilitate expense reimbursement based on the data that is stored in a [Common Data Service](http://aka.ms/commondataservice) environment. 

Key documentation:
- [Introduction to PowerApps](https://powerapps.microsoft.com/en-us/tutorials/getting-started/)

# Loading the PowerApps app
The PowerApps app used in this sample is made up of two simple screens and leverages the CDS connector to use the Fabrikam Ride Share (FabrikamRideShare_) entity as a datasource.

The definition for the PowerApps app is a MSAPP file: 
WorkRidesReimbursement.msapp

The PowerApps app can be opened saved into an environment using PowerApps Studio:
- Open PowerApps Studio
- Click on *Open*
- Click on *Browse* and supply the MSAPP file

Key documentation:
- [Environment and tenant app migration](https://powerapps.microsoft.com/en-us/tutorials/environment-and-tenant-migration/)

# From PowerApps to Common Data Service
[PowerApps](https://powerapps.microsoft.com/) has deep support for leveraging CDS and the [PowerApps portal](http://web.powerapps.com) is used to manage CDS environments. PowerApps apps use the CDS connector to leverage CDS entities as data sources. 

Key documentation:
- [Generate an app by using a Common Data Service database](https://powerapps.microsoft.com/en-us/tutorials/data-platform-create-app/)
- [Create an app from scratch using a Common Data Service database](https://powerapps.microsoft.com/en-us/tutorials/data-platform-create-app-scratch/)
