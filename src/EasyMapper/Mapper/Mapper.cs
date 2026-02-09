namespace EasyMapper.Mapper {
    using System.Reflection;
    public class Mapper : IMapper {
        private readonly IEnumerable<MapperProfile> _profiles;

        public Mapper(IEnumerable<MapperProfile> profiles) {
            _profiles = profiles;
        }

        public TDestination Map<TSource, TDestination>(TSource source) {
            if (source is null) {
                return default!;
            }

            var sourceType = typeof(TSource);
            var destType = typeof(TDestination);

            if (IsGenericEnumerable(sourceType) && IsGenericEnumerable(destType)) {
                var sourceItemType = sourceType.GetGenericArguments()[0];
                var destItemType = destType.GetGenericArguments()[0];

                var sourceEnumerable = source as System.Collections.IEnumerable;
                var listType = typeof(List<>).MakeGenericType(destItemType);
                var destList = (System.Collections.IList)Activator.CreateInstance(listType)!;

                foreach (var item in sourceEnumerable) {
                    var mapMethod = this.GetType()
                        .GetMethod(nameof(Map))!
                        .MakeGenericMethod(sourceItemType, destItemType);

                    var mappedItem = mapMethod.Invoke(this, new object[] { item })!;
                    destList.Add(mappedItem);
                }

                return (TDestination)destList;
            }

            var destination = Activator.CreateInstance<TDestination>();

            var profile = _profiles.FirstOrDefault(p => p.HasMappingFor<TSource, TDestination>());

            foreach (var destProp in destType.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                if (!destProp.CanWrite) continue;

                if (profile != null &&
                    profile.TryGetCustomMapping<TSource, TDestination>(destProp.Name, out var mapFunc)) {

                    var value = mapFunc!(source);
                    SetValue(destination, destProp, value);
                    continue;
                }

                var sourceProp = sourceType.GetProperty(destProp.Name);
                if (sourceProp != null && sourceProp.CanRead) {
                    var sourceValue = sourceProp.GetValue(source);

                    if (IsSimpleType(destProp.PropertyType)) {
                        SetValue(destination, destProp, sourceValue);
                    } else if (sourceValue != null) {
                        var mapMethod = this.GetType()
                            .GetMethod(nameof(Map))!
                            .MakeGenericMethod(sourceProp.PropertyType, destProp.PropertyType);

                        var mappedValue = mapMethod.Invoke(this, new object[] { sourceValue });
                        SetValue(destination, destProp, mappedValue);
                    }
                }

            }

            return destination;
        }

        private static bool IsSimpleType(Type type) {
            return type.IsPrimitive ||
                   type.IsEnum ||
                   type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(Guid);
        }

        private bool IsGenericEnumerable(Type type) {
            if (!type.IsGenericType) return false;

            var genTypeDef = type.GetGenericTypeDefinition();
            return genTypeDef == typeof(IEnumerable<>) ||
                   genTypeDef == typeof(IList<>) ||
                   genTypeDef == typeof(List<>);
        }

        private static void SetValue<TDestination>(TDestination destination, PropertyInfo destProp, object sourceValue) {
            if (sourceValue == null) return;

            var sourceTypeValue = sourceValue.GetType();

            // 1. simples → simples
            if (IsSimpleType(destProp.PropertyType)) {
                if (destProp.PropertyType == typeof(string) && sourceTypeValue != typeof(string)) {
                    destProp.SetValue(destination, sourceValue.ToString());
                } else if (destProp.PropertyType.IsEnum && sourceValue is string s) {
                    destProp.SetValue(destination, Enum.Parse(destProp.PropertyType, s));
                } else if (destProp.PropertyType.IsAssignableFrom(sourceTypeValue)) {
                    destProp.SetValue(destination, sourceValue);
                }
                return;
            }

            // 2. complexos (objetos ou listas)
            if (destProp.PropertyType.IsAssignableFrom(sourceTypeValue)) {
                destProp.SetValue(destination, sourceValue);
            } else {
                // tenta mapear recursivamente
                var mapMethod = typeof(Mapper).GetMethod(nameof(Map))!
                    .MakeGenericMethod(sourceTypeValue, destProp.PropertyType);

                var mappedValue = mapMethod.Invoke(new Mapper([]), new object[] { sourceValue });
                destProp.SetValue(destination, mappedValue);
            }
        }

    }
}