using System;

namespace ServiceBusInvoker
{
    public class MessageItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MessageContent { get; set; }
    }
}
