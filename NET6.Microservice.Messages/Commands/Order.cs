using System;

namespace NET6.Microservice.Messages.Commands
{
    public class Order
    {
        public Guid OrderId { get; set; }

        public double OrderAmount { get; set; }

        public string OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }
    }
}