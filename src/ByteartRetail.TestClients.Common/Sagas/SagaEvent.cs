using ByteartRetail.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteartRetail.TestClients.Common.Sagas
{
    public class SagaEvent : EventBase
    {
        public Guid SagaId { get; set; }

        public Guid SagaStepId { get; set; }

        public string? ServiceName { get; set; }

        public string? EventType { get; set; }

        public bool Succeeded { get; set; }

        public string? FailedReason { get; set; }

        public string? Payload { get; set; }
    }
}
