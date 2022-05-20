using System.Linq.Expressions;

namespace IntScience.Services.Helpers;

public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> True<T>() { return f => true; }
    public static Expression<Func<T, bool>> False<T>() { return f => false; }

    public static Expression<Func<T, bool>> Or<T> (this Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2)
    {
        var invokedExp = Expression.Invoke(exp2, exp1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(exp1.Body, invokedExp), exp1.Parameters);
    }

    public static Expression<Func<T, bool>> And<T> (this Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2)
    {
        var invokedExp = Expression.Invoke(exp2, exp1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(exp1.Body, invokedExp), exp1.Parameters);
    }
}
