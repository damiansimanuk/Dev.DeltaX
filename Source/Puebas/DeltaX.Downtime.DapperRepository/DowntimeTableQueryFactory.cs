namespace DeltaX.Downtime.DapperRepository
{
    using DeltaX.Downtime.DapperRepository.Models;
    using DeltaX.Repository.Common.Table;

    public class DowntimeTableQueryFactory : TableQueryFactory
    {
        public DowntimeTableQueryFactory() : base(Dialect.SQLite)
        {
            ConfigureTable<ProcessHistoryModel>("ProcessHistory", cfgTbl =>
            {
                cfgTbl.AddColumn(c => c.Id, null, false, true);
                cfgTbl.AddColumn(c => c.StartProcessDateTimeDb, nameof(ProcessHistoryModel.StartProcessDateTime));
                cfgTbl.AddColumn(c => c.FinishProcessDateTimeDb, nameof(ProcessHistoryModel.FinishProcessDateTime)); 
                cfgTbl.AddColumn(c => c.ProductSpecificationCode);
                cfgTbl.AddColumn(c => c.CreatedAt, c => { c.IgnoreInsert = true; c.IgnoreUpdate = true; });
            });

            ConfigureTable<InterruptionHistoryModel>("InterruptionHistory", cfgTbl =>
            {
                cfgTbl.AddColumn(c => c.Id, null, true, true);
                cfgTbl.AddColumn(c => c.ProcessHistoryId);
                cfgTbl.AddColumn(c => c.StartDateTime);
                cfgTbl.AddColumn(c => c.EndDateTime); 
                cfgTbl.AddColumn(c => c.CreatedAt, c => { c.IgnoreInsert = true; c.IgnoreUpdate = true; });
            });

            ConfigureTable<ProductSpecificationModel>("ProductSpecification", cfgTbl =>
            {
                cfgTbl.AddColumn(c => c.Id, null, true, false);
                cfgTbl.AddColumn(c => c.Code);
                cfgTbl.AddColumn(c => c.StandarDuration); 
                cfgTbl.AddColumn(c => c.CreatedAt, c => { c.IgnoreInsert = true; c.IgnoreUpdate = true; });
            });
        }
    }
}
