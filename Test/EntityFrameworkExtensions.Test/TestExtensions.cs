using Microsoft.EntityFrameworkCore;
using Data = SolidCP.EnterpriseServer.Data;

namespace EntityFrameworkExtensions.Test
{
	[TestClass]
	public sealed class TestExtensions
	{
		public const string SqliteConnectionString = DbFactory.SqliteConnectionString;
		public const string SqlServerConnectionString = DbFactory.SqlServerConnectionString;

		[TestInitialize]
		public void Init()
		{
			DbFactory.CreateTestDatabase();
		}

		public void TestInsertInto(string connectionString)
		{
			using (var db = new Data.DbContext(connectionString))
			{
				int minGroup = 1, maxGroup = 20;
				var providers = db.Providers
					.Where(p => minGroup <= p.GroupId && p.GroupId <= maxGroup);
				var now = DateTime.Now;
				var scope = Guid.NewGuid();
				providers
					.Select(p => new Data.Entities.TempId()
					{
						Id = p.ProviderId, Created = now, Level = 0, Scope = scope
					})
					.ExecuteInsertInto<Data.Entities.TempId>((DbContext)db.BaseContext);
			}
		}

		[TestMethod]
		public void TestInsertIntoSqlite() => TestInsertInto(SqliteConnectionString);

		[TestMethod]
		public void TestInsertIntoSqlServer() => TestInsertInto(SqlServerConnectionString);
	}
}
