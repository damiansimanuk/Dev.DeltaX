using DeltaX.Domain.Common;
using System;

namespace DeltaX.Downtime.Application
{
    public interface IDowntimeApplicationService: IApplicationService
    {
        Guid PruebaGetUpdate();
        Guid? PruebaInsertAndUpdate(); 
    }
}