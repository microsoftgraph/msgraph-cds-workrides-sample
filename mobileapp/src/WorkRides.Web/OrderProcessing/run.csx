using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, string category, int? id,
                                                TraceWriter log)
{
    if (id == null)
        return req.CreateResponse(HttpStatusCode.OK, $"All {category} items were requested.");
    else
        return req.CreateResponse(HttpStatusCode.OK, $"{category} item with id = {id} has been requested.");
}