using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Dialect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using NHibernate.Linq;
using HibernatingRhinos.Profiler.Appender.NHibernate;

namespace nHibDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            NHibernateProfiler.Initialize();

            var cfg = new Configuration();
            cfg.DataBaseIntegration(x =>
            {
                x.ConnectionString = "Server=SURFACEBOX\\SQLEXPRESS;Database=nHibDemo;Integrated Security=SSPI;";
                x.Driver<SqlClientDriver>();
                x.Dialect<MsSql2012Dialect>();
                x.LogSqlInConsole = true;
            });
            cfg.AddAssembly(Assembly.GetExecutingAssembly());

            var sessionFactory = cfg.BuildSessionFactory();
            using (var session = sessionFactory.OpenSession())
            using (var trx = session.BeginTransaction())
            {
                var query = from customer in session.Query<Customer>()
                            where customer.LastName == "Bloob"
                            select customer;

                var c = query.First();
                session.Delete(c);
                trx.Commit();
            }

            using (var session = sessionFactory.OpenSession())
            using (var trx = session.BeginTransaction())
            {
                //perform db logic
                var customers = from customer in session.Query<Customer>()
                                orderby customer.LastName
                                select customer;
                foreach (var customer in customers)
                {
                    Console.WriteLine("{0} {1} ({2})", customer.FirstName, customer.LastName, customer.Id);
                }

                Console.ReadLine();

                trx.Commit();
            }

        }


    }
}


