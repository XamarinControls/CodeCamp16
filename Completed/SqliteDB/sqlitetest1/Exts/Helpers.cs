using System;
using System.Linq.Expressions;

namespace sqlitetest1
{
	public static class Helpers
	{
		public static string NameOf<T>(Expression<Func<T>> expr)
		{
			return ((MemberExpression)expr.Body).Member.Name;
		}
	}
}
