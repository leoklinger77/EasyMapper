namespace EasyMapper {
	public interface IMapper {
		TDestination Map<TSource, TDestination>(TSource source);
	}
}
