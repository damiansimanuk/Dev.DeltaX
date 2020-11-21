using DeltaX.Repository.Common.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeltaX.LinqExpression
{

    class Poco1: ITableDto
    {
        public int id { get; set; }
        public string Name { get; set; }
        public DateTime Updated { get; set; }
        public bool Enable { get; set; }
    }

    class Poco2
    {
        public int id { get; set; }
        public string Name { get; set; }
        public DateTime Updated { get; set; }
        public bool Enable { get; set; }
    }

    class Poco3
    {
        public int Id { get; set; }
        public int IdPoco2 { get; set; }
    }


        class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var tf = TableQueryFactory.GetInstance();
            tf.ConfigureTable<Poco1>("Poco1", ct =>
            {
                ct.AddColumn(c => c.id, null, false, true);
                ct.AddColumn(c => c.Name);
                ct.AddColumn(c => c.Updated);
                ct.AddColumn(c => c.Enable);
            });


            bool valid = true;

            var qb1 = new QueryBuilder<Poco1>();

            qb1.Where<Poco1>(t => t.id == 2);



            var qb12 = qb1.Join<Poco2>((t1, t2) => t1.id == t2.id);
            var qb123 = qb12.Join<Poco3>((t1, t2, t3) => t1.id == t2.id);

            qb1.Select(t1 => new { t1.id, t1.Name, t1.Enable });
            qb12.Select(t1 => t1.id, t2 => t2.Name);
            qb123.Select(t1 => t1.id, t2 => t2.Name, t3 => t3.IdPoco2);
            var res22 = qb123.Where<Poco3>(t3 => t3.IdPoco2 == 2);
            var res21 = qb1.Where2<Poco3, Poco1>(t3 => t3.Id == 2);

           //  new QueryBuilderOld<Poco>(args => args.Enable);
           //  new QueryBuilderOld<Poco>(args => !args.Enable);
           //  new QueryBuilderOld<Poco>(args => args.Enable == true);
           //  new QueryBuilderOld<Poco>(args => args.Enable != true);
           //  new QueryBuilderOld<Poco>(args => args.Enable == valid);
           //  var queryBuilder = new QueryBuilderOld<Poco>(args => args.id == 3);

            // queryBuilder
            //     .Where<Poco>(a => a.Enable)
            //     .Select<Poco>(a => new { a.id, a.Name });

            // IQueryBuilder<Poco> qbt1 = default;
            // qbt1
            //     .Select(a => new { a.Enable, a.id })
            //     .Where(a => a.Enable);
            // 
            // 
            // var qbt1t2 = qbt1.Join<Poco, Poco2>((t1, t2) => t1.id == t2.id);
            // var qbt1t2t3 = qbt1t2.Join<Poco, Poco2, Poco3>((t1, t2 ,t3) => t1.id == t3.id);
            // 
            // qbt1t2t3
            //     .Select<Poco2>(a => a.Name)
            //     .Where(a => a.Enable);
            // 

            List<int> a = new List<int>();
            var aa = a.Select(i => i.ToString());

        }
    }
}
