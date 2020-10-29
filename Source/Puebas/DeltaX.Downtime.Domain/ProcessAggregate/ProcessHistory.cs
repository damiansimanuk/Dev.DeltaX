using DeltaX.Domain.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DeltaX.Downtime.Domain.ProcessAggregate
{
    public class ProcessHistory : AggregateRoot<Guid>
    {
        public DateTime StartProcessDateTime { get; private set; }

        public DateTime? FinishProcessDateTime { get; private set; }

        public ProductSpecification ProductSpecification { get; private set; }

        public InterruptionHistory Interruption { get; private set; }

        private ProcessHistory()
        {
        }

        public ProcessHistory(Guid id, DateTime start, ProductSpecification productSpecification, DateTime? finish = null)
            : this()
        {
            Id = id;
            StartProcessDateTime = start;
            ProductSpecification = productSpecification;
            FinishProcessDateTime = finish;
        }

        public void FinishProcess(DateTime finish)
        {
            FinishProcessDateTime = finish;

            if (ProductSpecification.StandarDuration < (DateTime.Now - StartProcessDateTime).TotalSeconds)
            {
                UpdateInterruption();
            }
        }

        public void SetProductSpecification(ProductSpecification productSpecification)
        {
            ProductSpecification = productSpecification;
        }

        private void UpdateInterruption(bool enableOpenInterruption = false)
        {
            DateTime interruptionStart = StartProcessDateTime.AddSeconds(ProductSpecification.StandarDuration);
            DateTime? interruptionEnd = null;

            if (FinishProcessDateTime.HasValue)
            {
                var processTime = (FinishProcessDateTime.Value - StartProcessDateTime).TotalSeconds;
                if (processTime > ProductSpecification.StandarDuration)
                {
                    interruptionEnd = FinishProcessDateTime;

                    Interruption = new InterruptionHistory(interruptionStart, interruptionEnd);
                }
            }
            else if (enableOpenInterruption && interruptionStart < DateTime.Now)
            {
                Interruption = new InterruptionHistory(interruptionStart, interruptionEnd);
            }
        }
    }
}
