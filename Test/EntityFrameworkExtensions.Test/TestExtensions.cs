#if NETCOREAPP
using Microsoft.EntityFrameworkCore;
#else
using System.Data.Entity;
#endif
using Data = SolidCP.EnterpriseServer.Data;
using System.Diagnostics;
using SolidCP.EnterpriseServer.Data;

namespace Test
{
	[TestClass]
	public sealed class TestExtensions
	{
		public const string SqliteConnectionString = DbFactory.SqliteConnectionString;
		public const string SqlServerConnectionString = DbFactory.SqlServerConnectionString;
		public const string PostgreSqlConnectionString = DbFactory.PostgreSqlConnectionString;
		public const string MySqlConnectionString = DbFactory.MySqlConnectionString;

		Guid Scope = Guid.NewGuid();

		[TestInitialize]
		public void Init()
		{
			DbFactory.CreateTestDatabase();
		}

		[TestCleanup]
		public void Cleanup()
		{
			/*Cleanup(SqliteConnectionString);
			Cleanup(SqlServerConnectionString);
			Cleanup(MySqlConnectionString);
			Cleanup(PostgreSqlConnectionString);*/
		}
		public void Cleanup(string connectionString)
		{
			using (var db = new Data.DbContext(connectionString))
			{
				db.TempIds
					.Where(tid => tid.Scope == Scope)
					.ExecuteDelete();
			}
		}

		public TestContext TestContext { get; set; }

		public class EntityWrapper : Data.Entities.TempId { }
		public void TestInsertInto(string connectionString)
		{
			using (var db = new Data.DbContext(connectionString))
			{
				var watch = new Stopwatch();
				int minGroup = 1, maxGroup = 20;
				var providers = db.Providers
					.Where(p => minGroup <= p.GroupId && p.GroupId <= maxGroup);
				var now = DateTime.Now;
				var date = default(DateTime).ToUniversalTime();
				var providersIds = providers
					.Select(p => new EntityWrapper()
					{
						Id = p.ProviderId,
						Created = now,
						Level = 0,
						Scope = Scope,
						Date = date
					});
				var pa = providersIds.ToArray();

				var n = providersIds
					.ExecuteInsert();

				watch.Start();
				n = providersIds
					.ExecuteInsert();
				watch.Stop();
				TestContext.WriteLine($"ExecuteInsert: {n} rows, {watch.Elapsed}");

				watch.Reset();
				watch.Start();
				db.TempIds.AddRange(providersIds);
				n = db.SaveChanges();
				watch.Stop();
				TestContext.WriteLine($"SaveChanges: {n} rows, {watch.Elapsed}");

				var source = providers.Select(p => p.ProviderId).ToArray();
				var dest = db.TempIds
					.Where(tid => tid.Scope == Scope)
					.OrderBy(tid => tid.Key)
					.Select(p => p.Id)
					.ToArray();
				Assert.AreEqual(source.Length, dest.Length/3);

				for (int i = 0; i<source.Length;i++)
					Assert.AreEqual(source[i], dest[i]);
			}
		}

		[TestMethod]
		public void TestInsertIntoSqlite() => TestInsertInto(SqliteConnectionString);

		[TestMethod]
		public void TestInsertIntoSqlServer() => TestInsertInto(SqlServerConnectionString);

		[TestMethod]
		public void TestInsertIntoPostgre() => TestInsertInto(PostgreSqlConnectionString);
		[TestMethod]
		public void TestInsertIntoMySql() => TestInsertInto(MySqlConnectionString);

	}
}
