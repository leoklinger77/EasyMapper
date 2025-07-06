namespace EasyMapper.Injector {
	using EasyMapper.Mapper;
	using Microsoft.Extensions.DependencyInjection;
	public static class EasyMapperInjector {
		public static void AddEasyMapper(this IServiceCollection services) {
			var profileType = typeof(MapperProfile);
			var profiles = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.Where(t => profileType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

			foreach (var type in profiles) {
				services.AddSingleton(profileType, type);
			}

			services.AddSingleton<ICustomMapper, CustomMapper>();
		}
	}
}