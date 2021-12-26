using ByteartRetail.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteartRetail.TestClients.Common.Sagas
{
    public class Saga : IEntity
    {
        #region Public Constructors

        // public Saga() { }

        public Saga(IEnumerable<SagaStep> steps)
        {
            Steps.AddRange(steps);
        }

        #endregion Public Constructors

        #region Public Properties

        public string? AbortingReason { get; set; }

        public Guid CurrentStepId { get; set; }

        /// <summary>
        /// Gets or sets the Saga ID.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        public List<Guid> ProcessedEventIds { get; set; } = new();
        public SagaStatus Status { get; set; } = SagaStatus.Created;
        public List<SagaStep> Steps { get; set; } = new();

        #endregion Public Properties

        #region Private Properties

        private SagaStep? CurrentStep => Steps.FirstOrDefault(s => s.Id == CurrentStepId);

        private SagaStep? NextStep
        {
            get
            {
                var idx = Steps.FindIndex(s => s.Id == CurrentStepId);
                if (idx == -1)
                {
                    return null;
                }

                Interlocked.Increment(ref idx);
                if (idx == Steps.Count)
                {
                    return null;
                }

                return Steps[idx];
            }
        }

        private SagaStep? PreviousStep
        {
            get
            {
                var idx = Steps.FindIndex(s => s.Id == CurrentStepId);
                if (idx == -1)
                {
                    return null;
                }

                Interlocked.Decrement(ref idx);
                if (idx < 0)
                {
                    return null;
                }

                return Steps[idx];
            }
        }

        #endregion Private Properties

        #region Public Methods

        public SagaEvent? ProcessEvent(SagaEvent sagaEvent)
        {
            if (EventProcessed(sagaEvent))
            {
                return null;
            }

            var currentStep = CurrentStep;
            if (currentStep == null) return null;
            
            if (sagaEvent.Succeeded)
            {
                currentStep.Status = currentStep.Status switch
                {
                    SagaStepStatus.Started => SagaStepStatus.Succeeded,
                    SagaStepStatus.Compensating => SagaStepStatus.Compensated,
                    _ => throw new InvalidOperationException()
                };
            }
            else
            {
                if (currentStep.Status == SagaStepStatus.Started)
                {
                    CancelSubsequentSteps();
                }
                currentStep.Status = currentStep.Status switch
                {
                    SagaStepStatus.Started => SagaStepStatus.Failed,
                    SagaStepStatus.Compensating => SagaStepStatus.Failed,
                    _ => throw new InvalidOperationException()
                };
            }

            SagaEvent? firingEvent = null;
            switch (currentStep.Status)
            {
                case SagaStepStatus.Succeeded:
                    firingEvent = GoNext()?.GetStepEvent(this.Id);
                    break;
                case SagaStepStatus.Compensated:
                case SagaStepStatus.Failed:
                    firingEvent = GoPrevious()?.GetStepCompensateEvent(this.Id);
                    break;
            }
                
            UpdateSagaStatus();
            MarkProcessed(sagaEvent);

            return firingEvent ?? null;
        }
        
        public SagaEvent? Start()
        {
            if (Status == SagaStatus.Created && Steps.Any())
            {
                var firstStep = Steps[0];
                CurrentStepId = firstStep.Id;
                Status = SagaStatus.Started;
                firstStep.Status = SagaStepStatus.Started;
                return firstStep.GetStepEvent(this.Id);
            }

            Status = SagaStatus.Aborted;
            AbortingReason = "Saga status is invalid or there is no steps defined in the Saga.";
            return null;
        }

        #endregion Public Methods

        #region Private Methods

        private bool EventProcessed(SagaEvent sagaEvent) => ProcessedEventIds.Contains(sagaEvent.Id);

        private SagaStep? GoNext()
        {
            var next = NextStep;
            if (next == null)
            {
                return null;
            }

            next.Status = SagaStepStatus.Started;
            CurrentStepId = next.Id;
            return next;
        }

        private SagaStep? GoPrevious()
        {
            var prev = PreviousStep;
            while (prev != null)
            {
                CurrentStepId = prev.Id;
                if (prev.RequiresCompensate)
                {
                    prev.Status = SagaStepStatus.Compensating;
                    return prev;
                }

                prev = PreviousStep;
            }

            return null;
        }
        private void MarkProcessed(SagaEvent sagaEvent) => ProcessedEventIds.Add(sagaEvent.Id);
        
        private void CancelSubsequentSteps()
        {
            var currentStepIdx = Steps.FindIndex(x => x.Id == CurrentStepId);
            if (currentStepIdx >= 0)
            {
                for (var idx = currentStepIdx + 1; idx < Steps.Count; idx++)
                {
                    Steps[idx].Status = SagaStepStatus.Cancelled;
                }
            }
        }

        private void UpdateSagaStatus()
        {
            if (Steps.All(s => s.Status == SagaStepStatus.Succeeded))
            {
                Status = SagaStatus.Completed;
            }
            else if (Steps.All(s => s.Status == SagaStepStatus.Succeeded || 
                        s.Status == SagaStepStatus.Started || 
                        s.Status == SagaStepStatus.Awaiting))
            {
                Status = SagaStatus.Started;
            }
            else if (Steps.All(s => s.Status == SagaStepStatus.Succeeded ||
                                    s.Status == SagaStepStatus.Failed ||
                                    s.Status == SagaStepStatus.Compensated ||
                                    s.Status == SagaStepStatus.Cancelled))
            {
                Status = SagaStatus.Aborted;
            }
            else
            {
                Status = SagaStatus.Aborting;
            }
        }

        #endregion Private Methods
    }
}
