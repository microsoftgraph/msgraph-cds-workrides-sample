using Microsoft.OData.Core;
using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CarPool.Clients.Core.Services.Data
{
    public static class ODataEntryExtensions
    {
        //Property removal code from ODataTeam: https://blogs.msdn.microsoft.com/odatateam/2013/07/26/using-the-new-client-hooks-in-wcf-data-services-client/
        public static void RemoveProperties(this ODataEntry entry, params string[] propertyNames)
        {
            var properties = entry.Properties as List<ODataProperty>;
            if (properties == null)
            {
                properties = new List<ODataProperty>(entry.Properties);
            }
            var propertiesToRemove = properties.Where(p => propertyNames.Any(rp => rp == p.Name));
            foreach (var propertyToRemove in propertiesToRemove.ToArray())
            {
                properties.Remove(propertyToRemove);
            }
            entry.Properties = properties;
        }

        public static DataServiceClientResponsePipelineConfiguration RemoveProperties<T>(
                          this DataServiceClientResponsePipelineConfiguration responsePipeline,
                          Func<string, Type> resolveType,
                          params string[] propertiesToRemove)
        {
            return responsePipeline.OnEntryEnded((args) =>
            {
                Type resolvedType = resolveType(args.Entry.TypeName);
                if (resolvedType != null && typeof(T).GetTypeInfo().IsAssignableFrom(resolvedType.GetTypeInfo()))
                {
                    args.Entry.RemoveProperties(propertiesToRemove);
                }
            });
        }

        public static DataServiceClientRequestPipelineConfiguration RemoveProperties<T>(
                            this DataServiceClientRequestPipelineConfiguration requestPipeline,
                            Func<string, Type> resolveType,
                            params string[] propertiesToRemove)
        {
            return requestPipeline.OnEntryStarting((args) =>
            {
                //For some reason, we need to trim the type name on the request pipeline
                string typeName = args.Entry.TypeName.Replace("CarPool.Clients.Core.Services.Data.", "");

                Type resolvedType = resolveType(typeName);
                if (resolvedType != null && typeof(T).GetTypeInfo().IsAssignableFrom(resolvedType.GetTypeInfo()))
                {
                    args.Entry.RemoveProperties(propertiesToRemove);
                }
            });
        }
    }
}
