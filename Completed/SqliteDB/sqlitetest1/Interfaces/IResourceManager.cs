using System;
using System.Collections.Generic;
using System.Reflection;

namespace sqlitetest1.Interfaces
{
    public interface IResourceManager  
    {
		/// <summary>
		/// get Assembly
		/// </summary>
		/// <param name="type">Type.</param>
		Assembly Get (string type);

		/// <summary>
		/// Finds the sub classes of.
		/// </summary>
		/// <returns>The sub classes of.</returns>
		/// <param name="ExcludeBase">Exclude base.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
        IEnumerable<Type> FindSubClassesOf<T>(bool ExcludeBase);


		/// <summary>
		/// find all classes in a namespace
		/// </summary>
		/// <returns>The namespace.</returns>
		/// <param name="Findtype">Findtype.</param>
        IEnumerable<Type> FindNamespace(params string[] Findtype);
    }
}
