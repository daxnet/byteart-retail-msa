﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteartRetail.TestClients.Common.Sagas
{
    public enum SagaStepStatus
    {
        Awaiting,
        Started,
        Failed,
        Succeeded,
        Compensating,
        Compensated
    }
}
