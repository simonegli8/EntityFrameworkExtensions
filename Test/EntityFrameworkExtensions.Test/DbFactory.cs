using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidCP.EnterpriseServer.Data;

namespace Test
{
	public class DbFactory
	{
		public const string SqliteConnectionString = "DbType=Sqlite;Data Source=Database.sqlite";
		public const string SqlServerConnectionString = "DbType=SqlServer;Server=(local);Database=SolidCP;User ID=SolidCP;Password=Password12;TrustServerCertificate=true";
		public const string PostgreSqlConnectionString = "DbType=PostgreSql;Server=localhost;Database=SolidCP;User ID=postgres;Password=Password12";
		public const string MySqlConnectionString = "DbType=MySql;Server=localhost;Uid=root;Pwd=Password12;Database=SolidCP";

		public static void CreateTestDatabase()
		{
			if (File.Exists("Database.sqlite"))
			{
				try
				{
					File.Delete("Database.sqlite");
					DatabaseUtils.InstallFreshDatabase(SqliteConnectionString, "Database.sqlite", "TestUser", "TestUser");
				}
				catch { }
			}
		}
	}
}
