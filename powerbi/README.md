# Overview
In [Power BI](https://powerbi.microsoft.com/), you can transform your company's data into rich visuals so you can focus on what matters to you. 

In this scenario, the HR manager is responsible for the success of the ridesharing program & creates a PowerBI dashboard based on the CDS data to understand riding patterns, common routes, and participation rates.

Key documentation
- [Power BI - basic concepts for Power BI service](https://powerbi.microsoft.com/en-us/documentation/powerbi-service-basic-concepts/)

# Connecting to Common Data Service data from Power BI
Entities in the Common Data Service can be exposed to Power BI by making the CDS environment visible to Power BI and then defining perspectives for the desired entities.  

Make the CDS environment visible to Power BI in the Admin Center:
- Open [PowerApps portal](http://web.powerapps.com)
- Click on *Settings* > *Admin center*
- In the Admin center, click on the desired environment
- Click on *Database* 
- Click the *Enable* button under the Power BI visibility message

Define perspectives for each desired entity:
- Open [PowerApps portal](http://web.powerapps.com)
- Expand *Common Data Service*
- Click on *Perspectives* (note that this feature is currently subject to a whitelist restiction)
- Click on *New perspective*
- Select the desired entity and save the perspective

# Loading the Power BI dashboard
The Power BI dashboard used in this sample provides views on the data in the *Fabrikam Ride Share* (FabrikamRideShare_) entity from CDS.

The definition for the Power BI dashboard is currently not available.

Key documentation:
- [Dashboards in Power BI service](https://powerbi.microsoft.com/en-us/documentation/powerbi-service-dashboards/)
