using System.Linq.Expressions;

namespace Midas.Extensions
{
    public static class PredicateHelper
    {
        public static Expression<Func<T, bool>> True<T>()
        {
            return (Expression<Func<T, bool>>)(input => true);
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return (Expression<Func<T, bool>>)(input => false);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>>? expression1, Expression<Func<T, bool>> expression2)
        {
            if (expression1 == null)
            {
                return expression2;
            }

            InvocationExpression invocationExpression = Expression.Invoke((Expression)expression2, expression1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>((Expression)Expression.OrElse(expression1.Body, (Expression)invocationExpression), (IEnumerable<ParameterExpression>)expression1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>>? expression1, Expression<Func<T, bool>> expression2)
        {
            if (expression1 == null)
            {
                return expression2;
            }

            InvocationExpression invocationExpression = Expression.Invoke((Expression)expression2, expression1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>((Expression)Expression.AndAlso(expression1.Body, (Expression)invocationExpression), (IEnumerable<ParameterExpression>)expression1.Parameters);
        }
    }
}
