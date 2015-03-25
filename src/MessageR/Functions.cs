using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageR
{
	/// <summary>
	/// Helper functions
	/// </summary>
	public static class Functions
	{
		/// <summary>
		/// An identity function which can be used where a reference to the passed in item is needed
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <returns></returns>
		public static T Identity<T>(T t)
		{
			return t;
		}

		/// <summary>
		/// Function that always returns true
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <returns></returns>
		public static bool True<T>(T t)
		{
			return true;
		}
	}
}
