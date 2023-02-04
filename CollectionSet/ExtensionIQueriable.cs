using System.ComponentModel;
using System.Linq.Expressions;

namespace CollectionSet
{    
    public static class ExtensionIQueriable
    {
        #region Metodos
            
        private static Expression CrearOrdenamientoExpression<T>(Expression expression, PropertyDescriptor propertyDescriptor, ListSortDirection listSortDirection)
        {
            var type = propertyDescriptor.ComponentType;
            var parameterExpression = Expression.Parameter(type);
            var lambda = Expression.Lambda(Expression.MakeMemberAccess(parameterExpression, type.GetProperty(propertyDescriptor.Name)!), parameterExpression);
            var argumentosType = new Type[] { type, propertyDescriptor.PropertyType };
            string? nombreMetodo;

            if (typeof(IOrderedQueryable<T>) == expression.Type)
            {
                if (listSortDirection == ListSortDirection.Ascending)
                {
                    nombreMetodo = "ThenBy";
                }
                else
                {
                    nombreMetodo = "ThenByDescending";
                }
            }
            else
            {
                if (listSortDirection == ListSortDirection.Ascending)
                {
                    nombreMetodo = "OrderBy";
                }
                else
                {
                    nombreMetodo = "OrderByDescending";
                }
            }

            if (propertyDescriptor.PropertyType == typeof(string))
            {
                return Expression.Call(typeof(Queryable), nombreMetodo, argumentosType, expression, lambda, Expression.Constant(new NumericStringComparer()));
            }
            
            return Expression.Call(typeof(Queryable), nombreMetodo, argumentosType, expression, lambda);
        }
            
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> iQueryable, PropertyDescriptor propertyDescriptor, ListSortDirection listSortDirection)
        {
            if (iQueryable == null)
            {
                throw new ArgumentNullException(nameof(iQueryable));
            }
            
            if (propertyDescriptor == null)
            {
                throw new ArgumentNullException(nameof(propertyDescriptor));
            }
            
            return (IOrderedQueryable<T>)iQueryable.Provider.CreateQuery<T>(CrearOrdenamientoExpression<T>(iQueryable.Expression, propertyDescriptor, listSortDirection));
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> iQueryable, ListSortDescriptionCollection listSortDescriptionCollection)
        {
            if (iQueryable == null)
            {
                throw new ArgumentNullException(nameof(iQueryable));
            }
            
            if (listSortDescriptionCollection == null)
            {
                throw new ArgumentNullException(nameof(listSortDescriptionCollection));
            }
            
            var expression = iQueryable.Expression;
            
            for (var i = 0; i < listSortDescriptionCollection.Count; i++)
            {
                expression = CrearOrdenamientoExpression<T>(expression, listSortDescriptionCollection[i]!.PropertyDescriptor!, listSortDescriptionCollection[i]!.SortDirection);
            }
            
            return (IOrderedQueryable<T>)iQueryable.Provider.CreateQuery<T>(expression);
        }

        #endregion
    }
}
