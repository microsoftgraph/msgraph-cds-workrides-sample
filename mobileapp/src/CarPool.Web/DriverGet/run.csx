#load "..\telemetrybridge.csx"
#load "..\EntityProxyClasses.csx"

using Microsoft.CommonDataService;
using Microsoft.CommonDataService.CommonEntitySets;
using Microsoft.CommonDataService.Configuration;
using Microsoft.CommonDataService.ServiceClient.Security;
using Microsoft.CommonDataService.ServiceClient.Security.Credentials;
using Microsoft.CommonDataService.Modeling;

using System;
using System.Collections.Generic;
using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, int? id,
                                                TraceWriter log)
{
    log.Info($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");

    string securityTokenString = "";
    if (req.Headers != null)
    {
        if (req.Headers.Authorization != null)
        {
            if (req.Headers.Authorization.Parameter != null)
            {
                securityTokenString = req.Headers.Authorization.Parameter;
            }
            else
            {
                log.Info($"req.Headers.Authorization.Parameter is null");
            }
        }
        else
        {
            log.Info($"req.Headers.Authorization is null");
        }
    }
    else
    {
        log.Info($"req.Headers is null");
    }
    log.Info($"1");

    log.Info($"securityTokenString={securityTokenString}");

    string payload;
    payload = await req.Content.ReadAsStringAsync();
    log.Info(payload);

    log.Info($"234");

    var connection = new ConnectionSettings
    {
        Tenant = "fabrikamco.onmicrosoft.com",
        EnvironmentId = "d54d812d-efb0-4787-b3c2-69659fb4e77f",
        Credentials = new UserImpersonationCredentialsSettings
        {
            ApplicationId = "1b54d472-a1b0-4844-984a-98703db55428",
            ApplicationSecret = "zb9UYbs/Mp8kDH/78CaHnASwjA6KjskaMxrXa1ySS3k="
        }
    };

    log.Info($"5");

    using (var client = await connection.CreateClient(req, new TraceWriterTelemetryBridge(log)))
    {
        log.Info($"Talking to CDS...");

        Console.WriteLine("Create query");
        var queryDriver = client.GetRelationalEntitySet<Driver>()
                          .CreateQueryBuilder()
                          .Project(entity => entity.SelectField(f => f.PrimaryId).SelectField(f => f.RouteTitle).SelectField(f => f.AverageMiles));

        Console.WriteLine("Run query");
        OperationResult<IReadOnlyList<Driver>> queryResultDriver = null;
        client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
              .Query(queryDriver, out queryResultDriver).ExecuteAsync().Wait();

        if (queryResultDriver.Result != null)
        {
            foreach (var i in queryResultDriver.Result)
            {
                //Console.WriteLine($"Driver record {i.PrimaryId} '{i.RouteTitle}', average miles {i.AverageMiles.ToString()}");
            }
            //Console.WriteLine("Query count for Driver: " + queryResultDriver.Result.Count.ToString());
        }
        //Console.WriteLine("Create, read, and update complete. Hit any key to continue...");


        string resultString = "";

        if (id == null)
            //Return all drivers
            resultString = "All drivers requested.";
        else
            //Return requested driver
            resultString = $"Driver item with id = {id} has been requested.";

        //            var exercise = Newtonsoft.Json.JsonConvert.DeserializeObject(payload, typeof(SessionLog)) as SessionLog;

        
        resultString += "Query count for Driver: " + queryResultDriver.Result.Count.ToString();

        log.Info($"C# HTTP trigger function completed.");
        return req.CreateResponse(HttpStatusCode.OK, resultString);
    }
    //}
}