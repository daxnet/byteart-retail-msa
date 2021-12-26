using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteartRetail.TestClients.Common.Sagas
{
    public enum SagaStatus
    {
        Created = 0,
        Started = 1,
        Aborting = 2,
        Aborted = 3,
        Completed = 4
    }
}
