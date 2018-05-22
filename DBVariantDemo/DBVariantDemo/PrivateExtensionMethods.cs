using System.Collections.Generic;
using System.Linq;

namespace DBVariantDemo
{
	internal static class PrivateExtensionMethods
	{
		public static T DequeueOrDefault<T>(this Queue<T> queue)
		{
			if (!queue.Any())
				return default;

			return queue.Dequeue();
		}
	}
}
