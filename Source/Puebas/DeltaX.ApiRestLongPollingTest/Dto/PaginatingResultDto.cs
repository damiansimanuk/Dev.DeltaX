namespace DeltaX.ApiRestLongPollingTest.Dto
{ 
    using System.Collections.Generic;

    public class PaginatingResultDto<T>
    {
        public PaginatingResultDto() { }

        public PaginatingResultDto(long totalCount, int maxResultCount, int skipCount, IEnumerable<T> items)
        {
            TotalCount = totalCount;
            SkipCount = skipCount;
            MaxResultCount = maxResultCount;
            Items = items;
        }

        public IEnumerable<T> Items { get; set; }

        public int MaxResultCount { get; set; }

        public int SkipCount { get; set; }

        public long TotalCount { get; set; }
    }
}
