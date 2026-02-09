namespace EasyMapper.Mapper {
	using System.Linq.Expressions;
	public class MapBuilder<TSource, TDestination> {
		private readonly MapperProfile _profile;

		public MapBuilder(MapperProfile profile) {
			_profile = profile;
		}

		public MapBuilder<TSource, TDestination> Map<TValue>(Expression<Func<TDestination, TValue>> srcMember, Expression<Func<TSource, TValue>> destMember) {
			var compiled = destMember.Compile();
			_profile.AddMapping<TSource, TDestination, TValue>(srcMember, compiled);
			return this;
		}
	}
}