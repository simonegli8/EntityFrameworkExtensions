using System.Text;

#if NETFRAMEWORK
namespace System.Data.Entity;
#else
namespace Microsoft.EntityFrameworkCore;
#endif

public static class ExecuteInsertIntoIQueryableExtensions
{
    public static int ExecuteInsertInto<EntityType>(this IQueryable<EntityType> query, DbContext context) where EntityType : class
    {
        var set = context.Set<EntityType>();
        if (set == null) throw new InvalidOperationException("InsertInto: EntityType must be an entity type.");

#if NETCOREAPP
        var provider = context.Database.ProviderName;
        switch (provider)
        {
            case "Microsoft.EntityFrameworkCore.Sqlite":
			case "Microsoft.EntityFrameWorkCore.SqlServer":

				var entityType = context.Model.FindEntityType(typeof(EntityType));

                var sql = new StringBuilder();
                sql.Append("INSERT INTO [");
				// Table info 
				sql.Append(entityType.GetSchema());
                sql.Append("].[");
                sql.Append(entityType.GetTableName());
				sql.Append("] (");
                // Column info
                bool first = true;
                foreach (var property in entityType.GetProperties())
                {
                    if (!first) sql.Append(", ");
                    first = false;
                    sql.Append(property.GetColumnName());
                }
                sql.AppendLine(")");
                sql.Append(query.ToQueryString());
                return context.Database.ExecuteSqlRaw(sql.ToString());
            default:
                set.AddRange(query);
                return context.SaveChanges();
        }
#else
		set.AddRange(query);
		return context.SaveChanges();
#endif
	}
}
