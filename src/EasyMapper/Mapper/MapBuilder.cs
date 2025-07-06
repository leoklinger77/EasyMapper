namespace EasyMapper.Mapper {
	using System.Linq.Expressions;
	public class MapBuilder<TSource, TDestination> {
		private readonly MapperProfile _profile;

		public MapBuilder(MapperProfile profile) {
			_profile = profile;
		}

		public MapBuilder<TSource, TDestination> Map<TValue>(Expression<Func<TDestination, TValue>> destMember, Expression<Func<TSource, TValue>> srcMember) {
			var compiled = srcMember.Compile();
			_profile.AddMapping<TSource, TDestination, TValue>(destMember, compiled);
			return this;
		}
	}
}