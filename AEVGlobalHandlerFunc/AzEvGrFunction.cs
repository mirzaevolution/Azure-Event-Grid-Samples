// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AEVGlobalHandlerFunc
{
    public static class AzEvGrFunction
    {
        [FunctionName("AzEvGrFunction")]
        public static async Task Run(
            [EventGridTrigger]EventGridEvent eventGridEvent,
            [CosmosDB("CoreDb", "StorageEvents", ConnectionStringSetting = "AzureCosmosDBConnectionString")] IAsyncCollector<StorageEventPayload> eventCollector,
            ILogger log)
        {
            log.LogInformation($"Receiving event grid payload: {eventGridEvent.EventType}");
            try
            {
                if (eventGridEvent.Data != null)
                {
                    StorageEventPayload storageEventPayload = new StorageEventPayload(
                            eventGridEvent.Id,
                            eventGridEvent.EventType,
                            eventGridEvent.EventTime,
                            eventGridEvent.Subject,
                            eventGridEvent.Data.ToString()
                        );

                    await eventCollector.AddAsync(storageEventPayload);
                    log.LogInformation("Event has been processed and sent to cosmos db");
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
            }
        }
    }
}
