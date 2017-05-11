#load "..\telemetrybridge.csx"

using Microsoft.CommonDataService;
using Microsoft.CommonDataService.CommonEntitySets;
using Microsoft.CommonDataService.Configuration;
using Microsoft.CommonDataService.ServiceClient.Security;
using Microsoft.CommonDataService.ServiceClient.Security.Credentials;
using Microsoft.CommonDataService.Modeling;

using System;
using System.Collections.Generic;
using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
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

    log.Info($"2");

    //using (var client1 = await Support.GetClient(securityTokenString, log))
    //{

    // parse query parameter
    string name = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
        .Value;

    log.Info($"3");

    if (name == null)
        {
            log.Info($"Name was not passed correctly.");
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

    log.Info($"4");

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

            // Query product categories for Surfaces and Phones
            var query = client.GetRelationalEntitySet<ProductCategory>()
                .CreateQueryBuilder()
                .Where(pc => pc.Name == "Surface" || pc.Name == "Phone")
                .Project(pc => pc.SelectField(f => f.CategoryId).SelectField(f => f.Name));

            OperationResult<IReadOnlyList<ProductCategory>> queryResult = null;
            client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                .Query(query, out queryResult)
                .ExecuteAsync().Wait();

            // Delete any Surfaces and Phones
            var deleteExecutor = client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional);
            foreach (var entry in queryResult.Result)
            {
                deleteExecutor.DeleteWithoutConcurrencyCheck(entry);
            }
            deleteExecutor.ExecuteAsync().Wait();

            log.Info($"Deleted product lines");

            // Insert Surface and Phone product lines
            var surfaceCategory = new ProductCategory() { Name = "Surface", Description = "Surface product line" };
            var phoneCategory = new ProductCategory() { Name = "Phone", Description = "Phone product line" };
            await client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                .Insert(surfaceCategory)
                .Insert(phoneCategory)
                .ExecuteAsync();

            log.Info($"Inserted product lines");

            // Query for Surface and Phone Product lines
            query = client.GetRelationalEntitySet<ProductCategory>()
                .CreateQueryBuilder()
                .Where(pc => pc.Name == name)
                .OrderByAscending(pc => new object[] { pc.CategoryId })
                .Project(pc => pc.SelectField(f => f.CategoryId).SelectField(f => f.Name).SelectField(f => f.Description));

            await client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                .Query(query, out queryResult)
                .ExecuteAsync();

            //// Update all selected Product Lines with description
            //var updateExecutor = client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional);
            //foreach (var entry in queryResult.Result)
            //{
            //    log.Info($"Updating '{entry.Name}'.");
            //    var updateProductCategory = client.CreateRelationalFieldUpdates<ProductCategory>();
            //    string updatedDescription = $"{DateTime.Now.ToString()} - Updated '{entry.Name}'";
            //    updateProductCategory.Update(pc => pc.Description, updatedDescription);

            //    updateExecutor.Update(entry, updateProductCategory);
            //}
            //await updateExecutor.ExecuteAsync();

            log.Info("Query count for ProductCategory: " + queryResult.Result.Count.ToString());

            log.Info($"C# HTTP trigger function completed.");
            return req.CreateResponse(HttpStatusCode.OK);
        }
    //}
}