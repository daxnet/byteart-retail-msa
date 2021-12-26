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

        protected abstract (string, string) GetStepEventDefinitionInternal();

        protected virtual (string?, string?) GetStepCompensateEventInternal() => (null, null);

        public bool RequiresCompensate
        {
            get
            {
                var (type, parameters) = GetStepCompensateEventInternal();
                return !string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(parameters);
            }
        }

        public SagaEvent GetStepEvent(Guid sagaId)
        {
            var (stepEventTypeName, stepEventParameters) = GetStepEventDefinitionInternal();
            
            return new SagaEvent
            {
                SagaStepId = Id,
                ServiceName = ServiceName,
                EventType = stepEventTypeName,
                SagaId = sagaId,
                Payload = stepEventParameters
            };
        }

        public SagaEvent? GetStepCompensateEvent(Guid sagaId)
        {
            var (stepCompensateEventTypeName, stepCompensateEventParameters) = GetStepCompensateEventInternal();
            if (string.IsNullOrEmpty(stepCompensateEventTypeName) && string.IsNullOrEmpty(stepCompensateEventParameters))
            {
                return null;
            }

            return new SagaEvent
            {
                SagaStepId = Id,
                ServiceName = ServiceName,
                EventType = stepCompensateEventTypeName,
                SagaId = sagaId,
                Payload = stepCompensateEventParameters
            };
        }
    }
}
