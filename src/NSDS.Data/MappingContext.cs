using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NSDS.Data
{
	class MappingContext
	{
		private IDictionary<Type, ICollection> references = new Dictionary<Type, ICollection>();

		class Map<TSource, TDest>
		{
			public TSource Source { get; set; }
			public TDest Dest { get; set; }

			public Map(TSource source, TDest dest = default(TDest))
			{
				this.Source = source;
				this.Dest = dest;
			}
		}

		public Func<TDest> Get<TSource, TKey, TDest>(TSource source, Func<TSource, TKey> p, Func<TDest> @default)
		{
			if (!this.references.ContainsKey(typeof(TSource)))
			{
				this.references.Add(typeof(TSource), new List<Map<TSource, TDest>>());
			}
			var arr = this.references[typeof(TSource)] as ICollection<Map<TSource, TDest>>;
			var cached = arr.Where(x => p(x.Source).Equals(p(source))).SingleOrDefault();
			if (cached == null)
			{
				cached = new Map<TSource, TDest>(source);
				arr.Add(cached);
				cached.Dest = @default();
			}
			return () => cached.Dest;
		}
	}
}
