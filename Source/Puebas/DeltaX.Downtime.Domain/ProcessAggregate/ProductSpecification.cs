using DeltaX.Domain.Common;
using DeltaX.Domain.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaX.Downtime.Domain.ProcessAggregate
{
    public class ProductSpecification : Entity <int>
    {
        public ProductSpecification  (string code) 
        {
            Code = code;
        }
          
        public string Code { get; private set; }

        public int StandarDuration { get; private set; }
    }
}
