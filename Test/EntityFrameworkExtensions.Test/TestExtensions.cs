using Microsoft.EntityFrameworkCore;
using Data = SolidCP.EnterpriseServer.Data;
using System.Diagnostics;

namespace Test
{
	[TestClass]
	public sealed class TestExtensions
	{
		public const string SqliteConnectionString = DbFactory.SqliteConnectionString;
		public const string SqlServerConnectionString = DbFactory.SqlServerConnectionString;
		public const string PostgreSqlConnectionString = DbFactory.PostgreSqlConnectionString;

		[TestInitialize]
		public void Init()
		{
			DbFactory.CreateTestDatabase();
		}

		public TestContext TestContext { get; set; }

		public void TestInsertInto(string connectionString)
		{
			using (var db = new Data.DbContext(connectionString))
			{
				var watch = new Stopwatch();
				int minGroup = 1, maxGroup = 20;
				var providers = db.Providers
					.Where(p => minGroup <= p.GroupId && p.GroupId <= maxGroup);
				var now = DateTime.Now;
				var scope = Guid.NewGuid();
				var providersIds = providers
					.Select(p => new Data.Entities.TempId()
					{
						Id = p.ProviderId,
						Created = (DateTime)(object)now,
						Level = 0,
						Scope = scope,
						Date = (DateTime)(object)default(DateTime)
					});

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
					.Where(tid => tid.Scope == scope)
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
	}
}
