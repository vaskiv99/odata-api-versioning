using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Microsoft.Examples
{
    public static class ReflectionExtensions
    {
        public static bool IsEnumerableType( this Type type )
            => type.IsArray || type.IsGenericEnumerableType();

        public static bool IsNullableValueType( this Type type )
            => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof( Nullable<> );

        public static Type UnwrapCollectionTypeIfNeeded( this Type type )
        {
            if ( type.IsArray )
            {
                return type.GetElementType()!;
            }

            if ( type.IsGenericEnumerableType() )
            {
                return type.GetGenericArguments().First();
            }

            return type;
        }

        public static Type UnwrapTaskIfNeeded( this Type type )
        {
            if ( type.IsGenericType && type.GetGenericTypeDefinition() == typeof( Task<> ) )
            {
                return type.GetGenericArguments().First();
            }

            if ( type == typeof( Task ) )
            {
                return typeof( void );
            }

            return type;
        }

        public static Expression<Func<T, bool>> OrElse<T>(
            this Expression<Func<T, bool>> expr,
            Expression<Func<T, bool>> expr2 )
        {
            var param = expr.Parameters[0];
            var replacer = new ReplacerVisitor( expr2.Parameters[0], param );
            return Expression.Lambda<Func<T, bool>>( Expression.OrElse( expr.Body, replacer.Visit( expr2.Body )! ), param );
        }

        private static bool IsGenericEnumerableType( this Type type )
            => type.IsGenericType && typeof( IEnumerable ).IsAssignableFrom( type ) && type != typeof( string );

        private class ReplacerVisitor : ExpressionVisitor
        {
            private readonly Expression _parameter;
            private readonly Expression _replacement;

            public ReplacerVisitor( Expression parameter, Expression replacement )
            {
                _parameter = parameter;
                _replacement = replacement;
            }

            protected override Expression VisitParameter( ParameterExpression node )
            {
                if ( ReferenceEquals( node, _parameter ) )
                {
                    return _replacement;
                }

                return node;
            }
        }
    }
}