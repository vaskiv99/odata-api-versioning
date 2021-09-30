using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Examples
{
    public static class ClassInfo<T>
    {
        public static IEnumerable<string> RootPropertyPaths =>
            typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance ).Select( q => q.Name );

        public static string GetPath( Expression<Func<T, object?>> selector )
        {
            var visitor = new PropertyVisitor();
            visitor.Visit( selector.Body );
            return string.Join( '.', visitor.Path.Reverse() );
        }

        public static IEnumerable<string> GetPath( params Expression<Func<T, object?>>[] selectors ) =>
            selectors.Select( GetPath );

        private class PropertyVisitor : ExpressionVisitor
        {
            private readonly List<string> _path = new List<string>();

            public IReadOnlyList<string> Path => _path;

            protected override Expression VisitMember( MemberExpression node )
            {
                if ( !( node.Member is PropertyInfo ) )
                {
                    throw new ArgumentException( "The path can only contain properties", nameof( node ) );
                }

                _path.Add( node.Member.Name );
                return base.VisitMember( node );
            }
        }
    }
}