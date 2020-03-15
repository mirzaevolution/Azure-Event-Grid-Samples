using Newtonsoft.Json;
using System;

namespace AEVWebHook.Models
{
    public class StorageEventPayload
    {
        public StorageEventPayload()
        {

        }
        public StorageEventPayload(string eventId, string eventType, DateTime eventTime, string subject, string jsonData)
        {
            EventId = eventId;
            EventType = eventType;
            EventTime = eventTime;
            Subject = subject;
            JsonData = jsonData;

        }
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonProperty("eventid")]
        public string EventId { get; set; }
        [JsonProperty("eventType")]
        public string EventType { get; set; }
        [JsonProperty("eventTime")]
        public DateTime EventTime { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("jsonData")]
        public string JsonData { get; set; }
    }
}
