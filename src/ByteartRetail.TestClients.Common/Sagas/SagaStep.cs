using ByteartRetail.Common.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteartRetail.TestClients.Common.Sagas
{
    public abstract class SagaStep
    {
        public SagaStep() { }

        public SagaStep(string serviceName)
        {
            ServiceName = serviceName;
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        public SagaStepStatus Status { get; set; } = SagaStepStatus.Awaiting;

        public string? ServiceName { get; set; }

        public string? FailedReason { get; set; }

        protected abstract IEvent GetStepEventInternal();

        protected virtual IEvent? GetStepCompensateEventInternal() => null;

        public SagaEvent GetStepEvent(Guid sagaId)
        {
            var stepEvent = GetStepEventInternal();
            var payload = JsonConvert.SerializeObject(stepEvent);

            return new SagaEvent
            {
                SagaStepId = Id,
                ServiceName = ServiceName,
                EventType = stepEvent.GetType().FullName,
                SagaId = sagaId,
                Payload = payload
            };
        }

        public SagaEvent? GetStepCompensateEvent(Guid sagaId)
        {
            var compensateEvent = GetStepCompensateEventInternal();
            if (compensateEvent == null)
            {
                return null;
            }

            var payload = JsonConvert.SerializeObject(compensateEvent);
            return new SagaEvent
            {
                SagaStepId = Id,
                ServiceName = ServiceName,
                EventType = compensateEvent.GetType().FullName,
                SagaId = sagaId,
                Payload = payload
            };
        }
    }
}
