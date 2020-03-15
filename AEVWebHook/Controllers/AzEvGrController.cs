using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AEVWebHook.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AEVWebHook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzEvGrController : ControllerBase
    {
        private readonly ILogger<AzEvGrController> _logger;
        private readonly string _cosmosConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private CosmosClient _cosmosClient;
        private readonly string _cosmosDefaultDb = "CoreDb";
        public AzEvGrController(ILogger<AzEvGrController> logger)
        {
            _logger = logger;
            _cosmosClient = new CosmosClient(_cosmosConnectionString);
        }

        [HttpPost("storages")]
        public async Task<IActionResult> HandleStorageEvent(
                [FromBody]EventGridEvent[] eventGridEvents
            )
        {
            try
            {

                Container storageEventsContainer = _cosmosClient.GetContainer(_cosmosDefaultDb, "StorageEvents");

                foreach (var eventGridEvent in eventGridEvents)
                {
                    if (eventGridEvent.EventType == EventTypes.EventGridSubscriptionValidationEvent)
                    {
                        SubscriptionValidationEventData subscriptionValidationEventData =
                            JsonConvert.DeserializeObject<SubscriptionValidationEventData>(eventGridEvent.Data.ToString());
                        return Ok(new SubscriptionValidationResponse
                        {
                            ValidationResponse = subscriptionValidationEventData.ValidationCode
                        });
                    }

                    StorageEventPayload payload = new StorageEventPayload(
                           eventGridEvent.Id,
                           eventGridEvent.EventType,
                           eventGridEvent.EventTime,
                           eventGridEvent.Subject,
                           eventGridEvent.Data?.ToString()
                        );
                    var result = await storageEventsContainer.CreateItemAsync<StorageEventPayload>(payload, new PartitionKey(payload.EventType));
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}