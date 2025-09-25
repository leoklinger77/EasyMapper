namespace EasyMapper.Mapper {
    using System.Linq.Expressions;
    public class MapperProfile {
        private readonly Dictionary<(Type source, Type destination, string destProp), Delegate> _customMappings
            = [];
        public MapBuilder<TSource, TDestination> FromMap<TSource, TDestination>(Expression<Func<TDestination, object>> srcMember, Expression<Func<TSource, object>> destMember) {
            var builder = new MapBuilder<TSource, TDestination>(this);
            builder.Map(srcMember, destMember);
            return builder;
        }

        public bool HasMappingFor<TSource, TDestination>() {
            return _customMappings.Keys.Any(k => k.source == typeof(TSource) && k.destination == typeof(TDestination));
        }

        internal void AddMapping<TSource, TDestination, TValue>(Expression<Func<TDestination, TValue>> destMember, Func<TSource, TValue> mapFunc) {
            MemberExpression memberExpr = destMember.Body as MemberExpression;

            if (memberExpr == null && destMember.Body is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression operandExpr)
                memberExpr = operandExpr;

            if (memberExpr == null)
                throw new InvalidOperationException("The provided expression is not a valid MemberExpression.");

            _customMappings[(typeof(TSource), typeof(TDestination), memberExpr.Member.Name)] = mapFunc;
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