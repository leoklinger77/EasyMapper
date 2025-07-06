namespace EasyMapper.Mapper {
	using System.Linq.Expressions;
	public class MapperProfile {
		private readonly Dictionary<(Type source, Type destination, string destProp), Delegate> _customMappings
			= new();
		public MapBuilder<TSource, TDestination> FromMap<TSource, TDestination>(Expression<Func<TDestination, object>> destMember, Expression<Func<TSource, object>> srcMember) {
			var builder = new MapBuilder<TSource, TDestination>(this);
			builder.Map(destMember, srcMember);
			return builder;
		}

		public bool HasMappingFor<TSource, TDestination>() {
			return _customMappings.Keys.Any(k => k.source == typeof(TSource) && k.destination == typeof(TDestination));
		}

		internal void AddMapping<TSource, TDestination, TValue>(
			Expression<Func<TDestination, TValue>> destMember,
			Func<TSource, TValue> mapFunc) {
			var memberName = ((MemberExpression)destMember.Body).Member.Name;
			_customMappings[(typeof(TSource), typeof(TDestination), memberName)] = mapFunc;
		}

		public bool TryGetCustomMapping<TSource, TDestination>(string destProp, out Func<TSource, object?>? func) {
			if (_customMappings.TryGetValue((typeof(TSource), typeof(TDestination), destProp), out var del)) {
				func = src => del.DynamicInvoke(src);
				return true;
			}
			func = null;
			return false;
		}
	}
}