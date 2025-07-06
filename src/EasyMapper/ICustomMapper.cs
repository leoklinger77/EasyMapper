namespace EasyMapper {
	public interface ICustomMapper {
		TDestination Map<TSource, TDestination>(TSource source);
	}
}
