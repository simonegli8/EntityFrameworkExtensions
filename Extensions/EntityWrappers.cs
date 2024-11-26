using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace EntityFrameworkExtensions
{
	public static class EntityWrappers
	{
		 
		public static Dictionary<Type, Type> Types = null;
		public static object Lock = new();
		public static void EnsureCreateWrappers(Func<IEnumerable<Type>> gettypes)
		{
			lock (Lock)
			{
				if (Types == null)
				{
					var types = gettypes();
				}
			}
		}

		public static Type Get(Type type, Func<IEnumerable<Type>> gettypes)
		{
			EnsureCreateWrappers(gettypes);
			return Types[type];
		}
	}
}
