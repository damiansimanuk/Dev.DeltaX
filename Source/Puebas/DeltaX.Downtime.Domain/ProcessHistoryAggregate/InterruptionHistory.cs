using DeltaX.Domain.Common;
using DeltaX.Domain.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaX.Downtime.Domain.ProcessAggregate
{
    public class InterruptionHistory : Entity<int>
    {
        public DateTime StartDateTime { get; private set; }

        public DateTime? EndDateTime { get; private set; }
          
        private InterruptionHistory() { }

        public InterruptionHistory(DateTime start, DateTime? end)
        {
            StartDateTime = start;
            EndDateTime = end; 
        }
    }
}
