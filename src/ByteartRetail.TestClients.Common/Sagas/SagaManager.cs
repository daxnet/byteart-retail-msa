using ByteartRetail.Common.DataAccess;
using ByteartRetail.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteartRetail.TestClients.Common.Sagas
{
    public class SagaManager
    {
        private readonly IDataAccessObject _dao;
        private readonly IEventPublisher _eventPublisher;

        public SagaManager(IDataAccessObject dao, IEventPublisher eventPublisher)
        {
            _dao = dao;
            _eventPublisher = eventPublisher;
        }

        public async Task<Saga> CreateAsync(IEnumerable<SagaStep> steps, CancellationToken cancellationToken = default)
        {
            var saga = new Saga(steps);
            await _dao.AddAsync(saga, cancellationToken);
            return saga;
        }

        public async Task Start(Saga saga, CancellationToken cancellationToken = default)
        {
            var sagaEvent = saga.Start();
            if (sagaEvent != null)
            {
                await _eventPublisher.PublishAsync(sagaEvent, sagaEvent.ServiceName, cancellationToken);
            }

            await _dao.UpdateByIdAsync(saga.Id, saga);
        }

        public async Task TransitAsync(SagaEvent sagaEvent, CancellationToken cancellationToken = default)
        {
            var saga = await _dao.GetByIdAsync<Saga>(sagaEvent.Id, cancellationToken);
            var nextStepEvent = saga.ProcessEvent(sagaEvent);
            if (nextStepEvent != null)
            {
                await _eventPublisher.PublishAsync(nextStepEvent, nextStepEvent.ServiceName, cancellationToken);
            }

            await _dao.UpdateByIdAsync(saga.Id, saga);
        }

        public async Task<Saga> GetByIdAsync(Guid sagaId, CancellationToken cancellationToken = default) 
            => await _dao.GetByIdAsync<Saga>(sagaId, cancellationToken);
    }
}
