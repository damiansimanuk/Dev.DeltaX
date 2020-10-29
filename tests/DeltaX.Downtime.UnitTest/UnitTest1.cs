using DeltaX.Downtime.Domain.ProcessAggregate; 
using DeltaX.Downtime.Repository.Mappers;
using DeltaX.Repository.Common.Table;
using NUnit.Framework;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DeltaX.Downtime.UnitTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        public PropertyInfo GetPropertyInfo<TSource, TProperty>(TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression
                ?? throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo
                ?? throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }


        public void GetIdentityFieldd<TTable>(Expression<Func<TTable, object>> expression)
            where TTable : ITableDto
        {

            var x = expression.Type;
            var x2 = expression.Name;
        }

        [Test]
        public void Test_object_reflection()
        {
            ProcessHistoryDto dto = null;
            var propertyInfo = GetPropertyInfo(dto, dto => dto.Interruption );

            GetIdentityFieldd<ProcessHistoryDto>(a => a.Id);
        }
        
        
        [Test]
        public void Test_object_mapper()
        {
            var processHistoryDto = Activator.CreateInstance<ProcessHistoryDto>();
            var t = processHistoryDto.GetType();
            var c = processHistoryDto.GetColumns();

            ProcessHistoryDto processHistoryDto2 = default;
            var c2 = processHistoryDto2.GetColumns();
        }

        [Test]
        public void Test_table_config_builder()
        {
            var tablesFactory = new TableQueryFactory(Dialect.SQLite);

            tablesFactory.ConfigureTable<ProcessHistoryDto>("ProcessHistory", cfgTbl =>
            {
                ProcessHistoryDto dto = cfgTbl.Table;
                cfgTbl.AddColumn(nameof(dto.Id), "ProcessHistoryId", false, true);
                cfgTbl.AddColumn(nameof(dto.StartProcessDateTime) ); 
                cfgTbl.AddColumn(nameof(dto.FinishProcessDateTime));
                cfgTbl.AddColumn(nameof(dto.ProductSpecificationCode));  
            });

            tablesFactory.ConfigureTable<InterruptionHistoryDto>("InterruptionHistory", cfgTbl =>
            {
                InterruptionHistoryDto dto = cfgTbl.Table;
                cfgTbl.AddColumn(nameof(dto.Id), null, true, true);
                cfgTbl.AddColumn(nameof(dto.ProcessHistoryId));
                cfgTbl.AddColumn(nameof(dto.StartDateTime));
                cfgTbl.AddColumn(nameof(dto.EndDateTime));
            });

            tablesFactory.ConfigureTable<ProductSpecificationDto>("ProductSpecification", cfgTbl =>
            {
                cfgTbl.AddColumn(c => c.Id, null, false, false);
                cfgTbl.AddColumn(c => c.Code);
                cfgTbl.AddColumn(c => c.StandarDuration);
                cfgTbl.AddColumn(c => c.CreatedAt, c => { c.IgnoreInsert = true; c.IgnoreUpdate = true; });
            });

            var res = tablesFactory.GetPagedListQuery<ProcessHistoryDto>();
            res = tablesFactory.GetSingleQuery<ProcessHistoryDto>();
            res = tablesFactory.GetDeleteQuery<ProcessHistoryDto>();
            res = tablesFactory.GetUpdateQuery<ProcessHistoryDto>();
            res = tablesFactory.GetUpdateQuery<ProductSpecificationDto>();

            bool isIdentity;
            var res2 = tablesFactory.GetInsertQuery<ProcessHistoryDto>();
            Console.WriteLine(res);
        }


        [Test]
        public void Test_mapper_simple()
        {
            /// ProcessHistoryRepository processHistoryRepository = new ProcessHistoryRepository(null, new DowntimeRepositoryMapper());
            var mapper = new DowntimeRepositoryMapper();

            var item = new ProcessHistoryDto { 
                Id =Guid.NewGuid(),
                FinishProcessDateTime = DateTime.Now,
                StartProcessDateTime = DateTime.Now.AddMinutes(-1)
            };
            var entity = mapper.Map<ProcessHistory>(item);
            var newItem = mapper.Map<ProcessHistoryDto>(entity);

            Assert.AreEqual(item.Id, newItem.Id); 
            Assert.AreEqual(item.StartProcessDateTime, newItem.StartProcessDateTime); 
        }

        [Test]
        public void Test_mapper_complex()
        {
            /// ProcessHistoryRepository processHistoryRepository = new ProcessHistoryRepository(null, new DowntimeRepositoryMapper());
            var mapper = new DowntimeRepositoryMapper();

            var id = Guid.NewGuid();
            var interruptionId = 23;

            var item = new ProcessHistoryDto
            {
                Id = id,
                FinishProcessDateTime = DateTime.Now,
                StartProcessDateTime = DateTime.Now.AddMinutes(-1),
                Interruption = new InterruptionHistoryDto
                {
                    Id = interruptionId,
                    StartDateTime = DateTime.Now.AddSeconds(-10),
                    EndDateTime = DateTime.Now
                },
                ProductSpecification = new ProductSpecificationDto
                {
                    Code = "Codio1",
                    StandarDuration = 3
                }
            };
            var entity = mapper.Map<ProcessHistory>(item);
            var newItem = mapper.Map<ProcessHistoryDto>(entity);

            Assert.AreEqual(id, newItem.Id);
            Assert.AreEqual(interruptionId, newItem.Interruption.Id);
            Assert.AreEqual(item.StartProcessDateTime, newItem.StartProcessDateTime);
            Assert.AreEqual("Codio1", newItem.ProductSpecification.Code);
            Assert.AreEqual(3, newItem.ProductSpecification.StandarDuration);
        }
    }
}