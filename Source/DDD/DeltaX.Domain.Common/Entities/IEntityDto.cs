namespace DeltaX.Domain.Common.Entities
{
    public interface IEntityDto
    {

    }
     
    public interface IEntityDto<TKey> : IEntityDto
    {
        TKey Id { get; }
    }
}