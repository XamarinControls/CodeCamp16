using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation;
using sqlitetest1.Exts;
using sqlitetest1.Interfaces;

namespace sqlitetest1.iOS
{
	public class ResourceManager : IResourceManager
	{

		public ResourceManager()
		{
		}

		public System.Threading.SynchronizationContext MainThread
		{
			get
			{
				return System.Threading.SynchronizationContext.Current;
			}
		}

		public bool IsMainThread
		{
			get
			{
				return NSThread.IsMain;
			}
		}

		public IEnumerable<Type> FindNamespace(params string[] Findtype)
		{
			if (Findtype != null)
			{
				Findtype.ConvertInPlace(x => x.ToUpper());

				Func<string, bool> Search = (name) =>
				{
					if (String.IsNullOrEmpty(name))
						return false;

					return Findtype.Any(x => x == name.ToUpper());
				};

				return (
					AppDomain.CurrentDomain.GetAssemblies()
					.Select(a => a.ManifestModule.GetTypes()))
						.SelectMany(t => t)
						.Where(t => t != null &&
							Search(t.Namespace) == true)
						.ToList();
			}
			else
			{
				return new List<Type>();
			}
		}


		public IEnumerable<Type> FindSubClassesOf<T>(bool ExcludeBase)
		{
			try
			{

				var baseType = typeof(T);
				var className = baseType.Name;
				var assembly = baseType.Assembly;
				var data = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
				if (ExcludeBase)
				{
					return data.Where(d => d.Name != className);
				}
				return data;
			}
			catch
			{
				return null;
			}
		}

		public Assembly Get(string type)
		{
			try
			{
				var t = Type.GetType (type);
				return Assembly.GetAssembly (t);
			}
			catch {
				return null;
			}
		}
	}
}