using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteartRetail.TestClients.Common.Sagas
{
    public enum SagaStepStatus
    {
        Awaiting = 0,
        Started = 1,
        Failed = 2,
        Succeeded = 3,
        Compensating = 4,
        Compensated = 5,
        Cancelled = 6
    }
}
