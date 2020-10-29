namespace DeltaX.Downtime.DapperRepository
{
    using AutoMapper;
    using DeltaX.Downtime.DapperRepository.Dto;
    using DeltaX.Downtime.Domain.ProcessAggregate; 

    public class DowntimeRepositoryMapper
    {
        IMapper mapper;

        public DowntimeRepositoryMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProcessHistoryDto, ProcessHistory>().ReverseMap();
                cfg.CreateMap<InterruptionHistoryDto, InterruptionHistory>().ReverseMap();
                cfg.CreateMap<ProductSpecificationDto, ProductSpecification>().ReverseMap();
            });

            mapper = config.CreateMapper();
        }

        public TDestination Map<TDestination>(object source)
        {
            return mapper.Map<TDestination>(source);
        } 
        
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return mapper.Map<TSource, TDestination>(source);
        }
    }
}
