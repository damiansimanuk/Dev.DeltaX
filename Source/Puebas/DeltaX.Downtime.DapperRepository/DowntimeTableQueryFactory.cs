namespace DeltaX.Downtime.DapperRepository
{
    using DeltaX.Downtime.DapperRepository.Dto;
    using DeltaX.Repository.Common.Table;

    public class DowntimeTableQueryFactory : TableQueryFactory
    {
        public DowntimeTableQueryFactory() : base(Dialect.SQLite)
        {
            ConfigureTable<ProcessHistoryDto>("ProcessHistory", cfgTbl =>
            {
                cfgTbl.AddColumn(c => c.Id, null, false, true);
                cfgTbl.AddColumn(c => c.StartProcessDateTimeDb, nameof(ProcessHistoryDto.StartProcessDateTime));
                cfgTbl.AddColumn(c => c.FinishProcessDateTimeDb, nameof(ProcessHistoryDto.FinishProcessDateTime)); 
                cfgTbl.AddColumn(c => c.ProductSpecificationCode);
                cfgTbl.AddColumn(c => c.CreatedAt, c => { c.IgnoreInsert = true; c.IgnoreUpdate = true; });
            });

            ConfigureTable<InterruptionHistoryDto>("InterruptionHistory", cfgTbl =>
            {
                cfgTbl.AddColumn(c => c.Id, null, true, true);
                cfgTbl.AddColumn(c => c.ProcessHistoryId);
                cfgTbl.AddColumn(c => c.StartDateTime);
                cfgTbl.AddColumn(c => c.EndDateTime); 
                cfgTbl.AddColumn(c => c.CreatedAt, c => { c.IgnoreInsert = true; c.IgnoreUpdate = true; });
            });

            ConfigureTable<ProductSpecificationDto>("ProductSpecification", cfgTbl =>
            {
                cfgTbl.AddColumn(c => c.Id, null, true, false);
                cfgTbl.AddColumn(c => c.Code);
                cfgTbl.AddColumn(c => c.StandarDuration); 
                cfgTbl.AddColumn(c => c.CreatedAt, c => { c.IgnoreInsert = true; c.IgnoreUpdate = true; });
            });
        }
    }
}
